using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Homework10
{
    public struct Message
    {
        /// <summary>
        /// Время получения
        /// </summary>
        public string Time { get; set; }
        
        /// <summary>
        /// UserId приславшего пользователя
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// сообщение
        /// </summary>
        public string Msg { get; set; }
        
        /// <summary>
        /// Тип сообщения
        /// </summary>
        //public string MsgType { get; set; }
        
        /// <summary>
        /// Имя приславшего пользователя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="Time">Время получения</param>
        /// <param name="Id">UserId пользователя</param>
        /// <param name="FirstName">Имя пользователя</param>
        /// <param name="MsgType">Тип сообщения</param>
        /// <param name="Msg">Сообщение</param>
        public Message(string Time, long Id, string FirstName, string Msg)
        {
            this.Time = Time;
            this.Id = Id;
            this.FirstName = FirstName;
            //this.MsgType = MsgType;
            this.Msg = Msg;
        }
    }
}
