using System;
using System.Collections.Generic;
using System.IO;


namespace IMDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            var fileName = Path.Combine(directory.FullName, "MovieData.csv");

            var secondFile = Path.Combine(directory.FullName, "movieList.txt");

            bool keepGoing = true;
            while (keepGoing == true)
            {
                
                Console.WriteLine("Welcome to the Internet Movie Database Manager!");
                Console.WriteLine("Select option:");
                Console.WriteLine();
                Console.WriteLine("Press A to view the top 250 films");
                Console.WriteLine("Press S to search");
                Console.WriteLine("Press F to view watch list");
                Console.WriteLine("Press Q to quit");

                string option = Console.ReadLine();

                if (option.ToUpper() == "A") //Calls the GetList method and prints out the ranking and title of all 250 movie objects
                {
                    List<Movies> movies = new List<Movies>();
                    movies = GetList(fileName);
                    foreach (Movies movie in movies)
                    {
                        Console.WriteLine(movie.Ranking + "." + movie.Title);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to return to the main menu");
                    Console.ReadLine();
                    Console.Clear();
                }

                if (option.ToUpper() == "S")        //searches for a movie in the top 250 using the GetList method.
                {                                   //It creates a Movies object if a result is found using the GetSpcificcMovie method
                    Console.Clear();                //and wrties the contents to the terminal.
                    Console.WriteLine("What movie would you like to search for?");
                    string query = Console.ReadLine();

                    var movieList = GetList(fileName);
                    Movies result = new Movies();

                    if (!movieList.Exists(movie => movie.Title.Trim().ToLower() == query.Trim().ToLower()))
                    {
                        Console.WriteLine("Sorry! Title not found.");
                        Console.WriteLine("Press Enter to return to the main menu");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    else
                    {
                        var searchResult = GetSpecifcMovie("MovieData.csv", query);
                        result = movieList.Find(movie => movie.Title.Trim().ToLower() == query.Trim().ToLower());
                        Console.WriteLine();
                        Console.WriteLine(result.Title);
                        Console.WriteLine("Rank: " + result.Ranking);
                        Console.WriteLine("Directed by " + searchResult.Director);
                        Console.WriteLine("Rating: " + searchResult.Rating);
                        Console.WriteLine("Release date: " + searchResult.ReleaseDate);
                        Console.WriteLine("Runtime: " + searchResult.RunTime);
                        Console.WriteLine("Budget: " + searchResult.Budget);
                        Console.WriteLine("Worldwide box office gross: " + searchResult.WorldWideGross);
                        Console.WriteLine("IMDb user score: " + searchResult.IMDbScore);
                        Console.WriteLine();

                        //Assigning values to some of the fields in searchResult causes an index out of range exception to be thrown.
                        //I was not able to fully figure out the cause of this -- perhaps some errors in the CSV file.
                        //As a result, some fields will either be left blank or will have '???' as a placeholder. 

                        
                        bool yay = true;

                        while (yay == true)
                        {
                            Console.WriteLine("Would you like to add this film to your watch list? (Y/N)");

                            string answer = Console.ReadLine();
                            if (answer.ToUpper() == "Y")
                            {

                                File.AppendAllText(secondFile, result.Title + Environment.NewLine); //adds the search result to the watchList.txt file.
                                Console.WriteLine("Movie added!");
                                yay = false;
                            }
                            if (answer.ToUpper() == "N")
                            {
                                yay = false;
                            }

                        }

                        Console.WriteLine("Press Enter to return to the main menu");
                        Console.ReadLine();
                        Console.Clear();
                    }

                }

                if (option.ToUpper() == "F")
                {
                    Console.Clear();

                    using (StreamReader reader = new StreamReader(secondFile)) //Prints the contents of watchList.txt
                    {

                        string line = "";
                        int x = 1;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(x + ". " + line);
                            x++;
                        }

                        
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press E to add a film to your watch list.");
                    Console.WriteLine("Press R to remove a film from your watch list");
                    Console.WriteLine("Press M to return to go back");
                    string answer = Console.ReadLine();

                    if (answer.ToUpper() == "E")
                    {
                        Console.WriteLine("Enter the name of the film you wish to add");
                        string addedMovie = Console.ReadLine();

                        File.AppendAllText(secondFile, addedMovie + Environment.NewLine);
                        Console.WriteLine("Movie added!");
                        Console.WriteLine("Pree Enter to return to the main menu");
                        Console.ReadLine();
                        Console.Clear();
                    }

                    if (answer.ToUpper() == "R")
                    {
                        Console.WriteLine("Type in the film you would like to remove");
                        string remove = Console.ReadLine();
                        
                        try
                        {
                            deleteItem(remove, "movieList.txt");  
                            Console.WriteLine("Film removed!");
                            Console.WriteLine("Press enter to continue");
                            Console.ReadLine();
                            Console.Clear();
                        }

                        catch(Exception ex)
                        {
                            Console.WriteLine("Something went wrong!");
                        }

                    }

                    if (answer.ToUpper() == "M")
                    {
                        Console.WriteLine("Press Enter to return to the main menu");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }

                if (option.ToUpper() == "Q")
                {
                    keepGoing = false;
                }

            }
        }

       
        public static List<string[]> GetMovies(string fileName)
        {
            List<string[]> movieData = new List<string[]>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    movieData.Add(values);
                }
            }
            return movieData;
        }


        //GetList: Returns the ranking and title for the entire IMDb top 250 list.
        //It reads the rank and title from the CSV file, stores the data in
        //a Movies object, and stores all the Movies in a list.

        //For reasons I couldn't quite figure out, I would often get erroneous data
        //from the CSV file that would render the list a mess if I tried to print
        //it to a console.

        //Additionally, I would occsionally get index out of range exceptions thrown
        //when trying to access some of the items in the 'values' array.

        //To fix this, I included a try/catch block to add placeholders to values
        //that were erronoous or couldn't be accessed.
        //Finally, I remove the erroneous entires in the list by using the RemoveAll
        //method.
        public static List<Movies> GetList(string fileName)
        {
            var movieList = new List<Movies>();
            using (var reader = new StreamReader(fileName))
            {

                reader.ReadLine();
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var movie = new Movies();
                    string[] values = line.Split(',');
                    int parseRank;

                    if (int.TryParse(values[0], out parseRank))
                    {
                        movie.Ranking = parseRank + 1;
                    }

                    try
                    {
                        movie.Title = values[1];
                    }

                    catch (IndexOutOfRangeException)
                    {
                        movie.Title = "???";
                    }


                    movieList.Add(movie);
                }
            }
            
            movieList.RemoveAll(movie => movie.Ranking == 0); //erroneous or junk entires always had a rank of 0. This removes all of them from the list
            return movieList;
        }



       

        //GetSpecificMovie: Searches for a specifc movie title within the array of values from the CSV file 
        //and, if it finds a mathc, intantiates a new movie from the class Movies
        //and, if possible, fills in all the properties
        public static Movies GetSpecifcMovie(string fileName, string query)
        {
            var movie = new Movies();
            using (var reader = new StreamReader(fileName))
            {

                reader.ReadLine();
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    
                    string[] values = line.Split(',');

                    try
                    {
                        if (values[1].ToUpper().Trim() == query.ToUpper().Trim())
                        {
                            int parseRank;
                            if (int.TryParse(values[0], out parseRank))
                            {
                                movie.Ranking = parseRank + 1;
                            }

                            try
                            {
                                movie.Title = values[1];
                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.Title = "???";
                            }

                            try
                            {
                                movie.Rating = values[2];
                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.Rating = "???";
                            }

                            try
                            {
                                movie.Genre = values[3];
                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.Title = "???";
                            }

                            try
                            {
                                movie.ReleaseDate = values[4];
                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.ReleaseDate = "???";
                            }

                            try
                            {
                                double parseDouble;
                                if (double.TryParse(values[5], out parseDouble))
                                {
                                    movie.IMDbScore = parseDouble;
                                }

                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.IMDbScore = 0;
                            }

                            try
                            {
                                movie.Director = values[6];
                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.Director = "???";
                            }

                            try
                            {
                                int parseInt;
                                if (int.TryParse(values[7], out parseInt))
                                {
                                    movie.Budget = parseInt;
                                }

                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.Budget = 0;
                            }


                            try
                            {
                                int parseInt;
                                if (int.TryParse(values[8], out parseInt))
                                {
                                    movie.RunTime = parseInt;
                                }

                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.RunTime = 0;
                            }


                            try
                            {
                                int parseInt;
                                if (int.TryParse(values[9], out parseInt))
                                {
                                    movie.WorldWideGross = parseInt;
                                }

                            }

                            catch (IndexOutOfRangeException)
                            {
                                movie.WorldWideGross = 0;
                            }
                        }
                    }
                    catch(IndexOutOfRangeException)
                    {
                        movie.Title = "???";
                    }
                    
                }
            }
            return movie;
        }




        //Deletes an item from a user's saved list.
        //Creates a temporary file and copies the items from the original file
        //to the temporary file. The item the user wants to removed is skipped over and not written to the temp file.
        //Finally, the original file is deleted and the temp file is moved to the original file's location. 
        public static void deleteItem(string remove, string filePath)
        {
            string tempFile = Path.GetTempFileName();
            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(tempFile))
            {
                string line = "";
                while((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().ToUpper() != remove.Trim().ToUpper())
                    {
                        writer.WriteLine(line);
                    }

                }
            }
            File.Delete(filePath);
            File.Move(tempFile, filePath);
        }


    }
}
