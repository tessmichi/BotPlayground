using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using BotPlayground.Models;

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

            var userAccount = new ChannelAccount(parsedPayload.ChannelUrl); // TODO CHECK THIS
            var botAccount = new ChannelAccount(message.Recipient.Id, message.Recipient.Name);
            var connector = new ConnectorClient(new Uri(parsedPayload.ServiceUrl)); // TODO CHECK THIS
            IMessageActivity notificationMessage = Activity.CreateMessageActivity();
            var conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;

            notificationMessage.ChannelId = parsedPayload.ChannelId;
            notificationMessage.From = botAccount;
            notificationMessage.Recipient = userAccount;
            notificationMessage.Conversation = new ConversationAccount(id: conversationId);
            notificationMessage.Text = $"You said {payload}";
            notificationMessage.Locale = "en-us";
            await connector.Conversations.SendToConversationAsync((Activity)notificationMessage);
            
            context.Wait(MessageReceivedAsync);
        }
    }
}