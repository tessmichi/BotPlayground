using BotPlayground.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BotPlayground.Dialogs
{
    [Serializable]
    public class ExternalEventDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var payload = message.Text;

            var parsedPayload = JsonConvert.DeserializeObject<NotificationEvent>(payload);
            
            var channelData = parsedPayload.ChannelData;
            IMessageActivity notificationMessage = Activity.CreateMessageActivity();
            notificationMessage.Type = ActivityTypes.Message;
            notificationMessage.Text = $"You said {payload}";
            notificationMessage.Locale = "en-us";
            notificationMessage.ChannelId = channelData.Channel.Id;
            ConversationParameters conversationParams = new ConversationParameters(
                isGroup: true,
                bot: null,
                members: null,
                topicName: "Test Conversation",
                activity: (Activity)notificationMessage,
                channelData: channelData);
            var connector = new ConnectorClient(new Uri(parsedPayload.ServiceUrl));
            var r = await connector.Conversations.CreateConversationAsync(conversationParams);
                        
            //var userAccount = new ChannelAccount(parsedPayload.ChannelUri); // TODO CHECK THIS
            //var botAccount = new ChannelAccount(message.Recipient.Id, message.Recipient.Name);
            //var connector = new ConnectorClient(new Uri(parsedPayload.ServiceUrl)); // TODO CHECK THIS
            //IMessageActivity notificationMessage = Activity.CreateMessageActivity();
            ////var conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            //notificationMessage.From = botAccount;
            //notificationMessage.Recipient = userAccount;

            await connector.Conversations.SendToConversationAsync((Activity)notificationMessage);
            
            context.Wait(MessageReceivedAsync);
        }
    }
}