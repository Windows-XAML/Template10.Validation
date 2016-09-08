using System;
using Template10.Validation;

namespace Template10.Samples.ValidationSample.Services
{
    public static class Validator
    {
        // this service is for validation logic

        public static void ValidateUser(Models.User user)
        {
            // validate first name
            if (string.IsNullOrEmpty(user.FirstName))
                user.Properties[nameof(user.FirstName)].Errors.Add("First name is required.");
            else if (user.FirstName.Length < 2)
                user.Properties[nameof(user.FirstName)].Errors.Add("First name length is invalid.");

            // validate last name
            if (string.IsNullOrEmpty(user.LastName))
                user.Properties[nameof(user.LastName)].Errors.Add("Last name is required.");
            else if (user.LastName.Length < 2)
                user.Properties[nameof(user.LastName)].Errors.Add("Last name length is invalid.");

            // validate email
            if (string.IsNullOrEmpty(user.Email))
                user.Properties[nameof(user.Email)].Errors.Add("Email is required.");
            else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(user.Email))
                user.Properties[nameof(user.Email)].Errors.Add("A valid Email is required.");

            // validate admin
            if (!user.IsAdmin)
            {
                var date = DateTime.Now.Subtract(TimeSpan.FromDays(365 * 20));
                if (user.Birth > date)
                    user.Properties[nameof(user.Birth)].Errors.Add($"Must be older than 20 years; after {date}");
            }
            var admin = user.Properties[nameof(user.IsAdmin)] as Property<bool>;
            if (admin.OriginalValue && !admin.Value)
                admin.Errors.Add("Administrator cannot be demoted.");
        }
    }
}
