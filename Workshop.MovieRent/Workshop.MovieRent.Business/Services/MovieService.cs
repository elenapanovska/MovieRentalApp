using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Workshop.MovieRent.Business.Helpers;
using Workshop.MovieRent.Business.Loaders;
using Workshop.MovieRent.Business.Menus;
using Workshop.MovieRent.Data.Database;
using Workshop.MovieRent.Data.Models;

namespace Workshop.MovieRent.Business.Services
{
    public class MovieService
    {
        private MovieRepository _movieRepository;

        public MovieService()
        {
            _movieRepository = new MovieRepository();
        }

        public void ViewMovieList(User user)
        {
            string errorMessage = string.Empty;
            var movies = _movieRepository.GetAllMovies();


            bool isFinished = false;
            while (!isFinished)
            {
                Screen.ClearScreen();
                Screen.ErrorMessage(errorMessage);
                if (movies.Count != 0)
                {
                    PrintMoviesInfo(movies);
                }
                Screen.OrderingMenu();
                var selection = InputParser.ToInteger(0, 9);

                switch (selection)
                {
                    case 1:
                        movies = _movieRepository.GetAllMovies();
                        break;
                    case 2:
                        movies = _movieRepository.OrderByGenre();
                        break;
                    case 3:
                        var genre = InputParser.ToGenre();
                        movies= _movieRepository.GetByGenre(genre);
                        break;
                    case 4:
                        movies = _movieRepository.OrderByRelaseDate();
                        break;
                    case 5:
                        Console.WriteLine("Enter year:");
                        var year = InputParser.ToInteger(
                            _movieRepository.GetAllMovies().Min(_movie => _movie.ReleaseDate.Year),
                            DateTime.Now.Year - 1
                            );
                        movies = _movieRepository.GetByYear(year);
                        break;
                    case 6:
                        movies= _movieRepository.OrederByAvaibility();
                        break;
                    case 7:
                        movies = _movieRepository.GetAvailableMovies();
                        break;
                    case 8:
                        Console.WriteLine("Enter search phrase:");
                        string titlePart = Console.ReadLine();
                        movies = _movieRepository.SearchMoviesByTitle(titlePart);
                        break;
                    case 9:
                        try
                        {
                            RentVideo(user);
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;

                        }
                        break;
                    case 0:
                        isFinished = !isFinished;
                        break;
                }
            }
        }

        private void RentVideo(User user)
        {
            Console.Write("Enter movie id:");
            var movieId = InputParser.ToInteger( 
                _movieRepository.GetAllMovies().Min(_movie => _movie.Id),
                _movieRepository.GetAllMovies().Max(_movie => _movie.Id));

            var movie = _movieRepository.GetMovieById(movieId);            
            if (movie != null)
            {
                if(user.RentedMovies.Any(_rental => _rental.Movie.Id == movieId))
                {
                    throw new Exception($"Already rented {movie.Title} please return it first!");
                }

                //var listOfRentedMoviesIds = user.RentedMovies.Select(rental => rental.Movie.Id).ToList();
                //if (listOfRentedMoviesIds.Contains(movieId))
                //{
                //    throw new Exception($"Already rented {movie.Title} please return it first!");
                //}

                if (!movie.IsAvailable)
                {
                    throw new Exception($"Movie {movie.Title} is not aviable at the moment!");
                    
                }
                Console.WriteLine($"Are you sure you want to rent {movie.Title}? y/n");
                bool confirm = InputParser.ToConfirm();
                if (!confirm)
                {
                    return;
                }
                Console.WriteLine("Renting movie please wait...");
                LoadingHelpers.Spinner();
                movie.Quantity--;
                if(movie.Quantity == 0)
                {
                    movie.IsAvailable = !movie.IsAvailable;
                }
                user.RentedMovies.Add(new RentalInfo(movie));
                Console.WriteLine("Succesfully rented movie");
                Thread.Sleep(2000);
            }
            else
            {
                throw new Exception($"No movie was found with {movieId} id!");
            }
        }

        private void PrintMoviesInfo(List<Movie> movies)
        {
            foreach (var movie in movies)
            {
                string aviability = movie.IsAvailable ? "Yes" : "No";
                Console.WriteLine(
                    String.Format("Rent id: {0} Title: {1} Release date: {2} Genre: {3} Avaiable: {4} Quantity: {5}",
                    movie.Id, movie.Title, movie.ReleaseDate.ToString("MMMM dd yyyy"),
                    movie.Genre, aviability, movie.Quantity));
            }
        }

        public void ViewRentedVideos(User user)
        {
            string errorMessage = string.Empty;
            var rentals = user.RentedMovies;
            bool isFinished = false;
            while (!isFinished)
            {
                try
                {
                    Screen.ClearScreen();
                    Screen.ErrorMessage(errorMessage);

                    if (rentals.Count != 0)
                    {
                        var movies = rentals.Select(_rental => _rental.Movie).ToList();
                        PrintMoviesInfo(movies);
                    }
                    else
                    {
                        Console.WriteLine("You have not rented any videos");
                    }
                    Screen.RentedMenu();
                    int selection = InputParser.ToInteger(0, 2);
                    switch (selection)
                    {
                        case 1:
                            rentals = user.RentedMovies;
                            break;
                        case 2:
                            ReturnMovie(user);
                            break;
                        case 0:
                            isFinished = !isFinished;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
        }

        private void ReturnMovie(User user)
        {
            if (user.RentedMovies.Count == 0)
            {
                throw new Exception("You do not have any rented videos!");
            }
            Console.WriteLine("Enter movie id in order to return a video");
            var movieId = InputParser.ToInteger(1, int.MaxValue);

            var rental = user.RentedMovies.FirstOrDefault(_rental => _rental.Movie.Id == movieId);
            LoadingHelpers.ShowSimplePercentage();
            if (rental != null)
            {
                rental.DateReturned = DateTime.Now;
                var movie = _movieRepository.GetMovieById(movieId);
                if(movie.Quantity == 0)
                {
                    movie.IsAvailable = !movie.IsAvailable;
                }
                movie.Quantity++;

                user.RentedMovies.Remove(rental);
                user.RentedMoviesHistory.Add(rental);

                Console.WriteLine("Succesfully returned.");
                Thread.Sleep(2000);
            }
            throw new Exception("You do not have that movie rented. Please enter valid movie id!");
        }

        public void ViewRentedHistoryVideos(User user)
        {
            if(user.RentedMoviesHistory.Count == 0)
            {
               throw new Exception("You do not have any videos rented history");
            }
            PrintRentedInfo(user.RentedMoviesHistory);

            Console.WriteLine("To h=go back press 0");
            int selection = InputParser.ToInteger(0, 0);
            if (selection == 0)
            {
                return;
            }
            
            

        }

        private void PrintRentedInfo(List<RentalInfo> rentals)
        {
            foreach (var rental in rentals)
            {
                Console.WriteLine($"{rental.Movie.Title} rented from {rental.DateRented} to {rental.DateReturned}");
            }
        }
    }
}
