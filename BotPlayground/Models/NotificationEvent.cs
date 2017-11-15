using Microsoft.Bot.Connector.Teams.Models;

namespace BotPlayground.Models
{
    public class NotificationEvent
    {
        public TeamsChannelData ChannelData { get; set; }
        public string ServiceUrl { get; set; }
        public string TeamsChannelId { get; set; } = "19:c9a093d2902d4727b80c8c9a5dfb8c4e @thread.skype";
    }
}