namespace eFakturADM.Logic.Objects
{
    public class SettingViewModel
    {

        public class EmailSetting
        {
            public string FromAddress { get; set; }
            public string FromDisplay { get; set; }
            public string SmtpHost { get; set; }
            public int SmtpPort { get; set; }
            public bool EnableSSL { get; set; }
            public bool UseDefaultCredential { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string SubjectEmail { get; set; }
        }

        public class EmailServiceSetting
        {
            public int TimeInterval { get; set; }
            public int ProcessInterval { get; set; }
            public int ItemPerBatch { get; set; }
            public int ProcessIntervalPerBatch { get; set; }
        }

        public class FileshareSetting
        {
            //public string FolderUploadDev { get; set; }
            //public string FolderUploadTempDev { get; set; }
            public string FolderUploadTemp { get; set; }
            //public string UsernameFileShare { get; set; }
            //public string PasswordFileShare { get; set; }
            public string FolderUpload { get; set; }
            /// <summary>
            /// in MB
            /// </summary>
            public int MaxUploadSize { get; set; }
            public string AllowedExtension { get; set; }

            public string FolderUploadTempLocalServer { get; set; }
            public string FolderUploadLocalServer { get; set; }

        }

        public class SharePointSettings
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string BaseSite { get; set; }
        }

    }

}
