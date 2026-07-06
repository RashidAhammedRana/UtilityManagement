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
        public virtual DbSet<TblCngRate> TblCngRate { get; set; }
        public virtual DbSet<TblLpgRate> TblLpgRate { get; set; }
        public virtual DbSet<TblLuboilRate> TblLuboilRates { get; set; }
        public virtual DbSet<TblNgRate> TblNgRate { get; set; }
        public virtual DbSet<TblNaclRate> TblNaclRate { get; set; }
        public virtual DbSet<TblSolarReadingInfo> TblSolarReadingInfos { get; set; }
        public virtual DbSet<TblBoilerRmsRoom> TblBoilerRmsRoom { get; set; }
        public virtual DbSet<TblGenRmsRoom> TblGenRmsRoom { get; set; }
        public virtual DbSet<TblBoilerReadingInfo> TblBoilerReadingInfo { get; set; }
        public virtual DbSet<TblWtpPlanCostInfo> TblWtpPlanCostInfo { get; set; }


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
                entity.Property(e => e.Uom).HasMaxLength(250).HasColumnName("UOM");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks).HasMaxLength(250).HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
            });
            //TblCngRate
            modelBuilder.Entity<TblCngRate>(entity =>
            {
                entity.HasKey(e => e.Cngrid);

                entity.ToTable("TBL_CNG_RATE");

                entity.Property(e => e.Cngrid).HasColumnName("CNGRID");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Uom).HasMaxLength(250).HasColumnName("UOM");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
            });
            //TblLpgRate
            modelBuilder.Entity<TblLpgRate>(entity =>
            {
                entity.HasKey(e => e.Lpgrid);

                entity.ToTable("TBL_LPG_RATE");

                entity.Property(e => e.Lpgrid).HasColumnName("LPGRID");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Uom).HasMaxLength(250).HasColumnName("UOM");
                entity.Property(e => e.Rate).HasColumnName("RATE");
                entity.Property(e => e.Remarks)
                    .HasMaxLength(250)
                    .HasColumnName("REMARKS");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
            });
            //TblNaclRate
            modelBuilder.Entity<TblNaclRate>(entity =>
            {
                entity.HasKey(e => e.Naclrid);

                entity.ToTable("TBL_NACL_RATE");

                entity.Property(e => e.Naclrid).HasColumnName("NACLRID");
                entity.Property(e => e.TrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRDATE");
                entity.Property(e => e.Uom).HasMaxLength(250).HasColumnName("UOM");
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
            //TblBoilerReadingInfo
            modelBuilder.Entity<TblBoilerReadingInfo>(entity =>
            {
                entity.HasKey(e => e.Trid);

                entity.ToTable("TBL_BOILER_READING_INFO");

                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.ChemicalTk).HasColumnName("CHEMICAL_TK");
                entity.Property(e => e.CngConM).HasColumnName("CNG_CON_M");
                entity.Property(e => e.CngMHr).HasColumnName("CNG_M_HR");
                entity.Property(e => e.CngRh).HasColumnName("CNG_RH");
                entity.Property(e => e.CngTkM).HasColumnName("CNG_TK_M");
                entity.Property(e => e.DslConLtr).HasColumnName("DSL_CON_LTR");
                entity.Property(e => e.DslLtrHr).HasColumnName("DSL_LTR_HR");
                entity.Property(e => e.DslRh).HasColumnName("DSL_RH");
                entity.Property(e => e.DslTkLtr).HasColumnName("DSL_TK_LTR");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.LpgConKg).HasColumnName("LPG_CON_KG");
                entity.Property(e => e.LpgKgHr).HasColumnName("LPG_KG_HR");
                entity.Property(e => e.LpgRh).HasColumnName("LPG_RH");
                entity.Property(e => e.LpgTkLtr).HasColumnName("LPG_TK_LTR");
                entity.Property(e => e.Maintenance).HasColumnName("MAINTENANCE");
                entity.Property(e => e.NgConM).HasColumnName("NG_CON_M");
                entity.Property(e => e.NgMHr).HasColumnName("NG_M_HR");
                entity.Property(e => e.NgRh).HasColumnName("NG_RH");
                entity.Property(e => e.NgTkM).HasColumnName("NG_TK_M");
                entity.Property(e => e.OtherConsumable).HasColumnName("OTHER_CONSUMABLE");
                entity.Property(e => e.Remarks).HasMaxLength(50).HasColumnName("REMARKS");
                entity.Property(e => e.ServiceCharge).HasColumnName("SERVICE_CHARGE");
                entity.Property(e => e.StaemGenKg).HasColumnName("STAEM_GEN_KG");
                entity.Property(e => e.SteamKgHr).HasColumnName("STEAM_KG_HR");
                entity.Property(e => e.TkKgSteamGenCost).HasColumnName("TK_KG_STEAM_GEN_COST");
                entity.Property(e => e.TotalRh).HasColumnName("TOTAL_RH");
                entity.Property(e => e.Trdate).HasColumnType("datetime").HasColumnName("TRDATE");
                entity.Property(e => e.Troubleshooting).HasColumnName("TROUBLESHOOTING");
                entity.Property(e => e.WaterInletLtr).HasColumnName("WATER_INLET_LTR");
                entity.Property(e => e.Kwh).HasColumnName("KWH");
                entity.Property(e => e.Total).HasColumnName("TOTAL");

                entity.HasOne(d => d.Eq).WithMany(p => p.TblBoilerReadingInfo)
                    .HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_BOILER_READING_INFO_TBL_BOILER_READING_INFO");
            });
            //TblWtpPlanCostInfo
            modelBuilder.Entity<TblWtpPlanCostInfo>(entity =>
            {
                entity.HasKey(e => e.Trid).HasName("PK_TBL_WTP_PLAN_COST");
                entity.ToTable("TBL_WTP_PLAN_COST_INFO");
                entity.Property(e => e.Trid).HasColumnName("TRID");
                entity.Property(e => e.CraetedBy).HasMaxLength(50).HasColumnName("CRAETED_BY");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("CREATED_AT");
                entity.Property(e => e.DeepPump1).HasColumnName("DEEP_PUMP_1");
                entity.Property(e => e.DeepPump2).HasColumnName("DEEP_PUMP_2");
                entity.Property(e => e.DeepPump3).HasColumnName("DEEP_PUMP_3");
                entity.Property(e => e.DeepPump4).HasColumnName("DEEP_PUMP_4");
                entity.Property(e => e.Eqid).HasColumnName("EQID");
                entity.Property(e => e.Kwh).HasColumnName("KWH");
                entity.Property(e => e.Maintenance).HasColumnName("MAINTENANCE");
                entity.Property(e => e.NaclConsumption).HasColumnName("NACL_CONSUMPTION");
                entity.Property(e => e.NaclCost).HasColumnName("NACL_COST");
                entity.Property(e => e.Opt01).HasColumnName("OPT01");
                entity.Property(e => e.Opt2).HasColumnName("OPT2");
                entity.Property(e => e.Opt3).HasColumnName("OPT3");
                entity.Property(e => e.Remarks).HasColumnName("REMARKS");
                entity.Property(e => e.Softner1).HasColumnName("SOFTNER_1");
                entity.Property(e => e.Softner2).HasColumnName("SOFTNER_2");
                entity.Property(e => e.Softner3).HasColumnName("SOFTNER_3");
                entity.Property(e => e.Softner4).HasColumnName("SOFTNER_4");
                entity.Property(e => e.SoftnerGeneration).HasColumnName("SOFTNER_GENERATION");
                entity.Property(e => e.TkMcSoftWater).HasColumnName("TK_MC_SOFT_WATER");
                entity.Property(e => e.TotalCost).HasColumnName("TOTAL_COST");
                entity.Property(e => e.TotalDrawing).HasColumnName("TOTAL_DRAWING");
                entity.Property(e => e.Trdate).HasColumnType("datetime").HasColumnName("TRDATE");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("UPDATED_AT");
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).HasColumnName("UPDATED_BY");
                entity.HasOne(d => d.Eq).WithMany(p => p.TblWtpPlanCostInfos).HasForeignKey(d => d.Eqid)
                    .HasConstraintName("FK_TBL_WTP_PLAN_COST_TBL_EQUIPMENT_DETAILS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}