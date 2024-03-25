using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using static Telegram_Bot.Weather;

namespace Telegram_Bot
{
    internal class Parser
    {
        private string _url;
        private HttpClient _httpClient;
        private string _html;
        private HtmlDocument _htmlDocument;
        public HtmlDocument HtmlDocument { get { return _htmlDocument;}}

        public async Task Initialize(WeatherType weatherType)
        {
            if (weatherType == WeatherType.Now)
                _url = "https://www.gismeteo.ru/weather-yekaterinburg-4517/now/";
            else if (weatherType == WeatherType.Today)
                _url = "https://www.gismeteo.ru/weather-yekaterinburg-4517/";
            else if (weatherType == WeatherType.Tomorrow)
                _url = "https://www.gismeteo.ru/weather-yekaterinburg-4517/tomorrow/";
            else
                _url = "https://www.gismeteo.ru/weather-yekaterinburg-4517/weekly/";

            _httpClient = new HttpClient();
            _html = await _httpClient.GetStringAsync(_url);
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(_html);
        }
    }
}
