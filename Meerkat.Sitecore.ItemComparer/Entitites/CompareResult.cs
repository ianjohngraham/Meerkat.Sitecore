using System;
using System.Collections.Generic;
using Meerkat.Sitecore.ItemComparer.ItemComparerService;

namespace Meerkat.Sitecore.ItemComparer
{
    public class Result
    {
        public string Path {get; set;}
        public Guid ID {get; set;}
        public DateTime CreatedDate {get;set;}
        public Item Item {get;set;}
        public string LanguageCode {get; set;}
        public DateTime Updated { get; set; }
        public int VersionInfo { get; set; }
        public bool IsDirty { get; set; }
        public Item ItemVersion { get; set; }
        public IEnumerable<VersionedField> Fields {get; set;}
        public IEnumerable<SharedField> SharedFields { get; set; }
        public IEnumerable<UnversionedField> UnversionedFields { get; set; }   
    }
}