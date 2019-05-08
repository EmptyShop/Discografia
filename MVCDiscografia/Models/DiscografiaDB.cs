using System.Data.Entity;

namespace MVCDiscografia.Models
{
    public partial class DiscografiaDB : DbContext
    {
        public DiscografiaDB()
            : base("name=DiscografiaDB")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiscografiaDB, MVCDiscografia.Migrations.Configuration>("DiscografiaDB"));
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Artista> Artistas { get; set; }
        public virtual DbSet<CancionAlbums> CancionAlbums { get; set; }
        public virtual DbSet<Cancion> Canciones { get; set; }
        public virtual DbSet<Formato> Formatos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>()
                .HasMany(e => e.Tracklist)
                .WithRequired(e => e.Album)
                .HasForeignKey(e => e.Album_AlbumID);

            modelBuilder.Entity<Album>()
                .HasMany(e => e.Artistas)
                .WithMany(e => e.Albums)
                .Map(m => m.ToTable("ArtistaAlbums"));

            modelBuilder.Entity<Artista>()
                .HasMany(e => e.Canciones)
                .WithMany(e => e.Artistas)
                .Map(m => m.ToTable("CancionArtistas").MapRightKey("Cancion_CancionID"));

            modelBuilder.Entity<Cancion>()
                .HasMany(e => e.CancionAlbums)
                .WithRequired(e => e.Cancion)
                .HasForeignKey(e => e.Cancion_CancionID);

            modelBuilder.Entity<Formato>()
                .HasMany(e => e.Albums)
                .WithRequired(e => e.Formato)
                .HasForeignKey(e => e.Formato_FormatoID);
        }
    }
}
