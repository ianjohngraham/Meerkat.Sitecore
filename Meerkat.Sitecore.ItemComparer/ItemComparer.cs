using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Globalization;
using Sitecore.StringExtensions;
using Sitecore.Update;
using Sitecore.Update.Commands;
using Sitecore.Update.Data.Items;
using SitecoreData = Sitecore.Data;
using Sitecore.Update.Interfaces;
using Meerkat.Sitecore.ItemComparer.ItemComparerService;
using PackageGenerator = Sitecore.Update.Engine.PackageGenerator;

namespace Meerkat.Sitecore.ItemComparer
{
    /// <summary>
    /// Main business logic class for comparing Sitecore Items
    /// </summary>
    public class ItemComparer
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string SourceUrl { get; set; }
        public string TargetUrl { get; set; }
        public string Database { get; set; }

        public class ItemProcessEventArgs : EventArgs
        {
            public string Message { get; set; }
            public string ItemName { get; set; }
        }
        public event EventHandler<ItemProcessEventArgs> ItemProcessed;

        public  List<ICommand> branchCommands = new List<ICommand>();

        public StringBuilder builder = new StringBuilder();

    
        /// <summary>
        /// Connect to WCF Data Service using 1 day as a test
        /// </summary>
        /// <returns></returns>
        public string TestConnection()
        {
            StringBuilder message = new StringBuilder();
            try
            {
                var entitiesSource = new MasterEntities(new Uri(SourceUrl));
                entitiesSource.SendingRequest += OnSendingRequestSource;
                if (entitiesSource != null)
                {
                    var queryItems = entitiesSource.Histories.Expand("Item").Where(i => i.Created > DateTime.Now.AddDays(-1));
                    queryItems.ToList();
                    message.AppendFormat("<p>Connection to source url {0} Successful</p>", SourceUrl);
                }

                var entitiesTarget = new MasterEntities(new Uri(TargetUrl));
                entitiesTarget.SendingRequest += OnSendingRequestSource;
                if (entitiesTarget != null)
                {
                    var queryItems = entitiesTarget.Histories.Expand("Item").Where(i => i.Created > DateTime.Now.AddDays(-1));
                    queryItems.ToList();
                    message.AppendFormat("<p>Connection to target url {0} Successful</p>", TargetUrl);
                }
            }
            catch (Exception ex)
            {
                return message.Append("Connection Failed:" + ex.InnerException.Message).ToString();
            }
            return message.ToString();
        }


        /// <summary>
        /// Get all the items changed in the history table in date range
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="endate"></param>
        /// <returns></returns>
        public IEnumerable<Result> GetHistory(DateTime startdate, DateTime endate)
        {
            var entities = new MasterEntities(new Uri(SourceUrl));
            entities.SendingRequest += OnSendingRequestSource;
            var listGuids = new List<Guid>();
            Result[] resultsQuery = null;
            if (startdate != DateTime.MinValue)
            {
                var queryItems = entities.Histories.Expand("Item").Where(i => i.Created >= startdate && i.Created <= endate);
                var results = queryItems.ToList();
                var filteredSource = (from r in results
                                      group r by r.ItemId into g
                                      let result = g.FirstOrDefault()
                                      let item = result.Item
                                      let fields = (result.Item != null ? result.Item.VersionedFields : null)
                                      let sharedfields = (result.Item != null ? result.Item.SharedFields : null)
                                      let unversionedfields = (result.Item != null ? result.Item.UnversionedFields : null)
                                      select new Result()
                                      {
                                          Path = result.ItemPath,
                                          ID = result.ItemId,
                                          CreatedDate = result.Created,
                                          Item = item,
                                          LanguageCode = result.ItemLanguage,
                                          VersionInfo = result.ItemVersion,
                                          Fields = fields,
                                          SharedFields = sharedfields,
                                          UnversionedFields = unversionedfields,
                                          Updated = result.Created
                                      });

                resultsQuery = filteredSource as Result[] ?? filteredSource.ToArray();
            }
            return resultsQuery;
        }

