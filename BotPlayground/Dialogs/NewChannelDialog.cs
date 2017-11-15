using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using System;
using System.Threading.Tasks;

namespace BotPlayground.Dialogs
{
    [Serializable]
    public class NewChannelDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(ChannelCreatedAsync);

            return Task.CompletedTask;
        }

        private async Task ChannelCreatedAsync(IDialogContext context, IAwaitable<object> result)
        {
            // context.ChannelId;
            // context.ConversationData.SetValue<>("something", 1);

            TeamsChannelData channelData = context.Activity.GetChannelData<TeamsChannelData>();

            string toId = "", toName = "", fromId = "", fromName = "", serviceUrl = "", conversationId = "", channelId = "";

            // Use the data stored previously to create the required objects.
            var userAccount = new ChannelAccount(toId,toName);
            var botAccount = new ChannelAccount(fromId, fromName);
            var connector = new ConnectorClient(new Uri(serviceUrl));

            // Create a new message.
            IMessageActivity message = Activity.CreateMessageActivity();
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))  
            {
                // If conversation ID and channel ID was stored previously, use it.
                message.ChannelId = channelId;
            }
            else
            {
                // Conversation ID was not stored previously, so create a conversation. 
                // Note: If the user has an existing conversation in a channel, this will likely create a new conversation window.
                conversationId = (await connector.Conversations.CreateDirectConversationAsync( botAccount, userAccount)).Id;
            }

            // Set the address-related properties in the message and send the message.
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            message.Text = $"Hello!  I noticed you created the " + channelData.Channel.Name + " channel.  I'll send notifications here as I receive them."
                         + "  Here's the channel ID: " + channelData.Channel.Id;
            message.Locale = "en-us";
            await connector.Conversations.SendToConversationAsync((Activity)message);

            // context
            await context.PostAsync(message);
        }
    }
}