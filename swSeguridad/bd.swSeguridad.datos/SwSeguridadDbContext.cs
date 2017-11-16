using bd.swseguridad.entidades.Negocio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace bd.swseguridad.datos
{
    public class SwSeguridadDbContext : DbContext
    {

        public SwSeguridadDbContext(DbContextOptions<SwSeguridadDbContext> options)
            : base(options) { }

        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscbdd> Adscbdd { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscexe> Adscexe { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscgrp> Adscgrp { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscmenu> Adscmenu { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscmiem> Adscmiem { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscsist> Adscsist { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscpassw> Adscpassw { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscswepwd> Adscswepwd { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscswext> Adscswext { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adsctoken> Adsctoken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adscbdd>(entity =>
            {
                entity.HasKey(e => e.AdbdBdd)
                    .HasName("PK_ADSCBDD");

                entity.ToTable("ADSCBDD");

                entity.Property(e => e.AdbdBdd)
                    .HasColumnName("ADBD_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdbdDescripcion)
                    .HasColumnName("ADBD_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdbdServidor)
                    .HasColumnName("ADBD_SERVIDOR")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Adscexe>(entity =>
            {
                entity.HasKey(e => new { e.AdexBdd, e.AdexGrupo, e.AdexSistema, e.AdexAplicacion })
                    .HasName("PK_ADSCEXE_1");

                entity.ToTable("ADSCEXE");

                entity.Property(e => e.AdexBdd)
                    .HasColumnName("ADEX_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdexGrupo)
                    .HasColumnName("ADEX_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdexSistema)
                    .HasColumnName("ADEX_SISTEMA")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.AdexAplicacion)
                    .HasColumnName("ADEX_APLICACION")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdexSql)
                    .HasColumnName("ADEX_SQL")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Del).HasColumnName("DEL");

                entity.Property(e => e.Ins).HasColumnName("INS");

                entity.Property(e => e.Sel).HasColumnName("SEL");

                entity.Property(e => e.Upd).HasColumnName("UPD");

                entity.HasOne(d => d.Adex)
                    .WithMany(p => p.Adscexe)
                    .HasForeignKey(d => new { d.AdexBdd, d.AdexGrupo })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCEXE_ADSCGRP");

                entity.HasOne(d => d.AdexNavigation)
                    .WithMany(p => p.Adscexe)
                    .HasForeignKey(d => new { d.AdexSistema, d.AdexAplicacion })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCEXE_ADSCMENU");
            });

            modelBuilder.Entity<Adscgrp>(entity =>
            {
                entity.HasKey(e => new { e.AdgrBdd, e.AdgrGrupo })
                    .HasName("PK_ADSCGRP");

                entity.ToTable("ADSCGRP");

                entity.Property(e => e.AdgrBdd)
                    .HasColumnName("ADGR_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdgrGrupo)
                    .HasColumnName("ADGR_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdgrDescripcion)
                    .HasColumnName("ADGR_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdgrNombre)
                    .HasColumnName("ADGR_NOMBRE")
                    .HasColumnType("varchar(32)");

                entity.HasOne(d => d.AdgrBddNavigation)
                    .WithMany(p => p.Adscgrp)
                    .HasForeignKey(d => d.AdgrBdd)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCGRP_ADSCBDD");
            });

            modelBuilder.Entity<Adscmenu>(entity =>
            {
                entity.HasKey(e => new { e.AdmeSistema, e.AdmeAplicacion })
                    .HasName("PK_ADSCMENU");

                entity.ToTable("ADSCMENU");

                entity.Property(e => e.AdmeSistema)
                    .HasColumnName("ADME_SISTEMA")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.AdmeAplicacion)
                    .HasColumnName("ADME_APLICACION")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdmeAccionControlador)
                    .HasColumnName("ADME_ACCION_CONTROLADOR")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdmeControlador)
                    .HasColumnName("ADME_CONTROLADOR")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdmeDescripcion)
                    .HasColumnName("ADME_DESCRIPCION")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.AdmeElemento)
                    .HasColumnName("ADME_ELEMENTO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmeEnsamblado)
                    .HasColumnName("ADME_ENSAMBLADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmeEstado)
                    .HasColumnName("ADME_ESTADO")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdmeObjetivo)
                    .HasColumnName("ADME_OBJETIVO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmeOrden).HasColumnName("ADME_ORDEN");

                entity.Property(e => e.AdmePadre)
                    .HasColumnName("ADME_PADRE")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdmeTipo)
                    .HasColumnName("ADME_TIPO")
                    .HasColumnType("varchar(1)");

                entity.Property(e => e.AdmeTipoObjeto)
                    .HasColumnName("ADME_TIPO_OBJETO")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.AdmeUrl)
                    .HasColumnName("ADME_URL")
                    .HasColumnType("varchar(150)");

                entity.HasOne(d => d.AdmeSistemaNavigation)
                    .WithMany(p => p.Adscmenu)
                    .HasForeignKey(d => d.AdmeSistema)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCMENU_ADSCSIST");
            });

            modelBuilder.Entity<Adscmiem>(entity =>
            {
                entity.HasKey(e => new { e.AdmiEmpleado, e.AdmiGrupo, e.AdmiBdd })
                    .HasName("PK_ADSCMIEM");

                entity.ToTable("ADSCMIEM");

                entity.Property(e => e.AdmiEmpleado)
                    .HasColumnName("ADMI_EMPLEADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmiGrupo)
                    .HasColumnName("ADMI_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmiBdd)
                    .HasColumnName("ADMI_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmiCodigoEmpleado)
                    .HasColumnName("ADMI_CODIGO_EMPLEADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmiTotal)
                    .HasColumnName("ADMI_TOTAL")
                    .HasColumnType("nchar(3)");

                entity.HasOne(d => d.Admi)
                    .WithMany(p => p.Adscmiem)
                    .HasForeignKey(d => new { d.AdmiBdd, d.AdmiGrupo })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCMIEM_ADSCGRP");
            });

            modelBuilder.Entity<Adscpassw>(entity =>
            {
                entity.HasKey(e => e.AdpsLogin)
                    .HasName("PK_ADSCPASSW");

                entity.ToTable("ADSCPASSW");

                entity.Property(e => e.AdpsLogin)
                    .HasColumnName("ADPS_LOGIN")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdpsCodigoEmpleado)
                    .HasColumnName("ADPS_CODIGO_EMPLEADO")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdpsFechaCambio)
                    .HasColumnName("ADPS_FECHA_CAMBIO")
                    .HasColumnType("datetime");

                entity.Property(e => e.AdpsFechaVencimiento)
                    .HasColumnName("ADPS_FECHA_VENCIMIENTO")
                    .HasColumnType("datetime");

                entity.Property(e => e.AdpsIdContacto)
                    .HasColumnName("ADPS_ID_CONTACTO")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdpsIdEntidad)
                    .HasColumnName("ADPS_ID_ENTIDAD")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdpsIntentos).HasColumnName("ADPS_INTENTOS");

                entity.Property(e => e.AdpsLoginAdm)
                    .HasColumnName("ADPS_LOGIN_ADM")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdpsPasswCg)
                    .HasColumnName("ADPS_PASSW_CG")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.AdpsPasswPoint)
                    .HasColumnName("ADPS_PASSW_POINT")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.AdpsPassword)
                    .HasColumnName("ADPS_PASSWORD")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.AdpsPreguntaRecuperacion)
                    .HasColumnName("ADPS_PREGUNTA_RECUPERACION")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdpsRespuestaRecuperacion)
                    .HasColumnName("ADPS_RESPUESTA_RECUPERACION")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdpsTipoUso)
                    .HasColumnName("ADPS_TIPO_USO")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdpsToken)
                    .HasColumnName("ADPS_TOKEN")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<Adscsist>(entity =>
            {
                entity.HasKey(e => e.AdstSistema)
                    .HasName("PK_ADSCSIST");

                entity.ToTable("ADSCSIST");

                entity.Property(e => e.AdstSistema)
                    .HasColumnName("ADST_SISTEMA")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.AdstBdd)
                    .HasColumnName("ADST_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdstDescripcion)
                    .HasColumnName("ADST_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdstHost)
                    .HasColumnName("ADST_HOST")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.AdstTipo)
                    .HasColumnName("ADST_TIPO")
                    .HasColumnType("varchar(3)");

                entity.HasOne(d => d.AdstBddNavigation)
                    .WithMany(p => p.Adscsist)
                    .HasForeignKey(d => d.AdstBdd)
                    .HasConstraintName("FK_ADSCSIST_ADSCBDD");
            });

            modelBuilder.Entity<Adscswepwd>(entity =>
            {
                entity.HasKey(e => new { e.AdpsLogin, e.AdseSw })
                    .HasName("PK_ADSCSWEPWD");

                entity.ToTable("ADSCSWEPWD");

                entity.Property(e => e.AdpsLogin)
                    .HasColumnName("ADPS_LOGIN")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdseSw)
                    .HasColumnName("ADSE_SW")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdspObs)
                    .HasColumnName("ADSP_OBS")
                    .HasColumnType("varchar(200)");

                entity.HasOne(d => d.AdpsLoginNavigation)
                    .WithMany(p => p.Adscswepwd)
                    .HasForeignKey(d => d.AdpsLogin)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCSWEPWD_ADSCPASSW");

                entity.HasOne(d => d.AdseSwNavigation)
                    .WithMany(p => p.Adscswepwd)
                    .HasForeignKey(d => d.AdseSw)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCSWEPWD_ADSCSWEXT");
            });

            modelBuilder.Entity<Adscswext>(entity =>
            {
                entity.HasKey(e => e.AdseSw)
                    .HasName("PK_ADSCSWEXT_1");

                entity.ToTable("ADSCSWEXT");

                entity.Property(e => e.AdseSw)
                    .HasColumnName("ADSE_SW")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AdseDesc)
                    .HasColumnName("ADSE_DESC")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdseUri)
                    .HasColumnName("ADSE_URI")
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<Adsctoken>(entity =>
            {
                entity.HasKey(e => e.AdtoId)
                    .HasName("PK_ADSCTOKEN");

                entity.ToTable("ADSCTOKEN");

                entity.Property(e => e.AdtoId).HasColumnName("ADTO_ID");

                entity.Property(e => e.AdpsLogin)
                    .IsRequired()
                    .HasColumnName("ADPS_LOGIN")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdtoToken)
                    .IsRequired()
                    .HasColumnName("ADTO_TOKEN")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.AdtoNombreServicio)
                    .IsRequired()
                    .HasColumnName("ADTO_NOMSERV")
                    .HasColumnType("varchar(500)");
            });
        }



    }




}






