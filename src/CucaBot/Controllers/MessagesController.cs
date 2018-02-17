using CucaBot.Dialogs;
using CucaBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CucaBot
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
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            var connector = new ConnectorClient(new Uri(message.ServiceUrl));

            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;

                case ActivityTypes.ConversationUpdate:
                    if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                    {
                        var reply = message.CreateReply();

                        int greetingIndex = new Random().Next() % CucaBotConfig.Greetings.Length;

                        reply.Text = $"{CucaBotConfig.Greetings[greetingIndex]}, sou o **CucaBot**, " +
                                     $"posso lhe ajudar com as seguintes tarefas: \n " +
                                     $"{CucaBotConfig.Tasks}";

                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    break;

                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;

                case ActivityTypes.Typing:
                    // Handle knowing tha the user is typing
                    break;

                case ActivityTypes.Ping:
                    break;

                default:
                    return null;
            }

            return null;
        }
    }
}