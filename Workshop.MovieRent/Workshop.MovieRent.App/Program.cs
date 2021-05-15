using System;
using Workshop.MovieRent.Business.Helpers;
using Workshop.MovieRent.Business.Menus;
using Workshop.MovieRent.Business.Services;
using Workshop.MovieRent.Data.Models;

namespace Workshop.MovieRent.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Video Rental";

            var _userService = new UserService();
            var _movieService = new MovieService();
            User user = null;
            string errorMessage = string.Empty;
            

            #region LogIn
            Screen.HomeScreen();

            bool isLoggedIn = false;
            while (!isLoggedIn)
            {
                Screen.StartMenu();
                var startMenuInput = InputParser.ToInteger(1,3);
                switch (startMenuInput)
                {
                    case 1:
                        while (!isLoggedIn)
                        {
                            user = _userService.LogIn();
                            isLoggedIn = !isLoggedIn;
                        }
                        break;
                    case 2:
                        while (!isLoggedIn)
                        {
                            user = _userService.SignUp();
                            isLoggedIn = !isLoggedIn;
                        }
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
            }

            #endregion


            while (true)
            {
                Screen.ClearScreen();
                Screen.ErrorMessage(errorMessage);
                errorMessage = string.Empty;
                Screen.MainMenu(user.FullName);
                var selection = InputParser.ToInteger(1, 4);
                switch (selection)
                {
                    case 1:
                        _movieService.ViewMovieList(user);
                        break;
                    case 2:
                        //View rented videos
                        _movieService.ViewRentedVideos(user);
                        break;
                    case 3:
                        try
                        {
                            _movieService.ViewRentedHistoryVideos(user);
                        }
                        catch (Exception ex)
                        { 
                            errorMessage = ex.Message;
                        }
                        
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                   
                }

            };

           
        }
    }
}
