using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Services;
using System.Data.Services.Common;
using System.Web;

namespace Meerkat.Sitecore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ItemComparerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ItemComparerService.svc or ItemComparerService.svc.cs at the Solution Explorer and start debugging.

    public class ItemComparerService : DataService<MasterEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("Items", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Histories", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("VersionedFields", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("SharedFields", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("UnversionedFields", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }
    }
}
