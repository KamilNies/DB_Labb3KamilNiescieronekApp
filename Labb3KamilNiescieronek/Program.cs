using Labb3KamilNiescieronek.Data;
using Labb3KamilNiescieronek.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Labb3KamilNiescieronek
{
    class Program
    {
        static void Main(string[] args)
        {
            //Note to self change Record mentions to rows, makes it clearer
            #region Test space
            //string testing = "albums";
            //InsertRow(testing);
            //Console.ReadKey();
            string testing = "albums";
            DeleteRow(testing);
            Console.ReadKey();
            #endregion Test space
            #region Check DB
            //Check connection. Create DB if connection does not exist
            Console.WriteLine("Checking connection. Please wait...");
            using (var context = new Labb3KamilNiescieronekContext())
            {
                if (!context.Database.CanConnect())
                {
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine("No connection found. Creating DB Labb3KamilNiescieronek. Please wait...");
                    try
                    {
                        var sql = File.ReadAllText("../../../script.sql");
                        context.Database.EnsureCreated();
                        context.Database.ExecuteSqlRaw(sql);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Something went wrong: ", e); //Dont forget to catch exception later on
                    }

                    Console.WriteLine(new string('-', 100));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("DB Labb3KamilNiescieronek created successfully.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine("Connection valid.");
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
                            Console.WriteLine(new string('-', 100));
                            ReadTable(parameters[1]);
                            ShowOptions(parameters[1]);
                            TableOptionsPrompt(parameters[1]);
                            return;
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the table name that you wish to display: ");
                            string tableName = Console.ReadLine()
                                .Trim(new char[] { ' ', '-', '.', ',', ';', '<', '>' }).ToLower();
                            Console.WriteLine(new string('-', 100));
                            ReadTable(tableName);
                            ShowOptions(tableName);
                            TableOptionsPrompt(tableName);
                            return;
                        }
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
            Console.WriteLine("| Choose any of the following parameters: ");
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
            Console.WriteLine(" | Choose any of the following parameters: ");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-tables");
            Console.WriteLine("-select <table name>");
            Console.WriteLine("-add");
            Console.WriteLine("-update");
            Console.WriteLine("-delete");
            Console.WriteLine("-options");
            Console.WriteLine("-clear");
            Console.WriteLine("-help");
            Console.WriteLine("-exit");
            Console.WriteLine(new string('-', 100));
        }
        private static void TableHeader(string table)
        {
            Console.WriteLine(new string('-', 100));
            Console.Write("Labb3KamilNiescieronek DB | ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{table.Substring(0, 1).ToUpper() + table.Substring(1, table.Length - 1)} table");
            Console.ResetColor();
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
                            RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {albums.Count() - RecordCount + 1}): ");
                            min = IntTryParser(Console.ReadLine(), 1, albums.Count() - RecordCount + 1);
                        }

                        //Table header
                        TableHeader(table);

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
                            RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {artists.Count() - RecordCount + 1}): ");
                            min = IntTryParser(Console.ReadLine(), 1, artists.Count() - RecordCount + 1);
                        }

                        //Table header
                        TableHeader(table);

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
                            RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {playlists.Count() - RecordCount + 1}): ");
                            min = IntTryParser(Console.ReadLine(), 1, playlists.Count() - RecordCount + 1);
                        }

                        //Table header
                        TableHeader(table);

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
                            RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {playlistTrack.Count() - RecordCount + 1}): ");
                            min = IntTryParser(Console.ReadLine(), 1, playlistTrack.Count() - RecordCount + 1);
                        }

                        //Table header
                        TableHeader(table);

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
                            RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                            Console.WriteLine(new string('-', 100));
                            Console.Write($"Input the row number you wish to start at (max {tracks.Count() - RecordCount + 1}): ");
                            min = IntTryParser(Console.ReadLine(), 1, tracks.Count() - RecordCount + 1);
                        }

                        //Disclaimer
                        Console.WriteLine(new string('-', 100));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The table {table} contains too many columns that can be displayed at once. \n" +
                            $"The columns related to the genre and media tables will be excluded from the results. \n" +
                            $"Other columns such as albumId, bytes and price will also be omitted from the results.");
                        Console.ResetColor();

                        //Table header
                        TableHeader(table);

                        //Column header
                        columnHeader = $"Id\t{properties[1]}\t\t\t\t{properties[5]}\t\t\t\t\t{properties[6]}";
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine(columnHeader);
                        Console.WriteLine(new string('-', 100));

                        //The EllipseString method ensures that the table is properly formatted i.e. string is not too large
                        if (tracks.Count() > 25)
                        {
                            for (int i = min - 1; i < RecordCount + min - 1; i++)
                            {
                                Console.WriteLine($"{tracks[i].TrackId}\t{EllipseString(tracks[i].Name, 3, -2)}" +
                                    $"{EllipseString(tracks[i].Composer, 4, -3)}\t{tracks[i].Milliseconds}");
                            }
                        }
                        else
                        {
                            foreach (var t in tracks)
                            {
                                Console.WriteLine($"{t.TrackId}\t{EllipseString(t.Name, 3, -2)}" +
                                    $"{EllipseString(t.Composer, 4, -3)}\t{t.Milliseconds}");
                            }
                        }
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions();
                        break;
                }
            }
        }
        private static void ReadJoinedTable(Labb3KamilNiescieronekContext db, int userInput)
        {
            var tracks = db.Tracks
                .Join(
                    db.PlaylistTracks,
                    t => t.TrackId,
                    pt => pt.TrackId,
                    (t, pt) => new
                    {
                        TrackId = t.TrackId,
                        Name = t.Name,
                        AlbumId = t.AlbumId,
                        MediaTypeId = t.MediaTypeId,
                        GenreId = t.GenreId,
                        Composer = t.Composer,
                        Milliseconds = t.Milliseconds,
                        Bytes = t.Bytes,
                        UnitPrice = t.UnitPrice,
                        PlaylistId = pt.PlaylistId
                    }
                )
                .Where(x => x.PlaylistId == userInput).ToList();

            int RecordCount = 0;
            int min = tracks.Count();
            if (tracks.Count() > 25)
            {
                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"The joined table contains a total of {tracks.Count()} tracks.");
                Console.WriteLine("Only a maximum of 25 tracks may be displayed at a time.");
                Console.WriteLine(new string('-', 100));
                Console.Write("Input the number of tracks you wish to display: ");
                RecordCount = IntTryParser(Console.ReadLine(), 1, 25);
                Console.WriteLine(new string('-', 100));
                Console.Write($"Input the row number you wish to start at (max {tracks.Count() - RecordCount + 1}): ");
                min = IntTryParser(Console.ReadLine(), 1, tracks.Count() - RecordCount + 1);
            }

            Console.WriteLine(new string('-', 100));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The joined table contains too many columns that can be displayed at once. \n" +
                $"The columns related to the genre and media tables will be excluded from the results. \n" +
                $"Other columns such as albumId, playlistId bytes and price will also be omitted.");
            Console.ResetColor();

            TableHeader($"tracks in playlist {userInput}");

            string columnHeader = "Id\tName\t\t\t\tComposer\t\t\t\t\tMilliseconds";
            Console.WriteLine(new string('-', 100));
            Console.WriteLine(columnHeader);
            Console.WriteLine(new string('-', 100));

            if (tracks.Count() > 25)
            {
                for (int i = min - 1; i < RecordCount + min - 1; i++)
                {
                    Console.WriteLine($"{tracks[i].TrackId}\t{EllipseString(tracks[i].Name, 3, -2)}" +
                        $"{EllipseString(tracks[i].Composer, 4, -3)}\t\t{tracks[i].Milliseconds}");
                }
            }
            else
            {
                foreach (var t in tracks)
                {
                    Console.WriteLine($"{t.TrackId}\t{EllipseString(t.Name, 3, -2)}" +
                        $"{EllipseString(t.Composer, 4, -3)}\t{t.Milliseconds}");
                }
            }
        }
        private static int InputArtistPrompt(Labb3KamilNiescieronekContext db, List<Album> album)
        {
            var artists = db.Artists.ToList();
            while (true)
            {
                Console.Write("Input an ArtistId: ");
                string artistInputId = Console.ReadLine().TrimStart();
                int artistId = 0;
                if (artistInputId == string.Empty)
                {
                    Console.WriteLine(new string('-', 100));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Prompt canceled. Unable to add last album to table.");
                    Console.ResetColor();
                    return 0;
                }
                else
                {
                    artistId = IntTryParser(artistInputId, album.Min(a => a.ArtistId), album.Max(a => a.ArtistId));
                }
                if (artistId > 0 && artistId <= album.Max(a => a.ArtistId))
                {
                    return artistId;
                }
            }
        }
        private static void InsertRow(string table)
        {
            Console.WriteLine("Loading. Please wait...");
            using (var db = new Labb3KamilNiescieronekContext())
            {
                switch (table)
                {
                    case "album":
                    case "albums":
                        var albums = db.Albums.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        int counter = 0;
                        while (true)
                        {
                            Console.Write("Input album name: ");
                            string albumName = Console.ReadLine().TrimStart();
                            if (albumName == string.Empty)
                            {
                                break;
                            }

                            if (albums.Any(a => a.Title.ToLower() == albumName.ToLower()))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Album {albumName} already exists.");
                                Console.ResetColor();
                            }
                            else
                            {
                                var album = new Album()
                                {
                                    AlbumId = albums.OrderBy(x => x.AlbumId).Last().AlbumId + 1,
                                    Title = albumName,
                                    ArtistId = InputArtistPrompt(db, albums)
                                };

                                if (album.ArtistId == 0)
                                {
                                    break;
                                }

                                counter++;
                                db.Add(album);
                                db.SaveChanges();
                                albums.Add(album);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Album {album.Title} added to the table.");
                                Console.ResetColor();
                            }
                        }

                        if (counter == 1)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} album was added to the artists table.");
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} albums were added to the artists table.");
                        }
                        ShowOptions(table);
                        break;
                    case "artist":
                    case "artists":
                        var artists = db.Artists.ToList();
                        var itemArtist = new Artist();
                        if (artists.Count > 0)
                        {
                            itemArtist = artists[artists.Count - 1];
                        }
                        Console.WriteLine(new string('-', 100));
                        counter = 0;
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        while (true)
                        {
                            Console.Write("Input artist name: ");
                            string artistName = Console.ReadLine().TrimStart();
                            if (artistName == string.Empty)
                            {
                                break;
                            }

                            if (artists.Any(a => a.Name.ToLower() == artistName.ToLower()))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Artist {artistName} already exists.");
                                Console.ResetColor();
                            }
                            else
                            {
                                counter++;
                                var artist = new Artist()
                                {
                                    ArtistId = itemArtist.ArtistId + counter,
                                    Name = artistName
                                };


                                db.Add(artist);
                                db.SaveChanges();
                                artists.Add(artist);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Artist {artist.Name} added to the table.");
                                Console.ResetColor();
                            }
                        }
                        if (counter == 1)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} artist was added to the artists table.");
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} artists were added to the artists table.");
                        }
                        ShowOptions(table);
                        break;
                    case "playlist":
                    case "playlists":
                        var tracks = db.Tracks.ToList();
                        var playlists = db.Playlists.ToList();
                        var itemPlaylist = new Playlist();
                        if (playlists.Count > 0)
                        {
                            itemPlaylist = playlists[playlists.Count - 1];
                        }
                        Console.WriteLine(new string('-', 100));

                        string playlistName = string.Empty;
                        while (true)
                        {
                            Console.Write("Name your playlist: ");
                            playlistName = Console.ReadLine().TrimStart();
                            if (playlists.Any(a => a.Name.ToLower() == playlistName.ToLower()))
                            {
                                Console.WriteLine("Invalid input. That playlist name already exists.");
                            }
                            else if (playlistName == string.Empty)
                            {
                                Console.WriteLine("The playlist name cannot be an empty string.");
                            }
                            else
                            {
                                break;
                            }
                        }

                        var playlist = new Playlist()
                        {
                            PlaylistId = itemPlaylist.PlaylistId + 1,
                            Name = playlistName
                        };

                        Console.WriteLine(new string('-', 100));
                        db.Add(playlist);
                        db.SaveChanges();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Playlist {playlist.Name} added successfully to the DB.");
                        Console.ResetColor();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Insert tracks into the playlist? (y/n): ");
                        bool answer = YesNoPrompt();
                        if (answer)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Display track table for additional help? (y/n): ");
                            answer = YesNoPrompt();
                            Console.WriteLine(new string('-', 100));
                            if (answer)
                            {
                                ReadTable("tracks");
                                Console.WriteLine(new string('-', 100));
                            }

                            Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                            counter = 0;
                            bool condi = true;
                            var inputTrack = new List<int>();
                            while (condi)
                            {
                                string input = string.Empty;
                                int output = 0;
                                bool parsed = false;
                                do
                                {
                                    Console.Write("Input the TrackId of the song you wish to add: ");
                                    input = Console.ReadLine();
                                    if (input == string.Empty)
                                    {
                                        condi = false;
                                        break;
                                    }

                                    parsed = int.TryParse(input, out output);
                                    if (!parsed || !tracks.Any(t => t.TrackId == output))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Cannot find TrackId.");
                                        Console.ResetColor();
                                    }
                                    else if (inputTrack.Contains(output))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("This track already exists.");
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        string AddQuery = $"INSERT INTO music.playlist_track VALUES ({itemPlaylist.PlaylistId + 1}, {output})";
                                        db.Database.ExecuteSqlRaw(AddQuery);
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"TrackId {output} added to the playlist.");
                                        Console.ResetColor();
                                        inputTrack.Add(output);
                                        counter++;
                                    }
                                } while (!parsed || !tracks.Any(t => t.TrackId == output));
                            }
                            if (counter == 1)
                            {
                                Console.WriteLine(new string('-', 100));
                                Console.WriteLine($"{counter} track was added to the playlist.");
                            }
                            else
                            {
                                Console.WriteLine(new string('-', 100));
                                Console.WriteLine($"{counter} tracks were added to the playlist.");
                            }
                        }
                        ShowOptions(table);
                        break;
                    case "playlist_track":
                    case "playlist_tracks":
                        tracks = db.Tracks.ToList();

                        var playlistTrack = db.PlaylistTracks.ToList();
                        itemPlaylist = new Playlist();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display playlist table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("playlist");
                            Console.WriteLine(new string('-', 100));
                        }

                        Console.Write("Select PlaylistId: ");
                        int playlistId = 
                            IntTryParser(Console.ReadLine(), playlistTrack.Min(x => x.PlaylistId), playlistTrack.Max(x => x.PlaylistId));
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        counter = 0;
                        bool condition = true;
                        var inputTracker = new List<int>();
                        while (condition)
                        {
                            string input = string.Empty;
                            int output = 0;
                            bool parsed = false;
                            do
                            {
                                Console.Write("Input the TrackId of the song you wish to add: ");
                                input = Console.ReadLine();
                                if (input == string.Empty)
                                {
                                    condition = false;
                                    break;
                                }

                                parsed = int.TryParse(input, out output);
                                if (!parsed || !tracks.Any(t => t.TrackId == output))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Cannot find TrackId.");
                                    Console.ResetColor();
                                }
                                else if (playlistTrack.Where(x => x.PlaylistId == playlistId).Any(z => z.TrackId == output))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("This track already exists.");
                                    Console.ResetColor();
                                }
                                else if (inputTracker.Contains(output))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("This track already exists.");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    string addQuery = $"INSERT INTO music.playlist_track VALUES ({playlistId}, {output})";
                                    db.Database.ExecuteSqlRaw(addQuery);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"TrackId {output} added to the playlist.");
                                    Console.ResetColor();
                                    inputTracker.Add(output);
                                    counter++;
                                }
                            } while (!parsed || !tracks.Any(t => t.TrackId == output));
                        }
                        if (counter == 1)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} track was added to the playlist.");
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} tracks were added to the playlist.");
                        }
                        ShowOptions(table);
                        break;
                    case "track":
                    case "tracks":
                        tracks = db.Tracks.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        counter = 0;
                        int milli = 0;
                        string trackName = string.Empty;
                        string composer = string.Empty;
                        while (true)
                        {
                            Console.Write("Input track name: ");
                            trackName = Console.ReadLine().TrimStart();
                            if (trackName == string.Empty)
                            {
                                break;
                            }

                            if (tracks.Any(a => a.Name.ToLower() == trackName.ToLower()))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Track {trackName} already exists.");
                                Console.ResetColor();
                            }
                            else
                            {
                                while (true)
                                {
                                    Console.Write("Input composer: ");
                                    composer = Console.ReadLine().TrimStart();
                                    if (composer == string.Empty)
                                    {
                                        Console.WriteLine(new string('-', 100));
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Prompt canceled. Unable to add last track to table.");
                                        Console.ResetColor();


                                        if (counter == 1)
                                        {
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine($"{counter} track was added to the track table.");
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine("The AlbumId, GenreId and Bytes column were set to null.");
                                            Console.WriteLine("The MediaTypeId and UnitPrice columns were set to 1.");
                                        }
                                        else
                                        {
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine($"{counter} tracks were added to the track table.");
                                            if (counter != 0)
                                            {
                                                Console.WriteLine(new string('-', 100));
                                                Console.WriteLine("The AlbumId, GenreId and Bytes column were set to null.");
                                                Console.WriteLine("The MediaTypeId and UnitPrice columns were set to 1.");
                                            }

                                        }
                                        ShowOptions(table);
                                        return;
                                    }

                                    Console.Write("Input track length in milliseconds: ");
                                    string milliseconds = Console.ReadLine();

                                    if (composer == string.Empty)
                                    {
                                        Console.WriteLine(new string('-', 100));
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Prompt canceled. Unable to add last track to table.");
                                        Console.ResetColor();

                                        if (counter == 1)
                                        {
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine($"{counter} track was added to the track table.");
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine("The AlbumId, GenreId and Bytes column were set to null.");
                                            Console.WriteLine("The MediaTypeId and UnitPrice columns were set to 1.");
                                        }
                                        else
                                        {
                                            Console.WriteLine(new string('-', 100));
                                            Console.WriteLine($"{counter} tracks were added to the track table.");
                                            if (counter != 0)
                                            {
                                                Console.WriteLine(new string('-', 100));
                                                Console.WriteLine("The AlbumId, GenreId and Bytes column were set to null.");
                                                Console.WriteLine("The MediaTypeId and UnitPrice columns were set to 1.");
                                            }
                                        }
                                        ShowOptions(table);
                                        return;
                                    }

                                    milli = IntTryParser(milliseconds, 1, 9999999);
                                    break;

                                }

                                var track = new Track()
                                {
                                    TrackId = tracks.OrderBy(x => x.TrackId).Last().TrackId + 1,
                                    Name = trackName,
                                    AlbumId = null,
                                    MediaTypeId = 1,
                                    GenreId = null,
                                    Composer = composer,
                                    Milliseconds = milli,
                                    Bytes = null,
                                    UnitPrice = 1
                                };

                                counter++;
                                db.Add(track);
                                db.SaveChanges();
                                tracks.Add(track);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"TrackId {track.TrackId} added to the tracks table.");
                                Console.ResetColor();
                            }
                            
                        }

                        if (counter == 1)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} track was added to the track table.");
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{counter} tracks were added to the track table.");
                        }
                        ShowOptions(table);
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions(table);
                        break;
                }
            }
        }
        private static void UpdateRow(string table)
        {
            Console.WriteLine("Loading. Please wait...");
            using (var db = new Labb3KamilNiescieronekContext())
            {
                switch (table)
                {
                    case "album":
                    case "albums":
                        var albums = db.Albums.ToList();
                        var artists = db.Artists.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display albums table for additional help? (y/n): ");
                        bool answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("album");
                            Console.WriteLine(new string('-', 100));
                        }
                        int albumId = 0;
                        bool exist = true;
                        Console.Write("Input AlbumId: ");
                        do
                        {
                            albumId = IntTryParser(Console.ReadLine(), albums.Min(a => a.AlbumId), albums.Max(a => a.AlbumId)); ;
                            exist = albums.Any(a => a.AlbumId == albumId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find AlbumId. Input another AlbumId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing title: {EllipseString(albums.SingleOrDefault(a => a.AlbumId == albumId).Title, 7, 0)}");
                        Console.Write("Input new title: ");
                        string newTitle = Console.ReadLine().TrimStart();
                        if (newTitle == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to title.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        var album = albums.SingleOrDefault(a => a.AlbumId == albumId);
                        if (album != null)
                        {
                            album.Title = newTitle;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Title successfully updated.");
                            Console.ResetColor();
                        }
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing ArtistId: {albums.SingleOrDefault(a => a.AlbumId == albumId).ArtistId}");
                        Console.Write("Input new ArtistId: ");
                        string newArtistStr = Console.ReadLine().TrimStart();
                        if (newArtistStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to ArtistId.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newArtistId = IntTryParser(newArtistStr, artists.Min(a => a.ArtistId), artists.Max(a => a.ArtistId));
                        album = albums.SingleOrDefault(a => a.AlbumId == albumId);
                        if (album != null)
                        {
                            album.ArtistId = newArtistId;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"ArtistId successfully updated.");
                            Console.ResetColor();
                        }
                        ShowOptions(table);
                        break;
                    case "artist":
                    case "artists":
                        artists = db.Artists.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display artists table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("artists");
                            Console.WriteLine(new string('-', 100));
                        }
                        int artistId = 0;
                        exist = true;
                        Console.Write("Input ArtistId: ");
                        do
                        {
                            artistId = IntTryParser(Console.ReadLine(), artists.Min(a => a.ArtistId), artists.Max(a => a.ArtistId));
                            exist = artists.Any(a => a.ArtistId == artistId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find ArtistId. Input another ArtistId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing name: {EllipseString(artists.SingleOrDefault(a => a.ArtistId == artistId).Name, 7, 0)}");
                        Console.Write("Input new name: ");
                        string newName = Console.ReadLine().TrimStart();
                        if (newName == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to name.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        var artist = artists.SingleOrDefault(a => a.ArtistId == artistId);
                        if (artist != null)
                        {
                            artist.Name = newName;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Name successfully updated.");
                            Console.ResetColor();
                        }
                        ShowOptions(table);
                        break;
                    case "playlist":
                    case "playlists":
                        var playlists = db.Playlists.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display playlist table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("playlist");
                            Console.WriteLine(new string('-', 100));
                        }
                        int playlistId = 0;
                        exist = true;
                        Console.Write("Input PlaylistId: ");
                        do
                        {
                            playlistId = IntTryParser(Console.ReadLine(), playlists.Min(p => p.PlaylistId), playlists.Max(p => p.PlaylistId));
                            exist = playlists.Any(a => a.PlaylistId == playlistId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find PlaylistId. Input another PlaylistId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing name: {EllipseString(playlists.SingleOrDefault(p => p.PlaylistId == playlistId).Name, 7, 0)}");
                        Console.Write("Input new name: ");
                        newName = Console.ReadLine().TrimStart();
                        if (newName == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to name.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        var playlist = playlists.SingleOrDefault(p => p.PlaylistId == playlistId);
                        if (playlist != null)
                        {
                            playlist.Name = newName;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Name successfully updated.");
                            Console.ResetColor();
                        }
                        ShowOptions(table);
                        break;
                    case "playlist_track":
                    case "playlist_tracks":
                        playlists = db.Playlists.ToList();
                        var tracks = db.Tracks.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display playlist table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("playlist");
                            Console.WriteLine(new string('-', 100));
                        }
                        playlistId = 0;
                        exist = true;
                        Console.Write("Select PlaylistId: ");
                        do
                        {
                            playlistId = IntTryParser(Console.ReadLine(), playlists.Min(p => p.PlaylistId), playlists.Max(a => a.PlaylistId)); ;
                            exist = playlists.Any(p => p.PlaylistId == playlistId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find PlaylistId. Input another PlaylistId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);

                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display available tracks in the selected playlist? (y/n): ");
                        answer = YesNoPrompt();
                        if (answer)
                        {
                            ReadJoinedTable(db, playlistId);
                        }
                        Console.WriteLine(new string('-', 100));
                        var tracksj = db.Tracks
                            .Join(
                                db.PlaylistTracks,
                                t => t.TrackId,
                                pt => pt.TrackId,
                                (t, pt) => new
                                {
                                    TrackId = t.TrackId,
                                    Name = t.Name,
                                    AlbumId = t.AlbumId,
                                    MediaTypeId = t.MediaTypeId,
                                    GenreId = t.GenreId,
                                    Composer = t.Composer,
                                    Milliseconds = t.Milliseconds,
                                    Bytes = t.Bytes,
                                    UnitPrice = t.UnitPrice,
                                    PlaylistId = pt.PlaylistId
                                }
                            )
                            .Where(x => x.PlaylistId == playlistId).ToList();
                        while (true)
                        {
                            int trackIdloop = 0;
                            exist = true;
                            Console.Write("Select TrackId from playlist: ");
                            do
                            {
                                trackIdloop = IntTryParser(Console.ReadLine(), tracksj.Min(t => t.TrackId), tracksj.Max(t => t.TrackId)); ;
                                exist = tracksj.Any(t => t.TrackId == trackIdloop);
                                if (!exist)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write($"Cannot find track in playlist {playlistId}. Input another TrackId: ");
                                    Console.ResetColor();
                                }
                            } while (!exist);
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"Selected TrackId: {playlistId}");
                            Console.Write("Input new TrackId from tracks table: ");
                            int newtrackId = 0;
                            exist = true;
                            do
                            {
                                newtrackId = IntTryParser(Console.ReadLine(), tracks.Min(t => t.TrackId), tracks.Max(t => t.TrackId)); ;
                                exist = tracks.Any(t => t.TrackId == trackIdloop);
                                if (!exist)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Cannot find TrackId. Input another TrackId: ");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    string updateQuary =
                                        $"UPDATE music.playlist_track " +
                                        $"SET TrackId = {newtrackId} " +
                                        $"WHERE PlaylistId = {playlistId} AND TrackId = {trackIdloop}";
                                    db.Database.ExecuteSqlRaw(updateQuary);
                                    Console.WriteLine(new string('-', 100));
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"Track successfully replaced.");
                                    Console.ResetColor();
                                }
                            } while (!exist);
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Continue? (y/n): ");
                            answer = YesNoPrompt();
                            Console.WriteLine(new string('-', 100));
                            if (answer)
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        ShowOptions(table);
                        break;
                    case "track":
                    case "tracks":
                        tracks = db.Tracks.ToList();
                        albums = db.Albums.ToList();
                        var genres = db.Genres.ToList();
                        var mediaTypes = db.MediaTypes.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display tracks table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("tracks");
                            Console.WriteLine(new string('-', 100));
                        }
                        int trackId = 0;
                        exist = true;
                        Console.Write("Select TrackId: ");
                        do
                        {
                            trackId = IntTryParser(Console.ReadLine(), tracks.Min(t => t.TrackId), tracks.Max(t => t.TrackId)); ;
                            exist = tracks.Any(t => t.TrackId == trackId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find TrackId. Input another TrackId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);

                        //Name
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing track name: {EllipseString(tracks.SingleOrDefault(t => t.TrackId == trackId).Name, 7, 0)}");
                        Console.Write("Input new track name: ");
                        newName = Console.ReadLine().TrimStart();
                        if (newName == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to track name.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        var track = tracks.SingleOrDefault(t => t.TrackId == trackId);
                        if (track != null)
                        {
                            track.Name = newName;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Track name successfully updated.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Something went wrong. Could not update track name.");
                            Console.ResetColor();
                        }

                        //AlbumId
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing AlbumId: {tracks.SingleOrDefault(t => t.TrackId == trackId).AlbumId}");
                        Console.Write("Input new AlbumId: ");
                        string newAlbumIdStr = Console.ReadLine().TrimStart();
                        if (newAlbumIdStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to AlbumId.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newAlbumId = IntTryParser(newAlbumIdStr, albums.Min(a => a.AlbumId), albums.Max(a => a.AlbumId));
                        album = albums.SingleOrDefault(a => a.AlbumId == trackId);
                        if (track != null)
                        {
                            track.AlbumId = newAlbumId;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"ArtistId successfully updated.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Something went wrong. Could not find new ArtistId.");
                            Console.ResetColor();
                        }

                        //MediaTypeId
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing MediaTypeId: {tracks.SingleOrDefault(t => t.TrackId == trackId).MediaTypeId}");
                        Console.Write("Input new MediaTypeId: ");
                        string newMediaTypeIdStr = Console.ReadLine().TrimStart();
                        if (newMediaTypeIdStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to MediaTypeId.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newMediaTypeId = IntTryParser(newMediaTypeIdStr, mediaTypes.Min(a => a.MediaTypeId), mediaTypes.Max(a => a.MediaTypeId));
                        var mediaType = mediaTypes.SingleOrDefault(m => m.MediaTypeId == newMediaTypeId);
                        if (mediaType != null)
                        {
                            track.MediaTypeId = newMediaTypeId;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"ArtistId successfully updated.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Something went wrong. Could not find new MediaTypeId.");
                            Console.ResetColor();
                        }

                        //GenreId
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing GenreId: {tracks.SingleOrDefault(t => t.TrackId == trackId).GenreId}");
                        Console.Write("Input new GenreId: ");
                        string newGenreIdStr = Console.ReadLine().TrimStart();
                        if (newGenreIdStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to GenreId.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newGenreId = IntTryParser(newGenreIdStr, genres.Min(g => g.GenreId), genres.Max(a => a.GenreId));
                        var genre = genres.SingleOrDefault(g => g.GenreId == newGenreId);
                        if (genre != null)
                        {
                            track.GenreId = newGenreId;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"ArtistId successfully updated.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Something went wrong. Could not find new MediaTypeId.");
                            Console.ResetColor();
                        }

                        //Composer
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing composer: {EllipseString(tracks.SingleOrDefault(t => t.TrackId == trackId).Composer, 7, 0)}");
                        Console.Write("Input new composer: ");
                        string newComposer = Console.ReadLine().TrimStart();
                        if (newComposer == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to composer name.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        if (track != null)
                        {
                            track.Composer = newComposer;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Composer successfully updated.");
                            Console.ResetColor();
                        }

                        //Millseconds
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing track length in millseconds: {tracks.SingleOrDefault(t => t.TrackId == trackId).Milliseconds}");
                        Console.Write("Input new track length in millseconds: ");
                        string newMillsecondsStr = Console.ReadLine().TrimStart();
                        if (newMillsecondsStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to track length.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newMillseconds = IntTryParser(newMillsecondsStr, 1, 9999999);
                        if (track != null)
                        {
                            track.Milliseconds = newMillseconds;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Track length successfully updated.");
                            Console.ResetColor();
                        }

                        //Bytes
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing track size in bytes: {tracks.SingleOrDefault(t => t.TrackId == trackId).Bytes}");
                        Console.Write("Input new track size in bytes: ");
                        string newBytesStr = Console.ReadLine().TrimStart();
                        if (newBytesStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to track size.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newBytes = IntTryParser(newBytesStr, 1, 9999999);
                        if (track != null)
                        {
                            track.Bytes = newBytes;
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Track length successfully updated.");
                            Console.ResetColor();
                        }

                        //UnitPrice
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Press \"enter\" to cancel the prompt below\n");
                        Console.WriteLine($"Existing unit price: {tracks.SingleOrDefault(t => t.TrackId == trackId).UnitPrice:0.00}");
                        Console.Write("Input new unit price (between 1 and 99): ");
                        string newUnitPriceStr = Console.ReadLine().TrimStart();
                        if (newUnitPriceStr == string.Empty)
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Prompt canceled. No changes made to unit price.");
                            Console.ResetColor();
                            ShowOptions(table);
                            return;
                        }
                        int newUnitPrice = IntTryParser(newUnitPriceStr, 1, 99);
                        if (track != null)
                        {
                            track.UnitPrice = Math.Round(((double)newUnitPrice / 100), 2);
                            db.SaveChanges();
                            Console.WriteLine(new string('-', 100));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Unit price successfully updated to {track.UnitPrice:0.00}.");
                            Console.ResetColor();
                        }
                        ShowOptions(table);
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions(table);
                        break;
                }
            }
        }
        private static void DeleteRow(string table)
        {
            /*private static void DeleteCustomer()
             {
                using (var context = new ITHSDemoContext())
                {
                    var customer = context.Customers.First();
                    context.Customers.Remove(customer);
            
                    context.SaveChanges();
                }
            }*/
            Console.WriteLine("Loading. Please wait...");
            using (var db = new Labb3KamilNiescieronekContext())
            {
                switch (table)
                {
                    case "album":
                    case "albums":
                        var albums = db.Albums.ToList();
                        var tracks = db.Tracks.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display albums table for additional help? (y/n): ");
                        bool answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("album");
                            Console.WriteLine(new string('-', 100));
                        }
                        int albumId = 0;
                        bool exist = true;
                        Console.Write("Delete row by selecting AlbumId: ");
                        do
                        {
                            albumId = IntTryParser(Console.ReadLine(), albums.Min(a => a.AlbumId), albums.Max(a => a.AlbumId)); ;
                            exist = albums.Any(a => a.AlbumId == albumId);
                            if (!exist)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Cannot find AlbumId. Input another AlbumId: ");
                                Console.ResetColor();
                            }
                        } while (!exist);

                        var joinTable = db.Tracks
                            .Join(
                                db.PlaylistTracks,
                                t => t.TrackId,
                                pt => pt.TrackId,
                                (t, pt) => new
                                {
                                    TrackId = t.TrackId,
                                    Name = t.Name,
                                    AlbumId = t.AlbumId,
                                    MediaTypeId = t.MediaTypeId,
                                    GenreId = t.GenreId,
                                    Composer = t.Composer,
                                    Milliseconds = t.Milliseconds,
                                    Bytes = t.Bytes,
                                    UnitPrice = t.UnitPrice,
                                    PlaylistId = pt.PlaylistId
                                }
                            )
                            .Where(x => x.AlbumId == albumId).ToList();

                        Console.WriteLine(new string('-', 100));
                        if (tracks.Any(a => a.AlbumId == albumId) && joinTable.Any(j => j.AlbumId == albumId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"AlbumId {albumId} is referenced in the track and playlist_track tables.");
                            Console.ResetColor();
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Delete the album and its associated references? (y/n): ");
                            answer = YesNoPrompt();
                            if (answer)
                            {
                                Console.WriteLine(new string('-', 100));
                                string deleteQuery = 
                                    $"DELETE music.playlist_track " +
                                    $"WHERE TrackId = " +
                                    $"(SELECT DISTINCT pt.TrackId " +
                                    $"FROM music.tracks t " +
                                    $"  JOIN music.playlist_track pt " +
                                    $"      ON pt.TrackId = t.TrackId " +
                                    $"WHERE AlbumId = {albumId}) " +
                                    $"" +
                                    $"DELETE music.tracks " +
                                    $"WHERE AlbumId = {albumId} " +
                                    $"" +
                                    $"DELETE music.albums " +
                                    $"WHERE AlbumId = {albumId}";

                                db.Database.ExecuteSqlRaw(deleteQuery);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"AlbumId {albumId} and its references deleted.");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine(new string('-', 100));
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Breaking operation. No changes were committed.");
                                Console.ResetColor();
                            }
                        }
                        else if (tracks.Any(a => a.AlbumId == albumId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"AlbumId {albumId} is referenced in the track table.");
                            Console.ResetColor();
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Delete the album and its associated references? (y/n): ");
                            answer = YesNoPrompt();
                            if (answer)
                            {
                                Console.WriteLine(new string('-', 100));
                                string deleteQuery =
                                    $"DELETE music.tracks " +
                                    $"WHERE AlbumId = {albumId} " +
                                    $"" +
                                    $"DELETE music.albums " +
                                    $"WHERE AlbumId = {albumId}";

                                db.Database.ExecuteSqlRaw(deleteQuery);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"AlbumId {albumId} and its references deleted.");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine(new string('-', 100));
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Breaking operation. No changes were committed.");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            string deleteQuery =
                                $"DELETE music.albums " +
                                $"WHERE AlbumId = {albumId}";

                            db.Database.ExecuteSqlRaw(deleteQuery);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"AlbumId {albumId} deleted.");
                            Console.ResetColor();
                        }
                        ShowOptions(table);
                        break;
                    case "artist":
                    case "artists":
                        var artists = db.Artists.ToList();
                        albums = db.Albums.ToList();
                        tracks = db.Tracks.ToList();
                        Console.WriteLine(new string('-', 100));
                        Console.Write("Display artists table for additional help? (y/n): ");
                        answer = YesNoPrompt();
                        Console.WriteLine(new string('-', 100));
                        if (answer)
                        {
                            ReadTable("artists");
                            Console.WriteLine(new string('-', 100));
                        }



                        break;
                    case "playlist":
                    case "playlists":

                        break;
                    case "playlist_track":
                    case "playlist_tracks":

                        break;
                    case "track":
                    case "tracks":

                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions(table);
                        break;
                }
            }
            /*private static void DeleteCustomer()
             {
                using (var context = new ITHSDemoContext())
                {
                    var customer = context.Customers.First();
                    context.Customers.Remove(customer);

                    context.SaveChanges();
                }
            }*/
        }
        private static bool YesNoPrompt()
        {
            while (true)
            {
                string answer = Console.ReadLine()
                .Trim(new char[] { ' ', '\"', '\'', '-', '.', ',', ';', '<', '>' }).ToLower();
                if (answer == "y" || answer == "yes")
                {
                    return true;
                }
                else if (answer == "n" || answer == "no")
                {
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid input. Please type \"yes\" or \"no\": ");
                    Console.ResetColor();
                }
            }
        }
        private static int IntTryParser(string input, int min, int max)
        {
            int output = 0;
            bool parsed = false;
            do
            {
                parsed = int.TryParse(input, out output);
                if (!parsed || (output < min || output > max))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"Invalid input. Please input a number between {min} and {max}: ");
                    Console.ResetColor();
                    input = Console.ReadLine();
                }
            } while (!parsed || (output < min || output > max));

            return output;
        }
        private static void TableOptionsPrompt(string table)
        {
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
                            Console.WriteLine(new string('-', 100));
                            ReadTable(parameters[1]);
                            ShowOptions(parameters[1]);
                            TableOptionsPrompt(parameters[1]);
                            return;
                        }
                        else
                        {
                            Console.WriteLine(new string('-', 100));
                            Console.Write("Input the table name that you wish to display: ");
                            string tableName = Console.ReadLine()
                                .Trim(new char[] { ' ', '-', '.', ',', ';', '<', '>' }).ToLower();
                            Console.WriteLine(new string('-', 100));
                            ReadTable(tableName);
                            ShowOptions(tableName);
                            TableOptionsPrompt(tableName);
                            return;
                        }
                    case "-add":
                        Console.WriteLine(new string('-', 100));
                        InsertRow(table);
                        break;
                    case "-update":
                        Console.WriteLine(new string('-', 100));
                        UpdateRow(table);
                        break;
                    case "-delete":
                        Console.WriteLine(new string('-', 100));
                        DeleteRow(table);
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
                        flag = false;
                        break;
                    default:
                        Console.WriteLine(new string('-', 100));
                        Console.WriteLine("Invalid input");
                        ShowOptions(table);
                        break;
                }
            }
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
            Console.WriteLine("-add \t\t\tAdd rows to whatever table that is specified in the header.");
            Console.WriteLine("-update \t\tUpdate the specified row/record.");
            Console.WriteLine("-delete \t\tDelete the entire row specified.");
            Console.WriteLine("-options \t\tPrints the options prompt on the screen.");
            Console.WriteLine("-clear \t\t\tPartially clears the console window.");
            Console.WriteLine("-exit \t\t\tExits application.");
            Console.WriteLine(new string('-', 100));
        }
        #endregion Methods
    }
}
