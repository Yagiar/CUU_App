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
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
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
                mediaPlayer.Source = new Uri(videoPath);
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке видео: {ex.Message}");
            }
        }
    }
}
