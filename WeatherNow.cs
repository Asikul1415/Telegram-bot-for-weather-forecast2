using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Telegram_Bot
{
    internal class WeatherNow : IWeather
    {
        private Parser parser;

        public WeatherNow(Parser parser)
        {
            this.parser = parser;
        }

        public string[] GetFallout()
        {
            throw new NotImplementedException();
        }

        public string[] GetHumidity()
        {
            var humidityTemp = parser.HtmlDocument.DocumentNode.SelectSingleNode(
                "//div[@class='now-info-item humidity']");

            var regex = new Regex(@"(\d+)");
            var humidity = regex.Split(humidityTemp.InnerText.Trim());

            return new[] { humidity[1] + "%" };
        }

        public string[] GetPressure()
        {
            var temp = parser.HtmlDocument.DocumentNode.SelectSingleNode(
                "//div[@class='unit unit_pressure_mm_hg']");

            var regex = new Regex(@"(\d+)(мм)(рт. ст.)");
            var pressure = regex.Split(temp.InnerText.Trim());

            var temp2 = "";
            foreach (var item in pressure)
            {
                temp2 += $"{item} ";
            }
            return new[] { temp2 };
        }

        public string[] GetTemperature()
        {
            var temperature = parser.HtmlDocument.DocumentNode.SelectNodes(
                "//span[@class='unit unit_temperature_c']");

            return new[] { 
                temperature[0].InnerText.Trim()
                .Replace("&minus;", "-"), 
                temperature[1].InnerText.Trim()
                .Replace("&minus;", "-") };
        }   

        public string[] GetWeather()
        {
            var weather = parser.HtmlDocument.DocumentNode.SelectSingleNode(
                "//div[@class='weathertab weathertab-block tooltip']");

            return new[] {weather.GetAttributeValue("data-text",string.Empty)}; 
        }

        public string[] GetWind()
        {
            var windTemp = parser.HtmlDocument.DocumentNode.SelectSingleNode(
                "//div[@class='unit unit_wind_m_s'] ");

            Regex regex = new Regex(@"(\d+)(м/c)(\D+)");
            var wind = regex.Split(windTemp.InnerText.Trim());

            var temp2 = "";
            foreach (var item in wind)
            {
                temp2 += $"{item} ";
            }
            return new[] {temp2};
        }

        public List<string> ReturnData()
        {
            string[] temperature_temp = GetTemperature();
            string temperature = temperature_temp[0];
            string temperatureByFeelings = temperature_temp[1];
            string weather = GetWeather()[0];
            string wind = GetWind()[0];
            string pressure = GetPressure()[0];
            string humidity = GetHumidity()[0];


            List<string> data = new List<string>()
            
            {
                "<b>Температура:</b> " + temperature + "℃" +
                "\n<b>По ощущениям:</b> " + temperatureByFeelings + "℃" +
                "\n<b>Погода:</b> " + weather +
                "\n<b>Ветер:</b>"+ wind +
                "\n<b>Давление:</b>" + pressure +
                "\n<b>Влажность:</b> " + humidity
            };
            return data;

        }
    }
}
