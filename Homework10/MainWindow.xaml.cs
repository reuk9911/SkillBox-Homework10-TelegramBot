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
//using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Homework10
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary
    public partial class MainWindow : Window
    {

        ObservableCollection<TelegramUser> Users;
        TelegramBotClient bot;

        public MainWindow()
        {
            InitializeComponent();
            Users = new ObservableCollection<TelegramUser>();

            userList.ItemsSource = Users; //установка источника данных

            string token = "2129445094:AAEGP9HqkpXcvIRV5HY5Fy_Fbh508bKI-rk";
            bot = new TelegramBotClient(token);

            //указываем допустимые протоколы
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | 
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | 
                SecurityProtocolType.Tls12;
            
            
            var me = bot.GetMeAsync().Result;
            Debug.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");

            bot.OnMessage += delegate (object sender, Telegram.Bot.Args.MessageEventArgs e)
            {
                string msg = $"{DateTime.Now}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
                File.AppendAllText("data.log", $"{msg}\n");
                Debug.WriteLine(msg);

                this.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);
                    if (!Users.Contains(person)) Users.Add(person);
                    Users[Users.IndexOf(person)].AddMessage($"{person.Nick}: {e.Message.Text}");
                });
            };
            bot.StartReceiving();
            btnSendMsg.Click += delegate {SendMsg(); };
            txtBxSendMsg.KeyDown += (s, e) => { if (e.Key == Key.Return) { SendMsg(); } };
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        public void SendMsg()
        {
            var concreteUser = Users[Users.IndexOf(userList.SelectedItem as TelegramUser)];
            string responseMsg = $"Support: {txtBxSendMsg.Text}";
            concreteUser.Messages.Add(responseMsg);
            
            bot.SendTextMessageAsync(concreteUser.Id, txtBxSendMsg.Text);
            string logText = $"{DateTime.Now}: >> {concreteUser.Nick} {concreteUser.Id} {responseMsg}";
            File.AppendAllText("data.log", logText);
            txtBxSendMsg.Text = String.Empty;
        }
    }
}
