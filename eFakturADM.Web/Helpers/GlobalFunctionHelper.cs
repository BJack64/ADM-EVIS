using System;
using System.Linq;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Web.Helpers
{
    public class GlobalFunctionHelper
    {
        public static SelectList GetMonthList(bool isAddDefault, string defaultText, int? selectedValue = null)
        {
            var monthData = MasaPajaks.GetAll();
            var monthList = new List<SelectListItem>();
            if (isAddDefault)
            {
                monthList.Add(new SelectListItem() { Value = "0", Text = defaultText, Selected = false });
            }
            var dtNow = selectedValue ?? DateTime.Now.Month;
            monthList.AddRange(monthData.Select(item => new SelectListItem()
            {
                Value = item.MonthNumber.ToString(),
                Text = item.MonthName,
                Selected = item.MonthNumber == dtNow
            }));

            return new SelectList(monthList, "Value", "Text", dtNow);

        }

        public static SelectList GetMasaPajakOpenList()
        {
            var monthData = MasaPajaks.GetOpenRegular();
            var monthList = new List<SelectListItem>();
            var dtNow = DateTime.Now;
            monthList.AddRange(monthData.Select(item => new SelectListItem()
            {
                Value = item.MonthNumber.ToString(),
                Text = item.MonthName,
                Selected = item.MonthNumber == dtNow.Month
            }));

            return new SelectList(monthList, "Value", "Text");
        }

        public static SelectList GetMasaPajakCloseList()
        {
            var monthData = MasaPajaks.GetCloseRegular();
            var monthList = new List<SelectListItem>();
            var dtNow = DateTime.Now;
            monthList.AddRange(monthData.Select(item => new SelectListItem()
            {
                Value = item.MonthNumber.ToString(),
                Text = item.MonthName,
                Selected = item.MonthNumber == dtNow.Month
            }));

            return new SelectList(monthList, "Value", "Text");
        }

        public static SelectList GetTahunPajakOpenList(int masaPajak)
        {
            var dbData = TahunPajaks.GetOpenRegularByMasaPajak(masaPajak);
            var monthList = new List<SelectListItem>();
            var dtNow = DateTime.Now;
            monthList.AddRange(dbData.Select(item => new SelectListItem()
            {
                Value = item.Year.ToString(),
                Text = item.Year.ToString(),
                Selected = item.Year == dtNow.Year
            }));

            return new SelectList(monthList, "Value", "Text");
        }

        public static SelectList GetFpTypeList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FPType>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpTypeListFpDigantiOutstanding(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FPType>();
            datList.AddRange(fpTypeEnums.Where(c => c != ApplicationEnums.FPType.ScanManual).Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpStatusDjpList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FpStatusDjp>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpStatusOutstanding(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FpStatusOutstanding>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpStatusPayment(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FpStatusPayment>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpSource(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FpSource>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = fpTypeEnum.ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetCreatedCsvList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.CreatedCsvSource>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = (fpTypeEnum == ApplicationEnums.CreatedCsvSource.Done).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpdoStatusDjpList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpdoTypeEnums = GeneralCategories.GetByCategories(ApplicationEnums.EnumGeneralCategory.FpDigantiOutstandingStatus);
            foreach (var items in fpdoTypeEnums)
            {
                datList.Add(new SelectListItem() { Value = items.Code, Text = items.Name, Selected = false });
            }
          
            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFpdoRemarkList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpdoTypeEnums = GeneralCategories.GetByCategoriesWithExclude(ApplicationEnums.EnumGeneralCategory.FpDigantiOutstandingRemarks, "1");
            foreach (var items in fpdoTypeEnums)
            {
                datList.Add(new SelectListItem() { Value = items.Name, Text = items.Name, Selected = false });
            }

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetFillingIndexTypeSearchList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.FillingIndexType>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = ((int)fpTypeEnum).ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }


        public static SelectList GetLogSapStatusList(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var stats = EnumHelper.GetValues<ApplicationEnums.SapStatusLog>();
            var lstStatus = (from sapStatusLog in stats
                             let statDesc = EnumHelper.GetDescription(sapStatusLog)
                             select new SelectListItem()
                             {
                                 Value = ((int)sapStatusLog).ToString(),
                                 Text = statDesc
                             }).ToList();
            datList.AddRange(lstStatus);
            return new SelectList(datList, "Value", "Text");
        }

        public static List<FPJenisTransaksi> GetJenisTransaksi()
        {
            return FPJenisTransaksis.GetByFCode(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm));
        }

        public static List<FPKdJenisTransaksi> GetKdJenisTransaksiByFCode()
        {
            var dbData = FPKdJenisTransaksis.GetByFCode(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm));
            return dbData;
        }

        public static SelectList GetAllPic(bool isAddDefault, string defaultText)
        {
            var dats = Users.Get();
            var datLists = new List<SelectListItem>();
            if (isAddDefault)
            {
                datLists.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }
            datLists.AddRange(dats.Select(item => new SelectListItem()
            {
                Value = item.UserName,
                Text = item.UserName
            }));

            return new SelectList(datLists, "Value", "Text");
        }

        public static SelectList StatusPelaporan(bool isAddDefault, string defaultText)
        {
            var lst = new List<SelectListItem>();
            if (isAddDefault)
            {
                lst.Add(new SelectListItem() { Value = "0", Text = defaultText, Selected = false });
            }

            lst.Add(new SelectListItem() { Value = "1", Text = "Sudah Dilapor", Selected = false });
            lst.Add(new SelectListItem() { Value = "2", Text = "Belum Dilapor", Selected = false });

            return new SelectList(lst, "Value", "Text",defaultText);

        }

        public static SelectList GetValidasiStatus(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.StatusValidasi>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = fpTypeEnum.ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetValidasiCheckingStatus(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.CheckingStatusValidasi>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = fpTypeEnum.ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }

        public static SelectList GetByPassOption(bool isAddDefault, string defaultText)
        {
            var datList = new List<SelectListItem>();
            if (isAddDefault)
            {
                datList.Add(new SelectListItem() { Value = "", Text = defaultText, Selected = true });
            }

            var fpTypeEnums = EnumHelper.GetValues<ApplicationEnums.ByPassOption>();
            datList.AddRange(fpTypeEnums.Select(fpTypeEnum => new SelectListItem()
            {
                Value = fpTypeEnum.ToString(),
                Text = EnumHelper.GetDescription(fpTypeEnum)
            }));

            return new SelectList(datList, "Value", "Text");
        }
    }
}