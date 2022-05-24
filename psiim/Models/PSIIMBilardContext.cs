using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace psiim.Models
{
    public partial class PSIIMBilardContext : DbContext
    {
        public PSIIMBilardContext()
        {
        }

        public PSIIMBilardContext(DbContextOptions<PSIIMBilardContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Club> Clubs { get; set; } = null!;
        public virtual DbSet<PeopleDatum> PeopleData { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<ReservedTable> ReservedTables { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=PSIIMBilard.mssql.somee.com;Database=PSIIMBilard;User Id=Kabanos_SQLLogin_1;Password=4o7idiiwda;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");

                entity.Property(e => e.AdminId)
                    .ValueGeneratedNever()
                    .HasColumnName("admin_id");

                entity.Property(e => e.ClubId).HasColumnName("club_id");

                entity.Property(e => e.PersonDataId).HasColumnName("person_data_id");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_admins_clubs");

                entity.HasOne(d => d.PersonData)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.PersonDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_admins_people_data");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("clients");

                entity.Property(e => e.ClientId)
                    .ValueGeneratedNever()
                    .HasColumnName("client_id");

                entity.Property(e => e.PersonDataId).HasColumnName("person_data_id");

                entity.HasOne(d => d.PersonData)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.PersonDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_clients_people_data");
            });

            modelBuilder.Entity<Club>(entity =>
            {
                entity.ToTable("clubs");

                entity.Property(e => e.ClubId)
                    .ValueGeneratedNever()
                    .HasColumnName("club_id");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Street)
                    .HasMaxLength(50)
                    .HasColumnName("street");
            });

            modelBuilder.Entity<PeopleDatum>(entity =>
            {
                entity.HasKey(e => e.PersonDataId);

                entity.ToTable("people_data");

                entity.Property(e => e.PersonDataId)
                    .ValueGeneratedNever()
                    .HasColumnName("person_data_id");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("date")
                    .HasColumnName("birth_date");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.HashPassword)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("hash_password");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("login");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.SecondName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("second_name");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("reservations");

                entity.Property(e => e.ReservationId)
                    .ValueGeneratedNever()
                    .HasColumnName("reservation_id");

                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Duration)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("duration");

                entity.Property(e => e.IsAccepted).HasColumnName("is_accepted");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reservations_clients");
            });

            modelBuilder.Entity<ReservedTable>(entity =>
            {
                entity.ToTable("reserved_tables");

                entity.Property(e => e.ReservedTableId)
                    .ValueGeneratedNever()
                    .HasColumnName("reserved_table_id");

                entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.HasOne(d => d.Reservation)
                    .WithMany(p => p.ReservedTables)
                    .HasForeignKey(d => d.ReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reserved_tables_reservations");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.ReservedTables)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reserved_tables_tables");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.ToTable("tables");

                entity.Property(e => e.TableId)
                    .ValueGeneratedNever()
                    .HasColumnName("table_id");

                entity.Property(e => e.ClubId).HasColumnName("club_id");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Tables)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tables_clubs");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
