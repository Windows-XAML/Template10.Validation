using System.Collections.Generic;
using System.Linq;
using Template10.Interfaces.Validation;
using Template10.Samples.ValidationSample.Models;
using Template10.Utils;

namespace Template10.Samples.ValidationSample.Services.UserService
{
    // this service is for interaction with the store/database

    public class UserService
    {
        private static List<User> database;

        public List<User> GetUsers()
        {
            if (database != null)
            {
                return database;
            }
            return database = Sample.Design.SampleData.SampleUsers(Validate).ToList();
        }

        public User CreateUser(int id)
        {
            return GetUsers().AddAndReturn(Sample.Design.SampleData.BuildUser(id, "Red", $"Shirt {id}", Validate));
        }

        public void DeleteUsers(params int[] id)
        {
            if (id != null)
            {
                GetUsers().RemoveAll(x => id.Contains(x.Id));
            }
        }

        void Validate(IValidatableModel user)
        {
            Validator.ValidateUser(user as User);
        }
    }
}
