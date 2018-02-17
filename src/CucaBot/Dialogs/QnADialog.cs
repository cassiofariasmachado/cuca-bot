using CucaBot.Utils;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace CucaBot.Dialogs
{
    [Serializable]
    public class QnADialog : QnAMakerDialog
    {
        private const double Threshold = 0.8;

        private static readonly string DefaultMessage = $"Desculpe, não entendi {EmojiType.Speechless}";

        private static readonly string _qnASubscriptionKey = ConfigurationManager.AppSettings["QnASubscriptionKey"];

        private static readonly string _qnAKnowleadgeBaseId = ConfigurationManager.AppSettings["QnAKnowledgebaseId"];

        public QnADialog() : base(new QnAMakerService(new QnAMakerAttribute(_qnASubscriptionKey, _qnAKnowleadgeBaseId, DefaultMessage, Threshold)))
        {
        }

        //Workarount for issue: https://github.com/Microsoft/BotBuilder-CognitiveServices/issues/57
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            context.Done<IMessageActivity>(null);
        }
    }
}