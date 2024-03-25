using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Telegram_Bot
{
    internal class WeatherForWeek : IWeather
    {
        private Parser parser;
        
        public WeatherForWeek(Parser parser)
        {
            this.parser = parser;
        }

        public string[] GetFallout()
        {
            var falloutTemp = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//div[@class='item-unit unit-blue'] | //div[@class='item-unit']");

            var fallout = falloutTemp.Select(x => x.InnerText + " мм");
            return fallout.ToArray();
        }

        public string[] GetHumidity()
        {
            throw new NotImplementedException();
        }

        public string[] GetPressure()
        {
            var pressureTemp = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//span[@class='unit unit_pressure_mm_hg']");
            pressureTemp.RemoveAt(0);

            var pressure = new List<string>();
            for (int i = 0; i + 1< pressureTemp.Count; i+= 2)
            {
                pressure.Add(
                    ((int.Parse(pressureTemp[i].InnerText) + int.Parse(pressureTemp[i + 1].InnerText)) / 2)
                    .ToString() + " мм.рт ст");
            }

            return pressure.ToArray();
        }

        public string[] GetTemperature()
        {
            var maximumTemperaturesTemp = parser.HtmlDocument.DocumentNode
                .SelectNodes("//div[@class='maxt']");

            var maximumTemperatures = maximumTemperaturesTemp
                .Select(x => x.SelectSingleNode(".//span[@class='unit unit_temperature_c']"))
                .Where(x => x != null)
                .Select(x => x.InnerText)
                .ToList();

            var minimumTemperaturesTemp = parser.HtmlDocument.DocumentNode
                .SelectNodes("//div[@class='mint']");

            var minimumTemperatures = minimumTemperaturesTemp
                .Select(x => x.SelectSingleNode(
                    ".//span[@class='unit unit_temperature_c']"))
                .Where(x => x != null)
                .Select(x => x.InnerText)
                .ToList();

            string[] results = new string[7];

            for (int i = 0; i < 7; i++)
            {
                results[i] = $"от {minimumTemperatures[i].Replace("&minus;","-")} ℃ до " +
                    $"{maximumTemperatures[i].Replace("&minus;", "-")} ℃";
            }

            return results;
        }

        public string[] GetWeather()
        {
            var weatherTemp = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//div[@class='weather-icon tooltip']");
            string[] weather = new string[7];

            for (int i = 0; i < 7; i++)
            {
                weather[i] = weatherTemp[i].GetAttributeValue("data-text", String.Empty);
            }
            return weather;
        }

        public string[] GetWind()
        {
            var windTemp = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//span[@class='wind-unit unit unit_wind_m_s']");
            var directions = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//div[@class='direction']");
            string[] wind = new string[7];

            for (int i = 0; i < 7; i++)
            {
                wind[i] = windTemp[i].InnerText + " м/с " + directions[i].InnerText;
            }

            return wind;
        }

        public string[] GetDays()
        {
            var now = DateTime.Now;
            var days = new string[7];  

            for (int i = 0; i < 7; i++)
            {
                days[i] = now.AddDays(i).ToShortDateString() + " "
                    + CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.
                    GetDayName(now.AddDays(i).DayOfWeek);
            }
            return days;
        }

        public List<string> ReturnData()
        {
            var dates = GetDays();
            var temperatures = GetTemperature();
            var weather = GetWeather();
            var winds = GetWind();
            var fallout = GetFallout();
            var pressure = GetPressure();

            List<string> results = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                results.Add(
                    $"{dates[i]} \n\n<i>{weather[i]}</i> " +
                    $"\n<b>Температура:</b> {temperatures[i]} " +
                    $"\n<b>Ветер: {winds[i]}</b>" +
                    $"\n<b>Осадки: {fallout[i]}</b>" +
                    $"\n<b>Давление: {pressure[i]} </b>"
                    );
            }  
            return results;
        }


    }
}
