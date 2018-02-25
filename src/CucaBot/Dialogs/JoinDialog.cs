using CucaBot.Models;
using CucaBot.Services;
using CucaBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace CucaBot.Dialogs
{
    [Serializable]
    public class JoinDialog : IDialog<object>
    {
        private readonly CucaService _cucaService;

        private int? idCucaToJoin;

        public JoinDialog(int? idCuca)
        {
            _cucaService = new CucaService();
            this.idCucaToJoin = idCuca;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (idCucaToJoin.HasValue)
            {
                await Join(context);
            }
            else
            {
                PromptDialog.Text(context, PromptIdCuca, "Certo, então digite o número da cuca que deseja participar");
            }
        }

        private async Task PromptIdCuca(IDialogContext context, IAwaitable<string> result)
        {
            var currentValue = await result;

            if (!int.TryParse(currentValue, out int idCuca))
            {
                PromptDialog.Text(context, PromptIdCuca, "Ops, número inválido por favor digite novamente");
            }
            else
            {
                idCucaToJoin = idCuca;
                await Join(context);
            }
        }

        private async Task Join(IDialogContext context)
        {
            string message = string.Empty;
            string errorMessage = $"Ocorreu um erro tente mais tarde {EmojiType.Cry}";

            try
            {
                var userModel = new UserModel { Id = context.Activity.From.Id, Name = context.Activity.From.Name };
                var result = await _cucaService.Join(idCucaToJoin.Value, userModel);

                if (result.IsSuccessStatusCode)
                {
                    message = $"Show, agora você está participando do racha {EmojiType.Wink}";
                }
                else
                {
                    message = errorMessage;
                }

                await context.PostAsync(message);
                context.Done<object>(null);
            }
            catch (Exception e)
            {
                await context.PostAsync(errorMessage);
                context.Fail(e);
            }
        }
    }
}
