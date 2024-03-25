using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Telegram_Bot
{
    internal class WeatherForTodayOrTomorrow : IWeather
    {
        private Parser parser;
        private Weather.WeatherType weatherType;

        public WeatherForTodayOrTomorrow(Parser parser, Weather.WeatherType weatherType)
        {
                this.parser = parser;
                this.weatherType = weatherType;
        }

        public string[] GetFallout()
        {
            var fallout = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//div[@class='precipitation']");

            if (weatherType == Weather.WeatherType.Today) 
                return new[] { fallout[0].InnerText.Trim() };
            else
            {
                if (fallout.Count == 1) 
                    return new[] { "0 мм" };
                else 
                    return new[] { fallout[1].InnerText };
            }
        }

        public string[] GetHumidity()
        {
            //К сожалению, не имею понятия как спарсить влажность.
            //HTML кода нужный просто не парсится --_("_*)_--
            throw new NotImplementedException();
            
        }

        public string[] GetPressure()
        {
            var pressureTemp = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//span[@class='unit unit_pressure_mm_hg']");

            var pressure = pressureTemp.Skip(1).Select(x => int.Parse(x.InnerText) );
            return new[] { (pressure.Sum() / pressure.Count() ).ToString() };
        }

        public string[] GetTemperature()
        {
            var temperature = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//span[@class='unit unit_temperature_c']");

            return new[] { temperature[2].InnerText.Trim(), temperature[3].InnerText.Trim() };
        }

        public string[] GetWeather()
        {
            var weather = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//div[@class='weathertab weathertab-block tooltip']")[0];

            return new[] { weather.GetAttributeValue("data-text", string.Empty) };
        }

        public string[] GetWind()
        {
            var windTemp = parser.HtmlDocument.DocumentNode.SelectNodes("" +
                "//span[@class='wind-unit unit unit_wind_m_s']");
            var windSpeed  = windTemp
                .Select(x => double.Parse(x.InnerText))
                .Take(8);

            var wind = windSpeed.Sum()/ windSpeed.Count();
            return new[] { wind.ToString() };

        }

        public string[] GetDate()
        {
            string date = String.Empty;
            string dayOfWeek = String.Empty;
            var now = DateTime.Now;

            if(weatherType == Weather.WeatherType.Today)
            {
                date = now.ToShortDateString();
                dayOfWeek = CultureInfo.GetCultureInfo("ru-RU")
                    .DateTimeFormat.GetDayName(now.DayOfWeek);
            }
            else if(weatherType == Weather.WeatherType.Tomorrow){
                date = now.AddDays(1).ToShortDateString();
                dayOfWeek = CultureInfo.GetCultureInfo("ru-RU")
                    .DateTimeFormat.GetDayName(now.AddDays(1).DayOfWeek);
            }

            return new[] { date + " " + dayOfWeek };
        }

        public string[] GetWindDirection()
        {
            var windDirectionTemp = parser.HtmlDocument.DocumentNode.SelectNodes("//div[@class='direction']")
                .ToList();

            var directionFrequency = windDirectionTemp
                .Select(x => x.InnerText.Trim())
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var direction = directionFrequency
                .FirstOrDefault(x => x.Value == directionFrequency.Values.Max()).Key;
            return new[] { direction.ToString() };
        }

        public string[] GetSunriseAndSunsetTime()
        {
            var sunTime = parser.HtmlDocument.DocumentNode.SelectSingleNode(
                "//div[@class='astro-times']");
            var regex = new Regex(
                @"(Долгота дня: \d+ ч \d+ мин)(Восход — \d+:\d+)(Заход — \d+:\d+)");
            var result =  regex
                .Split(sunTime.InnerText)
                .Where(x => x != String.Empty);

            return result.ToArray();
        }

        public List<string> ReturnData()
        {
            string todayOrTomorrow = string.Empty ;
            if (weatherType == Weather.WeatherType.Today) 
                todayOrTomorrow = "Сегодня: ";

            else if (weatherType == Weather.WeatherType.Tomorrow) 
                todayOrTomorrow = "Завтра: ";

            var date = GetDate()[0];
            var weather = GetWeather()[0];
            var temperatureTemp = GetTemperature();
            var tempertureLowest = temperatureTemp[0].Replace("&minus;", "-");
            var temperatureHighest = temperatureTemp[1].Replace("&minus;", "-");
            var fallout = GetFallout()[0];
            var wind = GetWind()[0];
            var windDirection = GetWindDirection()[0];
            var pressure = GetPressure()[0];
            var sunTime = GetSunriseAndSunsetTime();

            return new List<string>
            {
                $"{todayOrTomorrow} " +
                "\n<i>" + date + "</i>" +
                "\n\n<i>" + weather + "</i>" +
                "\n<b>Температура:</b>" + " от " + tempertureLowest + "℃ до " + temperatureHighest + " ℃"  +
                "\n<b>Осадки:</b> " + fallout +
                "\n<b>Ветер:</b> " + wind + " м/с " + windDirection + 
                "\n<b>Давление:</b> " + pressure + " мм рт.ст" +
                $"\n\n<b>{sunTime[0]}</b>" + $"\n<b>{sunTime[1]}</b>" + $"\n<b>{sunTime[2]}</b>"
            };
        }
    }
}
