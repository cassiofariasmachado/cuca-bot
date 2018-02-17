using CucaBot.Models;
using CucaBot.Services;
using CucaBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CucaBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private static readonly string _luisAppId = ConfigurationManager.AppSettings["LuisAppId"];

        private static readonly string _luisApiKey = ConfigurationManager.AppSettings["LuisAPIKey"];

        private readonly CucaService _cucaService;

        public RootDialog() : base(new LuisService(new LuisModelAttribute(_luisAppId, _luisApiKey)))
        {
            _cucaService = new CucaService();
        }

        [LuisIntent("Create")]
        public async Task Create(IDialogContext context, LuisResult result)
        {
            decimal? valueToCreate = null;
            DateTime? dateToCreate = null;

            if (result.TryFindEntity("builtin.currency", out EntityRecommendation value))
            {
                valueToCreate = Convert.ToDecimal(value.Resolution["value"]);
            }

            if (result.TryFindEntity("builtin.datetimev2", out EntityRecommendation date))
            {
                dateToCreate = Convert.ToDateTime(date.Resolution["value"]);
            }

            await context.Forward(new CreateDialog(valueToCreate, dateToCreate), AfterCreate, context.Activity, CancellationToken.None);
        }

        private async Task AfterCreate(IDialogContext context, IAwaitable<CucaModel> result)
        {
            var cucaCreated = await result;

            string message = $"Cuca salva com sucesso {EmojiType.Sunglasses} \n" +
                             $"* Valor: R$ {cucaCreated.Value} \n" +
                             $"* Data: {cucaCreated.Date.ToShortDateString()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("List")]
        public async Task List(IDialogContext context, LuisResult result)
        {
            var cucas = await _cucaService.ListNext();

            Activity replyToConversation = ((Activity)context.Activity).CreateReply($"Os próximos \"dia da cuca\" são... {EmojiType.Smiley}");

            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();

            foreach (var cuca in cucas)
            {
                var cardButtons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Value = $"participar da cuca {cuca.Id}",
                        Type = ActionTypes.PostBack,
                        Title = "Participar"
                    }
                };

                decimal valueByParticipant = cuca.Participants.Any() ? cuca.Value / cuca.Participants.Count : cuca.Value;

                var card = new HeroCard()
                {
                    Title = $"Cuca nº {cuca.Id} - {cuca.Date.ToShortDateString()}",
                    Subtitle = $"Total: R$ {cuca.Value} / R$ {valueByParticipant} por pessoa / {cuca.Participants.Count} participantes",
                    Buttons = cardButtons,
                };

                replyToConversation.Attachments.Add(card.ToAttachment());
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Join")]
        public async Task Join(IDialogContext context, LuisResult result)
        {
            int? idCuca = null;

            if (result.TryFindEntity("idCuca", out EntityRecommendation value))
            {
                idCuca = Convert.ToInt32(value.Entity);
            }

            await context.Forward(new JoinDialog(idCuca), MessageReceived, context.Activity, CancellationToken.None);
        }

        [LuisIntent("Recognize image")]
        public async Task RecognizeImage(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Digite a url da imagem..");
            context.Wait(ProcessImage);
        }

        public async Task ProcessImage(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var activity = await argument;

            var uri = activity.Attachments?.Any() == true ?
                new Uri(activity.Attachments[0].ContentUrl) :
                new Uri(activity.Text);

            var analisyResult = await new VisionService().Analyse(uri);

            var mostProbablePrediction = analisyResult.Predictions?
                                    .OrderByDescending(c => c.Probability)
                                    .FirstOrDefault();

            switch (mostProbablePrediction?.Tag)
            {
                case "cuca":
                    await context.PostAsync($"Isso é uma cuca {EmojiType.Smiley}");
                    break;
                case "pao":
                    await context.PostAsync($"Isso é apenas um pão {EmojiType.Speechless}");
                    break;
                default:
                    await context.PostAsync($"Não sei dizer o que é isso {EmojiType.Sad}");
                    break;
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}