using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using SitecoreCommands = Sitecore.Update.Commands;
using Sitecore.Update.Interfaces;
using Meerkat.Sitecore.ItemComparer;
using System.Globalization;

namespace Meerkat.Sitecore.ItemComparer
{
    /// <summary>
    /// SignalR Hub class 
    /// </summary>
    public class ItemComparerHub : Hub
    {

        public static List<ICommand> Commands
        {
            get;
            set;
        }

        /// <summary>
        /// Test the connection to the WCF Service
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetUrl"></param>
        /// <param name="targetPath"></param>
        /// <param name="database"></param>
        public void TestConnection(string sourceUrl, string sourcePath, string targetUrl, string targetPath, string database)
        {
            Clients.All.broadcastMessage("Testing Connections....");
            ItemComparer comparer = new ItemComparer();
            comparer.SourceUrl = sourceUrl;
            comparer.TargetUrl = targetUrl;
            comparer.SourcePath = sourcePath;
            comparer.TargetPath = targetPath;
            comparer.Database = database;
            string message = comparer.TestConnection();
            Clients.All.broadcastMessage(message);
        }

        /// <summary>
        /// Analyse the results before generating
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public string[] Analyse(string targetUrl, string sourceUrl, string sourcePath, string targetPath, string startdate, string enddate, string database)
        {
            Commands = new List<ICommand>();
            try
            {

                var comparer = new ItemComparer();
                comparer.ItemProcessed += OnItemProcessed;

                comparer.TargetUrl = targetUrl;
                comparer.TargetPath = targetPath;
                comparer.SourcePath = sourcePath;
                comparer.SourceUrl = sourceUrl;
                comparer.Database = database;

                IEnumerable<Result> results;
                DateTime startDateTime = DateTime.MinValue;
                DateTime endDateTime = DateTime.MinValue;

                if (DateTime.TryParseExact(startdate, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out startDateTime) && DateTime.TryParseExact(enddate, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out endDateTime))
                {
                    results = comparer.GetHistory(startDateTime, endDateTime);

                    foreach (var result in results)
                    {
                        Clients.All.broadcastMessage("Processing Item" + result.Path);
                        var commandsToAdd = comparer.CompareItem(result);

                        foreach (var command in commandsToAdd)
                        {
                            if (command is SitecoreCommands.AddItemCommand)
                            {
                                Clients.All.addCommand(result.Path, result.ID);
                            }
                            else if (command is SitecoreCommands.ChangeItemCommand)
                            {
                                Clients.All.updateCommand(result.Path, result.ID);
                            }
                            else
                            {
                                Clients.All.deleteCommand(result.Path, result.ID);
                            }
                        }

                        Commands.AddRange(commandsToAdd);
                    }
                    Clients.All.broadcastMessage("Done!");
                }

                Clients.All.getCommands(Commands);

                return Commands.Select(i => i.Description).ToArray();
            }
            catch (Exception ex)
            {
                Clients.All.broadcastMessage("An Error Occured - " + ex.Message);
                return Commands.Select(i => i.Description).ToArray();
            }
        }

        /// <summary>
        /// Generate the Update file based on the analysis
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public void Generate(string[] ids, string sourcePath, string targetPath)
        {
            try
            {
                var comparer = new ItemComparer();
                comparer.SourcePath = sourcePath;
                comparer.TargetPath = targetPath;

                if (Commands.Any())
                {
                    string url = comparer.GeneratePackage(Commands.Where(i => ids.Contains(i.Item.RootPath)).ToList());
                    Clients.All.showDownload(url);
                }
                else
                {
                    Clients.All.broadcastMessage("No data to generate - Use analyse first");
                }
            }
            catch (Exception ex)
            {
                Clients.All.broadcastMessage("An Error Occured - " + ex.Message);
            }
        }

        public void OnItemProcessed(object sender, ItemComparer.ItemProcessEventArgs e)
        {
            Clients.All.broadcastMessage(e.Message);
        }
    }
}
