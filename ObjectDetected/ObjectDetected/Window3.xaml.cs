using System;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Python.Runtime;
using OpenCvSharp;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Interop;
using System.Reflection.Emit;
using System.Windows.Shapes;
using IronPython.Runtime.Operations;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using static IronPython.Modules._ast;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace ObjectDetected
{
    /// <summary>
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Window3 : System.Windows.Window
    {
        private System.Windows.Threading.DispatcherTimer positionTimer;
        private bool isDraggingSlider = false;
        private MemoryStream imageStream;

        private RenderTargetBitmap bitmap;
        private static dynamic torch;
        private static dynamic model;
        private static dynamic posix_backup;
        dynamic s;
        private VideoCapture videoCapture;
        private Polygon selectArea;
        private BitmapSource bitmapSource;
        private double scaleX;
        private double scaleY;
        private readonly DrawingVisual visual = new DrawingVisual();
        System.Windows.Point p1, p2;
        System.Windows.Point dp11, dp12, dp13, dp14;
        bool flag;
        bool selectflag = false;
        Random random = new Random();
        public Window3()
        {
            ;
            InitializeComponent();
            InitializePythonEngine();

            mediaPlayer.MediaOpened += MediaElement_MediaOpened;
            // Инициализируем поток для передачи кадров в Python
            imageStream = new MemoryStream();

            // Запускаем таймер для отправки кадров в Python
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 30) // 30 кадров в секунду
            };
            timer.Tick += (sender, e) => CompositionTarget_Rendering();
            timer.Start();
        }
        private void InitializeCamera(string str)
        {
            videoCapture = new VideoCapture(0); // Используйте 0 для захвата с веб-камеры
            videoCapture.Open(str);

        }
        private void CompositionTarget_Rendering()
        {
            
            Mat frame;
            var width = mediaPlayer.NaturalVideoWidth;
            var height = mediaPlayer.NaturalVideoHeight;
            if (width > 0 && height > 0)
            {

                using (var dc = visual.RenderOpen())
                {
                    dc.DrawRectangle(
                        new VisualBrush(mediaPlayer), null,
                        new System.Windows.Rect(0, 0, width, height));
                }
                bitmap = new RenderTargetBitmap(width,height, 96, 96, PixelFormats.Default);
                bitmap.Render(visual);

                bitmapSource = BitmapFrame.Create(bitmap);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                if (p1.X > 0 && p2.X > 0 && selectflag == true)
                {
                    
                    try
                    {
                        int swidth = (int)((Math.Max(p1.X, p2.X) - Math.Min(p1.X, p2.X)) * (1 / scaleX));
                        int sheight = (int)((Math.Max(p1.Y, p2.Y) - Math.Min(p1.Y, p2.Y)) * (1 / scaleY));
                        CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect((int)(p1.X * (1 / scaleX)), (int)(p1.Y * (1 / scaleY)), swidth, sheight));
                        bitmapSource = croppedBitmap;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    
                }
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.Position = 0;

                    // Чтение изображения из MemoryStream в OpenCV Mat
                    frame = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color);

                }

                SendImageToPython(frame);
            }



        }
        private void InitializePythonEngine()
        {
            Runtime.PythonDLL = @"C:\Users\shabu\AppData\Local\Programs\Python\Python311\python311.dll";
            // Инициализация Python
            PythonEngine.Initialize();
        }

        private void SendImageToPython(Mat image)
        {
            using (Py.GIL())
            {
                if (torch == null)
                    torch = Py.Import("torch");
                if(posix_backup==null)
                {
                    s=Py.Import("pathlib");
                    posix_backup = s.PosixPath;
                    Console.WriteLine(posix_backup);
                    s.PosixPath = s.WindowsPath;
                }
                if (model == null)
                {
                    dynamic device = torch.device("cpu");
                    model = torch.hub.load(@"ultralytics\yolov5", "custom", path: @"best.pt", source: "local", device: device);
                    s.PosixPath = posix_backup;
                }    
                   
                dynamic np = Py.Import("numpy");
                dynamic cv2 = Py.Import("cv2");

                // Преобразование Mat в массив NumPy
                byte[] imageBytes = new byte[image.Total() * image.ElemSize()];
                Marshal.Copy(image.Data, imageBytes, 0, imageBytes.Length);
                var npImage = np.frombuffer(imageBytes, dtype: np.uint8).reshape(image.Height, image.Width, image.Channels());

                dynamic results = model(npImage);
                dynamic resultsJson = results.pandas().xyxy[0].to_json(orient: "records");
                string resultsJsonString = resultsJson.ToString();

                // Преобразование строки JSON в список (или массив)
                List<ResultItem> resultList = JsonConvert.DeserializeObject<List<ResultItem>>(resultsJsonString);
                paint_area.Children.Clear();
                // Теперь resultList содержит данные из resultsJson в виде списка объектов ResultItem
                scaleX = (mediaPlayer.ActualWidth / mediaPlayer.NaturalVideoWidth);
                scaleY = (mediaPlayer.ActualHeight / mediaPlayer.NaturalVideoHeight);
                ImageBrush brush = new ImageBrush(bitmapSource);
                paint_area.Background = brush;
                if (selectArea != null)
                    paint_area.Children.Add(selectArea);
                if (selectflag == true)
                    paint_area.Children.Remove(selectArea);
                // Проход по всем объектам и масштабирование координат
                foreach (var item in resultList)
                {
                    item.Xmin *= scaleX;
                    item.Ymin *= scaleY;
                    item.Xmax *= scaleX;
                    item.Ymax *= scaleY;
                }
                foreach (var item in resultList)
                {

                    if(select_class.SelectedItem != null)
                    {
                        string p = (select_class.SelectionBoxItem.ToString());
                        if (p == item.Name)
                        {
                            paint_area.Children.Add(draw(item.Xmin, item.Ymin, item.Xmax, item.Ymax, 2, System.Windows.Media.Brushes.LightGreen));
                            TextBlock text = new TextBlock();
                            text.Text = "Класс " + item.Name.ToString();
                            text.Foreground = System.Windows.Media.Brushes.LightGreen;
                            text.FontSize = 12;
                            Canvas.SetTop(text, item.Ymin - 20);
                            Canvas.SetLeft(text, item.Xmin + (item.Xmax - item.Xmin) / 2);
                            paint_area.Children.Add(text);
                            TextBlock text1 = new TextBlock();
                            text1.Text = "Confidence" + item.Confidence.ToString();
                            text1.Foreground = System.Windows.Media.Brushes.LightGreen;
                            text1.FontSize = 12;
                            Canvas.SetTop(text1, item.Ymin - 40);
                            Canvas.SetLeft(text1, item.Xmin + (item.Xmax - item.Xmin) / 2);
                            paint_area.Children.Add(text1);
                            // Use Dispatcher to invoke the SerializeObjects after rendering is complete
                            this.Dispatcher.Invoke(() =>
                            {
                                System.Threading.Thread.Sleep(100); // Добавьте небольшую задержку (миллисекунды)
                                char[] separators = { '.' };
                                string[] parts = mediaPlayer.Position.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                string s = parts[parts.Length - 1];
                                string lastElement = parts[0];
                                char[] separators2 = { ':' };
                                parts = lastElement.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                                string path = @"..\..\destruction\" + parts[0] + "-" + parts[1] + "-" + parts[2] + "-" + s + " " + p;
                                SerializeObjects(path, paint_area);
                            }, System.Windows.Threading.DispatcherPriority.Background);

                        }
                    }
                    else
                    {
                        paint_area.Children.Add(draw(item.Xmin, item.Ymin, item.Xmax, item.Ymax, 2, System.Windows.Media.Brushes.LightGreen));
                        TextBlock text = new TextBlock();
                        text.Text = "Класс " + item.Name.ToString();
                        text.Foreground = System.Windows.Media.Brushes.LightGreen;
                        text.FontSize = 12;
                        Canvas.SetTop(text, item.Ymin - 20);
                        Canvas.SetLeft(text, item.Xmin + (item.Xmax - item.Xmin) / 2);
                        paint_area.Children.Add(text);
                        TextBlock text1 = new TextBlock();
                        text1.Text = "Confidence " + item.Confidence.ToString();
                        text1.Foreground = System.Windows.Media.Brushes.LightGreen;
                        text1.FontSize = 12;
                        Canvas.SetTop(text1, item.Ymin - 40);
                        Canvas.SetLeft(text1, item.Xmin + (item.Xmax - item.Xmin) / 2);
                        paint_area.Children.Add(text1);
                    }
                   
                    //Console.WriteLine($"Class: {item.Class}, Name: {item.Name}");
                    //Console.WriteLine($"Confidence: {item.Confidence}");
                    //Console.WriteLine($"Bounding Box: ({item.Xmin}, {item.Ymin}, {item.Xmax}, {item.Ymax})");
                    //Console.WriteLine();
                }
                
                Console.WriteLine(results.ToString());
            }
        }
        private static void SerializeObjects(string filePath, FrameworkElement element)
        {
            try
            {
                var drawingVisual = new DrawingVisual();
                using (var context = drawingVisual.RenderOpen())
                {
                    var brush = new VisualBrush(element);
                    context.DrawRectangle(brush, null, new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(element.ActualWidth, element.ActualHeight)));
                }

                var renderTargetBitmap = new RenderTargetBitmap(
                    (int)Math.Ceiling(element.ActualWidth),
                    (int)Math.Ceiling(element.ActualHeight),
                    96,
                    96,
                    PixelFormats.Pbgra32
                );

                renderTargetBitmap.Render(drawingVisual);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                using (FileStream fs = new FileStream(filePath + ".png", FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}");
            }
        }


        private Polygon draw(double x1, double y1, double x2, double y2, int s, System.Windows.Media.Brush b)
        {
            Polygon r = new Polygon();
            dp11.X = x1;
            dp11.Y = y1;
            dp12.X = x1;
            dp12.Y = y2;
            dp13.X = x2;
            dp13.Y = y2;
            dp14.X = x2;
            dp14.Y = y1;
            r.Points.Add(dp11);
            r.Points.Add(dp12);
            r.Points.Add(dp13);
            r.Points.Add(dp14);
            r.StrokeThickness = s;
            r.Stroke = b;
            return r;
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            positionTimer.Start();
            isDraggingSlider = false;
            mediaPlayer.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
        }

        public void LoadVideo(string videoPath)
        {
            try
            {
                InitializeCamera(videoPath);
                mediaPlayer.Source = new Uri(videoPath);
                mediaPlayer.MediaOpened += MediaElement_MediaOpened;
                positionTimer = new System.Windows.Threading.DispatcherTimer();
                positionTimer.Interval = TimeSpan.FromMilliseconds(100);
                positionTimer.Tick += PositionTimer_Tick;
                mediaPlayer.MediaEnded += MediaElement_MediaEnded;
                progressSlider.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(Slider_DragStarted));
                progressSlider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(Slider_DragCompleted));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке видео: {ex.Message}");
            }
        }
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Обновление интерфейса, например, обновление продолжительности ProgressBar
            progressSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            positionTimer.Start();
            isDraggingSlider = false;
            CompositionTarget_Rendering();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            isDraggingSlider = true;
            // Сброс времени воспроизведения и слайдера по завершении видео
            mediaPlayer.Stop();
            progressSlider.Value = 0;

            // Остановка таймера при завершении медиа
            positionTimer.Stop();
        }

        // Подписка на событие изменения времени воспроизведения во время воспроизведения
        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            // Обновление положения слайдера при изменении времени воспроизведения
            if (!isDraggingSlider)
            {
                progressSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void select_btn_Click_1(object sender, RoutedEventArgs e)
        {
            if (selectflag == false)
            {
                selectflag = true;
                select_btn.Content = "unselect";
                
            }
            else
            {
                selectflag = false;
                select_btn.Content = "select";
            }
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDraggingSlider = true;
            mediaPlayer.Pause();
        }
        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDraggingSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            mediaPlayer.Play();
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Обновление времени воспроизведения при изменении положения слайдера
            if (!isDraggingSlider)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            }
        }

        private void paint_area_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            paint_area.Children.Clear();
            flag = true;
            p1 = e.GetPosition(paint_area);
        }

        private void paint_area_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (flag == true)
            {
                p2 = e.GetPosition(paint_area);
            }
        }


        private void paint_area_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            flag = false;
            if (p1.X > 0 && p1.Y > 0)
            {
                p2 = e.GetPosition(paint_area);
                selectArea = draw(p1.X, p1.Y, p2.X, p2.Y, 10, System.Windows.Media.Brushes.Black);
            }

        }
    }
}