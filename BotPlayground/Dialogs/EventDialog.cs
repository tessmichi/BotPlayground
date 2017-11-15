﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BotPlayground.Dialogs
{
    [Serializable]
    public class EventDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(EventReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task EventReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var user = activity.From.Name;
            var timestamp = activity.Timestamp;
            var url = "https://example.com";

            var message = $"{user} completed a poor call at {timestamp}, [click here]({url}) for details.";

            // return our reply to the user
            await context.PostAsync(message);

            context.Wait(EventReceivedAsync);
        }
    }
}