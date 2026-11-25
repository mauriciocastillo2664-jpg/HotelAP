using System;
using System.Collections.Generic;
using Hotel.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Hotel.AccesoDatos.Contexto;

public partial class HotelContext : DbContext
{
    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<AjusteCuenta> AjustesCuenta { get; set; }

    public virtual DbSet<Amenidad> Amenidades { get; set; }

    public virtual DbSet<Auditoria> Auditoria { get; set; }

    public virtual DbSet<CargoAdicional> CargosAdicionales { get; set; }

    public virtual DbSet<CategoriaPlatillo> CategoriasPlatillo { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Correo> Correos { get; set; }

    public virtual DbSet<Descuento> Descuentos { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Estadia> Estadias { get; set; }

    public virtual DbSet<EstadoCorreo> EstadosCorreo { get; set; }

    public virtual DbSet<EstadoHabitacion> EstadosHabitacion { get; set; }

    public virtual DbSet<EstadoPago> EstadosPago { get; set; }

    public virtual DbSet<EstadoPedido> EstadosPedido { get; set; }

    public virtual DbSet<EstadoReservacion> EstadosReservacion { get; set; }

    public virtual DbSet<EstadoUsuario> EstadosUsuario { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FacturaDetalle> FacturaDetalles { get; set; }

    public virtual DbSet<HabitacionFoto> HabitacionFotos { get; set; }

    public virtual DbSet<Habitacion> Habitaciones { get; set; }

    public virtual DbSet<HistorialEstadoHabitacion> HistorialEstadosHabitacion { get; set; }

    public virtual DbSet<HistorialEstadoPedido> HistorialEstadosPedido { get; set; }

    public virtual DbSet<HistorialReservacion> HistorialReservaciones { get; set; }

    public virtual DbSet<HuespedReservacion> HuespedesReservacion { get; set; }

    public virtual DbSet<MetodoPago> MetodosPago { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Parametro> Parametros { get; set; }

    public virtual DbSet<PedidoRestaurante> PedidosRestaurante { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Platillo> Platillos { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReservacionHabitacion> ReservacionHabitaciones { get; set; }

    public virtual DbSet<Reservacion> Reservaciones { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<Temporada> Temporadas { get; set; }

    public virtual DbSet<TipoCorreo> TiposCorreo { get; set; }

    public virtual DbSet<TipoHabitacion> TiposHabitacion { get; set; }

    public virtual DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }

    public virtual DbSet<TokenUsuario> TokensUsuario { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwEstadoCuenta> VwEstadoCuentas { get; set; }

    public virtual DbSet<VwEstadoUsuario> VwEstadoUsuarios { get; set; }

    public virtual DbSet<VwFacturaAbono> VwFacturaAbonos { get; set; }

    public virtual DbSet<VwFacturaConcepto> VwFacturaConceptos { get; set; }

    public virtual DbSet<VwHistorialCorreo> VwHistorialCorreos { get; set; }

    public virtual DbSet<VwIngresosPorMes> VwIngresosPorMes { get; set; }

    public virtual DbSet<VwOcupacionActual> VwOcupacionActual { get; set; }

    public virtual DbSet<VwPlatillosMasVendido> VwPlatillosMasVendidos { get; set; }

    public virtual DbSet<VwReservacionesDetalle> VwReservacionesDetalle { get; set; }

    public virtual DbSet<VwTiemposPreparacion> VwTiemposPreparacion { get; set; }

    public virtual DbSet<VwUsuarioPermiso> VwUsuarioPermisos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.ToTable("Abonos");

            entity.HasKey(e => e.Id).HasName("PK__Abonos__3214EC07DBB0895E");

            entity.HasIndex(e => e.FechaAbono, "IX_Abonos_Fecha");

            entity.HasIndex(e => e.PagoId, "IX_Abonos_Pago");

            entity.HasIndex(e => e.Referencia, "UX_Abonos_Referencia")
                .IsUnique()
                .HasFilter("([Referencia] IS NOT NULL)");

            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("Confirmado");
            entity.Property(e => e.FechaAbono)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MotivoReverso).HasMaxLength(300);
            entity.Property(e => e.Referencia).HasMaxLength(100);
            entity.Property(e => e.UltimosDigitosTarjeta)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.MetodoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abonos__MetodoPa__3CF40B7E");

            entity.HasOne(d => d.Pago).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.PagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abonos__PagoId__3BFFE745");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Abonos__UsuarioI__41B8C09B");
        });

        modelBuilder.Entity<AjusteCuenta>(entity =>
        {
            entity.ToTable("AjustesCuenta");

            entity.HasKey(e => e.Id).HasName("PK__AjustesC__3214EC07DA2B413E");

            entity.HasIndex(e => e.ReservacionId, "IX_Ajustes_Reservacion");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.AutorizadoPor).WithMany(p => p.AjustesCuentumAutorizadoPors)
                .HasForeignKey(d => d.AutorizadoPorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AjustesCu__Autor__477199F1");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.AjustesCuenta)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AjustesCu__Reser__44952D46");

            entity.HasOne(d => d.Usuario).WithMany(p => p.AjustesCuentumUsuarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AjustesCu__Usuar__467D75B8");
        });

        modelBuilder.Entity<Amenidad>(entity =>
        {
            entity.ToTable("Amenidades");

            entity.HasKey(e => e.Id).HasName("PK__Amenidad__3214EC07FA708CCD");

            entity.HasIndex(e => e.Nombre, "UQ__Amenidad__75E3EFCF8B09E07E").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("Auditoria");

            entity.HasKey(e => e.Id).HasName("PK__Auditori__3214EC07864D09CA");

            entity.HasIndex(e => e.FechaUtc, "IX_Auditoria_Fecha");

            entity.Property(e => e.Accion).HasMaxLength(100);
            entity.Property(e => e.Datos).HasMaxLength(500);
            entity.Property(e => e.DireccionIp)
                .HasMaxLength(45)
                .HasColumnName("DireccionIP");
            entity.Property(e => e.Entidad).HasMaxLength(100);
            entity.Property(e => e.FechaUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Resultado)
                .HasMaxLength(10)
                .HasDefaultValue("Exito");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Auditoria__Usuar__22751F6C");
        });

        modelBuilder.Entity<CargoAdicional>(entity =>
        {
            entity.ToTable("CargosAdicionales");

            entity.HasKey(e => e.Id).HasName("PK__CargosAd__3214EC07F0A60310");

            entity.Property(e => e.Concepto).HasMaxLength(150);
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.CargosAdicionales)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CargosAdi__Reser__72910220");

            entity.HasOne(d => d.Usuario).WithMany(p => p.CargosAdicionales)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__CargosAdi__Usuar__76619304");
        });

        modelBuilder.Entity<CategoriaPlatillo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC075F887A78");

            entity.ToTable("CategoriasPlatillo");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCFEDA37E13").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");

            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC07802BA938");

            entity.HasIndex(e => new { e.Apellidos, e.Nombres }, "IX_Clientes_Apellidos");

            entity.HasIndex(e => new { e.TipoIdentificacionId, e.NumeroIdentificacion }, "UQ_Clientes_Identificacion").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Clientes__A9D10534A17E7B0A").IsUnique();

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EstadoUsuarioId).HasDefaultValue(1);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nacionalidad).HasMaxLength(50);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroIdentificacion).HasMaxLength(30);
            entity.Property(e => e.Preferencias).HasMaxLength(300);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Telefono).HasMaxLength(20);

            entity.HasOne(d => d.Descuento).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.DescuentoId)
                .HasConstraintName("FK__Clientes__Descue__797309D9");

            entity.HasOne(d => d.EstadoUsuario).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.EstadoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Clientes__Estado__7B5B524B");

            entity.HasOne(d => d.TipoIdentificacion).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.TipoIdentificacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Clientes__TipoId__787EE5A0");
        });

        modelBuilder.Entity<Correo>(entity =>
        {
            entity.ToTable("Correos");

            entity.HasKey(e => e.Id).HasName("PK__Correos__3214EC078E895E2A");

            entity.HasIndex(e => e.ClienteId, "IX_Correos_Cliente");

            entity.HasIndex(e => new { e.EstadoCorreoId, e.FechaRegistro }, "IX_Correos_Estado");

            entity.Property(e => e.Asunto).HasMaxLength(200);
            entity.Property(e => e.Destinatario).HasMaxLength(150);
            entity.Property(e => e.Direccion).HasMaxLength(10);
            entity.Property(e => e.FechaEnvio).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MensajeError).HasMaxLength(500);
            entity.Property(e => e.Remitente).HasMaxLength(150);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Correos)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Correos__Cliente__4E1E9780");

            entity.HasOne(d => d.CorreoRespuesta).WithMany(p => p.InverseCorreoRespuesta)
                .HasForeignKey(d => d.CorreoRespuestaId)
                .HasConstraintName("FK__Correos__CorreoR__50FB042B");

            entity.HasOne(d => d.EstadoCorreo).WithMany(p => p.Correos)
                .HasForeignKey(d => d.EstadoCorreoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Correos__EstadoC__4D2A7347");

            entity.HasOne(d => d.Factura).WithMany(p => p.Correos)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("FK__Correos__Factura__5006DFF2");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.Correos)
                .HasForeignKey(d => d.ReservacionId)
                .HasConstraintName("FK__Correos__Reserva__4F12BBB9");

            entity.HasOne(d => d.TipoCorreo).WithMany(p => p.Correos)
                .HasForeignKey(d => d.TipoCorreoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Correos__TipoCor__4C364F0E");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Correos)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Correos__Usuario__52E34C9D");
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.ToTable("Descuentos");

            entity.HasKey(e => e.Id).HasName("PK__Descuent__3214EC0753D4048A");

            entity.HasIndex(e => e.Codigo, "UX_Descuentos_Codigo")
                .IsUnique()
                .HasFilter("([Codigo] IS NOT NULL)");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AplicaA).HasMaxLength(15);
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.Tipo).HasMaxLength(15);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallePedidos");

            entity.HasKey(e => e.Id).HasName("PK__DetalleP__3214EC07AC10E309");

            entity.HasIndex(e => new { e.PedidoId, e.PlatilloId }, "UQ_DetallePedidos").IsUnique();

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([Cantidad]*[PrecioUnitario])", false)
                .HasColumnType("decimal(21, 2)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePe__Pedid__18B6AB08");

            entity.HasOne(d => d.Platillo).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PlatilloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePe__Plati__19AACF41");
        });

        modelBuilder.Entity<Estadia>(entity =>
        {
            entity.ToTable("Estadias");

            entity.HasKey(e => e.Id).HasName("PK__Estadias__3214EC070E4D34B1");

            entity.HasIndex(e => e.ReservacionId, "UQ__Estadias__145B3ED43CFD3671").IsUnique();

            entity.Property(e => e.CheckInReal).HasColumnType("datetime");
            entity.Property(e => e.CheckOutReal).HasColumnType("datetime");
            entity.Property(e => e.CodigoAcceso).HasMaxLength(20);

            entity.HasOne(d => d.Reservacion).WithOne(p => p.Estadia)
                .HasForeignKey<Estadia>(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Estadias__Reserv__6DCC4D03");

            entity.HasOne(d => d.UsuarioCheckIn).WithMany(p => p.EstadiaUsuarioCheckIns)
                .HasForeignKey(d => d.UsuarioCheckInId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Estadias__Usuari__6EC0713C");

            entity.HasOne(d => d.UsuarioCheckOut).WithMany(p => p.EstadiaUsuarioCheckOuts)
                .HasForeignKey(d => d.UsuarioCheckOutId)
                .HasConstraintName("FK__Estadias__Usuari__6FB49575");
        });

        modelBuilder.Entity<EstadoCorreo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosC__3214EC07F608A034");

            entity.ToTable("EstadosCorreo");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosC__75E3EFCFCA67D6D5").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<EstadoHabitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosH__3214EC07DB89C8C4");

            entity.ToTable("EstadosHabitacion");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosH__75E3EFCF16E352CE").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosP__3214EC07B1A744EE");

            entity.ToTable("EstadosPago");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosP__75E3EFCF41290EA0").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosP__3214EC0764781AC7");

            entity.ToTable("EstadosPedido");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosP__75E3EFCF38BF4921").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<EstadoReservacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosR__3214EC079F43A7B3");

            entity.ToTable("EstadosReservacion");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosR__75E3EFCF0AB7B43E").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosU__3214EC07327C7E64");

            entity.ToTable("EstadosUsuario");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosU__75E3EFCFAC414C06").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.ToTable("Facturas");

            entity.HasKey(e => e.Id).HasName("PK__Facturas__3214EC0752B77A62");

            entity.HasIndex(e => e.NumeroFactura, "UQ__Facturas__CF12F9A61853F0C8").IsUnique();

            entity.HasIndex(e => e.PedidoId, "UX_Facturas_Pedido")
                .IsUnique()
                .HasFilter("([PedidoId] IS NOT NULL)");

            entity.HasIndex(e => e.ReservacionId, "UX_Facturas_Reservacion")
                .IsUnique()
                .HasFilter("([ReservacionId] IS NOT NULL)");

            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroFactura).HasMaxLength(25);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPagado).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Facturas__Client__251C81ED");

            entity.HasOne(d => d.Pedido).WithOne(p => p.Factura)
                .HasForeignKey<Factura>(d => d.PedidoId)
                .HasConstraintName("FK__Facturas__Pedido__2704CA5F");

            entity.HasOne(d => d.Reservacion).WithOne(p => p.Factura)
                .HasForeignKey<Factura>(d => d.ReservacionId)
                .HasConstraintName("FK__Facturas__Reserv__2610A626");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Facturas__Usuari__28ED12D1");
        });

        modelBuilder.Entity<FacturaDetalle>(entity =>
        {
            entity.ToTable("FacturaDetalles");

            entity.HasKey(e => e.Id).HasName("PK__FacturaD__3214EC07FEE6074B");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoConcepto).HasMaxLength(20);

            entity.HasOne(d => d.Factura).WithMany(p => p.FacturaDetalles)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FacturaDe__Factu__2CBDA3B5");
        });

        modelBuilder.Entity<HabitacionFoto>(entity =>
        {
            entity.ToTable("HabitacionFotos");

            entity.HasKey(e => e.Id).HasName("PK__Habitaci__3214EC07A08B0997");

            entity.HasIndex(e => new { e.HabitacionId, e.Orden }, "IX_HabitacionFotos_Habitacion");

            entity.HasIndex(e => e.HabitacionId, "UX_HabitacionFotos_Principal")
                .IsUnique()
                .HasFilter("([EsPrincipal]=(1))");

            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Orden).HasDefaultValue(1);
            entity.Property(e => e.Url).HasMaxLength(300);

            entity.HasOne(d => d.Habitacion).WithMany(p => p.HabitacionFotos)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__Habit__3F115E1A");
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Habitaci__3214EC079AE66D55");

            entity.ToTable("Habitaciones", table =>
            {
                table.HasTrigger("trg_Habitaciones_Estado");
                table.UseSqlOutputClause(false);
            });

            entity.HasIndex(e => e.Numero, "UQ__Habitaci__7E532BC637964381").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.EstadoHabitacionId).HasDefaultValue(1);
            entity.Property(e => e.Numero).HasMaxLength(10);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.EstadoHabitacion).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.EstadoHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__Estad__37703C52");

            entity.HasOne(d => d.TipoHabitacion).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.TipoHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__TipoH__3587F3E0");

            entity.HasMany(d => d.Amenidads).WithMany(p => p.Habitacions)
                .UsingEntity<Dictionary<string, object>>(
                    "HabitacionAmenidade",
                    r => r.HasOne<Amenidad>().WithMany()
                        .HasForeignKey("AmenidadId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Habitacio__Ameni__3C34F16F"),
                    l => l.HasOne<Habitacion>().WithMany()
                        .HasForeignKey("HabitacionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Habitacio__Habit__3B40CD36"),
                    j =>
                    {
                        j.HasKey("HabitacionId", "AmenidadId").HasName("PK__Habitaci__433D3AEDC7F69BB5");
                        j.ToTable("HabitacionAmenidades");
                    });
        });

        modelBuilder.Entity<HistorialEstadoHabitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC07126FBA45");

            entity.ToTable("HistorialEstadoHabitacion");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Motivo).HasMaxLength(200);

            entity.HasOne(d => d.EstadoAnterior).WithMany(p => p.HistorialEstadoHabitacionEstadoAnteriors)
                .HasForeignKey(d => d.EstadoAnteriorId)
                .HasConstraintName("FK__Historial__Estad__44CA3770");

            entity.HasOne(d => d.EstadoNuevo).WithMany(p => p.HistorialEstadoHabitacionEstadoNuevos)
                .HasForeignKey(d => d.EstadoNuevoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Estad__45BE5BA9");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.HistorialEstadosHabitacion)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Habit__43D61337");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialEstadosHabitacion)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Historial__Usuar__46B27FE2");
        });

        modelBuilder.Entity<HistorialEstadoPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC076569CE9A");

            entity.ToTable("HistorialEstadoPedido");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.HistorialEstadosPedido)
                .HasForeignKey(d => d.EstadoPedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Estad__1E6F845E");

            entity.HasOne(d => d.Pedido).WithMany(p => p.HistorialEstadosPedido)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Pedid__1D7B6025");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialEstadosPedido)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Historial__Usuar__1F63A897");
        });

        modelBuilder.Entity<HistorialReservacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC07F0E644C7");

            entity.ToTable("HistorialReservacion");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.EstadoAnterior).WithMany(p => p.HistorialReservacionEstadoAnteriors)
                .HasForeignKey(d => d.EstadoAnteriorId)
                .HasConstraintName("FK__Historial__Estad__671F4F74");

            entity.HasOne(d => d.EstadoNuevo).WithMany(p => p.HistorialReservacionEstadoNuevos)
                .HasForeignKey(d => d.EstadoNuevoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Estad__681373AD");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.HistorialReservaciones)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Reser__662B2B3B");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialReservaciones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Historial__Usuar__690797E6");
        });

        modelBuilder.Entity<HuespedReservacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Huespede__3214EC079941AD6E");

            entity.ToTable("HuespedesReservacion");

            entity.Property(e => e.NombreCompleto).HasMaxLength(200);
            entity.Property(e => e.NumeroIdentificacion).HasMaxLength(30);

            entity.HasOne(d => d.Reservacion).WithMany(p => p.HuespedesReservacion)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Huespedes__Reser__607251E5");

            entity.HasOne(d => d.TipoIdentificacion).WithMany(p => p.HuespedesReservacion)
                .HasForeignKey(d => d.TipoIdentificacionId)
                .HasConstraintName("FK__Huespedes__TipoI__6166761E");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MetodosP__3214EC0732BE4FFF");

            entity.ToTable("MetodosPago");

            entity.HasIndex(e => e.Nombre, "UQ__MetodosP__75E3EFCFE8A7553F").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("Pagos");

            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC078D825A5D");

            entity.HasIndex(e => e.EstadoPagoId, "IX_Pagos_Estado");

            entity.HasIndex(e => e.PedidoId, "UX_Pagos_Pedido")
                .IsUnique()
                .HasFilter("([PedidoId] IS NOT NULL)");

            entity.HasIndex(e => e.ReservacionId, "UX_Pagos_Reservacion")
                .IsUnique()
                .HasFilter("([ReservacionId] IS NOT NULL)");

            entity.Property(e => e.EstadoPagoId).HasDefaultValue(1);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Saldo)
                .HasComputedColumnSql("([MontoTotal]-[TotalAbonado])", false)
                .HasColumnType("decimal(11, 2)");
            entity.Property(e => e.TotalAbonado).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pagos__ClienteId__308E3499");

            entity.HasOne(d => d.EstadoPago).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.EstadoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pagos__EstadoPag__36470DEF");

            entity.HasOne(d => d.Factura).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("FK__Pagos__FacturaId__373B3228");

            entity.HasOne(d => d.Pedido).WithOne(p => p.Pago)
                .HasForeignKey<Pago>(d => d.PedidoId)
                .HasConstraintName("FK__Pagos__PedidoId__32767D0B");

            entity.HasOne(d => d.Reservacion).WithOne(p => p.Pago)
                .HasForeignKey<Pago>(d => d.ReservacionId)
                .HasConstraintName("FK__Pagos__Reservaci__318258D2");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.ToTable("Parametros");

            entity.HasKey(e => e.Id).HasName("PK__Parametr__3214EC071764D419");

            entity.HasIndex(e => e.Clave, "UQ__Parametr__E8181E1188DBBA0B").IsUnique();

            entity.Property(e => e.Clave).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TipoDato).HasMaxLength(10);
            entity.Property(e => e.Valor).HasMaxLength(100);
        });

        modelBuilder.Entity<PedidoRestaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PedidosR__3214EC0775649E76");

            entity.ToTable("PedidosRestaurante");

            entity.HasIndex(e => new { e.EstadoPedidoId, e.FechaPedido }, "IX_Pedidos_Estado");

            entity.HasIndex(e => e.ReservacionId, "IX_Pedidos_Reservacion");

            entity.Property(e => e.CargoAhabitacion).HasColumnName("CargoAHabitacion");
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EstadoPedidoId).HasDefaultValue(1);
            entity.Property(e => e.FechaHoraDeseada).HasColumnType("datetime");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Instrucciones).HasMaxLength(300);
            entity.Property(e => e.Modalidad).HasMaxLength(20);
            entity.Property(e => e.Recargo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidosRe__Clien__0697FACD");

            entity.HasOne(d => d.DescuentoNavigation).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.DescuentoId)
                .HasConstraintName("FK__PedidosRe__Descu__0D44F85C");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.EstadoPedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidosRe__Estad__0C50D423");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.HabitacionId)
                .HasConstraintName("FK__PedidosRe__Habit__0880433F");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.ReservacionId)
                .HasConstraintName("FK__PedidosRe__Reser__078C1F06");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PedidosRestaurante)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__PedidosRe__Usuar__13F1F5EB");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.ToTable("Permisos");

            entity.HasKey(e => e.Id).HasName("PK__Permisos__3214EC07C885180D");

            entity.HasIndex(e => e.Codigo, "UQ__Permisos__06370DACD543F26C").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
        });

        modelBuilder.Entity<Platillo>(entity =>
        {
            entity.ToTable("Platillos");

            entity.HasKey(e => e.Id).HasName("PK__Platillo__3214EC07B71415B1");

            entity.HasIndex(e => e.CategoriaId, "IX_Platillos_Categoria");

            entity.HasIndex(e => e.Nombre, "UQ__Platillo__75E3EFCFC4A7793C").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Disponible).HasDefaultValue(true);
            entity.Property(e => e.ImagenUrl).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TiempoPreparacion).HasDefaultValue(15);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Platillos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Platillos__Categ__7EF6D905");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07B973A7EA");

            entity.HasIndex(e => e.TokenHash, "UQ__RefreshT__BCB33F92E5CF7D1B").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRevocacion).HasColumnType("datetime");
            entity.Property(e => e.TokenHash).HasMaxLength(256);

            entity.HasOne(d => d.TokenReemplazo).WithMany(p => p.InverseTokenReemplazo)
                .HasForeignKey(d => d.TokenReemplazoId)
                .HasConstraintName("FK__RefreshTo__Token__19DFD96B");

            entity.HasOne(d => d.Usuario).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__Usuar__17F790F9");
        });

        modelBuilder.Entity<ReservacionHabitacion>(entity =>
        {
            entity.ToTable("ReservacionHabitaciones");

            entity.HasKey(e => e.Id).HasName("PK__Reservac__3214EC07BF041539");

            entity.HasIndex(e => new { e.HabitacionId, e.FechaInicio, e.FechaFin }, "IX_ResHab_Disponibilidad");

            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Habit__5D95E53A");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Reser__5CA1C101");
        });

        modelBuilder.Entity<Reservacion>(entity =>
        {
            entity.ToTable("Reservaciones");

            entity.HasKey(e => e.Id).HasName("PK__Reservac__3214EC07FC23993F");

            entity.HasIndex(e => new { e.ClienteId, e.EstadoReservacionId }, "IX_Reservaciones_Cliente");

            entity.HasIndex(e => new { e.FechaInicio, e.FechaFin }, "IX_Reservaciones_Fechas");

            entity.HasIndex(e => e.Codigo, "UQ__Reservac__06370DACB5A204D4").IsUnique();

            entity.Property(e => e.AnticipoRequerido).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Codigo).HasMaxLength(25);
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EstadoReservacionId).HasDefaultValue(1);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MotivoCancelacion).HasMaxLength(300);
            entity.Property(e => e.Observaciones).HasMaxLength(300);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Clien__4C6B5938");

            entity.HasOne(d => d.DescuentoNavigation).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.DescuentoId)
                .HasConstraintName("FK__Reservaci__Descu__5224328E");

            entity.HasOne(d => d.EstadoReservacion).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.EstadoReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Estad__51300E55");

            entity.HasOne(d => d.UsuarioCreacion).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.UsuarioCreacionId)
                .HasConstraintName("FK__Reservaci__Usuar__57DD0BE4");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07585010CD");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__75E3EFCF15023954").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasMany(d => d.Permisos).WithMany(p => p.Rols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolPermis__Permi__10566F31"),
                    l => l.HasOne<Rol>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolPermis__RolId__0F624AF8"),
                    j =>
                    {
                        j.HasKey("RolId", "PermisoId").HasName("PK__RolPermi__D04D0E8316A94A9F");
                        j.ToTable("RolPermisos");
                    });
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.ToTable("Temporadas");

            entity.HasKey(e => e.Id).HasName("PK__Temporad__3214EC070A3F4472");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.PorcentajeRecargo).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<TipoCorreo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TiposCor__3214EC070CD01598");

            entity.ToTable("TiposCorreo");

            entity.HasIndex(e => e.Nombre, "UQ__TiposCor__75E3EFCFF218378C").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(40);
        });

        modelBuilder.Entity<TipoHabitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TiposHab__3214EC079233DF4E");

            entity.ToTable("TiposHabitacion");

            entity.HasIndex(e => e.Nombre, "UQ__TiposHab__75E3EFCF7FD02BFA").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CargoPersonaAdicional).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.PersonasIncluidas).HasDefaultValue(2);
            entity.Property(e => e.PrecioBasePorNoche).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TiposIde__3214EC07A64B8652");

            entity.ToTable("TiposIdentificacion");

            entity.HasIndex(e => e.Nombre, "UQ__TiposIde__75E3EFCF787CE5F1").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Formato).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<TokenUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TokensUs__3214EC0720699A69");

            entity.ToTable("TokensUsuario");

            entity.HasIndex(e => new { e.UsuarioId, e.Tipo }, "IX_TokensUsuario_Usuario");

            entity.HasIndex(e => e.TokenHash, "UQ__TokensUs__BCB33F92CDB00F9D").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.FechaUso).HasColumnType("datetime");
            entity.Property(e => e.Tipo).HasMaxLength(20);
            entity.Property(e => e.TokenHash).HasMaxLength(256);

            entity.HasOne(d => d.Usuario).WithMany(p => p.TokensUsuario)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TokensUsu__Usuar__1DB06A4F");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC075D852490");

            entity.HasIndex(e => new { e.TipoIdentificacionId, e.NumeroIdentificacion }, "UQ_Usuarios_Identificacion").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__6B0F5AE01BAD1040").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D10534081190D1").IsUnique();

            entity.HasIndex(e => e.ClienteId, "UX_Usuarios_Cliente")
                .IsUnique()
                .HasFilter("([ClienteId] IS NOT NULL)");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EstadoUsuarioId).HasDefaultValue(1);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroIdentificacion).HasMaxLength(30);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UltimoAcceso).HasColumnType("datetime");

            entity.HasOne(d => d.Cliente).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.ClienteId)
                .HasConstraintName("FK__Usuarios__Client__05D8E0BE");

            entity.HasOne(d => d.EstadoUsuario).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.EstadoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__Estado__03F0984C");

            entity.HasOne(d => d.TipoIdentificacion).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.TipoIdentificacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__TipoId__02084FDA");

            entity.HasMany(d => d.Rols).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRole",
                    r => r.HasOne<Rol>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioRo__RolId__14270015"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioRo__Usuar__1332DBDC"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "RolId").HasName("PK__UsuarioR__24AFD797394DB819");
                        j.ToTable("UsuarioRoles");
                    });
        });

        modelBuilder.Entity<VwEstadoCuenta>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_EstadoCuentas");

            entity.Property(e => e.Cliente).HasMaxLength(201);
            entity.Property(e => e.Concepto).HasMaxLength(38);
            entity.Property(e => e.EstadoPago).HasMaxLength(30);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Saldo).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.TotalAbonado).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VwEstadoUsuario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_EstadoUsuarios");

            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.Identificador).HasMaxLength(150);
            entity.Property(e => e.NombreCompleto).HasMaxLength(201);
            entity.Property(e => e.NumeroIdentificacion).HasMaxLength(30);
            entity.Property(e => e.TipoIdentificacion).HasMaxLength(50);
            entity.Property(e => e.TipoRegistro)
                .HasMaxLength(7)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwFacturaAbono>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_FacturaAbonos");

            entity.Property(e => e.Estado).HasMaxLength(15);
            entity.Property(e => e.EstadoPago).HasMaxLength(30);
            entity.Property(e => e.FechaAbono).HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(30);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroFactura).HasMaxLength(25);
            entity.Property(e => e.Referencia).HasMaxLength(100);
            entity.Property(e => e.Saldo).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.TotalAbonado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalFactura).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UltimosDigitosTarjeta)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<VwFacturaConcepto>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_FacturaConceptos");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Cliente).HasMaxLength(201);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaEmision).HasColumnType("datetime");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroFactura).HasMaxLength(25);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Reservacion).HasMaxLength(25);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubtotalFactura).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoConcepto).HasMaxLength(20);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VwHistorialCorreo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_HistorialCorreos");

            entity.Property(e => e.Asunto).HasMaxLength(200);
            entity.Property(e => e.Cliente).HasMaxLength(201);
            entity.Property(e => e.Destinatario).HasMaxLength(150);
            entity.Property(e => e.Direccion).HasMaxLength(10);
            entity.Property(e => e.EnviadoPor).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.FechaEnvio).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.MensajeError).HasMaxLength(500);
            entity.Property(e => e.NumeroFactura).HasMaxLength(25);
            entity.Property(e => e.Remitente).HasMaxLength(150);
            entity.Property(e => e.Reservacion).HasMaxLength(25);
            entity.Property(e => e.Tipo).HasMaxLength(40);
        });

        modelBuilder.Entity<VwIngresosPorMes>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_IngresosPorMes");

            entity.Property(e => e.MetodoPago).HasMaxLength(30);
            entity.Property(e => e.Origen)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwOcupacionActual>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OcupacionActual");

            entity.Property(e => e.Estado).HasMaxLength(30);
        });

        modelBuilder.Entity<VwPlatillosMasVendido>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_PlatillosMasVendidos");

            entity.Property(e => e.Categoria).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.TotalVendido).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwReservacionesDetalle>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ReservacionesDetalle");

            entity.Property(e => e.AnticipoRequerido).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Cliente).HasMaxLength(201);
            entity.Property(e => e.Codigo).HasMaxLength(25);
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.Habitacion).HasMaxLength(10);
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Saldo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoHabitacion).HasMaxLength(50);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VwTiemposPreparacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TiemposPreparacion");

            entity.Property(e => e.InicioPreparacion).HasColumnType("datetime");
            entity.Property(e => e.Listo).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwUsuarioPermiso>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_UsuarioPermisos");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.Permiso).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(50);
        });
        modelBuilder.HasSequence("SeqFactura");
        modelBuilder.HasSequence("SeqReservacion");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
