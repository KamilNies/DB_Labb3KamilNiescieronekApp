using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Labb3KamilNiescieronek.Models;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Labb3KamilNiescieronek.Data
{
    public partial class Labb3KamilNiescieronekContext : DbContext
    {
        public static string connectionString;
        public Labb3KamilNiescieronekContext() : base()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);

            var configuration = configurationBuilder.Build();
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Labb3KamilNiescieronekContext(DbContextOptions<Labb3KamilNiescieronekContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<MediaType> MediaTypes { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("albums", "music");

                entity.Property(e => e.AlbumId).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(160);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_albums_artists");
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.ToTable("artists", "music");

                entity.Property(e => e.ArtistId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genres", "music");

                entity.Property(e => e.GenreId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<MediaType>(entity =>
            {
                entity.ToTable("media_types", "music");

                entity.Property(e => e.MediaTypeId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.ToTable("playlists", "music");

                entity.Property(e => e.PlaylistId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<PlaylistTrack>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("playlist_track", "music");

                entity.HasOne(d => d.Playlist)
                    .WithMany()
                    .HasForeignKey(d => d.PlaylistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_playlist_track_playlists");

                entity.HasOne(d => d.Track)
                    .WithMany()
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_playlist_track_tracks");
            });

            modelBuilder.Entity<Track>(entity =>
            {
                entity.ToTable("tracks", "music");

                entity.Property(e => e.TrackId).ValueGeneratedNever();

                entity.Property(e => e.Composer).HasMaxLength(220);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Tracks)
                    .HasForeignKey(d => d.AlbumId)
                    .HasConstraintName("FK_tracks_albums");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Tracks)
                    .HasForeignKey(d => d.GenreId)
                    .HasConstraintName("FK_tracks_genres");

                entity.HasOne(d => d.MediaType)
                    .WithMany(p => p.Tracks)
                    .HasForeignKey(d => d.MediaTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tracks_media_types");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
