﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DiplomaWork.Models
{
    public partial class laboratory_2023Context : DbContext
    {
        public laboratory_2023Context()
        {
        }

        public laboratory_2023Context(DbContextOptions<laboratory_2023Context> options)
            : base(options)
        {
        }

        public virtual DbSet<EmailCode> EmailCodes { get; set; } = null!;
        public virtual DbSet<LaboratoryDay> LaboratoryDays { get; set; } = null!;
        public virtual DbSet<LaboratoryMonth> LaboratoryMonths { get; set; } = null!;
        public virtual DbSet<LaboratoryMonthChemical> LaboratoryMonthChemicals { get; set; } = null!;
        public virtual DbSet<Month> Months { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Profile> Profiles { get; set; } = null!;
        public virtual DbSet<ProfileHasLengthsPerimeter> ProfileHasLengthsPerimeters { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleHasPermission> RoleHasPermissions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserHasRole> UserHasRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;database=laboratory_2023;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.22-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<EmailCode>(entity =>
            {
                entity.ToTable("email_codes");

                entity.HasIndex(e => e.UserId, "fk_users_email_codes_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .HasColumnName("code");

                entity.Property(e => e.ExpiredAt)
                    .HasColumnType("datetime")
                    .HasColumnName("expired_at");

                entity.Property(e => e.IsValid)
                    .HasColumnType("tinyint(1) unsigned")
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EmailCodes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_email_codes");
            });

            modelBuilder.Entity<LaboratoryDay>(entity =>
            {
                entity.ToTable("laboratory_day");

                entity.HasIndex(e => e.MonthId, "fk_months_laboratory_day_idx");

                entity.HasIndex(e => e.ProfileHasLengthsPerimeterId, "fk_profile_has_lengths_perimeter_laboratory_day_idx");

                entity.HasIndex(e => e.CreatedBy, "fk_users_laboratory_day_profile1_idx");

                entity.HasIndex(e => e.UpdatedBy, "fk_users_laboratory_day_profile2_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("created_by");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.KilogramsPerMeter)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("kilograms_per_meter");

                entity.Property(e => e.MetersSquaredPerSample)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("meters_squared_per_sample");

                entity.Property(e => e.MonthId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("month_id");

                entity.Property(e => e.PaintedMetersSquared)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("painted_meters_squared");

                entity.Property(e => e.PaintedSamplesCount)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("painted_samples_count");

                entity.Property(e => e.ProfileHasLengthsPerimeterId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("profile_has_lengths_perimeter_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("updated_by");

                entity.Property(e => e.Year)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("year");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.LaboratoryDayCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_laboratory_day1");

                entity.HasOne(d => d.Month)
                    .WithMany(p => p.LaboratoryDays)
                    .HasForeignKey(d => d.MonthId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_months_laboratory_day");

                entity.HasOne(d => d.ProfileHasLengthsPerimeter)
                    .WithMany(p => p.LaboratoryDays)
                    .HasForeignKey(d => d.ProfileHasLengthsPerimeterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_profile_has_lengths_perimeter_laboratory_day");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.LaboratoryDayUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_laboratory_day2");
            });

            modelBuilder.Entity<LaboratoryMonth>(entity =>
            {
                entity.ToTable("laboratory_months");

                entity.HasIndex(e => e.MonthId, "fk_months_laboratory_months");

                entity.HasIndex(e => e.UpdatedBy, "fk_users2_laboratory_months_idx");

                entity.HasIndex(e => e.CreatedBy, "fk_users_laboratory_months_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("created_by");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Kilograms)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("kilograms");

                entity.Property(e => e.MetersSquared)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("meters_squared");

                entity.Property(e => e.MonthId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("month_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("updated_by");

                entity.Property(e => e.Year)
                    .HasColumnType("smallint(5)")
                    .HasColumnName("year");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.LaboratoryMonthCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_laboratory_months");

                entity.HasOne(d => d.Month)
                    .WithMany(p => p.LaboratoryMonths)
                    .HasForeignKey(d => d.MonthId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_months_laboratory_months");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.LaboratoryMonthUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users2_laboratory_months");
            });

            modelBuilder.Entity<LaboratoryMonthChemical>(entity =>
            {
                entity.ToTable("laboratory_month_chemicals");

                entity.HasIndex(e => e.MonthId, "fk_laboratory_month_chemicals_months_idx");

                entity.HasIndex(e => e.UpdatedBy, "fk_users2_laboratory_month_chemicals_idx");

                entity.HasIndex(e => e.CreatedBy, "fk_users_laboratory_month_chemicals_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.ChemicalExpenditure)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("chemical_expenditure");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("created_by");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ExpensePerMeterSquared)
                    .HasColumnType("decimal(7,3) unsigned")
                    .HasColumnName("expense_per_meter_squared");

                entity.Property(e => e.MonthId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("month_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("updated_by");

                entity.Property(e => e.Year)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("year");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.LaboratoryMonthChemicalCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_laboratory_month_chemicals");

                entity.HasOne(d => d.Month)
                    .WithMany(p => p.LaboratoryMonthChemicals)
                    .HasForeignKey(d => d.MonthId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_laboratory_month_chemicals_months");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.LaboratoryMonthChemicalUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users2_laboratory_month_chemicals");
            });

            modelBuilder.Entity<Month>(entity =>
            {
                entity.ToTable("months");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Slug, "slug_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Slug)
                    .HasMaxLength(50)
                    .HasColumnName("slug");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Slug, "slug_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Slug)
                    .HasMaxLength(50)
                    .HasColumnName("slug");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("profiles");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ProfileHasLengthsPerimeter>(entity =>
            {
                entity.ToTable("profile_has_lengths_perimeter");

                entity.HasIndex(e => e.ProfileId, "fk_profile_profile_has_lengths_perimeter_idx");

                entity.HasIndex(e => e.CreatedBy, "fk_users1_profile_has_lengths_perimeter_idx");

                entity.HasIndex(e => e.UpdatedBy, "fk_users2_profile_has_lengths_perimeter_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("created_by");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Length)
                    .HasColumnType("decimal(21,13) unsigned")
                    .HasColumnName("length");

                entity.Property(e => e.Perimeter)
                    .HasColumnType("decimal(21,13) unsigned")
                    .HasColumnName("perimeter");

                entity.Property(e => e.ProfileId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("profile_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProfileHasLengthsPerimeterCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users1_profile_has_lengths_perimeter");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProfileHasLengthsPerimeters)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_profile_profile_has_lengths_perimeter");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProfileHasLengthsPerimeterUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users2_profile_has_lengths_perimeter");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Slug, "slug_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Slug)
                    .HasMaxLength(50)
                    .HasColumnName("slug");
            });

            modelBuilder.Entity<RoleHasPermission>(entity =>
            {
                entity.ToTable("role_has_permissions");

                entity.HasIndex(e => e.PermissionId, "fk_permissions_role_has_permissions_idx");

                entity.HasIndex(e => e.RoleId, "fk_roles_role_has_permissions_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.PermissionId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("permission_id");

                entity.Property(e => e.RoleId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("role_id");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RoleHasPermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_permissions_role_has_permissions");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleHasPermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_roles_role_has_permissions");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.EMail, "e-mail_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.CreatedBy, "fk_users_users1_idx");

                entity.HasIndex(e => e.UpdatedBy, "fk_users_users2_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "username_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("created_by");

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.EMail).HasColumnName("e-mail");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(64)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsLocked).HasColumnName("is_locked");

                entity.Property(e => e.LastName)
                    .HasMaxLength(64)
                    .HasColumnName("last_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(64)
                    .HasColumnName("middle_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.PasswordResetRequired).HasColumnName("password_reset_required");

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(255)
                    .HasColumnName("password_salt");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("updated_by");

                entity.Property(e => e.Username).HasColumnName("username");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_users1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.InverseUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_users2");
            });

            modelBuilder.Entity<UserHasRole>(entity =>
            {
                entity.ToTable("user_has_roles");

                entity.HasIndex(e => e.RoleId, "fk_roles_user_has_roles_idx");

                entity.HasIndex(e => e.UserId, "fk_users_user_has_roles_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.RoleId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("role_id");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserHasRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_roles_user_has_roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHasRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_user_has_roles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
