using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private System.Windows.Threading.DispatcherTimer positionTimer;
        private bool isDraggingSlider = false;
        public Window3()
        {
            InitializeComponent();
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
                mediaPlayer.Source = new Uri(videoPath);
                mediaPlayer.Play();
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
