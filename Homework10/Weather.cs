using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Homework10
{
    public class Weather
    {
        private TemperatureInfo TempInfo;
        private WeatherResponse WResponse;
        private WindInfo Wind;

        public string nameOfCity { get; set; }
        public float tempOfCity { get; set; }
        public string windDirection { get; set; }
        public float windSpeed { get; set; }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CityName">Город</param>
        public Weather(/*string CityName*/)
        {
            TempInfo = new TemperatureInfo();
            WResponse = new WeatherResponse();
            Wind = new WindInfo();

            //GetWeather(CityName);
        }

        /// <summary>
        /// Запрашивает погоду
        /// </summary>
        /// <param name="CityName">Город</param>
        public bool GetWeather(string CityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + CityName +
                "&unit=metric&appid=d91ecce171aaba7bcdfc082ce33edd58";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
                windDirection = WindDegToDirection(weatherResponse.Wind.Deg);
                windSpeed = weatherResponse.Wind.Speed;
                return true;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                return false;
            }
        }

        /// <summary>
        /// находит направление ветра по градусам
        /// </summary>
        /// <param name="Degree">градус</param>
        /// <returns>возвращает направление ветра</returns>
        private string WindDegToDirection(float Degree)
        {
            if (Degree > 22.5 && Degree <= 67.5) return "СВ";
            else if (Degree > 67.5 && Degree <= 112.5) return "В";
            else if (Degree > 112.5 && Degree <= 157.5) return "ЮВ";
            else if (Degree > 157.5 && Degree <= 202.5) return "Ю";
            else if (Degree > 202.5 && Degree <= 247.5) return "ЮЗ";
            else if (Degree > 247.5 && Degree <= 292.5) return "З";
            else if (Degree > 292.5 && Degree <= 337.5) return "СЗ";
            else return "С";
        }
    }
}
