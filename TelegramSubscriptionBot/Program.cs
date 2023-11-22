using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6540204661:AAGwKJ6LFYo3LVJ6f7X6GIb0DGgvTe_I48k");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
//cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    long adminCahntId = 101758258;

    var chatId = message.Chat.Id;
    var username = message.Chat.Username;

    Console.WriteLine($"Received a '{messageText}' message in chat {username}.");



    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Your Converted Link is : \n Please Copy and Add Subscription links to your client");

     sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: $"{ConvertToApiPath(messageText)}");

     sentMessage = await botClient.SendTextMessageAsync(
         chatId: adminCahntId,
         text: $"Received a '{messageText}' message in chat {username}.");
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}


static string ConvertToApiPath(string inputUrl)
{
    try
    {
        // Use Uri class to parse the input URL
        Uri uri = new Uri(inputUrl);

        // Extract segments from the URL path
        string[] pathSegments = uri.AbsolutePath.Trim('/').Split('/');

        // Extract parameters from the query string
        string nameParameter = uri.Query.TrimStart('?').Split('=')[1];

        // Extract the server name from the path (assuming it's the third segment)
        string serverName = uri.Host;

        string baseUrl = "https://sub.irnetfree.info";
        // Use a switch statement to handle different cases
        switch (serverName.ToLower())
        {
            case "subro.irnetfree.info":
                return $"{baseUrl}/api/Subs/98/{nameParameter}";
            case "subger.irnetfree.info":
                return $"{baseUrl}/api/Subs/12/{nameParameter}";
            case "subfr.irnetfree.info":
                return $"{baseUrl}/api/Subs/41/{nameParameter}";
            case "subpor.irnetfree.info":
                return $"{baseUrl}/api/Subs/31/{nameParameter}";
            // Add more cases as needed
            default:
                return "Not Valid Link";
                return $"api/Unknown/{serverName}/{nameParameter}";
        }
        
    }
    catch (Exception ex)
    {
        return "Not Valid Link";
    }
}
