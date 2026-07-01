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
        public virtual DbSet<TblEquipmentDetail> TblEquipmentDetails { get; set; }
        public virtual DbSet<TblRebReadingInfo> TblRebReadingInfo { get; set; }
        public virtual DbSet<TblNgGeneratorReadingInfo> TblNgGeneratorReadingInfos { get; set; }
        public virtual DbSet<TblDiselGeneratorReadingInfo> TblDiselGeneratorReadingInfo { get; set; }
        public virtual DbSet<TblDieselRate> TblDieselRates { get; set; }
        public virtual DbSet<TblLuboilRate> TblLuboilRates { get; set; }
        public virtual DbSet<TblNgRate> TblNgRate { get; set; }
        public virtual DbSet<TblSolarReadingInfo> TblSolarReadingInfos { get; set; }
        public virtual DbSet<TblBoilerRmsRoom> TblBoilerRmsRoom { get; set; }
        public virtual DbSet<TblGenRmsRoom> TblGenRmsRoom { get; set; }


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
            modelBuilder.Entity<TblEquipmentDetail>(entity =>
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
            //TblNgGeneratorReadingInfo
            modelBuilder.Entity<TblNgGeneratorReadingInfo>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_NG_GENERATOR_READING_INFO");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.ChemicalCost).HasColumnName("CHEMICAL_COST");
                entity.Property(e => e.CngConsumption).HasColumnName("CNG_CONSUMPTION");
                entity.Property(e => e.CngCost).HasColumnName("CNG_COST");
                entity.Property(e => e.EGeneration).HasColumnName("E_GENERATION");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.KwhUnitNgConsumption).HasColumnName("KWH_UNIT_NG_CONSUMPTION");
                entity.Property(e => e.LubOilConsumption).HasColumnName("LUB_OIL_CONSUMPTION");
                entity.Property(e => e.LubOilCost).HasColumnName("LUB_OIL_COST");
                entity.Property(e => e.NgConsumptionKwh).HasColumnName("NG_CONSUMPTION_KWH");
                entity.Property(e => e.NgConsumptionHr).HasColumnName("NG_CONSUMPTION_HR");
                entity.Property(e => e.NgConsumptionM).HasColumnName("NG_CONSUMPTION_M");
                entity.Property(e => e.NgTkM).HasColumnName("NG_TK_M");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.RepairCharge).HasColumnName("REPAIR_CHARGE");
                entity.Property(e => e.RunningHr).HasColumnName("RUNNING_HR");
                entity.Property(e => e.ServiceCharge).HasColumnName("SERVICE_CHARGE");
                entity.Property(e => e.TkKwh).HasColumnName("TK_KWH");
                entity.Property(e => e.Total).HasColumnName("TOTAL");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Troubleshooting).HasColumnName("TROUBLESHOOTING");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblNgGeneratorReadingInfos)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_NG_GENERATOR_READING_INFO_TBL_EQUIPMENT_DETAILS");
            });
            //TblDiselGeneratorReadingInfo
            modelBuilder.Entity<TblDiselGeneratorReadingInfo>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_DISEL_GENERATOR_READING_INFO");

                entity.Property(e => e.Trid)
                    .ValueGeneratedNever()
                    .HasColumnName("TRID");
                entity.Property(e => e.Chemical).HasColumnName("CHEMICAL");
                entity.Property(e => e.DieselConsumptionHr).HasColumnName("DIESEL_CONSUMPTION_HR");
                entity.Property(e => e.DieselConsumptionKwh).HasColumnName("DIESEL_CONSUMPTION_KWH");
                entity.Property(e => e.DieselConsumptionLtr).HasColumnName("DIESEL_CONSUMPTION_LTR");
                entity.Property(e => e.DieselTkLtr).HasColumnName("DIESEL_TK_LTR");
                entity.Property(e => e.ElectricityGenerationKwh).HasColumnName("ELECTRICITY_GENERATION_KWH");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.LubOilConsumption).HasColumnName("LUB_OIL_CONSUMPTION");
                entity.Property(e => e.LubOilTkLtr).HasColumnName("LUB_OIL_TK_LTR");
                entity.Property(e => e.OthersConsumable).HasColumnName("OTHERS_CONSUMABLE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.RepairCharge).HasColumnName("REPAIR_CHARGE");
                entity.Property(e => e.RunningHr).HasColumnName("RUNNING_HR");
                entity.Property(e => e.ServiceCharge).HasColumnName("SERVICE_CHARGE");
                entity.Property(e => e.TkKwh).HasColumnName("TK_KWH");
                entity.Property(e => e.Total).HasColumnName("TOTAL");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Troubleshooting).HasColumnName("TROUBLESHOOTING");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblDiselGeneratorReadingInfos)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_DISEL_GENERATOR_READING_INFO_TBL_EQUIPMENT_DETAILS");
            });
            //TblDieselRate
            modelBuilder.Entity<TblDieselRate>(entity =>
            {
                entity.HasKey(e => e.Drid);

                entity.ToTable("TBL_DIESEL_RATE");

                entity.Property(e => e.Drid).HasColumnName("DRID");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
            });
            //TblLuboilRate
            modelBuilder.Entity<TblLuboilRate>(entity =>
            {
                entity.HasKey(e => e.Loid);

                entity.ToTable("TBL_LUBOIL_RATE");

                entity.Property(e => e.Loid).HasColumnName("LOID");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Uom)
                    .HasMaxLength(50)
                    .HasColumnName("UOM");
            });
            //TblNgRate
            modelBuilder.Entity<TblNgRate>(entity =>
            {
                entity.HasKey(e => e.NgrId);

                entity.ToTable("TBL_NG_RATE");

                entity.Property(e => e.NgrId).HasColumnName("NGRID");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Uom)
                    .HasMaxLength(50)
                    .HasColumnName("UOM");
            });
            //TblSolarReadingInfo
            modelBuilder.Entity<TblSolarReadingInfo>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_SOLAR_READING_INFO");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.GenerationKwh).HasColumnName("GENERATION_KWH");
                entity.Property(e => e.PerUnitGenCost).HasColumnName("PER_UNIT_GEN_COST");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.ServiceChargeCost).HasColumnName("SERVICE_CHARGE_COST");
                entity.Property(e => e.SparePartsCost).HasColumnName("SPARE_PARTS_COST");
                entity.Property(e => e.TotalCost).HasColumnName("TOTAL_COST");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblSolarReadingInfos)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_SOLAR_READING_INFO_TBL_EQUIPMENT_DETAILS");
            });
            //TblBoilerRmsRoom
            modelBuilder.Entity<TblBoilerRmsRoom>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_BOILER_RMS_ROOM");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.NgConsumption).HasColumnName("NG_CONSUMPTION");
                entity.Property(e => e.NgConsumptionHr).HasColumnName("NG_CONSUMPTION_HR");
                entity.Property(e => e.NgTk).HasColumnName("NG_TK");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.ToalRh).HasColumnName("TOAL_RH");
                entity.Property(e => e.TkHr).HasColumnName("TK_HR");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblBoilerRmsRooms)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_BOILER_RMS_ROOM_TBL_EQUIPMENT_DETAILS");
            });

            //TblGenRmsRoom
            modelBuilder.Entity<TblGenRmsRoom>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_GEN_RMS_ROOM");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.NgConsumption).HasColumnName("NG_CONSUMPTION");
                entity.Property(e => e.NgConsumptionHr).HasColumnName("NG_CONSUMPTION_HR");
                entity.Property(e => e.NgTk).HasColumnName("NG_TK");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.TkHr).HasColumnName("TK_HR");
                entity.Property(e => e.ToalRh).HasColumnName("TOAL_RH");
                entity.Property(e => e.Trdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblGenRmsRooms)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_GEN_RMS_ROOM_TBL_EQUIPMENT_DETAILS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}