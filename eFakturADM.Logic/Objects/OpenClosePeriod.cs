using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class OpenClosePeriod
    {
        public int TotalItems { get; set; }
        public string StatusRegularText { get; set; }
        public string StatusSp2Text { get; set; }
        public string MonthName { get; set; }
    }
    public partial class OpenClosePeriod
    {
        public int OpenClosePeriodId { get; set; }
        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
        public bool StatusRegular { get; set; }
        public bool StatusSp2 { get; set; }
        public string DocumentSP2 { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public partial class OpenClosePeriod : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {

            IsValid = false;

            OpenClosePeriodId = DBUtil.GetIntField(dr, "OpenClosePeriodId");
            MasaPajak = DBUtil.GetIntField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntField(dr, "TahunPajak");
            StatusRegular = DBUtil.GetBoolField(dr, "StatusRegular");
            StatusSp2 = DBUtil.GetBoolField(dr, "StatusSp2");
            DocumentSP2 = DBUtil.GetCharField(dr, "DocumentSP2");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");
            StatusRegularText = DBUtil.GetCharField(dr, "StatusRegularText");
            StatusSp2Text = DBUtil.GetCharField(dr, "StatusSp2Text");
            MonthName = DBUtil.GetCharField(dr, "MonthName");
            
            IsValid = true;
            return IsValid;

        }
    }
}
