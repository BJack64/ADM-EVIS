using System;
using System.Runtime.Serialization;

namespace eFakturADM.ExternalLib.Model
{
    [DataContract]
    public class UpdateCategoryMetadataRequest
    {
        [DataMember(Name = "339014_8")]
        public string ReportingDate { get; set; }
    }
}
