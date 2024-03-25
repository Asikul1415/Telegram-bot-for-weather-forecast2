using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading;
using static Telegram_Bot.Weather;
using static Telegram_Bot.Keyboard;
using static Telegram_Bot.Token;
namespace Telegram_Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient client = new TelegramBotClient(_token);
            client.StartReceiving(Update, Error);
            keyboard.ResizeKeyboard = true;

            Console.ReadLine();
        }

        private static Task Error(ITelegramBotClient botClient, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        async private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            

            if(message.Text != null)
            {
                Weather weather = new Weather();
                var text = message.Text.ToLower();

                if (text.Contains("погода сейчас"))
                {
                    await weather.GetWeather(WeatherType.Now);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: weather.GetResult()[0],
                        parseMode: ParseMode.Html, 
                        replyMarkup: keyboard);
                }
                else if (text.Contains("погода на сегодня"))
                {
                    await weather.GetWeather(WeatherType.Today);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: weather.GetResult()[0]
                        ,parseMode: ParseMode.Html,
                        replyMarkup: keyboard);
                }
                else if (text.Contains("погода на завтра"))
                {
                    await weather.GetWeather(WeatherType.Tomorrow);
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text:  weather.GetResult()[0],
                        parseMode: ParseMode.Html, 
                        replyMarkup: keyboard);
                }
                else if (text.Contains("погода на неделю"))
                {
                    weather = new Weather();
                    await weather.GetWeather(WeatherType.Week);
                    var result = weather.GetResult();
                    for (int i = 0; i < 7; i++)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id, 
                            text: result[i], 
                            parseMode: ParseMode.Html, 
                            replyMarkup: keyboard);
                    }
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Null????");
            }
        }
    }
}
