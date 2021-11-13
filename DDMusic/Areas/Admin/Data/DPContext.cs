using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Data
{
    public class DPContext : IdentityDbContext<UserModel>
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
        public DbSet<SingerModel> Singer { get; set; }
        public DbSet<AlbumModel> Album { get; set; }
        public DbSet<SongModel> Song { get; set; }
        public DbSet<TopSongOnWeek> TopSongOnWeek { get; set; }
        public DbSet<TopSongOnWeekDetail> TopSongOnWeekDetail { get; set; }
        public DbSet<TopSongOnMonth> TopSongOnMonth { get; set; }
        public DbSet<TopSongOnMonthDetail> TopSongOnMonthDetail { get; set; }
        public DbSet<CommentModel> Comment { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<PlaylistDetail> PlaylistDetail { get; set; }
        public DbSet<ReactSong> ReactSong { get; set; }
        public DbSet<ViewSongOfDay> ViewSongOfDay { get; set; }
        public DbSet<ViewSongOfDayDetail> ViewSongOfDayDetail { get; set; }
        public DbSet<ViewSongOfWeek> ViewSongOfWeek { get; set; }
        public DbSet<ViewSongOfWeekDetail> ViewSongOfWeekDetail { get; set; }
        public DbSet<ViewSongOfMonth> ViewSongOfMonth { get; set; }
        public DbSet<ViewSongOfMonthDetail> ViewSongOfMonthDetail { get; set; }
        public DbSet<CountNewAccountModel>CountNewAccount { get; set; }
        public DbSet<SingerOfSong> SingerOfSong { get; set; }
    }
}
