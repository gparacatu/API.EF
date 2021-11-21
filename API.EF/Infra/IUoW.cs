using API.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace API.EF.Infra
{
    public class IUoW : DbContext
    {
        public IUoW(DbContextOptions<IUoW> options) : base(options)
        { }

        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Produto>? Produtos { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var connectionString = configuration.GetConnectionString("AppDb");
        //    optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0)));
        //}

    }
}
