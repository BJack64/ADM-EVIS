using System;
using System.IO;
using System.Reflection;

namespace WatcherLogger
{
    public class Log
    {
        private static string CurrentTime
        {
            get
            {
                DateTime dateTime = DateTime.Now;
                int i = dateTime.Hour;
                string s1 = i.ToString();
                i = dateTime.Minute;
                string s2 = i.ToString();
                i = dateTime.Second;
                string s3 = i.ToString();
                i = dateTime.Millisecond;
                string s4 = i.ToString();
                return s1 + ":" + s2 + ":" + s3 + "." + s4;
            }
        }

        private static string GetClassName(MethodBase aMethod)
        {
            return aMethod.Module.FullyQualifiedName;
        }

        private static string GetFileLog()
        {
            DateTime dateTime = DateTime.Now;
            int i = dateTime.Year;
            string s1 = i.ToString();
            dateTime = DateTime.Now;
            i = dateTime.Month;
            string s2 = i.ToString();
            s2 = s2.Length == 1 ? "0" + s2 : s2;
            dateTime = DateTime.Now;
            i = dateTime.Day;
            string s3 = i.ToString();
            s3 = s3.Length == 1 ? "0" + s3 : s3;
            return s1 + s2 + s3 + ".txt";
        }

        private static string GetMethodName(MethodBase aMethod)
        {
            return aMethod.Name;
        }

        private static string GetYearFolder()
        {
            string str = DateTime.Now.Year.ToString();
            return (Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\ErrLog\" + str);
        }

        private static bool IsDirectoryExist()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Log.GetYearFolder());
            return directoryInfo.Exists;
        }

        private static bool IsFileLogExist()
        {
            FileInfo info = new FileInfo(GetYearFolder() + @"\" + GetFileLog());
            return info.Exists;
        }

        public static void WriteLog(string Message, MethodBase aMethod)
        {
            bool flag;

            DirectoryInfo directoryInfo = null;
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                try
                {
                    flag = Log.IsDirectoryExist();
                    if (!flag)
                    {
                        directoryInfo = new DirectoryInfo(Log.GetYearFolder());
                        directoryInfo.Create();
                    }
                    string s = Log.GetYearFolder() + "\\" + Log.GetFileLog();
                    flag = !Log.IsFileLogExist();
                    if (!flag)
                        fileStream = new FileStream(s, FileMode.Append, FileAccess.Write);
                    else
                        fileStream = new FileStream(s, FileMode.CreateNew, FileAccess.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.BaseStream.Seek((long)0, SeekOrigin.End);
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("Error Aplikasi");
                    streamWriter.WriteLine(String.Format("\t Nama Method : {0}", Log.GetMethodName(aMethod)));
                    streamWriter.WriteLine(String.Format("\t Nama Class : {0}", Log.GetClassName(aMethod)));
                    streamWriter.WriteLine(String.Format("Pesan Error: {0}", Message));
                    streamWriter.WriteLine(String.Format("Jam Log: {0}", Log.CurrentTime));
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("=====================================================");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch
                { }
            }
            finally
            {
                directoryInfo = null;
                flag = streamWriter == null;
                if (!flag)
                {
                    streamWriter.Dispose();
                    streamWriter = null;
                }
                flag = fileStream == null;
                if (!flag)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }
            }
        }

        public static void JustLog(string Message, MethodBase aMethod)
        {
            bool flag;

            DirectoryInfo directoryInfo = null;
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                try
                {
                    flag = Log.IsDirectoryExist();
                    if (!flag)
                    {
                        directoryInfo = new DirectoryInfo(Log.GetYearFolder());
                        directoryInfo.Create();
                    }
                    string s = Log.GetYearFolder() + "\\" + Log.GetFileLog();
                    flag = !Log.IsFileLogExist();
                    if (!flag)
                        fileStream = new FileStream(s, FileMode.Append, FileAccess.Write);
                    else
                        fileStream = new FileStream(s, FileMode.CreateNew, FileAccess.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.BaseStream.Seek((long)0, SeekOrigin.End);
                    streamWriter.WriteLine();
                    //streamWriter.WriteLine("Error occured in");
                    streamWriter.WriteLine(String.Format("Log: {0}", Message));
                    streamWriter.WriteLine(String.Format("Jam Log: {0}", Log.CurrentTime));
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("=====================================================");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch
                { }
            }
            finally
            {
                directoryInfo = null;
                flag = streamWriter == null;
                if (!flag)
                {
                    streamWriter.Dispose();
                    streamWriter = null;
                }
                flag = fileStream == null;
                if (!flag)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }
            }
        }
    }
}
