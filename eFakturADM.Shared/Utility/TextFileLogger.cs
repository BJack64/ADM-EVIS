using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace eFakturADM.Shared.Utility
{
    public class TextFileLogger
    {
        protected Mutex m_mutex;					// The mutex to synchronize (and protect) the logging
        protected StreamWriter m_stream;			// Instance of stream writer for the output log
        protected string m_strApplicationName;		// The name of the calling application
        protected string m_strHostName;				// The name of the host we are logging for
        protected string filePath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fullPath">The full path to the log file.</param>
        public TextFileLogger(string hostname, string applicationName)
        {

            // Initialize defaults
            m_strApplicationName = applicationName;
            m_strHostName = hostname;

            // Append to the specified file
            DateTime datet = DateTime.Now;

            // Create the synchronization mutex
            m_mutex = new Mutex();

            string dir = ConfigurationManager.AppSettings["LogFolderPath"];

            this.filePath = dir + hostname + "-Log-" + datet.ToString("yyyy-MM-dd") + ".txt";

            if (!File.Exists(filePath))
            {
                FileStream files = File.Create(filePath);
                files.Close();
            }

        }

        /// <summary>
        /// Add an entry to the log
        /// </summary>
        /// <param name="s">The string to add to the log</param>
        /// <param name="_params">Arguments to a formatted "s" value</param>
        public void Add(string s, params object[] _params)
        {
            // Wait for the mutex to become available
            m_mutex.WaitOne();

            string strEntry;		// Syslog-style log entry

            // Format the log entry
            strEntry = string.Format("{0} {1} {2} : {3}",
                DateTime.Now.ToString("MMM dd HH:mm:ss"),
                m_strHostName,
                m_strApplicationName,
                s
            );

            try
            {
                m_stream = File.AppendText(this.filePath);
                m_stream.WriteLine(strEntry);
                // Write the formatted string to the log
                m_stream.WriteLine(strEntry);

                // Write any pending text
                m_stream.Flush();

                // Release the mutex
                m_mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }


        }

        /// <summary>
        /// The calling application name
        /// </summary>
        public string applicationName
        {
            get { return m_strApplicationName; }
            set { m_strApplicationName = value; }
        }

        /// <summary>
        /// The calling application host name
        /// </summary>
        public string hostName
        {
            get { return m_strHostName; }
            set { m_strHostName = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    }


    public class LogFactory
    {

        public enum LogType
        {
            TextFileLog,
            DatabaseLog,
            EventLog
        };

        ///<summary>
        /// The logger factory that encapsulate log operation.  
        ///</summary>
        ///<param name="hostName">The string hostname of the application</param>
        ///<param name="appName">The string application name</param>
        ///<param name="message">The string log message</param>
        ///<param name="type">The type of log to be writen</param>
        public static void CreateLog(string hostName, string appName, string message, LogType? type)
        {
            if (type == LogType.TextFileLog)
            {
                TextFileLogger logger = new TextFileLogger(hostName, appName);
                logger.Add(message);
            }
        }

        public static void CreateLog(string hostName, string appName, Exception e)
        {
            var msg = e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException;
            CreateLog(hostName, appName, msg, LogType.TextFileLog);
        }

    }
}
