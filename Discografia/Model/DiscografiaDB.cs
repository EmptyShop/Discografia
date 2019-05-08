namespace Discografia
{
    using System.Data.Entity;

    public class DiscografiaDB : DbContext
    {
        // Your context has been configured to use a 'DiscografiaDB' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Discografia.DiscografiaDB' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DiscografiaDB' 
        // connection string in the application configuration file.
        public DiscografiaDB()
            : base("name=DiscografiaDB")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiscografiaDB, Discografia.Migrations.Configuration>("DiscografiaDB"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Formato>().ToTable("Formatos");
            modelBuilder.Entity<Cancion>().ToTable("Canciones");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        }

        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Cancion> Canciones { get; set; }
        public DbSet<Formato> Formatos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<CancionAlbums> CancionAlbums { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}