using System;
using System.IO;

namespace WatcherLogger
{
    public class WriteLog
    {
        public static void CreateLog(string content, string folder, string filename)
        {
            try
            {
                String pathlog = Path.Combine(Temp.LogFolder);
                String pathlog2 = Path.Combine(Temp.LogFolder + "\\" + folder);
                if (!Directory.Exists(pathlog)) Directory.CreateDirectory(pathlog);
                if (!Directory.Exists(pathlog2)) Directory.CreateDirectory(pathlog2);

                FileStream fs = new FileStream(Temp.LogFolder + "\\" + folder + "\\" + filename + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
            }
            catch(Exception ex)
            {
                //WriteLog.CreateLog("Error at : " + DateTime.Now, "ErrorWriteLog");
                //WriteLog.CreateLog(ex.ToString(), "ErrorWriteLog");
                //WriteLog.CreateLog("-----------------------------------------------", "ErrorWriteLog");
                //WriteLog.CreateLog(string.Empty, "ErrorWriteLog");
            }
        }
    }
}
