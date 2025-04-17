using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eFakturADM.WebApi.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class StringLengthValidatorHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Validate(object model, out string msg)
        {

            List<ValidationResult> errors = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model, null, null);
            msg = string.Empty;
            if (!Validator.TryValidateObject(model, context, errors, true))
            {

                foreach (ValidationResult e in errors)
                    msg = ((string[])e.MemberNames)[0] + e.ErrorMessage;
                return false;
            }

            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool ValidateList(object model, out List<string> msg)
        {

            List<ValidationResult> errors = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model, null, null);
            msg = new List<string>();
            if (!Validator.TryValidateObject(model, context, errors, true))
            {

                foreach (ValidationResult e in errors)
                    msg.Add(((string[])e.MemberNames)[0] + e.ErrorMessage);
                return false;
            }

            return true;

        }
    }
}