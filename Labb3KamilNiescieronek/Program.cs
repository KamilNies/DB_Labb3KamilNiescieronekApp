using Labb3KamilNiescieronek.Data;
using Labb3KamilNiescieronek.Models;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Linq;

namespace Labb3KamilNiescieronek
{
    class Program
    {
        static void Main(string[] args)
        {
            string test = "playlist";
            bool boolTest = TableOptionsPrompt(test, "-exit");
            if (boolTest)
            {
                Console.WriteLine("Normal return");
            }
            else
            {
                Console.WriteLine("Exit return successful");
            }
            Console.ReadKey();
            #region Check DB
            //Check connection. Create DB if connection does not exist
            Console.WriteLine("Checking connection. Please wait...");
            using (var context = new Labb3KamilNiescieronekContext())
            {
                if (!context.Database.CanConnect())
                {
                    Console.WriteLine("No connection found.");
                    Console.WriteLine("Creating DB Labb3KamilNiescieronek...");
                    try
                    {
                        var sql = File.ReadAllText("../../../script.sql");
                        context.Database.EnsureCreated();
                        using (var command = new SqlConnection(Labb3KamilNiescieronekContext.connectionString))
                        {
                            command.Open();
                            using (var cmd = command.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                            command.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Something went wrong: ", e); //Dont forget to catch exception later on
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nDB Labb3KamilNiescieronek created successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("\nConnection valid.");
                }
            }
            #endregion Check DB
            #region Main
            //Create new try catch in Main
            ShowOptions();
            bool flag = true;
            while (flag)
            {
                string[] parameters = Console.ReadLine()
                    .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                parameters = Array.ConvertAll(parameters, x => x.ToLower());
                switch (parameters[0])
                {
                    case "-table":
                    case "-tables":
                        DisplayTableNames();
                        break;
                    case "-select":
                        if (parameters.Length > 1)
                        {
                            bool secondFlag = true;
                            while (secondFlag)
                            {
                                Console.WriteLine(new string('-', 100));
                                ReadTable(parameters[1]);

                                string[] tableOptions = Console.ReadLine()
                                    .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                                tableOptions = Array.ConvertAll(tableOptions, x => x.ToLower());

                                //Placeholder for TableOptionsPrompt method when done

                                /*
                                ----------------------------------------------------------------------------------------------------
                                Labb3KamilNiescieronek DB | Playlist table | Select any of the following parameters:
                                ----------------------------------------------------------------------------------------------------
                                -tables
                                -select <table name>
                                -add
                                -update <PK_Id>
                                -delete <PK_Id>
                                -options
                                -clear
                                -help
                                -exit
                                ----------------------------------------------------------------------------------------------------
                                */
                            }

                            return; //Maybe change later. Also need a way to return to regular loop. Or do I?
                        }
                        else
                        {
                            #region Ignore for now
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the table name that you wish to display: ");
                            string tableName = Console.ReadLine()
                                .Trim(new char[] { ' ', '-', '.', ',', ';', '<', '>' }).ToLower();
                            Console.WriteLine(new string('-', 100));
                            ReadTable(tableName);
                            #endregion Ignore for now
                        }
                        break;
                    case "-option":
                    case "-options":
                        ShowOptions();
                        break;
                    case "-clear":
                        Console.Clear();
                        ShowOptions();
                        break;
                    case "-help":
                        ShowHelp();
                        break;
                    case "-exit":
                        flag = false;
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions();
                        break;
                }
            }
            #endregion Main
        }
        #region Methods
        private static void ShowOptions()
        {
            Console.WriteLine(new string('-', 100));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Labb3KamilNiescieronek DB ");
            Console.ResetColor();
            Console.WriteLine("| Select any of the following parameters: ");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-tables");
            Console.WriteLine("-select <table name>");
            Console.WriteLine("-options");
            Console.WriteLine("-clear");
            Console.WriteLine("-help");
            Console.WriteLine("-exit");
            Console.WriteLine(new string('-', 100));
        }
        private static void ShowOptions(string table)
        {
            Console.WriteLine(new string('-', 100));
            Console.Write("Labb3KamilNiescieronek DB | ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{table.Substring(0, 1).ToUpper() + table.Substring(1, table.Length - 1)} table");
            Console.ResetColor();
            Console.WriteLine(" | Select any of the following parameters: ");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-tables");
            Console.WriteLine("-select <table name>");
            Console.WriteLine("-add");
            Console.WriteLine("-update <PK_Id>");
            Console.WriteLine("-delete <PK_Id>");
            Console.WriteLine("-options");
            Console.WriteLine("-clear");
            Console.WriteLine("-help");
            Console.WriteLine("-exit");
            Console.WriteLine(new string('-', 100));
        }
        private static void ReadTable(string table)
        {
            Console.WriteLine("Loading table. Please wait...");
            using (var db = new Labb3KamilNiescieronekContext())
            {
                switch (table)
                {
                    case "album":
                    case "albums":
                        var albums = db.Albums.ToList();
                        var properties = typeof(Album)
                            .GetProperties()
                            .Select(p => p.Name)
                            .ToList();

                        int RecordCount = 0, min = albums.Count();
                        if (albums.Count() > 25)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"The table {table} contains a total of {albums.Count()} records.");
                            Console.WriteLine("Only a maximum of 25 records may be displayed at a time.");
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the number of records you wish to display: ");
                            RecordCount = TryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {albums.Count() - RecordCount + 1}): ");
                            min = TryParser(Console.ReadLine(), 1, albums.Count() - RecordCount + 1);
                        }

                        //Column header
                        string columnHeader = $"{properties[0]}\t\t{properties[1]}" + new string('\t', 5) + $"{properties[2]}";
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        //The EllipseString method ensures that the table is properly formatted i.e. string is not too large
                        if (albums.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{albums[i].AlbumId}\t\t{EllipseString(albums[i].Title, 3, -2)}" +
                                    $"\t{albums[i].ArtistId}");
                            }
                        }
                        else
                        {
                            foreach (var a in albums)
                            {
                                Console.WriteLine($"{a.AlbumId}\t\t{EllipseString(a.Title, 3, -2)}" +
                                    $"\t{a.ArtistId}");
                            }
                        }
                 
                        ShowOptions(table);
                        break;
                    case "artist":
                    case "artists":
                        var artists = db.Artists.ToList();
                        properties = typeof(Artist)
                            .GetProperties()
                            .Select(p => p.Name)
                            .ToList();

                        RecordCount = 0;
                        min = artists.Count();
                        if (artists.Count() > 25)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"The table {table} contains a total of {artists.Count()} records.");
                            Console.WriteLine("Only a maximum of 25 records may be displayed at a time.");
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the number of records you wish to display: ");
                            RecordCount = TryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {artists.Count() - RecordCount + 1}): ");
                            min = TryParser(Console.ReadLine(), 1, artists.Count() - RecordCount + 1);
                        }

                        //Column header
                        columnHeader = $"{properties[0]}\t{properties[1]}";
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        //The EllipseString method ensures that the table is properly formatted i.e. string is not too large
                        if (artists.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{artists[i].ArtistId}\t\t{EllipseString(artists[i].Name, 9, 0)}"); ;
                            }
                        }
                        else
                        {
                            foreach (var a in artists)
                            {
                                Console.WriteLine($"{a.ArtistId}\t\t{EllipseString(a.Name, 9, 0)}");
                            }
                        }

                        ShowOptions(table);
                        break;
                    case "playlist":
                    case "playlists":
                        var playlists = db.Playlists.ToList();
                        properties = typeof(Playlist)
                            .GetProperties()
                            .Select(p => p.Name)
                            .ToList();

                        RecordCount = 0;
                        min = playlists.Count();
                        if (playlists.Count() > 25)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"The table {table} contains a total of {playlists.Count()} records.");
                            Console.WriteLine("Only a maximum of 25 records may be displayed at a time.");
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the number of records you wish to display: ");
                            RecordCount = TryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {playlists.Count() - RecordCount + 1}): ");
                            min = TryParser(Console.ReadLine(), 1, playlists.Count() - RecordCount + 1);
                        }

                        //Column header
                        columnHeader = string.Join("\t", properties);
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        //The EllipseString method ensures that the table is properly formatted i.e. string is not too large
                        if (playlists.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{playlists[i].PlaylistId}\t\t{EllipseString(playlists[i].Name, 9, 0)}");
                            }
                        }
                        else
                        {
                            foreach (var p in playlists)
                            {
                                Console.WriteLine($"{p.PlaylistId}\t\t{EllipseString(p.Name, 9, 0)}");
                            }
                        }
                        
                        ShowOptions(table);
                        break;

                    case "playlist_track":
                    case "playlist_tracks":
                        var playlistTrack = db.PlaylistTracks.ToList();
                        properties = typeof(PlaylistTrack)
                            .GetProperties()
                            .Select(p => p.Name)
                            .ToList();

                        RecordCount = 0;
                        min = playlistTrack.Count();
                        if (playlistTrack.Count() > 25)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"The table {table} contains a total of {playlistTrack.Count()} records.");
                            Console.WriteLine("Only a maximum of 25 records may be displayed at a time.");
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the number of records you wish to display: ");
                            RecordCount = TryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {playlistTrack.Count() - RecordCount + 1}): ");
                            min = TryParser(Console.ReadLine(), 1, playlistTrack.Count() - RecordCount + 1);
                        }

                        //Column header
                        columnHeader = $"{properties[0]}\t{properties[1]}";
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        if (playlistTrack.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{playlistTrack[i].PlaylistId}\t\t{playlistTrack[i].TrackId}");
                            }
                        }
                        else
                        {
                            foreach (var pt in playlistTrack)
                            {
                                Console.WriteLine($"{pt.PlaylistId}\t\t{pt.TrackId}");
                            }
                        }

                        ShowOptions(table);
                        break;
                    case "track":
                    case "tracks":
                        var tracks = db.Tracks.ToList();
                        properties = typeof(Track)
                            .GetProperties()
                            .Select(p => p.Name)
                            .ToList();

                        RecordCount = 0;
                        min = tracks.Count();
                        if (tracks.Count() > 25)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"The table {table} contains a total of {tracks.Count()} records.");
                            Console.WriteLine("Only a maximum of 25 records may be displayed at a time.");
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the number of records you wish to display: ");
                            RecordCount = TryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {tracks.Count() - RecordCount + 1}): ");
                            min = TryParser(Console.ReadLine(), 1, tracks.Count() - RecordCount + 1);
                        }

                        //Disclaimer
                        Console.WriteLine(new string('-', 100));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The table {table} contains too many columns that can be displayed at once. \n" +
                            $"The columns related to the genre and media tables will be excluded from the results. \n" +
                            $"Other columns such as albumId, bytes and price will also be omitted from the results.");
                        Console.ResetColor();
                        Console.WriteLine(new string('-', 100));

                        //Column header
                        columnHeader = $"Id\t{properties[1]}\t\t\t{properties[5]}\t\t\t\t\t{properties[6]}";
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        //The EllipseString method ensures that the table is properly formatted i.e. string is not too large
                        if (tracks.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{tracks[i].TrackId}\t{EllipseString(tracks[i].Name, 2, -1)}" +
                                    $"{EllipseString(tracks[i].Composer, 4, -3)}\t{tracks[i].Milliseconds}");
                            }
                        }
                        else
                        {
                            foreach (var t in tracks)
                            {
                                Console.WriteLine($"{t.TrackId}\t{EllipseString(t.Name, 2, -1)}" +
                                    $"{EllipseString(t.Composer, 4, -3)}\t{t.Milliseconds}");
                            }
                        }

                        ShowOptions(table);
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions();
                        break;
                }
            }
        }
        private static int TryParser(string input, int min, int max)
        {
            int output = 0;
            bool parsed = false;
            do
            {
                parsed = int.TryParse(input, out output);
                if (!parsed || (output < min || output > max))
                {
                    Console.Write($"Invalid input. Please input a number between {min} and {max}: ");
                    input = Console.ReadLine();
                }
            } while (!parsed || (output < min || output > max));

            return output;
        }
        private static bool TableOptionsPrompt(string table, params string[] tableOptions)
        {
            switch (tableOptions[0])
            {
                case "-table":
                case "-tables":
                    DisplayTableNames();
                    break;
                case "-select":
                    break;
                case "-add":
                    break;
                case "-update":
                    break;
                case "-delete":
                    break;
                case "-option":
                case "-options":
                    ShowOptions(table);
                    break;
                case "-clear":
                    Console.Clear();
                    ShowOptions(table);
                    break;
                case "-help":
                    ShowHelp(table);
                    break;
                case "-exit":
                    return false;
                default:
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine("Invalid input");
                    ShowOptions();
                    break;
            }
            return true;

            /*
            ----------------------------------------------------------------------------------------------------
            Labb3KamilNiescieronek DB | Playlist table | Select any of the following parameters:
            ----------------------------------------------------------------------------------------------------
            -tables
            -select <table name>
            -add
            -update <PK_Id>
            -delete <PK_Id>
            -options
            -clear
            -help
            -exit
            ----------------------------------------------------------------------------------------------------
            */
        }
        private static string EllipseString(string input, int tabMultiplier, int offset)
        {
            //One \t corresponds to 9 characters worth of 'spaces' in the console 
            int maxStringLength = tabMultiplier * 9;
            if (input != null)
            {
                if (input.Length >= maxStringLength)
                {
                    string shortendInput = input.Substring(0, maxStringLength) + "...";
                    return shortendInput + new string(' ', 4 + offset);
                }
                return input + new string(' ', maxStringLength - input.Length + 7 + offset);
            }
            return new string(' ', maxStringLength + 7 + offset);
        }
        private static void DisplayTableNames()
        {
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("Available tables:\n");
            Console.WriteLine("albums");
            Console.WriteLine("artists");
            Console.WriteLine("playlists");
            Console.WriteLine("playlist_track");
            Console.WriteLine("tracks");
            Console.WriteLine(new string('-', 100));
        }
        private static void ShowHelp()
        {
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-tables \t\tDisplays the available table names in the DB.");
            Console.WriteLine("-select <table name> \tSelect whatever table that you wish to work towards.");
            Console.WriteLine("-options \t\tPrints the options prompt on the screen.");
            Console.WriteLine("-clear \t\t\tPartially clears the console window.");
            Console.WriteLine("-exit \t\t\tExits application.");
            Console.WriteLine(new string('-', 100));
        }
        private static void ShowHelp(string table)
        {
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-tables \t\tDisplays the available table names in the DB.");
            Console.WriteLine("-select <table name> \tSelect whatever table that you wish to work towards.");
            Console.WriteLine("-add \t\t\tAdd records to whatever table is loaded or specified in the header.");
            Console.WriteLine("-update <PK_Id> \t\tUpdate the specified row/record.");
            Console.WriteLine("-delete <PK_Id> \t\tDelete the entire row specified.");
            Console.WriteLine("-options \t\tPrints the options prompt on the screen");
            Console.WriteLine("-clear \t\t\tPartially clears the console window.");
            Console.WriteLine("-exit \t\t\tExits application.");
            Console.WriteLine(new string('-', 100));
        }
        #endregion Methods

        /*/// <summary>
        /// Create - The C in CRUD
        /// </summary>
        private static void InsertCustomer()
        {
            using (var context = new ITHSDemoContext())
            {
                var customer = new Customer()
                {
                    FirstName = "Claes",
                    LastName = "Engelin"
                };

                context.Add(customer);

                customer = new()
                {
                    FirstName = "Anna",
                    LastName = "Engelin"
                };

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Read - The R in CRUD
        /// </summary>
        private static void ReadCustomer()
        {
            using (var context = new ITHSDemoContext())
            {
                var customers = context.Customers.ToList();

                foreach (var c in customers)
                {
                    Console.WriteLine($"{c.Id}  {c.FirstName} {c.LastName}");
                }
            }
            return;
        }

        /// <summary>
        /// Update - The U in CRUD
        /// </summary>
        private static void UpdateCustomer()
        {
            using (var context = new ITHSDemoContext())
            {
                var customer = context.Customers.First();
                customer.FirstName = "Michael";
                customer.LastName = "Jordan";

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete - The D in CRUD
        /// </summary>
        private static void DeleteCustomer()
        {
            using (var context = new ITHSDemoContext())
            {
                var customer = context.Customers.First();
                context.Customers.Remove(customer);

                context.SaveChanges();
            }
        }*/
    }
}
