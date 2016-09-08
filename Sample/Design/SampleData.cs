using System;
using System.Collections.Generic;
using Template10.Interfaces.Validation;

namespace Sample.Design
{
    public static class SampleData
    {
        public delegate void Validate(IValidatableModel model);

        static int id = 1;
        public static IEnumerable<Template10.Samples.ValidationSample.Models.User> SampleUsers(Validate validator)
        {
            yield return BuildUser(id++, "Jonathan", "Archer", validator);
            yield return BuildUser(id++, "T'Pol", "Main", validator);
            yield return BuildUser(id++, "Charles 'Trip'", "Tucker III", validator);
            yield return BuildUser(id++, "Malcolm", "Reed", validator);
            yield return BuildUser(id++, "Hoshi", "Sato Main", validator);
            yield return BuildUser(id++, "Travis", "Mayweather", validator);
            yield return BuildUser(id++, "Doctor", "Phlox", validator);
            yield return BuildUser(id++, "Thy'lek", "Shran", validator);
            yield return BuildUser(id++, "Maxwell", "Forrest", validator);
            yield return BuildUser(id++, "Matt", "Winston", validator);
        }

        static Random _random = new Random((int)DateTime.Now.Ticks);
        public static Template10.Samples.ValidationSample.Models.User BuildUser(int id, string first, string last, Validate validator)
        {
            var user = new Template10.Samples.ValidationSample.Models.User
            {
                Id = id,
                FirstName = first,
                LastName = last,
                Birth = DateTime.Now.Subtract(TimeSpan.FromDays(_random.Next(19, 40) * 365)),
                Email = $"{last}@domain.com",
                IsAdmin = id % 3 == 0,
                Validator = e => validator.Invoke(e)
            };
            user.Validate();
            return user;
        }
    }
}
