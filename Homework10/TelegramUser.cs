using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Homework10
{
    public class TelegramUser: INotifyPropertyChanged, IEquatable<TelegramUser>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Nickname">Имя пользователя</param>
        /// <param name="ChatId">UserId чата</param>
        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.userid = ChatId;
            MessageLog = new ObservableCollection<Message>();
            Files = new ObservableCollection<SentFile>();
        }

        private string nick;

        public string Nick
        {
            get { return this.nick; }
            set
            {
                this.nick = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.nick)));
            }
        }

        private long userid;
        public long UserId
        {
            get { return this.userid; }
            set { this.userid = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.userid))); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // оповещение внешних агентов
        public bool Equals(TelegramUser other) => other.UserId == this.userid;

        /// <summary>
        /// Все сообщения
        /// </summary>
        public ObservableCollection<Message> MessageLog { get; set; }

        public void AddMessage(string Time, long Id, string FirstName, string Msg) 
        {
            Message msg = new Message(Time, Id, FirstName, Msg);
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                MessageLog.Add(msg);
            });
             
        }

        /// <summary>
        /// Коллекция присланных файлов
        /// </summary>
        public ObservableCollection<SentFile> Files { get; set; }
        public void AddSentFile(string FileName, string FileType)
        {
            SentFile f = new SentFile(FileName, FileType);
            Files.Add(f);
        }
    }
}
