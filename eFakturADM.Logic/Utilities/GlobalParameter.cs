using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace eFakturADM.Logic.Utilities
{
    public class GlobalParameter
    {
       
    }

    /// <summary>
    /// message for validation
    /// </summary>
    public partial class ValidationMessage
    {
        public string MandatoryField = "Field {0} must be filled";
        public string DataExists = "{0} has been exists";
        public string ValueGreatedThan = "{0} must greater than {1}";
        public string ValueLessThen = "{0} must less than {1}";
    }

    /// <summary>
    /// message for success or failed result
    /// </summary>
    public partial class ResultMessage
    {
        public string CreateSuccess = "{0} {1} has been created";
        public string CreateFailed = "{0} {1} failed to create";
        public string UpdateSuccess = "{0} {1} has been updated";
        public string UpdateFailed = "{0} {1} failed to update";
        public string Delete_Success = "{0} {1} has been deleted";
        public string Delete_Failed = "{0} {1} failed to deleted";
    }
}
