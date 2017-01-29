using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoldersApp.Repositories
{
    public class FoldersContext : DbContext
    {
        public FoldersContext(DbContextOptions<FoldersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSystemItem>().HasIndex(b => b.FullPath).IsUnique();
        }

        public DbSet<FileSystemItem> FileSystem { get; set; }
    }
}
