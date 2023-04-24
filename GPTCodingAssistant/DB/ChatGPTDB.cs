using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GPTCodingAssistant.DB;

public partial class ChatGPTDB : DbContext
{
    public ChatGPTDB()
    {
    }

    public ChatGPTDB(DbContextOptions<ChatGPTDB> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Ip> Ips { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ChatGPTSqlServerConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasOne(d => d.Session).WithMany(p => p.ChatMessages).HasConstraintName("FK_ChatMessage_Session");
        });

        modelBuilder.Entity<Ip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_IPSession");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasOne(d => d.Ip).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_IP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
