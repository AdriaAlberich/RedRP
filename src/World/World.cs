/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using GTANetworkAPI;

namespace redrp
{
    /// <summary>
    /// World time and dynamic weather systems
    /// </summary>
    public class World : Script
    {
        #pragma warning disable 649

        /// <summary>
        /// World hour
        /// </summary>
        public static int hour = 0;

        /// <summary>
        /// World minute
        /// </summary>
        public static int minute = 0;

        /// <summary>
        /// World second
        /// </summary>
        public static int second = 0;

        /// <summary>
        /// World temperature in celsius
        /// </summary>
        public static float temperature = 20f;

        /// <summary>
        /// World humidity
        /// </summary>
        public static int humidity = 50;

        /// <summary>
        /// World barometric pressure
        /// </summary>
        public static int pressure = 1014;

        /// <summary>
        /// Current active weather type
        /// </summary>
        public static int currentWeather;

        /// <summary>
        /// This disables the dynamic weather system if set to true
        /// </summary>
        public static bool overrideWeather = false;

        /// <summary>
        /// List of available ingame weathers
        /// </summary>
        public enum WeatherType
        {
            Extra_Sunny,
            Clear,
            Clouds,
            Smog,
            Foggy,
            Overcast,
            Rain,
            Thunder,
            Light_Rain,
            Smoggy_Light_Rain, //DO NOT USE (cause weird effects in some day hours in the northern part of the map).
            Very_Light_Snow,
            Windy_Light_Snow,
            Light_Snow
        }

        /// <summary>
        /// OpenWeatherMap API call
        /// </summary>
        public const string ApiCall = "http://api.openweathermap.org/data/2.5/weather?q=Los%20Angeles,us&units=metric&lang=es&APPID=307dd383d3d50b107f97e1cedffcf522";

        /// <summary>
        /// Helper classes
        /// </summary>
        private class RootObject
        {
            public Coord coord;
            public List<Weather> weather;
            public string bases;
            public Main main;
            public string visibility;
            public Wind wind;
            public Clouds clouds;
            public string dt;
            public Sys sys;
            public string id;
            public string name;
            public string cod;
        }

        private class Coord
        {
            public string lon;
            public string lat;
        }

        private class Weather
        {
            public string id;
            public string main;
            public string description;
            public string icon;
        }

        private class Main
        {
            public string temp;
            public string pressure;
            public string humidity;
            public string temp_min;
            public string temp_max;
        }

        private class Wind
        {
            public string speed;
            public string deg;
            public string gust;
        }

        private class Clouds
        {
            public string all;
        }

        private class Sys
        {
            public string type;
            public string id;
            public string message;
            public string country;
            public string sunrise;
            public string sunset;
        }

        /// <summary>
        /// Initializes world time with the current server time
        /// </summary>
        public static void InitTime()
        {
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;
            second = DateTime.Now.Second;
        }

        /// <summary>
        /// Calculates ingame time (x2 real time)
        /// </summary>
        public static void SyncTime()
        {
            second++;
            if(second > 59)
            {
                second = 0;
                minute++;
                if(minute > 59)
                {
                    minute = 0;
                    hour++;
                    if(hour > 23)
                    {
                        hour = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Syncronizes the real weather with the ingame weather
        /// </summary>
        public static void SyncWeather()
        {
            if (!overrideWeather)
            {
                UpdateWeatherData();

                ProcessCurrentWeather();

                NAPI.World.SetWeather(currentWeather.ToString());
            }
        }

        /// <summary>
        /// Calls the API and updates weather data
        /// </summary>
        public static void UpdateWeatherData()
        {
            try
            {
                string response = new WebClient().DownloadString(World.ApiCall);
                dynamic result = NAPI.Util.FromJson(response);

                RootObject obj = result.ToObject<RootObject>();

                temperature = float.Parse(obj.main.temp);
                humidity = int.Parse(obj.main.humidity);
                pressure = int.Parse(obj.main.pressure);

            }
            catch (Exception e)
            {
                Log.Debug("[CLIMA] No se han podido obtener los datos climaticos.");
                Log.Debug(e.Message);
            }
            
        }

        /// <summary>
        /// Selects the ingame weather depending on the weather data
        /// </summary>
        public static void ProcessCurrentWeather()
        {
            int selectedWeather = (int)WeatherType.Extra_Sunny;
            //Hight pressure (good weather)
            if (pressure > 1014)
            {
                if (pressure > 1020)
                {
                    if (humidity > 60)
                    {
                        if (temperature > 30)
                        {
                            selectedWeather = (int)WeatherType.Smog;
                        }
                        else
                        {
                            if (temperature < 20)
                            {
                                selectedWeather = (int)WeatherType.Foggy;
                            }
                            else
                            {
                                selectedWeather = (int)WeatherType.Clear;
                            }
                        }
                    }
                    else
                    {
                        selectedWeather = (int)WeatherType.Extra_Sunny;
                    }
                }
                else
                {
                    if (humidity > 60)
                    {
                        if (temperature > 30)
                        {
                            selectedWeather = (int)WeatherType.Smog;
                        }
                        else
                        {
                            if (temperature < 20)
                            {
                                selectedWeather = (int)WeatherType.Clouds;
                            }
                            else
                            {
                                selectedWeather = (int)WeatherType.Clear;
                            }
                        }
                    }
                    else
                    {
                        selectedWeather = (int)WeatherType.Extra_Sunny;
                    }
                }
            }
            else
            {
                if (pressure < 1000)
                {
                    if (humidity > 90)
                    {
                        if (temperature < 5)
                        {
                            Random rand = new Random();
                            int randomWeather = rand.Next(3);
                            switch (randomWeather)
                            {
                                case 0: selectedWeather = (int)WeatherType.Light_Snow; break;
                                case 1: selectedWeather = (int)WeatherType.Windy_Light_Snow; break;
                                case 2: selectedWeather = (int)WeatherType.Very_Light_Snow; break;
                            }

                        }
                        else
                        {
                            selectedWeather = (int)WeatherType.Thunder;
                        }
                    }
                    else
                    {
                        if (humidity < 50)
                        {
                            selectedWeather = (int)WeatherType.Clouds;
                        }
                        else
                        {
                            selectedWeather = (int)WeatherType.Overcast;
                        }

                    }
                }
                else
                {
                    if (humidity > 90)
                    {
                        if (temperature < 5)
                        {
                            selectedWeather = (int)WeatherType.Very_Light_Snow;
                        }
                        else
                        {
                            Random rand = new Random();
                            int randomWeather = rand.Next(2);
                            switch (randomWeather)
                            {
                                case 0: selectedWeather = (int)WeatherType.Rain; break;
                                case 1: selectedWeather = (int)WeatherType.Light_Rain; break;
                            }
                        }
                    }
                    else
                    {
                        if (humidity < 50)
                        {
                            selectedWeather = (int)WeatherType.Clouds;
                        }
                        else
                        {
                            selectedWeather = (int)WeatherType.Overcast;
                        }

                    }
                }
            }

            currentWeather = selectedWeather;
        }

    }

}
