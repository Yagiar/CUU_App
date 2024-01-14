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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObjectDetected
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Log_Click(object sender, RoutedEventArgs e)
        {
            // Здесь вы можете добавить логику проверки логина и пароля,
            // например, сравнение с данными в базе данных.

            MessageBox.Show("Авторизация прошла успешно!");
            Window2 window2 = new Window2();
            // Показываем окно Window1
            window2.Show();
            this.Close();
            
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // В этом месте вы можете обработать данные регистрации, например, отправив их на сервер.

            MessageBox.Show("Пользователь успешно зарегистрирован!");
            Window1 window1 = new Window1();
            // Показываем окно Window1
            window1.Show();

            this.Close();
        }
    }
}
