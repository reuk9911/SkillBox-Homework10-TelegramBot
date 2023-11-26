using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot;
using System.Net;
using System.Net.Http;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using Telegram.Bot.Args;

namespace Homework10
{
    public class MyBot
    {

        private string Token;
        public ObservableCollection<TelegramUser> Users;
        private TelegramBotClient Bot;
        public Weather Forecast { get; set; }

        /// <summary>
        /// конструктор
        /// </summary>
        public MyBot()
        {
            Token = "2129445094:AAEGP9HqkpXcvIRV5HY5Fy_Fbh508bKI-rk"; //reukBot
            Users = new ObservableCollection<TelegramUser>();
            Bot = new TelegramBotClient(Token);
            Bot.StartReceiving();
            Forecast = new Weather();
            if (File.Exists("MessageLog.txt")) { DeserializeJson("MessageLog.txt", ref Users); }

        }
        public void Start()
        {
            Bot.OnMessage += MessageListener;
            //Bot.StartReceiving();
        }

        private async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string msg = $"{DateTime.Now}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            Debug.WriteLine(msg);
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    {
                        var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);
                        if (!Users.Contains(person))
                            Users.Add(person);
                        Users[Users.IndexOf(person)].AddMessage
                             (DateTime.Now.ToString(), person.UserId, person.Nick, e.Message.Text);

                        // обработчик команды /start
                        if (e.Message.Text.ToLower() == "/start") { Command_Start(e); }

                        // обработчик команды /filelist
                        if (e.Message.Text.ToLower() == "/filelist") { Command_FileList(e, person); }

                        // обработчик команды /download
                        // загружает выбранный файл по номеру из /filelist
                        if (e.Message.Text.ToLower().Split(' ')[0] == "/download") { await Command_Download(e, person); }

                        // обработчик команды /weather
                        if (e.Message.Text.ToLower().Split(' ')[0] == "/weather")
                        {
                            await Command_Weather(e);
                        }

                        break;
                    }
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    {
                        var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);
                        if (!Users.Contains(person)) Users.Add(person);

