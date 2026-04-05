using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context
{
    public class ApiContext : DbContext 
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-4PT8UQA\\SQLEXPRESS; initial catalog=DbGuvenliBelgeAnonimlestirmeSistemi; integrated security=true;TrustServerCertificate=True;");
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleField> ArticleFields { get; set; }
        public DbSet<Editor> Editors { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldTopic> FieldTopics { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<ReviewerFieldTopic> ReviewerFieldTopics { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
