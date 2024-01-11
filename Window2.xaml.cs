using System;
using System.Collections.Generic;
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
        private List<string> videoPaths = new List<string>
        {
            "path/to/video1.mp4",
            "path/to/video2.mp4",
            "path/to/video3.mp4",
            "path/to/video4.mp4"
        };
        public Window2()
        {
            InitializeComponent();
            LoadVideos();
        }
        private int videosPerRow = 2; // Задайте количество видео в каждой строке (например, 2)

        private void LoadVideos()
        {
            int videoIndex = 0;

            for (int rowIndex = 0; rowIndex < videoPaths.Count / videosPerRow; rowIndex++)
            {
                StackPanel rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                for (int colIndex = 0; colIndex < videosPerRow; colIndex++)
                {
                    if (videoIndex < videoPaths.Count)
                    {
                        MediaElement mediaElement = new MediaElement
                        {
                            Source = new Uri(videoPaths[videoIndex]),
                            Stretch = System.Windows.Media.Stretch.UniformToFill
                        };
                        mediaElement.LoadedBehavior = MediaState.Manual;
                        mediaElement.MediaOpened += (sender, e) => mediaElement.Play();

                        StackPanel videoPanel = new StackPanel();
                        videoPanel.Children.Add(mediaElement);

                        rowPanel.Children.Add(videoPanel);
                        videoIndex++;
                    }
                }

                videosContainer.Children.Add(rowPanel);
            }
        }
    }
}
