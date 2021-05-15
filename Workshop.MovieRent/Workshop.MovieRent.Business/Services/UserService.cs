using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Workshop.MovieRent.Business.Helpers;
using Workshop.MovieRent.Business.Loaders;
using Workshop.MovieRent.Data.Database;
using Workshop.MovieRent.Data.Models;

namespace Workshop.MovieRent.Business.Services
{
    public class UserService
    {
        private UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public User LogIn()
        {
            while (true)
            {
                Console.WriteLine("Enter card id number: ");
                var idCard = InputParser.ToInteger(100, 999);

                var user = _userRepository.GetUserByIdCard(idCard);
                LoadingHelpers.Spinner();

                if(user != null)
                {
                    Console.WriteLine($"Welcome {user.FullName}");
                    return user;
                }
                Console.WriteLine("User card id does not exist");
                Console.WriteLine("Try again y/n");
                if (!InputParser.ToConfirm())
                {
                    Console.WriteLine("Thank you for using rent a video app");
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
                

            }
        }

        public User SignUp()
        {
            while (true)
            {
                Console.WriteLine("Enter full name: ");
                string name = Console.ReadLine();
                Console.WriteLine("Enter date of birth");
                DateTime dob = InputParser.ToDateTime();
                int cardNum = GenerateNewCardNumber();

                Console.WriteLine("Creating user please wait");
                LoadingHelpers.ShowSimplePercentage();

                var user = new User
                {
                    CardNumber = cardNum,
                    FullName = name,
                    DateOfBirth = dob
                };
                Console.WriteLine($"\rWelcome {user.FullName} CardNumber{user.CardNumber}");
                return user;
            }
            
        }

        private int GenerateNewCardNumber()
        {
            const int max = 1000;
            const int min = 100;
            var rand = new Random();
            var takenCardNumbers = _userRepository.GetAllCardNumbers();

            int cardNum;
            do
            {
                cardNum = rand.Next(min, max);
            } 
            while (takenCardNumbers.Contains(cardNum));
            return cardNum;
        }
    }
}
