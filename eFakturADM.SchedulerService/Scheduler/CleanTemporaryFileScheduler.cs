using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using System;
using System.Configuration;
using System.IO;

namespace eFakturADM.SchedulerService.Scheduler
{
    public class CleanTemporaryFileScheduler
    {
        public static void Run()
        {
            var config = ConfigurationManager.AppSettings["eFakturADM.Connection.String"];
            try
            {
                string folder = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmTempFolder)?.ConfigValue;
                if (!string.IsNullOrEmpty(folder))
                {
                    string folderPath = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).Parent.FullName, folder);
                    Console.WriteLine("Folder Path : " + folderPath);
                    bool exists = Directory.Exists(folderPath);
                    if (exists)
                    {
                        Console.WriteLine("Deleted this folder : " + folderPath);
                        Directory.Delete(folderPath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when Deleted this folder : " + ex?.Message);
            }
        }
    }
}
