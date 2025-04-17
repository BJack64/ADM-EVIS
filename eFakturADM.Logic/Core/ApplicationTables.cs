using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Core
{
    /// <summary>
    /// It supports wrapping database objects and keeps correspondence between the C# classes and database tables.
    /// </summary>
    public class ApplicationTables
    {
        /// <summary>
        /// Correspondence between the C# classes and database tables.
        /// </summary>
        private static Hashtable _Names = new Hashtable()
        {            
           
        };

        /// <summary>
        /// Gets Hashtable of correspondence between the C# classes and database tables.
        /// </summary>
        protected static Hashtable Names { get { return _Names; } }

        /// <summary>
        /// Gets table name by class type.
        /// </summary>
        public static string TableName(object ObjectType)
        {
            return (string)Names[ObjectType.GetType().ToString()];
        }

        /// <summary>
        /// Gets table name by type name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string TableByTypeName(string Name)
        {
            return (string)Names[Name];
        }
    }
}

