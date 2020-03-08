using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading;
using System.Net.NetworkInformation;

using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;


namespace ForTile
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        Functions allFunc = new Functions();
        string RSSString;
        bool loaded = false;

        private static volatile bool _classInitialized;

        /// <remarks>
        /// Конструктор ScheduledAgent, инициализирует обработчик UnhandledException
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Подпишитесь на обработчик управляемых исключений
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Код для выполнения на необработанных исключениях
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Агент, запускающий назначенное задание
        /// </summary>
        /// <param name="task">
        /// Вызванная задача
        /// </param>
        /// <remarks>
        /// Этот метод вызывается при запуске периодических или ресурсоемких задач
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: добавьте код для выполнения задачи в фоновом режиме
#if DEBUG
            ScheduledActionService.LaunchForTest(task.Name, System.TimeSpan.FromSeconds(30));
#endif            

            if ((!NetworkInterface.GetIsNetworkAvailable()) || (allFunc.readCount < 4))
            {
                allFunc.readCount ++;
                NotifyComplete();
                return;
            }

            allFunc.readCount = 0;

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("http://kraj.by/belarus/news/?rss=articles"));

            while (!loaded)
                Thread.Sleep(50);
            
            NotifyComplete();
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    RSSString = e.Result;
                    XElement twitterElements = XElement.Parse(RSSString);
                    DateTime LastRead = allFunc.lastRead;
                    DateTime now = DateTime.Now;

                    if (now.Day != LastRead.Day)
                    {
                        DateTime newLastRead = new DateTime(now.Year, now.Month, now.Day, 0, 0, 1);
                        LastRead = newLastRead;
                        allFunc.lastRead = LastRead;
                    }

                    var postList =
                        from tweet in twitterElements.Descendants("item")
                        where Convert.ToDateTime(tweet.Element("pubDate").Value) > LastRead
                        select new PostMessage
                        {
                            title = tweet.Element("title").Value,
                            mainImage = allFunc.GetImageFromPostContents(tweet.Element("description").Value),
                            description = Regex.Replace(tweet.Element("description").Value, "<.*?>", String.Empty),
                            pubDate = Convert.ToDateTime(tweet.Element("pubDate").Value).ToString(),
                            link = tweet.Element("link").Value
                        };

                    if (postList.Count<PostMessage>() > 0)
                        allFunc.ChangeTile(postList.First<PostMessage>(), postList.Count<PostMessage>(), false);
                    loaded = true;
                }
            }
            catch
            {
            }
        }
    }
}