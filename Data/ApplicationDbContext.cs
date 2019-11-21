using lonefire.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using lonefire.Models.UtilModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace lonefire.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly ILoggerFactory _loggerFactory;

        public ApplicationDbContext(DbContextOptions options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
            ChangeTracker.Tracked += OnEntityTracked;
            ChangeTracker.StateChanged += OnEntityStateChanged;
        }

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Link> Link { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<ArticleImage> ArticleImage { get; set; }
        public virtual DbSet<ArticleTag> ArticleTag { get; set; }
        public virtual DbSet<NoteImage> NoteImage { get; set; }
        public virtual DbSet<NoteTag> NoteTag { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<Article>().ToTable("Articles");
            builder.Entity<Comment>().ToTable("Comments");
            builder.Entity<Tag>().ToTable("Tags");
            builder.Entity<Note>().ToTable("Notes");
            builder.Entity<Link>().ToTable("Links");
            builder.Entity<Image>().ToTable("Images");
            builder.Entity<ArticleImage>().ToTable("ArticleImages");
            builder.Entity<ArticleTag>().ToTable("ArticleTags");
            builder.Entity<NoteImage>().ToTable("NoteImages");
            builder.Entity<NoteTag>().ToTable("NoteTags");


            builder.Entity<Article>().HasData(
               new Article
               {
                   Id = Guid.NewGuid(),
                   Title = "Test Article",
                   Content = "Sample Article content"
               }
           );
            // Article Image many to many
            builder.Entity<ArticleImage>()
                .HasKey(ai => new { ai.ArticleId, ai.ImageId });
            builder.Entity<ArticleImage>()
                .HasOne(ai => ai.Article)
                .WithMany(a => a.ArticleImages)
                .HasForeignKey(ai => ai.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ArticleImage>()
                .HasOne(ai => ai.Image)
                .WithMany(i => i.ArticleImages)
                .HasForeignKey(ai => ai.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Article Header Image one to one
            builder.Entity<Article>()
                .HasOne(a => a.HeaderImage)
                .WithOne(i => i.Article)
                .HasForeignKey<Article>(a => a.HeaderImageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aritcle Tag many to many
            builder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });
            builder.Entity<ArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ArticleTag>()
               .HasOne(at => at.Tag)
               .WithMany(t => t.ArticleTags)
               .HasForeignKey(at => at.TagId)
               .OnDelete(DeleteBehavior.Cascade);

            // Note Image many to many
            builder.Entity<NoteImage>()
                .HasKey(ni => new { ni.NoteId, ni.ImageId });
            builder.Entity<NoteImage>()
                .HasOne(ni => ni.Note)
                .WithMany(n => n.NoteImages)
                .HasForeignKey(nt => nt.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<NoteImage>()
               .HasOne(ni => ni.Image)
               .WithMany(i => i.NoteImages)
               .HasForeignKey(ni => ni.ImageId)
               .OnDelete(DeleteBehavior.Restrict);

            // Note Tag many to many
            builder.Entity<NoteTag>()
                .HasKey(nt => new { nt.NoteId, nt.TagId });
            builder.Entity<NoteTag>()
                .HasOne(nt => nt.Note)
                .WithMany(n => n.NoteTags)
                .HasForeignKey(nt => nt.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<NoteTag>()
               .HasOne(nt => nt.Tag)
               .WithMany(t => t.NoteTags)
               .HasForeignKey(nt => nt.TagId)
               .OnDelete(DeleteBehavior.Cascade);

            //User Note one to many
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Notes)
                .WithOne(n => n.Owner)
                .HasForeignKey(n => n.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            //User Article many to one
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Articles)
                .WithOne(a => a.Owner)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            //Article Comment one to many
            builder.Entity<Article>()
                .HasMany(a => a.Comments)
                .WithOne(c => c.Article)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            //Comment Comment one to many
            builder.Entity<Comment>()
                .HasMany(c => c.Comments)
                .WithOne()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            //User Image one to one
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Avatar)
                .WithOne(i => i.UserAvatar)
                .HasForeignKey<ApplicationUser>(u => u.AvatarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }

        // Auto Model CreateTime EditTime updates
        void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is IEntityDate entity)
                entity.CreateTime = DateTimeOffset.Now;
            if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is ApplicationUser user)
                user.RegisterTime = DateTimeOffset.Now;
        }

        void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.NewState == EntityState.Modified && e.Entry.Entity is IEntityDate entity)
                entity.EditTime = DateTimeOffset.Now;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}
