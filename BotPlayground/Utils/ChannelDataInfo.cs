using Microsoft.Bot.Connector.Teams.Models;

namespace BotPlayground.Utils
{
    public class ChannelDataInfo
    {
        private string serviceUrl;
        private TeamsChannelData channelData;
        public ChannelDataInfo(string serviceUrl, TeamsChannelData channelData)
        {
            this.serviceUrl = serviceUrl;
            this.channelData = channelData;
        }
    }
}