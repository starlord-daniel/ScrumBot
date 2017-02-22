using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ScrumBot.Data
{
    public enum Status { TODO= 1, DO, DONE}

    public class ScrumTask
    {
        public int Id { get; set; }
        public String Titel { get; set; }
        public Status Status { get; set; }

        public ScrumTask(int id, String titel, Status status)
        {
            Id = id;
            Titel = titel;
            Status = status;
        }

        public static String TasksToString(List<ScrumTask> tasks)
        {
            String result = "";
            String modifier = "";
            foreach (ScrumTask task in tasks)
            {
                result += modifier + task.Titel ;
                modifier = " ";
            }
            return result;
        }
    }

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
            
            List<CardImage> cardImages = new List<CardImage> {
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/TODO.png"),
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/DO.png"),
                new CardImage(url: "https://dheinze.blob.core.windows.net/dev/DONE.png")
            };
            
            var array = Enum.GetValues(typeof(Status));
            for (int i = 0; i < array.Length; i++)
            {
                HeroCard card = new HeroCard()
                {
                    Title = array.GetValue(i).ToString(),
                    Text = GetTaskInformation((Status)array.GetValue(i), 1),
                    Images = new List<CardImage> { cardImages[i] }
                };
                m.Attachments.Add(card.ToAttachment());
            }
            return m;
        }

        /// <summary>
        /// Ask the SQL Database for 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static string GetTaskInformation(Status status, int project)
        {
            List<ScrumTask> tasks = new List<ScrumTask>();
            using(SqlConnection connection = new SqlConnection(String.Format("Server=tcp:scrumbot.database.windows.net,1433;Initial Catalog=scrum;Persist Security Info=False;User ID={0};Password={1};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", "scrumAdmin", "xce3518@")))
            {
                var query = String.Format("SELECT TOP (5) [Id], [Titel] FROM [SCRUM].[Aufgaben] WHERE [Status] = {0} AND [ProjektId] = {1}", (int) status, project);
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        tasks.Add(new ScrumTask(reader.GetInt32(0), reader.GetString(1), status));
                }
                connection.Close();
            }
            return ScrumTask.TasksToString(tasks);
        }

        /// <summary>
        /// Call the database to get all tasks in a project
        /// </summary>
        /// <param name="project">The current project the task is in.</param>
        internal static IMessageActivity GetTasks(IDialogContext context, string project)
        {
            List<ScrumTask> tasks = new List<ScrumTask>();
            using (SqlConnection connection = new SqlConnection(String.Format("Server=tcp:scrumbot.database.windows.net,1433;Initial Catalog=scrum;Persist Security Info=False;User ID={0};Password={1};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", "scrumAdmin", "xce3518@")))
            {
                var query = String.Format("SELECT TOP (10) [Id], [Titel] FROM [SCRUM].[Aufgaben] WHERE [Status] = 1 AND [ProjektId] = {0} AND [SprintId] IS NULL", project);
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        tasks.Add(new ScrumTask(reader.GetInt32(0), reader.GetString(1), Status.TODO));
                }
                connection.Close();
            }
            
            var message = GetTasksCards(context, project, tasks);

            return message;
        }

        private static IMessageActivity GetTasksCards(IDialogContext context, string project, List<ScrumTask> tasks)
        {
            var m = context.MakeMessage();
            m.Recipient = context.MakeMessage().From;
            m.Type = "message";
            m.Attachments = new List<Attachment>();
            m.AttachmentLayout = AttachmentLayoutTypes.List;
            
            foreach (var task in tasks)
            {
                ThumbnailCard card = new ThumbnailCard()
                {
                    Title = task.Titel
                };
    
                m.Attachments.Add(card.ToAttachment());
            }

            return m;
        }
    }
}