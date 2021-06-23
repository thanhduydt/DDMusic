using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Data
{
    public class DPContext:IdentityDbContext<UserModel>
    {
        public DPContext(DbContextOptions<DPContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
        public DbSet<UserModel> User { get; set; }
        public DbSet<SongModel> Song { get; set; }
        public DbSet<SingerModel> Singer { get; set; }
        public DbSet<AlbumModel> Album { get; set; }
    }
}
