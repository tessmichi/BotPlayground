using BotPlayground.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BotPlayground
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.ServiceUrl.Equals("https://directline.botframework.com/") &&
                activity.ChannelId.Equals("directline")) // TODO more secure check
            {
                await Conversation.SendAsync(activity, () => new Dialogs.ExternalEventDialog());
            }
            else if (activity.Type == ActivityTypes.Message)
            {
            }
            else if (activity.Type == ActivityTypes.Event)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.EventDialog());
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        
        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                TeamsChannelData channelData = message.GetChannelData<TeamsChannelData>();
                if(channelData.EventType == "channelCreated")
                {
                    // HERE is where you will replace printing out the channel id with storing it in your database

                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    IMessageActivity newMessage = Activity.CreateMessageActivity();
                    newMessage.Type = ActivityTypes.Message;
                    newMessage.Text = $"Hello!  I noticed you created the " + channelData.Channel.Name + " channel.  I'll send notifications here as I receive them."
                         + "  Here's the channel ID: " + channelData.Channel.Id;
                    var stateClient = message.GetStateClient();
                    ConversationParameters conversationParams = new ConversationParameters(
                        isGroup: true,
                        bot: null,
                        members: null,
                        topicName: channelData.Channel.Name + " Created",
                        activity: (Activity)newMessage,
                        channelData: channelData);

                    var result = await connector.Conversations.CreateConversationAsync(conversationParams);

                    // TODO: Save this data to an external store for use later.  For the hack, we will just manually track the info we care about.
                    // BotData userData = stateClient.BotState.GetConversationData(message.ChannelId, result.Id);
                    // userData.SetProperty(channelData.Channel.Id, new ChannelDataInfo(message.ServiceUrl, channelData));
                    // await stateClient.BotState.SetUserDataAsync(message.ChannelId, message.From.Id, userData);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}