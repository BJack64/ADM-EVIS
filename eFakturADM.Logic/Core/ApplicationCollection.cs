using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Web;
using System.Web.Script.Serialization;

namespace eFakturADM.Logic.Core
{

    /// <summary>
    /// Provides the possibility to wrap SQL specific object to business logic object and collection.
    /// </summary>
    /// <typeparam name="TObject">Type of business object which must implement IApplicationObject interface.</typeparam>
    /// <typeparam name="TStore">Type of database object which must inherit from DbBase class.</typeparam>
    /// <typeparam name="TProcedure">Type of database command object which must inherit from SpBase class.</typeparam>
    public abstract class ApplicationCollection<TObject, TProcedure>
        where TObject : IApplicationObject
        where TProcedure : SpBase
    {
        /// <summary>
        /// Executes SQL statement and wraps database objects to .NET collection. 
        /// </summary>
        /// <param name="Sp">Bussines logic command object instance.</param>
        /// <returns>Collection of bussines logic object instances.</returns>
        protected static List<TObject> GetApplicationCollection(TProcedure Sp)
        {
            List<TObject> _collection = new List<TObject>();

            using (IDataReader _dr = (IDataReader)Sp.GetType().InvokeMember("ExecuteReader", BindingFlags.InvokeMethod, null, Sp, null))
            {
                while (_dr.Read())
                {
                    TObject _object = (TObject)Activator.CreateInstance(typeof(TObject));
                    _collection.Add(_object);
                    _object.Load(_dr);
                }

                if (_dr != null)
                    _dr.Close();

                Sp.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, Sp, null);
            }

            return _collection;
        }

        protected static DataSet GetApplicationDataSet(SpBase Sp)
        {

            DataSet _collection = new DataSet();

            /*using (DataSet _dr = (DataSet)Sp.GetType().InvokeMember("ExecuteDataset", BindingFlags.InvokeMethod, null, Sp, null))
            {
                _collection = _dr;

                Sp.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, Sp, null);
            }*/

            _collection = Sp.ExecuteDataSet("test");

            return _collection;
        }

        /// <summary>
        /// Executes SQL statement and wraps database object to .NET object. 
        /// </summary>
        /// <param name="Sp">Bussines logic command object instance.</param>
        /// <returns>Instance of bussines logic object.</returns>
        protected static TObject GetApplicationObject(SpBase Sp)
        {
            TObject _object;

            using (IDataReader _dr = Sp.ExecuteReader())
            {
                if (_dr.Read())
                {
                    _object = (TObject)Activator.CreateInstance(typeof(TObject));
                    _object.Load(_dr);
                }
                else
                    _object = (TObject)Activator.CreateInstance(typeof(TObject));

                if (_dr != null)
                    _dr.Close();

                Sp.Close();
            }

            return (TObject)_object;
        }

        /// <summary>
        /// Creates instance of class T from JSON string.
        /// </summary>
        /// <typeparam name="T">Class type.</typeparam>
        /// <param name="input">JSON representation of object.</param>
        /// <returns>Instance of class T</returns>
        public static T FromJson<T>(string input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(input);
        }

        /// <summary>
        /// Creates JSON string from bussines object.
        /// </summary>
        /// <param name="input">Object instance.</param>
        /// <returns>JSON representation of an object.</returns>
        public static string ToJson(object input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(input);
        }

        /// <summary>
        /// Creates JSON string from list of bussines objects.
        /// </summary>
        /// <param name="Object">Object collection.</param>
        /// <returns>JSON representation of object collection.</returns>
        public static string JsonCollection(List<TObject> Object)
        {
            String JsonResponse = "\"Message\":\"\", \"Data\":{1}";
            String JsonResponseException = "\"Message\":{0}, \"StackTrace\":{1}, \"Data\":[]";
            StringBuilder sb = new StringBuilder();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(List<TObject>));

                    using (XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(ms))
                    {
                        json.WriteObject(ms, Object);
                        writer.Flush();

                        String Json = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                        sb.Append("{");
                        sb.Append(String.Format(JsonResponse, "", Json));
                        sb.Append("}");

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Append("{");
                sb.Append(String.Format(JsonResponseException, ToJson(ex.Message) + (ex.InnerException != null ? ToJson(ex.InnerException.Message) : ""), ToJson(ex.StackTrace)));
                sb.Append("}");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates JSON string from bussines object.
        /// </summary>
        /// <param name="Object">Object instance.</param>
        /// <returns>JSON representation of an object.</returns>
        public static string JsonItem(TObject Object)
        {
            String JsonResponse = "\"Message\":\"\", \"Data\":{1}";
            String JsonResponseException = "\"Message\":{0}, \"StackTrace\":{1}, \"Data\":null";
            StringBuilder sb = new StringBuilder();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TObject));

                    using (XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(ms))
                    {
                        json.WriteObject(ms, Object);
                        writer.Flush();

                        String Json = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                        sb.Append("{");
                        sb.Append(String.Format(JsonResponse, "", Json));
                        sb.Append("}");

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Append("{");
                sb.Append(String.Format(JsonResponseException, ToJson(ex.Message) + (ex.InnerException != null ? ToJson(ex.InnerException.Message) : ""), ToJson(ex.StackTrace)));
                sb.Append("}");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets table name by collection type.
        /// </summary>
        protected static string Table
        {
            get { return ApplicationTables.TableByTypeName(typeof(TObject).ToString()); }
        }
    }
}
