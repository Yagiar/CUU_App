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

namespace ObjectDetected
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private int widths=16;
        private int heights=9;
        private int modifire=10;
        private List<string> videoPaths = new List<string>
        {
            
        };
        public Window2()
        {
            InitializeComponent();
            LoadVideos();
            this.SizeChanged += Window2_SizeChanged;
        }
        private int videosPerRow = 2; // Задайте количество видео в каждой строке (например, 2)
        
        private void file_list(string s)
        {
            string folderPath = s;

            try
            {
                // Проверяем, существует ли папка
                if (Directory.Exists(folderPath))
                {
                    // Получаем список файлов в папке
                    videoPaths = Directory.GetFiles(folderPath).ToList();
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
        private string separete_video(string s)
        {
            char[] separators = { '\\' };

            // Разбиваем строку на подстроки с использованием разделителей
            string[] parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string lastElement = parts[parts.Length - 1];
            return lastElement;

        }
        private void Window2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Обновляем максимальные размеры videosContainer при изменении размеров окна
            
            videosScrollViewer.MaxWidth = this.ActualWidth;
            videosScrollViewer.MaxHeight = this.ActualHeight;
        }
        private void LoadVideos()
        {
            int videoIndex = 0;
            file_list(@"C:\Users\shabu\source\repos\ObjectDetected\ObjectDetected\Senri");

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
                        MediaElement mediaElement = new MediaElement
                        {
                            Source = new Uri(path),
                            LoadedBehavior = MediaState.Manual,
                            Width = widths * modifire,
                            Height = heights * modifire,
                            Margin = new Thickness(10),
                            Stretch = System.Windows.Media.Stretch.UniformToFill
                        };
                        mediaElement.Play();
                        mediaElement.Volume = 0;
                        TextBlock videoTitle = new TextBlock
                        {
                            Text = separete_video(path),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Width = widths * modifire,
                            Height = heights * modifire,
                        };

                        StackPanel videoPanel = new StackPanel();
                        videoPanel.Children.Add(mediaElement);
                        videoPanel.Children.Add(videoTitle);
                       

                        rowPanel.Children.Add(videoPanel);
                        mediaElement.MouseLeftButtonDown += (s, e) =>
                        {
                            OpenVideoPlayer(path);
                        };
                        videoIndex++;
                    }
                }
                videosContainer.Children.Add(rowPanel);
            }
        }
        private void OpenVideoPlayer(string videoPath)
        {
            Window3 videoPlayerWindow = new Window3();
            videoPlayerWindow.LoadVideo(videoPath);
            videoPlayerWindow.Show();
        }
    }
}
