using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Validation;

namespace MvvmSample.Models
{
    public class SetNameModel : ValidatableModelBase
    {
        public string FirstName { get { return Read<string>(); } set { Write(value); } }
        public string LastName { get { return Read<string>(); } set { Write(value); } }
        public string MidName { get { return Read<string>(); } set { Write(value); } }
        private Gender gender = Gender.Male;
        public Gender Gender { get { return gender; } set { Set(ref gender, value); } }
        private bool isSimpleIdentify;
        public bool IsSimpleIdentify { get { return isSimpleIdentify; } set { Set(ref isSimpleIdentify, value); } }
        public string PasportSeriesAndNumber { get { return Read<string>(); } set { Write(value); } }
        private DateTimeOffset passportIssueDate = DateTime.Today.AddYears(-11);
        public DateTimeOffset PassportIssueDate { get { return passportIssueDate; } set { Set(ref passportIssueDate, value); } }
        public string SnilsNumber { get { return Read<string>(); } set { Write(value); } }
        private DateTimeOffset birthDate = DateTime.Today.AddYears(-25);
        public DateTimeOffset BirthDate { get { return birthDate; } set { Set(ref birthDate, value); } }

        public static SetNameModel Instance()
        {
            var retVal = new SetNameModel();
            retVal.Validator = model =>
            {
                var user = model as SetNameModel;
                if (string.IsNullOrEmpty(user.FirstName))
                    user.Properties[nameof(user.FirstName)].Errors.Add("Name is a required field");
                if (string.IsNullOrEmpty(user.LastName))
                    user.Properties[nameof(user.LastName)].Errors.Add("Last name is a required field");
                if (user.IsSimpleIdentify) // проверки если включена упрощенная идентификация
                {
                    if (string.IsNullOrEmpty(user.PasportSeriesAndNumber))
                        user.Properties[nameof(user.LastName)].Errors.Add("Passport ia a required field");
                    else
                    {
                        if (user.PasportSeriesAndNumber.Length != 10)
                            user.Properties[nameof(user.LastName)].Errors.Add("Passport format is 0000 000000");
                    }

                    if (string.IsNullOrEmpty(user.SnilsNumber))
                        user.Properties[nameof(user.SnilsNumber)].Errors.Add("SNILS number ia a required field");
                    else
                    {
                        if (user.SnilsNumber.Length != 11)
                            user.Properties[nameof(user.SnilsNumber)].Errors.Add("SNILS number format is 000-000-000 00");
                    }
                }
            };

            return retVal;
        }
    }
}
