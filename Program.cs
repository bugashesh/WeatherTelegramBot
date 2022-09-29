using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using IAC.Bot.Weather;

namespace IAC.Bot
{
    class Program
    {
        //добавить сюда хост не с компа
        private static readonly string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");

        private const string startCommand = "/start";
        private const string randomCommand = "/random";
        private const string weatherCommand = "/weather";
        private const string predictionCommand = "/prediction";

        static void Main() => ConfigureBot().GetAwaiter().GetResult();

        private static async Task ConfigureBot()
        {
            Console.WriteLine("Starting bot...");
            BotClient.Instance.On(startCommand, HandleStartCommandAsync);
            BotClient.Instance.On(randomCommand, HandleRandomAsync);
            BotClient.Instance.On(weatherCommand, HandleWeatherCommandAsync);
            BotClient.Instance.On(predictionCommand, HandlePredictionCommandAsync);
            await BotClient.Instance.StartBotAsync(botToken);
        }

        private static async Task HandleWeatherCommandAsync(ITelegramBotClient client, Message message, string[] args)
        {
            if (args == null || args.Length == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Укажите город для прогноза погоды.\n Например /weather Санкт-Петербург");
                return;
            }

            string query = string.Join(' ', args);
            try
            {
                Weather.Weather w = await WeatherForecast.Instance.GetByCityAsync(query);
                await client.SendTextMessageAsync(message.Chat.Id, $"Температура: {w.Main.Temp} °C\nОщущается как: {w.Main.FeelsLike} °C");
            }
            catch (Exception)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Город не был найден.\nПожалуйста, предержитесь формата:\n/weather Москва");
            }
        }

        private static async Task HandleStartCommandAsync(ITelegramBotClient client, Message message, string[] args)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Привет, дорогой друг! Чтобы узнать список команд, нажмите на Меню:)");
        }

        private static async Task HandleRandomAsync(ITelegramBotClient client, Message message, string[] args)
        {
            var random = new Random();
            await client.SendTextMessageAsync(message.Chat.Id, "Ваше число для предсказания: " + random.Next(1, 7));
        }


        //если не лень то сделаю кнопки + обработчик
        private static async Task HandlePredictionCommandAsync(ITelegramBotClient client, Message message, string[] args)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Прочтите ваше предсказание, под ранее выпавшем числом: " );
            await client.SendTextMessageAsync(message.Chat.Id, "1 - Вам пора отдохнуть.");
            await client.SendTextMessageAsync(message.Chat.Id, "2 - Вас ждет приятный сюрприз.");
            await client.SendTextMessageAsync(message.Chat.Id, "3 - Ваши надежды и планы сбудутся сверх всяких ожиданий.");
            await client.SendTextMessageAsync(message.Chat.Id, "4 - Новогодние праздники отметьте в кругу друзей.");
            await client.SendTextMessageAsync(message.Chat.Id, "5 - Путешествие вас ждет и билет на самолет.");
            await client.SendTextMessageAsync(message.Chat.Id, "6 - Настало время, чтобы просто расслабиться и отдохнуть.");
        }
    }
}
