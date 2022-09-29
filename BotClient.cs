using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IAC.Bot
{
    public sealed class BotClient
    {
        #region Singleton pattern
        private static BotClient _instance;
        private BotClient()
        {
            _commandHandlers = new Dictionary<string, Func<ITelegramBotClient, Message, string[], Task>>();
        }

        public static BotClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BotClient();
                }

                return _instance;
            }
        }
        #endregion

        private Dictionary<string, Func<ITelegramBotClient, Message, string[], Task>> _commandHandlers;

        private ITelegramBotClient _client;

        public async Task StartBotAsync([NotNull] string botToken)
        {
            _client = new TelegramBotClient(botToken);
            _client.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
            await Task.Delay(-1);
        }

        private Task HandleErrorAsync(
            ITelegramBotClient client,
            Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"EXCEPTION: {exception}");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient client,
            Update update,
            CancellationToken cancellationToken
        )
        {
            if (!string.IsNullOrEmpty(update.Message.Text))
            {
                Console.WriteLine($"Received message from @{update.Message.From.Username}: {update.Message.Text}");
                await HandleCommandAsync(client, update.Message);
            }
        }

        private Task HandleCommandAsync(ITelegramBotClient client, Message message)
        {
            string[] parts = message.Text.Split(' ');
            string command = parts[0];
            string[] args = parts.Skip(1).ToArray();

            if (_commandHandlers.TryGetValue(command, out var handler))
            {
                return handler(client, message, args);
            }
            return Task.CompletedTask;
        }

        public void On([NotNull] string command, [NotNull] Func<ITelegramBotClient, Message, string[], Task> handler)
        {
            _commandHandlers.Add(command, handler);
        }
    }
}
