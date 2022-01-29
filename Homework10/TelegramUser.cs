using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Homework10
{
    public class TelegramUser: INotifyPropertyChanged, IEquatable<TelegramUser>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Nickname">Имя пользователя</param>
        /// <param name="ChatId">Id чата</param>
        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.id = ChatId;
            Messages = new ObservableCollection<string>();
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

        private long id;
        public long Id
        {
            get { return this.id; }
            set { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.id))); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // оповещение внешних агентов
        public bool Equals(TelegramUser other) => other.Id == this.id;

        /// <summary>
        /// Все сообщения
        /// </summary>
        public ObservableCollection<string> Messages { get; set; }

        public void AddMessage(string Text) => Messages.Add(Text);
    }
}