                        DownloadFile(e.Message.Document.FileId, e.Message.Document.FileName);
                        BotSendMsg(e.Message.Chat.Id, "Получен новый документ");
                        Users[Users.IndexOf(person)].AddSentFile(e.Message.Document.FileName, e.Message.Type.ToString());
                        break;
                    }
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    {
                        var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);
                        if (!Users.Contains(person)) Users.Add(person);


                        DownloadFile(e.Message.Photo[e.Message.Photo.Length - 1].FileId,
                            $"{e.Message.Photo[e.Message.Photo.Length - 1].FileId}.jpg");
                        BotSendMsg(e.Message.Chat.Id, "Получено новое изображение");
                        Users[Users.IndexOf(person)].AddSentFile(e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Type.ToString());
                        break;
                    }
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    {
                        var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);
                        if (!Users.Contains(person)) Users.Add(person);

                        DownloadFile(e.Message.Audio.FileId, $"{e.Message.Audio.FileName}");
                        BotSendMsg(e.Message.Chat.Id, "Получено новое аудио");
                        Users[Users.IndexOf(person)].AddSentFile(e.Message.Audio.FileName, e.Message.Type.ToString());
                        break;
                    }
            }
        }

        private async Task Command_Weather(MessageEventArgs e)
        {
            string[] words = e.Message.Text.ToLower().Split(' ');
            if (words.Length < 2)
            {
                BotSendMsg(e.Message.Chat.Id, "Введите /weather [город]");
            }
            else
            {
                if (Forecast.GetWeather(words[1]) == true)
                {
                BotSendMsg(e.Message.Chat.Id,
                    $" \n\nТемпература в {Forecast.nameOfCity}: {Math.Round(Forecast.tempOfCity)} °C" +
                        $" \n\n Ветер {Forecast.windSpeed} м/с,  направление {Forecast.windDirection}");
                }
                else BotSendMsg(e.Message.Chat.Id, "Город не найден");
            }
        }

        private async Task Command_Download(MessageEventArgs e, TelegramUser person)
        {
            bool resultRead;
            int index;
            if (e.Message.Text.ToLower().Split(' ').Length < 2)
                BotSendMsg(e.Message.Chat.Id, "Введите /download [номер файла]");
            else
            {
                resultRead = Int32.TryParse(e.Message.Text.ToLower().Split(' ')[1], out index);

                if (!resultRead ||
                     index > Users[Users.IndexOf(person)].Files.Count() ||
                     index < 0)
                    BotSendMsg(e.Message.Chat.Id, "Ошибка ввода");
                else
                {
                    switch (Users[Users.IndexOf(person)].Files[index].FileType)
                    {
                        case "Photo":
                            await Bot.SendPhotoAsync(
                                e.Message.Chat.Id, Users[Users.IndexOf(person)].Files[index].FileName);
                            break;
                        case "Document":
                            using (var stream = File.OpenRead(Users[Users.IndexOf(person)].Files[index].FileName))
                            {
                                InputOnlineFile iof = new InputOnlineFile(stream);
                                iof.FileName = Users[Users.IndexOf(person)].Files[index].FileName;
                                await Bot.SendDocumentAsync(e.Message.Chat.Id, iof, "Сообщение");
                            }
                            break;
                        case "Audio":
                            using (var stream = File.OpenRead(Users[Users.IndexOf(person)].Files[index].FileName))
                            {
                                InputOnlineFile iof = new InputOnlineFile(stream);
                                iof.FileName = Users[Users.IndexOf(person)].Files[index].FileName;
                                await Bot.SendAudioAsync(e.Message.Chat.Id, iof, "Сообщение");
                            }
                            break;
                        default:
                            BotSendMsg(e.Message.Chat.Id, "Неизвестный тип файла");
                            break;
                    }
                }
            }
        }

        private void Command_FileList(MessageEventArgs e, TelegramUser person)
        {
            if (Users[Users.IndexOf(person)].Files.Count == 0)
                BotSendMsg(e.Message.Chat.Id, "Файлов для загрузки нет");
            else
            {
                string answer = "";
                for (int i = 0; i < Users[Users.IndexOf(person)].Files.Count; i++)
                {
                    answer += i + ". " + Users[Users.IndexOf(person)].Files[i].FileName + "\n";
                }
                BotSendMsg(e.Message.Chat.Id, answer);
            }
        }

        /// <summary>
        /// Обработчик комманды /start
        /// </summary>
        /// <param name="e"></param>
        private void Command_Start(MessageEventArgs e)
        {
            string responseText = $"Здравствуйте, {e.Message.Chat.FirstName}\n" +
                                            $"Бот позвооляет просматривать погоду, сохраняет фото, документы и аудиофайлы\n\n" +
                                            $"Доступные команды:\n\n" +
                                            $"/weather - показать прогноз погоды в выбранном городе\n\n" +
                                            $"/filelist - список файлов, которые можно загрузить\n\n" +
                                            $"/download - загрузить выбранный файл";
            BotSendMsg(e.Message.Chat.Id, responseText);
        }

        /// <summary>
        /// Отправка сообщения поддержкой
        /// </summary>
        public void SupportSendMsg(TelegramUser User, string Msg)
        {
            var concreteUser = Users[Users.IndexOf(User)];
            string responseMsg = Msg;

            concreteUser.AddMessage(DateTime.Now.ToString(), (long)Bot.BotId, "reukBot", responseMsg);

            Bot.SendTextMessageAsync(concreteUser.UserId, Msg);
            
        }
        /// <summary>
        /// Отправка сообщения ботом
        /// </summary>
        public void BotSendMsg(long ChatId, string Msg)
        {
            var selection = from i in Users
                            where i.UserId == ChatId
                            select i;

            var concreteUser = selection.ToArray()[0];
            concreteUser.AddMessage(DateTime.Now.ToString(), ChatId, "reukBot", Msg);

            Bot.SendTextMessageAsync(concreteUser.UserId, Msg);
        }

        /// <summary>
        /// Загрузка файла
        /// </summary>
        /// <param name="fileId">UserId файла</param>
        /// <param name="path">Путь для загрузки</param>
        private async void DownloadFile(string fileId, string path)
        {
            var file = await Bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await Bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// десериализация
        /// </summary>
        /// <param name="path">путь к файлу Json</param>
        /// <param name="User">переменная с информацией о пользователях и их файлах</param>
        /// <returns>true, если десериализация прошла успешно, false иначе</returns>
        public bool DeserializeJson(string path, ref ObservableCollection<TelegramUser> telegramUsers)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            string json = File.ReadAllText(path);
            telegramUsers = JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(json);

            return true;

        }

        /// <summary>
        /// сериализация 
        /// </summary>
        /// <param name="path">путь к файлу Json</param>
        /// <param name="chatFiles">переменная с информацией о пользователях и их файлах</param>
        public void SerializeJson(string path)
        {
            string json = JsonConvert.SerializeObject(Users);
            File.WriteAllText(path, json);
        }



    }
}
