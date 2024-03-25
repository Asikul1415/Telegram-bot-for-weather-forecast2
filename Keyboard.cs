using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_Bot
{
    internal class Keyboard
    {
        private static readonly KeyboardButton[][] _buttons =
            new[]
            {
                new[]
                {
                    new KeyboardButton("Погода сейчас"),
                    new KeyboardButton("Погода на сегодня")
                },
                new[]
                {
                    new KeyboardButton("Погода на завтра"),
                    new KeyboardButton("Погода на неделю")
                }
            };
        public static  readonly ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(_buttons);
    }
}
