using System;
using System.Collections.Generic;
using System.Text;

namespace Homework10
{
    public class WeatherResponse
    {
        /// <summary>
        /// Информация о температуре воздуха
        /// </summary>
        public TemperatureInfo Main { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Информация о ветре
        /// </summary>
        public WindInfo Wind { get; set; }
    }
}
