//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Meerkat.Sitecore.Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class History
    {
        public System.Guid Id { get; set; }
        public string Category { get; set; }
        public string Action { get; set; }
        public System.Guid ItemId { get; set; }
        public string ItemLanguage { get; set; }
        public int ItemVersion { get; set; }
        public string ItemPath { get; set; }
        public string UserName { get; set; }
        public string TaskStack { get; set; }
        public string AdditionalInfo { get; set; }
        public System.DateTime Created { get; set; }
    
        public virtual Item Item { get; set; }
    }
}
