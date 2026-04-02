using AppChat.Models;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Define DB sets for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ReadReceipt> ReadReceipts { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Chat relationships
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.UserA)
                .WithMany(u => u.ChatsAsUserA)
                .HasForeignKey(c => c.UserAId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.UserB)
                .WithMany(u => u.ChatsAsUserB)
                .HasForeignKey(c => c.UserBId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Message relationships
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ReadReceipt relationships
            modelBuilder.Entity<ReadReceipt>()
                .HasOne(rr => rr.Message)
                .WithMany(m => m.ReadReceipts)
                .HasForeignKey(rr => rr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReadReceipt>()
                .HasOne(rr => rr.User)
                .WithMany(u => u.ReadReceipts)
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure BlockedUser relationships
            modelBuilder.Entity<BlockedUser>()
                .HasOne(bu => bu.Blocker)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(bu => bu.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlockedUser>()
                .HasOne(bu => bu.BlockedUserNav)
                .WithMany(u => u.BlockedByUsers)
                .HasForeignKey(bu => bu.BlockedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Contact relationships
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.UserA)
                .WithMany(u => u.ContactsInitiated)
                .HasForeignKey(c => c.UserIdContactA)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.UserB)
                .WithMany(u => u.ContactsReceived)
                .HasForeignKey(c => c.UserIdContactB)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
