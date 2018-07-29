using System;
using System.Globalization;
using System.Windows.Controls;

namespace Urho3DMaterialEditor.Views
{
    public class NumericValidationRule : ValidationRule
    {
        public Type ValidationType { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, $"Value cannot be coverted to string.");
            var canConvert = false;
            if (ValidationType == null)
                return new ValidationResult(true, null);
            switch (ValidationType.Name)
            {
                case "Boolean":
                    var boolVal = false;
                    canConvert = bool.TryParse(strValue, out boolVal);
                    return canConvert
                        ? new ValidationResult(true, null)
                        : new ValidationResult(false, $"Input should be type of boolean");
                case "Int32":
                    var intVal = 0;
                    canConvert = int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out intVal);
                    return canConvert
                        ? new ValidationResult(true, null)
                        : new ValidationResult(false, $"Input should be type of Int32");
                case "Double":
                    double doubleVal = 0;
                    canConvert = double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture,
                        out doubleVal);
                    return canConvert
                        ? new ValidationResult(true, null)
                        : new ValidationResult(false, $"Input should be type of Double");
                case "Single":
                    float floatVal = 0;
                    canConvert = float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out floatVal);
                    return canConvert
                        ? new ValidationResult(true, null)
                        : new ValidationResult(false, $"Input should be type of Float");
                case "Int64":
                    long longVal = 0;
                    canConvert = long.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out longVal);
                    return canConvert
                        ? new ValidationResult(true, null)
                        : new ValidationResult(false, $"Input should be type of Int64");
                default:
                    throw new InvalidCastException($"{ValidationType.Name} is not supported");
            }
        }
    }
}