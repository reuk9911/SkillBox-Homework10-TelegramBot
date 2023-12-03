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
using System.Collections.ObjectModel;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot;
using System.Net;
using System.Net.Http;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Homework10
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary
    public partial class MainWindow : Window
    {

        MyBot Bot = new MyBot();

        public MainWindow()
        {
            InitializeComponent();
            this.Dispatcher.Invoke(() =>
            {
                Bot.Start();
                
            });
            userList.ItemsSource = Bot.Users; //установка источника данных

            //указываем допустимые протоколы
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            btnSendMsg.Click += delegate 
            {
                Bot.SupportSendMsg(userList.SelectedItem as TelegramUser, txtBxSendMsg.Text);
                txtBxSendMsg.Text = String.Empty;
            };

            txtBxSendMsg.KeyDown += (s, e) => 
            {
                if (e.Key == Key.Return) 
                {
                    Bot.SupportSendMsg(userList.SelectedItem as TelegramUser, txtBxSendMsg.Text);
                    txtBxSendMsg.Text = String.Empty;
                }
            };
        }

        

        private void Window_Closed(object sender, EventArgs e)
        {
            Bot.SerializeJson("MessageLog.txt");
        }
    }


}
