using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class ReportSuratPemberitahuanMasa
    {
        public long Id { get; set; }
        public int MasaPajak { get; set; }
        public string NamaMasaPajak { get; set; }
        public int TahunPajak { get; set; }
        public int Versi { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public partial class ReportSuratPemberitahuanMasa : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetLongField(dr, "Id");
            MasaPajak = DBUtil.GetIntField(dr, "MasaPajak");
            NamaMasaPajak = DBUtil.GetCharField(dr, "NamaMasaPajak");
            TahunPajak = DBUtil.GetIntField(dr, "TahunPajak");
            Versi = DBUtil.GetIntField(dr, "Versi");

            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            IsValid = true;
            return IsValid;
        }
    }
}
