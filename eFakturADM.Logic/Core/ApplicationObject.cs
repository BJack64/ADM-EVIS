using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Data;


namespace eFakturADM.Logic.Core
{
    /// <summary>
    /// Provides mandatory interface for wrapped objects.
    /// </summary>
    public interface IApplicationObject
    {
        /// <summary>
        /// Loads bussines object properties from DataReader.
        /// </summary>
        /// <param name="dr">Valid instance of DataReader class.</param>
        /// <returns>True if all properties was read fine.</returns>
        bool Load(IDataReader dr);
    }

    /// <summary>
    /// Provides part of logic which wrapped object must inherit.
    /// </summary>
    [DataContract]
    public abstract class ApplicationObject
    {
        /// <summary>
        /// An object was read from database successfully flag;
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// An object was saved in database successfully flag;
        /// </summary>
        public bool WasSaved { get; set; }

        /// <summary>
        /// An object was deleted from database successfully flag;
        /// </summary>
        public bool WasDeleted { get; set; }

        /// <summary>
        /// Message for admin UI.
        /// </summary>
        public String UIMessage { get; set; }

        /// <summary>
        /// Message type for admin UI.
        /// </summary>
        public UIMessageType UIMessageType { get; set; }

        /// <summary>
        /// instance line number in collection.
        /// </summary>
        public int UILineNumber { get; set; }

        /// <summary>
        /// The appropriate to object table name .
        /// </summary>
        protected string Table
        {
            get { return ApplicationTables.TableByTypeName(this.GetType().ToString()); }
        }
    }

    /// <summary>
    /// Provides admin UI message types:
    /// Information;
    /// Success;
    /// Warning;
    /// ErrorOrDanger.
    /// </summary>
    public enum UIMessageType
    {
        /// <summary>
        /// General infoamtion test.
        /// </summary>
        Information = 0,
        /// <summary>
        /// An action was Successfully finished. 
        /// </summary>
        Success = 1,
        /// <summary>
        /// An action was finished but with warnings. 
        /// </summary>
        Warning = 2,
        /// <summary>
        /// An action was failed. 
        /// </summary>
        ErrorOrDanger = 3
    }

    /// <summary>
    /// Provides serializing objects to JSON string.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class APIJson<TObject>
    {
        /// <summary>
        /// Gets JSON string. 
        /// </summary>
        /// <param name="Object">Object type.</param>
        public string Json(TObject Object)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TObject));

                using (XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(ms))
                {
                    json.WriteObject(ms, Object);
                    writer.Flush();

                    return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                }
            }
        }
    }

}
