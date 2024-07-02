using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DCI_BATCH_CAL_MAIN_WIP.Models;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DCI_BATCH_CAL_MAIN_WIP.Contexts
{
    public partial class DBSCM : DbContext
    {
        public DBSCM()
        {
        }

        public DBSCM(DbContextOptions<DBSCM> options)
            : base(options)
        {
        }

        public virtual DbSet<EkbWipPartStock> EkbWipPartStock { get; set; }
        public virtual DbSet<WmsMdw27ModelMaster> WmsMdw27ModelMaster { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=192.168.226.86;Database=dbSCM;TrustServerCertificate=True;uid=sa;password=decjapan");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EkbWipPartStock>(entity =>
            {
                entity.HasKey(e => new { e.Ym, e.Wcno, e.Partno, e.Cm })
                    .HasName("PK_EKB_LINE_STOCK_MONIOTR");

                entity.ToTable("EKB_WIP_PART_STOCK");

                entity.Property(e => e.Ym)
                    .HasColumnName("YM")
                    .HasMaxLength(8);

                entity.Property(e => e.Wcno)
                    .HasColumnName("WCNO")
                    .HasMaxLength(3);

                entity.Property(e => e.Partno)
                    .HasColumnName("PARTNO")
                    .HasMaxLength(25);

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(2);

                entity.Property(e => e.Bal)
                    .HasColumnName("BAL")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Issqty)
                    .HasColumnName("ISSQTY")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Lbal)
                    .HasColumnName("LBAL")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PartDesc).HasMaxLength(250);

                entity.Property(e => e.Ptype).HasMaxLength(15);

                entity.Property(e => e.Recqty)
                    .HasColumnName("RECQTY")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WmsMdw27ModelMaster>(entity =>
            {
                entity.HasKey(e => new { e.Model, e.Modelgroup, e.Pltype, e.Strloc, e.Rev, e.Lrev, e.Strdate, e.Enddate, e.Remark, e.Sebango, e.Diameter })
                    .HasName("PK_WMS_MDW27_MODEL_MASTER_1");

                entity.ToTable("WMS_MDW27_MODEL_MASTER");

                entity.Property(e => e.Model)
                    .HasColumnName("MODEL")
                    .HasMaxLength(50);

                entity.Property(e => e.Modelgroup)
                    .HasColumnName("MODELGROUP")
                    .HasMaxLength(3);

                entity.Property(e => e.Pltype)
                    .HasColumnName("PLTYPE")
                    .HasMaxLength(50);

                entity.Property(e => e.Strloc)
                    .HasColumnName("STRLOC")
                    .HasMaxLength(50);

                entity.Property(e => e.Rev).HasColumnName("REV");

                entity.Property(e => e.Lrev).HasColumnName("LREV");

                entity.Property(e => e.Strdate)
                    .HasColumnName("STRDATE")
                    .HasMaxLength(8);

                entity.Property(e => e.Enddate)
                    .HasColumnName("ENDDATE")
                    .HasMaxLength(8);

                entity.Property(e => e.Remark)
                    .HasColumnName("REMARK")
                    .HasMaxLength(50);

                entity.Property(e => e.Sebango)
                    .HasColumnName("SEBANGO")
                    .HasMaxLength(50);

                entity.Property(e => e.Diameter)
                    .HasColumnName("DIAMETER")
                    .HasMaxLength(50);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasColumnName("ACTIVE")
                    .HasMaxLength(20)
                    .HasDefaultValueSql("(N'INACTIVE')");

                entity.Property(e => e.Area)
                    .HasColumnName("AREA")
                    .HasMaxLength(20);

                entity.Property(e => e.CreateBy)
                    .HasColumnName("CREATE_BY")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'every 8.30 AM')");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("CREATE_DATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("UPDATE_DATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
