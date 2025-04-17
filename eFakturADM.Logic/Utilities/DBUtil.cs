using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Globalization;

namespace eFakturADM.Logic.Utilities
{
    /// <summary>
    /// Helps to wrap database result set row to C# object. It has methods which read value from the data REader object put those values
    /// to C# object with appropriate type.
    /// </summary>
    public class DBUtil
    {
        /// <summary>
        /// The class constructor.
        /// </summary>
        public DBUtil()
        {

        }

        /// <summary>
        /// Gets Int32 value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Integer value or MinValue if it is null in the database.</returns>
        public static int GetIntField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (int)dr[field];
            else
                return int.MinValue;
        }
        /// <summary>
        /// Gets Int? value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Integer value or MinValue if it is null in the database.</returns>
        public static int? GetIntNullField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (int?)dr[field];
            else
                return null;
        }
        /// <summary>
        /// Gets Int64 value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Long  value or MinValue if it is null in the database.</returns>
        public static long GetLongField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (long)dr[field];
            else
                return long.MinValue;
        }

        /// <summary>
        /// Gets Nullable Int64 value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Long  value or null in the database.</returns>
        public static long? GetLongNullField(IDataReader dr, string field)
        {
            var cekValue = dr[field];
            if (dr[field] != DBNull.Value)
                return (long?)dr[field];
            else
                return null;
        }

        /// <summary>
        /// Gets string value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>String value or MinValue if it is null in the database.</returns>
        public static string GetCharField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (string)dr[field];
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets Guid value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents the current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Guid value or Guid.Empty if it is null or invalid in the database.</returns>
        public static Guid GetGuidField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
            {
                // Check if the field is already a Guid
                if (dr[field] is Guid)
                {
                    return (Guid)dr[field];  // Return the Guid directly
                }

                // Try to parse the string into a Guid if it's a string representation
                if (Guid.TryParse(dr[field].ToString(), out Guid parsedGuid))
                {
                    return parsedGuid;  // Return the parsed Guid
                }
            }

            return Guid.Empty;  // Return Guid.Empty if it's DBNull or invalid
        }


        /// <summary>
        /// Gets boolean value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Boolean value or MinValue if it is null in the database.</returns>
        public static bool GetBoolField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (bool)dr[field];
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets boolean value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Boolean value or MinValue if it is null in the database.</returns>
        public static bool? GetBoolNullField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (bool)dr[field];
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets byte value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Byte value or MinValue if it is null in the database.</returns>
        public static byte GetByteField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (byte)dr[field];
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets byte array value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Byte array value or MinValue if it is null in the database.</returns>
        public static byte[] GetBinaryField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (byte[])dr[field];
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets decimal value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Decimal value or MinValue if it is null in the database.</returns>
        public static decimal GetDecimalField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
            {
                string str = dr[field].ToString();
                return decimal.Parse(str.Replace(",", "."), new NumberFormatInfo());
            }
            else
            {
                return decimal.MinValue;
            }
        }

        /// <summary>
        /// Gets float value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Float value or MinValue if it is null in the database.</returns>
        public static decimal? GetDecimalNullField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
            {
                string str = dr[field].ToString();
                return decimal.Parse(str.Replace(",", "."), new NumberFormatInfo());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets double value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Double value or MinValue if it is null in the database.</returns>
        public static double GetDoubleField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (double)dr[field];
            else
                return double.MinValue;
        }

        /// <summary>
        /// Gets float value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Float value or MinValue if it is null in the database.</returns>
        public static float GetFloatField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (float)(double)dr[field];
            else
                return float.MinValue;
        }

        /// <summary>
        /// Gets float value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Float value or MinValue if it is null in the database.</returns>
        public static float? GetFloatNullField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (float)(double)dr[field];
            else
                return null;
        }

        /// <summary>
        /// Gets datime value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>DateTime value or MinValue if it is null in the database.</returns>
        public static DateTime GetDateTimeField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (DateTime)dr[field];
            else
            {
                // http://blog.benpowell.co.uk/2011/09/wcf-json-serialization-error-with.html
                DateTime date = DateTime.MinValue;
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                return date;
            }
        }
        /// <summary>
        /// Gets datime value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>DateTime value or null if it is NULL in the database.</returns>
        public static DateTime? GetDateTimeNullField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (DateTime)dr[field];
            else
            {
                // http://blog.benpowell.co.uk/2011/09/wcf-json-serialization-error-with.html
                return null;
            }
        }

        public static TimeSpan GetTimeField(IDataReader dr, string field)
        {
            if (dr[field] != DBNull.Value)
                return (TimeSpan)dr[field];
            else
            {
                // http://blog.benpowell.co.uk/2011/09/wcf-json-serialization-error-with.html
                DateTime dt = new DateTime();
                TimeSpan time = dt.TimeOfDay;

                return time;
            }
        }

        /// <summary>
        /// Gets Int16 value from database field.
        /// </summary>
        /// <param name="dr">DataReader object which represents current resultset row.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Small integer value or MinValue if it is null in the database.</returns>
        public static Int16 GetSmallIntField(IDataReader dr, string field)
        {
            object o = dr[field];
            return (o != DBNull.Value) ? (Int16)o : Int16.MinValue;
        }
    }
}