        /// <summary>
        /// Compare an entire branch -  not currently used but hoping to expand upon
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public IEnumerable<Result> CompareBranches(Guid parentId, string itemName)
        {
            builder.AppendFormat("/{0}", itemName);
            var commands = new List<ICommand>();

            SyncItem syncItemSource = null;
            ContentDataItem sourceContentItem = null;

            var entities = new MasterEntities(new Uri(SourceUrl));
            entities.SendingRequest += OnSendingRequestSource;
            var listGuids = new List<Guid>();
            Result[] resultsQuery = null;

            // Get children of current Item
            var queryItems = entities.Items.Where(i => parentId != null && i.ParentID == parentId);
            var results = queryItems.ToList();
            var filteredSource = (from r in results
                                  group r by r.ID into g
                                  let result = g.FirstOrDefault()
                                  let item = result
                                  let fields = (result != null ? result.VersionedFields : null)
                                  let sharedfields = (result != null ? result.SharedFields : null)
                                  let unversionedfields = (result != null ? result.UnversionedFields : null)
                                  select new Result()
                                  {
                                      Path = builder.ToString(),
                                      ID = result.ID,
                                      CreatedDate = result.Created,
                                      Item = item,
                                      Fields = fields,
                                      SharedFields = sharedfields,
                                      UnversionedFields = unversionedfields,
                                      Updated = result.Created
                                  });

            resultsQuery = filteredSource as Result[] ?? filteredSource.ToArray();

            var ids = resultsQuery.Select(r => r.ID).ToArray();

            var entitiesTarget = new MasterEntities(new Uri(TargetUrl));
            entitiesTarget.SendingRequest += OnSendingRequestSource;
            var queryTarget = (from r in entitiesTarget.Items.AsEnumerable()
                               where ids.Contains(r.ID)
                               select r);

            var resultsTarget = queryTarget.ToList();
            var targetItems = (from t in resultsTarget
                               let fields = t.VersionedFields
                               let sharedfields = t.SharedFields
                               let unversionedfields = t.UnversionedFields
                               select new Result()
                               {
                                   ID = t.ID,
                                   CreatedDate = t.Created,
                                   Item = t,
                                   Fields = fields,
                                   UnversionedFields = unversionedfields,
                                   SharedFields = sharedfields
                               });

            foreach (var result in resultsQuery)
            {
                var targetItem = targetItems.SingleOrDefault(i => i.ID.Equals(result.ID));

                if (result.Item != null)
                {
                    entities.LoadProperty(result.Item, "SharedFields");
                    entities.LoadProperty(result.Item, "UnversionedFields");
                    entities.LoadProperty(result.Item, "VersionedFields");

                    if (targetItem != null && targetItem.Item != null)
                    {
                        entitiesTarget.LoadProperty(targetItem.Item, "SharedFields");
                        entitiesTarget.LoadProperty(targetItem.Item, "UnversionedFields");
                        entitiesTarget.LoadProperty(targetItem.Item, "VersionedFields");

                        syncItemSource = ProcessItem(result, entities);
                        sourceContentItem = new ContentDataItem(result.Path, result.Path, result.Item.Name,
                                                                syncItemSource);
                        targetItem.Path = result.Path;
                        targetItem.LanguageCode = result.LanguageCode;
                        targetItem.VersionInfo = result.VersionInfo;
                        SyncItem syncItemTarget = ProcessItem(targetItem, entitiesTarget);

                        var targetContentItem = new ContentDataItem(targetItem.Path, targetItem.Path, result.Item.Name, syncItemTarget);
                        int compareResult = targetContentItem.CompareTo(sourceContentItem);
                        
                        branchCommands.AddRange(sourceContentItem.GenerateDiff(targetContentItem, sourceContentItem, compareResult));
                    }
                    else
                    {
                        syncItemSource = ProcessItem(result, entities);
                        sourceContentItem = new ContentDataItem(result.Path, result.Path, result.Item.Name, syncItemSource);
                        branchCommands.Add(new AddItemCommand(sourceContentItem));
                    }
                }
          

                ItemProcessEventArgs args = new ItemProcessEventArgs();
                if (result.Item != null)
                {
                    args.Message = string.Format("Processed {0}", result.Item.Name);
                    args.ItemName = result.Item.Name;
                }

                OnItemProcessed(args);

                if (result.Item != null) CompareBranches(result.Item.ID, result.Item.Name);
            }

            return resultsQuery;
        }


