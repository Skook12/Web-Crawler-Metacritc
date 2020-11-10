using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcr
{
    class GameContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public void Iniciar()
        {
            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Data Source = localhost\\SQLEXPRESSS; Initial Catalog = GameDB; Integrated Security = True");
    }
}
