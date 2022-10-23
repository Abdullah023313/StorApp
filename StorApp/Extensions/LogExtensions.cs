using Azure.Messaging;
using StorApp.Controllers;
using System.Drawing;

namespace StorApp.Extensions
{
    static public class LogExtensions
    {
        static public void myLogInformation(this ILogger logger, string message, Exception exception)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nInformation:{exception}\n");
            Console.ForegroundColor = color;
            logger.LogInformation(exception,message);
        }
    }
}