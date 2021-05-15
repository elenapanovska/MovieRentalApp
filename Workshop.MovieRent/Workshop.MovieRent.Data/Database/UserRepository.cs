using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workshop.MovieRent.Data.Models;

namespace Workshop.MovieRent.Data.Database
{
    public class UserRepository : InMemoryDatabase
    {
        public User GetUserByIdCard(int idCard)
        {
            return Users.FirstOrDefault(_user => _user.CardNumber == idCard);
        }

        public bool CreateUser(User user)
        {
            var beforeCount = Users.Count;
            Users.Add(user);

            return beforeCount != Users.Count;
        }

        public List<int> GetAllCardNumbers()
        {
            return Users.Select(_user => _user.CardNumber).ToList();
        }
    }
}
