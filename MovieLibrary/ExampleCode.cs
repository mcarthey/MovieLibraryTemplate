﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    public static void MainMethod()
    {
        // path to movie data file
        string file = $"{Environment.CurrentDirectory}/Files/movies.csv";

        // make sure movie file exists
        if (!File.Exists(file))
        {
            Console.WriteLine($"File does not exist: {file}");
        }
        else
        {
            string choice;
            do
            {
                // display choices to user
                Console.WriteLine("1) Add Movie");
                Console.WriteLine("2) Display All Movies");
                Console.WriteLine("Enter to quit");

                // input selection
                choice = Console.ReadLine();
                Console.WriteLine($"User choice: {choice}");

                // create parallel lists of movie details
                // lists must be used since we do not know number of lines of data
                List<UInt64> MovieIds = new List<UInt64>();
                List<string> MovieTitles = new List<string>();
                List<string> MovieGenres = new List<string>();
                // to populate the lists with data, read from the data file
                try
                {
                    StreamReader sr = new StreamReader(file);
                    // first line contains column headers
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        // first look for quote(") in string
                        // this indicates a comma(,) in movie title
                        int idx = line.IndexOf('"');
                        if (idx == -1)
                        {
                            // no quote = no comma in movie title
                            // movie details are separated with comma(,)
                            string[] movieDetails = line.Split(',');
                            // 1st array element contains movie id
                            MovieIds.Add(UInt64.Parse(movieDetails[0]));
                            // 2nd array element contains movie title
                            MovieTitles.Add(movieDetails[1]);
                            // 3rd array element contains movie genre(s)
                            // replace "|" with ", "
                            MovieGenres.Add(movieDetails[2].Replace("|", ", "));
                        }
                        else
                        {
                            // quote = comma in movie title
                            // extract the movieId
                            MovieIds.Add(UInt64.Parse(line.Substring(0, idx - 1)));
                            // remove movieId and first quote from string
                            line = line.Substring(idx + 1);
                            // find the next quote
                            idx = line.IndexOf('"');
                            // extract the movieTitle
                            MovieTitles.Add(line.Substring(0, idx));
                            // remove title and last comma from the string
                            line = line.Substring(idx + 2);
                            // replace the "|" with ", "
                            MovieGenres.Add(line.Replace("|", ", "));
                        }
                    }
                    // close file when done
                    sr.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine($"Movies in file {MovieIds.Count}");

                if (choice == "1")
                {
                    // Add Movie
                    // ask user to input movie title
                    Console.WriteLine("Enter the movie title");
                    // input title
                    string movieTitle = Console.ReadLine();
                    // check for duplicate title
                    List<string> LowerCaseMovieTitles = MovieTitles.ConvertAll(t => t.ToLower());
                    if (LowerCaseMovieTitles.Contains(movieTitle.ToLower()))
                    {
                        Console.WriteLine("That movie has already been entered");
                        Console.WriteLine($"Duplicate movie title {movieTitle}");
                    }
                    else
                    {
                        // generate movie id - use max value in MovieIds + 1
                        UInt64 movieId = MovieIds.Max() + 1;
                        // input genres
                        List<string> genres = new List<string>();
                        string genre;
                        do
                        {
                            // ask user to enter genre
                            Console.WriteLine("Enter genre (or done to quit)");
                            // input genre
                            genre = Console.ReadLine();
                            // if user enters "done"
                            // or does not enter a genre do not add it to list
                            if (genre != "done" && genre.Length > 0)
                            {
                                genres.Add(genre);
                            }
                        } while (genre != "done");
                        // specify if no genres are entered
                        if (genres.Count == 0)
                        {
                            genres.Add("(no genres listed)");
                        }
                        // use "|" as delimeter for genres
                        string genresString = string.Join("|", genres);
                        // if there is a comma(,) in the title, wrap it in quotes
                        movieTitle = movieTitle.IndexOf(',') != -1 ? $"\"{movieTitle}\"" : movieTitle;
                        // display movie id, title, genres
                        Console.WriteLine($"{movieId},{movieTitle},{genresString}");
                        // create file from data
                        StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine($"{movieId},{movieTitle},{genresString}");
                        sw.Close();
                        // add movie details to Lists
                        MovieIds.Add(movieId);
                        MovieTitles.Add(movieTitle);
                        MovieGenres.Add(genresString);
                        // log transaction
                        Console.WriteLine($"Movie id {movieId} added");
                    }
                }
                else if (choice == "2")
                {
                    // Display All Movies
                    // loop thru Movie Lists
                    for (int i = 0; i < MovieIds.Count; i++)
                    {
                        // display movie details
                        Console.WriteLine($"Id: {MovieIds[i]}");
                        Console.WriteLine($"Title: {MovieTitles[i]}");
                        Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                        Console.WriteLine();
                    }
                }
            } while (choice == "1" || choice == "2");
        }

        Console.WriteLine("Program ended");
    }
}
