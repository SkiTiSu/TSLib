using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Net
{
    public partial class Weather
    {
        public class BDWeather
        {
            public int error;
            public string status;
            public string date;
            public BDResults[] results;
        }

        public class BDResults
        {
            public string currentCity;
            public int pm25;
            public BDIndex[] index;
            public BDWeatherData[] weather_data;
        }

        public class BDIndex
        {
            public string title;
            public string zs;
            public string tipt;
            public string des;
        }

        public class BDWeatherData
        {
            public string date;
            public string dayPictureUrl;
            public string nightPictureUrl;
            public string weather;
            public string wind;
            public string temperature;
        }

        /// <summary>
        /// AQI数据结构
        /// </summary>
        public class AQIData
        {
            public Boolean OK;
            public string City = "";
            public string Level = "";
            public string UpdateTime = "";
            public int AQI;
            public int PM25;
            public int PM10;
            public float CO;
            public int NO2;
            public int O3;
            public int O3p8h;
            public int SO2;
        }
    }

}