        public void OnItemProcessed(ItemProcessEventArgs e)
        {
            EventHandler<ItemProcessEventArgs> handler = ItemProcessed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public string GeneratePackage(List<ICommand> filteredCommands)
        {
            var info = new DiffInfo(filteredCommands);
            var fileName = "Sitecore.Meerkat_{0}.update".FormatWith(Guid.NewGuid());
            var filePath = string.Format("{0}/{1}", "/temp", fileName);
            info.Author = "Ian Graham";
            info.Title = string.Format("Diff between folders '{0}' and '{1}'", TargetPath, SourcePath);

            PackageGenerator.GeneratePackage(info, filePath);
            return filePath;
        }


        /// <summary>
        /// Compare each Sitecore Item using data from WCF 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<ICommand> CompareItem(Result result)
        {
            var entitiesSource = new MasterEntities(new Uri(SourceUrl));
            entitiesSource.SendingRequest += OnSendingRequestSource;

            var entitiesTarget = new MasterEntities(new Uri(TargetUrl));
            entitiesTarget.SendingRequest += OnSendingRequestTarget;
            var queryTarget = (from r in entitiesTarget.Items.AsEnumerable()
                               where r.ID.Equals(result.ID)
                               select r);

            var resultsTarget = queryTarget.ToList();
            var targetItems = (from t in resultsTarget
                               let fields = t.VersionedFields
                               let sharedfields = t.SharedFields
                               let unversionedfields = t.UnversionedFields
                               select new Result()
                               {
                                   ID = t.ID,
                                   CreatedDate = t.Created,
                                   Item = t,
                                   Fields = fields,
                                   UnversionedFields = unversionedfields,
                                   SharedFields = sharedfields
                               });

            var commands = new List<ICommand>();

            SyncItem syncItemSource = null;
            ContentDataItem sourceContentItem = null;
            var targetItem = targetItems.SingleOrDefault(i => i.ID.Equals(result.ID));

            // If item to compare in history table still has item data then in needs to be added or changed
            if (result.Item != null)
            {
                entitiesSource.AttachTo("Items", result.Item);
                entitiesSource.AttachTo("Histories", result);

                entitiesSource.LoadProperty(result.Item, "SharedFields");
                entitiesSource.LoadProperty(result.Item, "UnversionedFields");
                entitiesSource.LoadProperty(result.Item, "VersionedFields");

                // Item exists in both instances so get changes if any
                if (targetItem != null && targetItem.Item != null)
                {
                    entitiesTarget.LoadProperty(targetItem.Item, "SharedFields");
                    entitiesTarget.LoadProperty(targetItem.Item, "UnversionedFields");
                    entitiesTarget.LoadProperty(targetItem.Item, "VersionedFields");

                    syncItemSource = ProcessItem(result, entitiesSource);
                    sourceContentItem = new ContentDataItem(result.Path, result.Path, result.Item.Name,
                                                            syncItemSource);
                    targetItem.Path = result.Path;
                    targetItem.LanguageCode = result.LanguageCode;
                    targetItem.VersionInfo = result.VersionInfo;
                    SyncItem syncItemTarget = ProcessItem(targetItem, entitiesTarget);

                    var targetContentItem = new ContentDataItem(targetItem.Path, targetItem.Path, result.Item.Name, syncItemTarget);
                    int compareResult = targetContentItem.CompareTo(sourceContentItem);
                    commands.AddRange(sourceContentItem.GenerateDiff(targetContentItem, sourceContentItem, compareResult));
                }
                else
                {
                    // Item doesn't exist in target so create add command
                    syncItemSource = ProcessItem(result, entitiesSource);
                    sourceContentItem = new ContentDataItem(result.Path, result.Path, result.Item.Name, syncItemSource);
                    commands.Add(new AddItemCommand(sourceContentItem));
                }
            } // If no item data then it has been deleted so check if on target and add delete comment
            else if (result.Item == null && (targetItem != null && targetItem.Item != null))
            {
                targetItem.Path = result.Path;
                syncItemSource = ProcessItem(targetItem, entitiesSource);
                if (syncItemSource != null)
                {
                    sourceContentItem = new ContentDataItem(targetItem.Path, targetItem.Path, targetItem.Item.Name, syncItemSource);
                    commands.AddRange(sourceContentItem.GenerateDeleteCommand());
                }
            }

            return commands;
        }




        /// <summary>
        /// Converts an entity from the WCF into a SyncItem that can be used to generate update commands
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private SyncItem ProcessItem(Result result, MasterEntities context)
        {
            if (result != null && result.Item != null)
            {
                var definition = new SitecoreData.ItemDefinition(SitecoreData.ID.Parse(result.ID), result.Item.Name, SitecoreData.ID.Parse(result.Item.TemplateID), global::Sitecore.Data.ID.Parse(result.Item.ParentID));
                var fieldList = new SitecoreData.FieldList();

                foreach (var field in result.SharedFields)
                {
                    fieldList.Add(SitecoreData.ID.Parse(field.FieldId), field.Value);
                }

                var itemData = new SitecoreData.ItemData(definition, (Language)Language.Parse("en-GB"), new SitecoreData.Version(1), fieldList);
                var item = new SitecoreData.Items.Item(SitecoreData.ID.Parse(result.ID), itemData, SitecoreData.Database.GetDatabase(Database));

                var syncItem = ItemSynchronization.BuildSyncItem(item);
                foreach (var version in syncItem.GetLatestVersions())
                {
                    if (version != null)
                    {
                        foreach (
                            var field in
                                result.Fields.Where(
                                    f =>
                                    f.Version.ToString().Equals(version.Version) &&
                                    f.Language.Equals(version.Language, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            context.LoadProperty(field, "ItemVersion");

                            var syncfield = new SyncField();
                            syncfield.FieldID = field.FieldId.ToString();
                            syncfield.FieldKey = field.FieldId.ToString();

                            if (field.ItemVersion != null)
                            {
                                syncfield.FieldName = field.ItemVersion.Name;
                            }
                            syncfield.FieldValue = field.Value;
                            version.Fields.Add(syncfield);
                        }
                    }
                }
                return syncItem;

            }
            return null;
        }

        /// <summary>
        /// Event handler to send extra headers to the service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendingRequestSource(object sender, SendingRequestEventArgs e)
        {
            e.Request.Headers.Clear();
            e.Request.Headers.Add("database", Database);
            e.Request.Headers.Add("filePath", SourcePath);
        }

        private void OnSendingRequestTarget(object sender, SendingRequestEventArgs e)
        {
            e.Request.Headers.Clear();
            e.Request.Headers.Add("database", Database);
            e.Request.Headers.Add("filePath", TargetPath);
        }
    }
}
