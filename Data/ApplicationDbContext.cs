using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Models;

namespace UtilityManagement.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DbSets
        // =========================

        public virtual DbSet<TblUserPermission> TblUserPermission { get; set; }

        public virtual DbSet<TblModule> TblModule { get; set; }

        public virtual DbSet<TblMenu> TblMenu { get; set; }

        public virtual DbSet<TblPermissionAction> TblPermissionAction { get; set; }
        public virtual DbSet<TblEquipmentDetails> TblEquipmentDetails { get; set; }
        public virtual DbSet<TblRebReadingInfo> TblRebReadingInfo { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // TblUserPermissions
            // =========================

            modelBuilder.Entity<TblUserPermission>(entity =>
            {
                entity.HasKey(e => e.UserPermissionId)
                    .HasName("PK_UserPermissions");

                entity.ToTable("UserPermissions");

                entity.Property(e => e.UserId)
                    .HasColumnType("nvarchar(450)");

                entity.Property(e => e.IsAllowed)
                    .HasDefaultValue(true);

                // 👇 NEW ADDITIONS (RELATIONS)

                entity.HasOne(x => x.Module)
                    .WithMany()
                    .HasForeignKey(x => x.ModuleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Menu)
                    .WithMany()
                    .HasForeignKey(x => x.MenuId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Action)
                    .WithMany()
                    .HasForeignKey(x => x.ActionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // Modules
            // =========================

            modelBuilder.Entity<TblModule>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("Modules");

                entity.Property(e => e.ModuleName)
                    .HasMaxLength(100)
                    .IsRequired();
            });


            // =========================
            // Menus
            // =========================

            modelBuilder.Entity<TblMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId);

                entity.ToTable("Menus");

                entity.Property(e => e.MenuName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .HasMaxLength(300);

                entity.HasOne<TblModule>()
                    .WithMany()
                    .HasForeignKey(e => e.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // PermissionActions
            // =========================

            modelBuilder.Entity<TblPermissionAction>(entity =>
            {
                entity.HasKey(e => e.ActionId);

                entity.ToTable("PermissionActions");

                entity.Property(e => e.ActionName)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // TblEquipmentDetails
            modelBuilder.Entity<TblEquipmentDetails>(entity =>
            {
                entity.HasKey(e => e.Eqid);

                entity.ToTable("TBL_EQUIPMENT_DETAILS");

                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.Brand)
                    .HasMaxLength(50)
                    .HasColumnName("BRAND");
                entity.Property(e => e.Capacity)
                    .HasMaxLength(50)
                    .HasColumnName("CAPACITY");
                entity.Property(e => e.CurrentLocation)
                    .HasMaxLength(50)
                    .HasColumnName("CURRENT_LOCATION");
                entity.Property(e => e.EquipmentName)
                    .HasMaxLength(50)
                    .HasColumnName("EQUIPMENT_NAME");
                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .HasColumnName("MODEL");
                entity.Property(e => e.Slno)
                    .HasMaxLength(50)
                    .HasColumnName("SLNO");
            });
            // TblRebReadingInfo
            modelBuilder.Entity<TblRebReadingInfo>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_REB_READING_INFO");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.BdtKwh).HasColumnName("BDT_KWH");
                entity.Property(e => e.ElecGen).HasColumnName("ELEC_GEN");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.OtConsumable).HasColumnName("OT_CONSUMABLE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.RepairCharge).HasColumnName("REPAIR_CHARGE");
                entity.Property(e => e.RunHr).HasColumnName("RUN_HR");
                entity.Property(e => e.ServiceCharge).HasColumnName("SERVICE_CHARGE");
                entity.Property(e => e.TkKwh).HasColumnName("TK_KWH");
                entity.Property(e => e.Total).HasColumnName("TOTAL");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Troubleshoot).HasColumnName("TROUBLESHOOT");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblRebReadingInfos)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_REB_READING_INFO_TBL_REB_READING_INFO");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}