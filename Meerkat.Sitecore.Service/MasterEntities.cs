using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Data.Entity;

namespace Meerkat.Sitecore.Service
{
    public partial class MasterEntities : DbContext
    {
        public MasterEntities()
            : base(ServiceConfig.GetConnectionString(WebOperationContext.Current.IncomingRequest.Headers["database"], WebOperationContext.Current.IncomingRequest.Headers["filepath"]))
        {
        }
    }
}