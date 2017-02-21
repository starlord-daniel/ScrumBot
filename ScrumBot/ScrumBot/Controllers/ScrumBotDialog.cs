using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ScrumBot.Data;

namespace ScrumBot.Controllers
{
    [Serializable]
    public class ScrumBotDialog : IDialog<object>
    {
        private string project = "Hackathon";


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
                    options: new List<string> { "Neues Projekt", "Backlog", "Planung", "Status" },
                    prompt: "Hi, ich bin der Scrum Helper. was kann ich für dich erledigen?",
                    retry: "Bitte wähle eine der angezeigten Optionen.",
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
                    {
                        PromptDialog.Choice(
                            context: context,
                            resume: AddOrDeleteTask,
                            options: new List<string> { "Aufgabe erstellen", "Aufgabe löschen" },
                            prompt: "Was möchtest du im Backlog machen?",
                            retry: "Bitte wähle eine der angezeigten Optionen.",
                            promptStyle: PromptStyle.Auto
                            );
                    }
                    break;
                case "Planung":
                    {
                        // Get tasks from database
                        var taskMessage = ScrumData.GetTasks(context, project);

                        await context.PostAsync(taskMessage);
                        context.Wait(HomeBotDialog);
                    }
                    break;
                case "Status":
                    { 
                        // Return the current status
                        var message = ScrumData.GetStatus(context, project);

                        // Post Status
                        await context.PostAsync(message);
                        context.Wait(HomeBotDialog);
                    }
                    break;
                default:
                    break;
            }

            // await context.PostAsync($"You chose: " + r);
        }

        private async Task AddOrDeleteTask(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            if (answer == "Aufgabe erstellen")
            {
                PromptDialog.Text(
                    context: context,
                    resume: CreateTask,
                    prompt: "Gib den Titel des neuen Tasks ein."
                );
            }
            else
            {
                PromptDialog.Text(
                    context: context,
                    resume: DeleteTask,
                    prompt: "Gib den Titel des zu löschenden Tasks ein."
                );
            }
        }

        private async Task DeleteTask(IDialogContext context, IAwaitable<string> result)
        {
            // Call SQL DELETE
            await context.PostAsync("Aufgabe gelöscht.");
            context.Wait(HomeBotDialog);
        }

        private async Task CreateTask(IDialogContext context, IAwaitable<string> result)
        {
            // Call SQL INSERT
            await context.PostAsync("Aufgabe erstellt.");
            context.Wait(HomeBotDialog);
        }

        private async Task PlanningDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var r = await result;
            throw new NotImplementedException();
        }

        private async Task BacklogDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var r = await result;
            throw new NotImplementedException();
        }

        public virtual async Task NewProjectDialog(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var r = await argument;
            throw new NotImplementedException();
        }
    }
}