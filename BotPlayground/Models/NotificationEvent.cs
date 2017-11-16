using Microsoft.Bot.Connector.Teams.Models;

namespace BotPlayground.Models
{
    public class NotificationEvent
    {
        public string TeamsServiceUrl { get; set; }
        public string TeamsChannelId { get; set; }
    }
}