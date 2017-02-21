using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ScrumBot.Controllers
{
    [Serializable]
    public class ScrumBotDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(HomeBotDialog); 
        }

        public virtual async Task HomeBotDialog(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // var message = await argument;
            PromptDialog.Choice(
                    context: context,
                    resume: AfterResetAsync,
                    options: new List<string> { "Neues Projekt", "Backlog", "Planning", "Status" },
                    prompt: "Hi, I'm your Scrum Helper. What do you want to do?",
                    retry: "Please choose one of the displayed options.",
                    promptStyle: PromptStyle.Auto
                    );
        }

        private async Task AfterResetAsync(IDialogContext context, IAwaitable<string> result)
        {
            var r = await result;

            switch (r)
            {
                case "Neues Projekt":
                    context.Wait(NewProjectDialog);
                    break;
                case "Backlog":
                    context.Wait(BacklogDialog);
                    break;
                case "Planning":
                    context.Wait(PlanningDialog);
                    break;
                case "Status":
                    context.Wait(StatusDialog);
                    break;
                default:
                    break;
            }

            await context.PostAsync($"You chose: " + r);
        }

        private async Task StatusDialog(IDialogContext context, IAwaitable<object> result)
        {
            var r = await result;
            throw new NotImplementedException();
        }

        private Task PlanningDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private Task BacklogDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        public virtual async Task NewProjectDialog(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            throw new NotImplementedException();
        }
}