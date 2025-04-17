using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class ByPassValidasi
    {
        public int TotalItems { get; set; }
    }
    public partial class ByPassValidasi
    {
        public string FPdjpID { get; set; }
        public string VendorNPWP { get; set; }
        public string VendorName { get; set; }
        public string FPdjpNumber { get; set; }
        public DateTime? FPdjpDate { get; set; }
        public decimal? DPP { get; set; }
        public decimal? PPN { get; set; }
        public string Sources { get; set; }
        public string Status { get; set; }
        public string CheckingStatus { get; set; }
        public int CheckingCount { get; set; }
        public DateTime? CheckingStart { get; set; }
        public DateTime? CheckingLast { get; set; }
        public string ErrorMessage { get; set; }
        public bool? IsByPass { get; set; }

        public string FPdjpDateString { get; set; }
        public string DPPString { get; set; }
        public string PPNString { get; set; }
        public string CheckingStartString { get; set; }
        public string CheckingLastString { get; set; }

        public DateTime? ByPassOn { get; set; }
        public string ByPassBy { get; set; }
        public string ByPassOnString { get; set; }

    }

    /// <summary>
    /// Provides a wrapper on single item in the Location database table. The properties of this class mapped on appropriate database fields and methods provide saving and loading into/from database.
    /// An instance of this class can be created by new word or loaded from a database using Location class which returns collection of Location by different condition.  
    /// </summary>     
    public partial class ByPassValidasi : ApplicationObject, IApplicationObject
    {
        
        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            IsValid = false;

            FPdjpID = DBUtil.GetCharField(dr, "FPdjpID");
            VendorNPWP = DBUtil.GetCharField(dr, "VendorNPWP");
            VendorName = DBUtil.GetCharField(dr, "VendorName");
            FPdjpNumber = DBUtil.GetCharField(dr, "FPdjpNumber");
            FPdjpDate = DBUtil.GetDateTimeNullField(dr, "FPdjpDate");
            DPP = DBUtil.GetDecimalNullField(dr, "DPP");
            PPN = DBUtil.GetDecimalNullField(dr, "PPN");
            Sources = DBUtil.GetCharField(dr, "Sources");
            Status = DBUtil.GetCharField(dr, "Status");
            CheckingStatus = DBUtil.GetCharField(dr, "CheckingStatus");
            CheckingCount = DBUtil.GetIntField(dr, "CheckingCount");
            CheckingStart = DBUtil.GetDateTimeNullField(dr, "CheckingStart");
            CheckingLast = DBUtil.GetDateTimeNullField(dr, "CheckingLast");
            ErrorMessage = DBUtil.GetCharField(dr, "ErrorMessage");
            IsByPass = DBUtil.GetBoolNullField(dr, "IsByPass");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");

            ByPassOn = DBUtil.GetDateTimeNullField(dr, "ByPassOn");
            ByPassBy = DBUtil.GetCharField(dr, "ByPassBy");

            FPdjpDateString = ConvertHelper.DateTimeConverter.ToShortDateString(FPdjpDate);
            DPPString = ConvertHelper.DecimalConverter.ToString(DPP, 2);
            PPNString = ConvertHelper.DecimalConverter.ToString(PPN, 2);
            CheckingStartString = ConvertHelper.DateTimeConverter.ToLongDateString(CheckingStart);
            CheckingLastString = ConvertHelper.DateTimeConverter.ToLongDateString(CheckingLast);
            ByPassOnString = ConvertHelper.DateTimeConverter.ToLongDateString(ByPassOn);

            IsValid = true;
            return IsValid;
        }

    }
}
