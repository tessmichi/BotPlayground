using BotPlayground.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
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

            MicrosoftAppCredentials.TrustServiceUrl(parsedPayload.TeamsServiceUrl);
            
            // We are parsing the payload, based on information that was printed at the start of the conversation
            // The reason we get it from the payload here is simply because, for this example, the messages
            // being sent through DirectLine are formatted with the information that was saved in the external
            // database when it was initially saved. Saving the data to the database is not in this repo though
            var channelId = parsedPayload.TeamsChannelId;
            var channelData = new TeamsChannelData {
                Channel = new ChannelInfo(id:channelId),
                EventType = "message"
            };
            IMessageActivity notificationMessage = Activity.CreateMessageActivity();
            notificationMessage.Type = ActivityTypes.Message;
            notificationMessage.Text = $"Guess what I just heard? {payload}";

            ConversationParameters conversationParams = new ConversationParameters(
                isGroup: true,
                bot: null,
                members: null,
                topicName: "Something Happened - Notification",
                activity: (Activity)notificationMessage,
                channelData: channelData);
            var connector = new ConnectorClient(
                new Uri(parsedPayload.TeamsServiceUrl),
                ConfigurationManager.AppSettings["MicrosoftAppId"],
                ConfigurationManager.AppSettings["MicrosoftAppPassword"]);
            
            var test = await connector.Conversations.CreateConversationAsync(conversationParams);
        }
    }
}
