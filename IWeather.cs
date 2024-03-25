using System.Collections.Generic;

namespace Telegram_Bot
{
    internal interface IWeather
    {
        string[] GetTemperature();
        string[] GetWeather();
        string[] GetPressure();
        string[] GetWind();
        string[] GetHumidity();
        string[] GetFallout();
        List<string> ReturnData();
    }
}
