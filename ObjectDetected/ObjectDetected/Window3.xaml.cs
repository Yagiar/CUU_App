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
        private static dynamic torch;
        private static dynamic model;
        private VideoCapture videoCapture;
        private readonly DrawingVisual visual = new DrawingVisual();
        public Window3()
        {
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
            RenderTargetBitmap bitmap;
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
                bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                bitmap.Render(visual);
                BitmapSource bitmapSource = BitmapFrame.Create(bitmap);

                images.Source = bitmapSource;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
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
                if (model == null)
                    model = torch.hub.load(@"ultralytics\yolov5", "custom", path: @"C:\Users\shabu\CUU_App\best.pt", source: "local");
                dynamic np = Py.Import("numpy");
                dynamic cv2 = Py.Import("cv2");

                // Преобразование Mat в массив NumPy
                byte[] imageBytes = new byte[image.Total() * image.ElemSize()];
                Marshal.Copy(image.Data, imageBytes, 0, imageBytes.Length);
                var npImage = np.frombuffer(imageBytes, dtype: np.uint8).reshape(image.Height, image.Width, image.Channels());

                dynamic results = model(npImage);
                dynamic resultsJson = results.pandas().xyxy[0].to_json(orient: "records");
                Console.WriteLine(resultsJson.ToString());
<<<<<<< HEAD
<<<<<<< HEAD
                Console.WriteLine(results.ToString());
=======
>>>>>>> e37fd4c (repair)
=======
>>>>>>> f536cc8564ab11dc55402358b6204f1106bda681
            }
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
    }
}
