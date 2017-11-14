using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using System;
using System.Threading.Tasks;

namespace BotPlayground.Dialogs
{
    [Serializable]
    public class SetupDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(ChannelCreatedAsync);

            return Task.CompletedTask;
        }

        private async Task ChannelCreatedAsync(IDialogContext context, IAwaitable<object> result)
        {
            TeamsChannelData channelData = context.Activity.GetChannelData<TeamsChannelData>();

            var message = $"Hello!  I noticed you created the " + channelData.Channel.Name + " channel.  I'll send notifications here as I receive them."
                        + "  Here's the channel ID: " + channelData.Channel.Id;

            await context.PostAsync(message);
        }
    }
}