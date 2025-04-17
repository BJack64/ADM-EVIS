using System.ComponentModel;

namespace eFakturADM.Shared
{
    public enum LogSapStatusEnum
    {
        Failed = 1,
        Success = 2
    }

    public enum AttachmentDocumentCategory
    {
        [Description("Faktur Pajak Terlapor")]
        FakturPajakTerlapor
        
    }
}
