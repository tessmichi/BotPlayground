using Microsoft.Bot.Connector.Teams.Models;

namespace BotPlayground.Models
{
    public class NotificationEvent
    {
        public TeamsChannelData ChannelData { get; set; }
        public string ServiceUrl { get; set; } = "https://smba.trafficmanager.net/amer-client-ss.msg/";
    }
}