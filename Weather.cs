using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Telegram_Bot.Weather;

namespace Telegram_Bot
{
    internal class Weather
    {
       public enum WeatherType
        {
            Now,
            Today,
            Tomorrow,
            Week
        }

        private IWeather weather;
        private Parser parser;

        public Weather()
        {
            
        }

        public async Task GetWeather(WeatherType weatherType)
        {
            parser = new Parser();
            await parser.Initialize(weatherType);

            if (weatherType == WeatherType.Now)
                weather = new WeatherNow(parser);

            else if (weatherType == WeatherType.Today)
                weather = new WeatherForTodayOrTomorrow(parser,WeatherType.Today);

            else if (weatherType == WeatherType.Tomorrow)
                weather = new WeatherForTodayOrTomorrow(parser, WeatherType.Tomorrow);

            else if (weatherType == WeatherType.Week)
                weather = new WeatherForWeek(parser);
        }

        public List<string> GetResult()
        {
            var data = weather.ReturnData();

            return  data;
        }
    }
}
