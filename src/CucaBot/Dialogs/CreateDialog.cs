using CucaBot.Models;
using CucaBot.Services;
using CucaBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CucaBot.Dialogs
{
    [Serializable]
    public class CreateDialog : IDialog<CucaModel>
    {
        private readonly CucaService _cucaService;

        private decimal? valueToCreate;

        private DateTime? dateToCreate;

        public CreateDialog(decimal? value, DateTime? date)
        {
            _cucaService = new CucaService();
            this.valueToCreate = value;
            this.dateToCreate = date;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (valueToCreate.HasValue)
            {
                PromptDialog.Text(context, PromptDate, "Beleza, digite quando vai ocorrer (no formato dd/mm/yyyy)");
            }
            else
            {
                PromptDialog.Text(context, PromptValue, "Certo, digite o valor da cuca");
            }
        }

        private async Task PromptValue(IDialogContext context, IAwaitable<string> result)
        {
            var currentValue = await result;

            if (!decimal.TryParse(currentValue, out decimal value))
            {
                PromptDialog.Text(context, PromptValue, $"Ops, valor inválido digite novamente {EmojiType.Cry}");
            }
            else
            {
                valueToCreate = value;
                PromptDialog.Text(context, PromptDate, "Ok, digite a data que vai ocorrer (no formato dd/mm/yyyy)");
            }
        }

        private async Task PromptDate(IDialogContext context, IAwaitable<string> result)
        {
            var currentDate = await result;

            if (!DateTime.TryParse(currentDate, out DateTime date))
            {
                PromptDialog.Text(context, PromptDate, $"Puts, data inválida digite novamente (no formato dd/mm/yyyy) {EmojiType.Speechless}");
            }
            else
            {
                dateToCreate = date;
                await this.CreateCuca(context);
            }
        }

        public async Task CreateCuca(IDialogContext context)
        {
            var participants = new List<UserModel> { new UserModel { Id = context.Activity.From.Id, Name = context.Activity.From.Name } };
            var cuca = new CucaModel { Date = dateToCreate.Value, Value = valueToCreate.Value, Participants = participants };

            try
            {
                var cucaCreated = await _cucaService.Create(cuca);
                context.Done(cucaCreated);
            }
            catch (Exception e)
            {
                await context.PostAsync($"Ocorreu um erro tente mais tarde {EmojiType.Cry}");
                context.Fail(e);
            }
        }
    }
}