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

        private Dictionary<string, string> newProjectInfos = new Dictionary<string, string>();
        Dictionary<string, int> taskMapping = new Dictionary<string, int>();

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
                    options: new List<string> { "Neues Projekt", "Backlog", "Status", "Kommentieren" },
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
                    {
                        PromptDialog.Text(
                            context: context,
                            resume: CreateProject,
                            prompt: "Wie soll ihr neues Projekt heißen?"
                        );
                    }
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
                case "Status":
                    { 
                        // Return the current status
                        var message = ScrumData.GetStatus(context, project);

                        // Post Status
                        await context.PostAsync(message);
                        context.Wait(HomeBotDialog);
                    }
                    break;
                case "Kommentieren":
                    {
                        PromptDialog.Text(
                            context: context,
                            resume: NewComment,
                            prompt: "Schreibe deinen Kommentar zur aktuellen Aufgabe."
                            );
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task NewComment(IDialogContext context, IAwaitable<string> result)
        {
            var comment = await result;

            ScrumData.InsertComment(comment);

            await context.PostAsync("Der Kommentar wurde erfolgreich hochgeladen.");
            context.Wait(HomeBotDialog);
        }

        private async Task CreateProject(IDialogContext context, IAwaitable<string> result)
        {
            // save project name
            newProjectInfos["projectName"] = await result;

            // Get length and points for sprints
            PromptDialog.Text(
                context: context,
                resume: SprintDefinition,
                prompt: "Wie lang soll ein Sprint sein (Tage) und wie viele Points pro Sprint? Format: 3,5"
            );

        }

        private async Task SprintDefinition(IDialogContext context, IAwaitable<string> result)
        {
            // save project name
            newProjectInfos["sprintInfos"] = await result;

            // Get length and points for sprints
            PromptDialog.Text(
                context: context,
                resume: StakeholdersDefinition,
                prompt: "Wer sind die Stakeholders?"
            );
        }

        private async Task StakeholdersDefinition(IDialogContext context, IAwaitable<string> result)
        {
            // save project name
            newProjectInfos["stakeholders"] = await result;

            // Get length and points for sprints
            PromptDialog.Text(
                context: context,
                resume: TeammatesDefinition,
                prompt: "Wer sind die Teammitglieder (Komma getrennt)?"
            );
        }

        private async Task TeammatesDefinition(IDialogContext context, IAwaitable<string> result)
        {
            // save project name
            newProjectInfos["stakeholders"] = await result;

            // create project in database

            // post result
            await context.PostAsync("Das Projekt wurde angelegt.");
            context.Wait(HomeBotDialog);
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