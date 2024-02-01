using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using static IronPython.Modules._ast;

namespace ObjectDetected
{
    /// <summary>
    /// Логика взаимодействия для Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private int widths = 16;
        private int heights = 9;
        private int modifire = 10;
        private List<string> videoPaths = new List<string>();
        private int videosPerRow = 2;
        public Window4()
        {
            InitializeComponent();
            LoadScreen();
            this.SizeChanged += Window4_SizeChanged;
        }
        private void Window4_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Обновляем максимальные размеры videosContainer при изменении размеров окна

            videosScrollViewer.MaxWidth = this.ActualWidth;
            videosScrollViewer.MaxHeight = this.ActualHeight;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window2 videoPlayerWindow = new Window2();
            videoPlayerWindow.Show();
            this.Close();
        }
        private string separete_video(string s)
        {
            char[] separators = { '\\' };

            // Разбиваем строку на подстроки с использованием разделителей
            string[] parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string lastElement = parts[parts.Length - 1];
            return lastElement;

        }
        private void OpenVideoPlayer(string videoPath)
        {
            Window3 videoPlayerWindow = new Window3();
            videoPlayerWindow.LoadVideo(videoPath);
            videoPlayerWindow.Show();
        }
        private void file_list(string s)
        {
            string folderPath = s;

            try
            {
                // Проверяем, существует ли папка
                if (Directory.Exists(folderPath))
                {
                    // Получаем список файлов в папке
                    videoPaths = Directory.GetFiles(folderPath).Select(System.IO.Path.GetFullPath).ToList();
                    // Выводим имена файлов (можете использовать как угодно)
                }
                else
                {
                    Console.WriteLine("Указанная папка не существует.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        private void LoadScreen()
        {
            int videoIndex = 0;
            file_list(@"..\..\destruction");

            // Определение общего количества строк
            int totalRows = (int)Math.Ceiling((double)videoPaths.Count / videosPerRow);

            for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
            {
                StackPanel rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                // Определение количества видео в текущей строке
                int videosInCurrentRow = Math.Min(videosPerRow, videoPaths.Count - videoIndex);

                for (int colIndex = 0; colIndex < videosInCurrentRow; colIndex++)
                {
                    if (videoIndex < videoPaths.Count)
                    {
                        string path = videoPaths[videoIndex];
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        bitmap.EndInit();
                        Image i = new Image() { 
                            Source = bitmap,
                            Width = widths * modifire,
                            Height = heights * modifire,
                            Margin = new Thickness(10),
                            Stretch= Stretch.Uniform
                        };
                        TextBlock videoTitle = new TextBlock
                        {
                            Text = separete_video(path),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Width = widths * modifire,
                            Height = heights * modifire,
                        };

                        StackPanel videoPanel = new StackPanel();
                        videoPanel.Children.Add(i);
                        videoPanel.Children.Add(videoTitle);


                        rowPanel.Children.Add(videoPanel);
                        i.MouseLeftButtonDown += (s, e) =>
                        {
                            Process.Start(path);
                        };
                        videoIndex++;
                    }
                }
                videosContainer.Children.Add(rowPanel);
            }
        }
    }
}
