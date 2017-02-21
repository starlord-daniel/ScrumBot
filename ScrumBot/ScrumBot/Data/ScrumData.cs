using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScrumBot.Data
{
    public static class ScrumData
    {
        /// <summary>
        /// Return the status of the current project: TODO, DO, DONE
        /// </summary>
        /// <param name="project">The identifier of the project</param>
        /// <returns>The message to return.</returns>
        internal static IMessageActivity GetStatus(IDialogContext context, string project)
        {
            var m = context.MakeMessage();
            m.Recipient = context.MakeMessage().From;
            m.Type = "message";
            m.Attachments = new List<Attachment>();
            m.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            List<string> titles = new List<string>
            {
                "TODO",
                "DO",
                "DONE"
            };

            List<CardImage> cardImages = new List<CardImage> {
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/TODO.png"),
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/DO.png"),
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/DONE.png")
            };

            for (int i = 0; i < cardImages.Count; i++)
            {
                HeroCard card = new HeroCard()
                {
                    Title = titles[i],
                    Text = GetStatusText(titles[i], project),
                    Images = new List<CardImage> { cardImages[i] }
                };

                Attachment plAttachment = card.ToAttachment();
                m.Attachments.Add(plAttachment);
            }

            return m;

        }

        /// <summary>
        /// Ask the SQL Database for 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static string GetStatusText(string statusTitle, string project)
        {
            var result = "";
            switch (statusTitle)
            {
                case "TODO":
                    PerformSQLQuery("");
                    result = "Facial Recognition";
                    break;
                case "DO":
                    PerformSQLQuery("");
                    result = "Language Understanding";
                    break;
                case "DONE":
                    PerformSQLQuery("");
                    result = "Sentiment Detection";
                    break;
                default:
                    PerformSQLQuery("");
                    result = "No entries found";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Call the database to get all tasks in a project
        /// </summary>
        /// <param name="project">The current project the task is in.</param>
        internal static IMessageActivity GetTasks(IDialogContext context, string project)
        {
            var tasks = new List<string> {
                "Facial Recognition",
                "Video Parsing",
                "Sentiment Detection",
                "Language Understanding"
            };

            PerformSQLQuery("");

            var message = GetTasksCards(context, project, tasks);

            return message;
        }

        private static IMessageActivity GetTasksCards(IDialogContext context, string project, List<string> tasks)
        {
            var m = context.MakeMessage();
            m.Recipient = context.MakeMessage().From;
            m.Type = "message";
            m.Attachments = new List<Attachment>();
            m.AttachmentLayout = AttachmentLayoutTypes.List;

            List<string> titles = new List<string>();

            foreach (var item in tasks)
            {
                titles.Add(item);
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                ThumbnailCard card = new ThumbnailCard()
                {
                    Title = titles[i]
                };

                Attachment plAttachment = card.ToAttachment();
                m.Attachments.Add(plAttachment);
            }

            return m;
        }

        private static void PerformSQLQuery(string query)
        {
            // Performs the SQL query
        }
    }
}