using System;

namespace eFakturADM.Logic.Utilities
{
    public class ConvertHelper
    {
        public class DateTimeConverter
        {
            
            public static string ToShortDateString(DateTime? dateTime)
            {
                return dateTime.HasValue ? dateTime.Value.ToString("dd/MM/yyyy") : "";
            }

            public static string ToShortDateString(DateTime? dateTime, string format)
            {
                return dateTime.HasValue ? dateTime.Value.ToString(format) : "";
            }

            public static string ToLongDateString(DateTime? dateTime)
            {
                return dateTime.HasValue ? dateTime.Value.ToString("dd/MM/yyyy HH:mm") : "";
            }

            public static string ToLongDateString(DateTime? dateTime, string format)
            {
                return dateTime.HasValue ? dateTime.Value.ToString(format) : "";
            }

        }

        public class DecimalConverter
        {

            public static string ToString(decimal? dVal)
            {
                return dVal.HasValue ? dVal.Value.ToString("N0") : "";
            }

            public static string ToString(decimal? dVal, int decAccuracy)
            {
                return dVal.HasValue ? dVal.Value.ToString(string.Format("N{0}", decAccuracy)) : "";
            }

            public static string ToString(decimal? dVal, int decAccuracy, bool isZeroDefault)
            {
                return dVal.HasValue ? dVal.Value.ToString(string.Format("N{0}", decAccuracy)) : (isZeroDefault ? "0" : "");
            }

            public static string ToString(decimal? dVal, bool isZeroDefault)
            {
                return dVal.HasValue ? dVal.Value.ToString("N0") : (isZeroDefault ? "0" : "");
            }
            
            public static string ToString(decimal? dVal, string nullDefault)
            {
                return dVal.HasValue ? dVal.Value.ToString("N0") : nullDefault;
            }

        }

    }
}
