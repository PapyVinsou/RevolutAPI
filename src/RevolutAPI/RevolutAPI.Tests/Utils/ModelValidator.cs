using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevolutAPI.Tests.Utils
{
    public class ModelValidator
    {
        public static bool IsValid(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }
    }
}