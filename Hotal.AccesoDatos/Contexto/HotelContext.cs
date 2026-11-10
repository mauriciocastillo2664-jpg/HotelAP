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

    public virtual DbSet<Amenidad> Amenidades { get; set; }

    public virtual DbSet<Auditoria> Auditoria { get; set; }

    public virtual DbSet<CategoriaPlatillo> CategoriasPlatillos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Estadia> Estadias { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Habitacion> Habitaciones { get; set; }

    public virtual DbSet<HuespedReservacion> HuespedesReservacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PedidoRestaurante> PedidosRestaurantes { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Platillo> Platillos { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReservacionHabitacion> ReservacionHabitaciones { get; set; }

    public virtual DbSet<Reservacion> Reservaciones { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<TipoHabitacion> TiposHabitacions { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Amenidad__3214EC0754650155");

            entity.HasIndex(e => e.Nombre, "UQ__Amenidad__75E3EFCF026730FA").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Auditori__3214EC07CF5CA656");

            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Entidad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntidadId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Auditoria__Usuar__607251E5");
        });

        modelBuilder.Entity<CategoriaPlatillo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC079BF322F9");

            entity.ToTable("CategoriasPlatillo");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCF1FCD596E").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC07A2AEEA74");

            entity.HasIndex(e => new { e.TipoDocumento, e.NumeroDocumento }, "UQ_Clientes_Documento").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Clientes__A9D10534E7396AC7").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Descuento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nacionalidad).HasMaxLength(50);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Preferencias).HasMaxLength(500);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetalleP__3214EC073CB84033");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePe__Pedid__59C55456");

            entity.HasOne(d => d.Platillo).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PlatilloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePe__Plati__5AB9788F");
        });

        modelBuilder.Entity<Estadia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Estadias__3214EC070010A0BE");

            entity.HasIndex(e => e.ReservacionId, "UQ__Estadias__145B3ED4B78E3993").IsUnique();

            entity.Property(e => e.CheckInReal).HasColumnType("datetime");
            entity.Property(e => e.CheckOutReal).HasColumnType("datetime");

            entity.HasOne(d => d.Reservacion).WithOne(p => p.Estadia)
                .HasForeignKey<Estadia>(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Estadias__Reserv__2EDAF651");

            entity.HasOne(d => d.UsuarioCheckIn).WithMany(p => p.EstadiaUsuarioCheckIns)
                .HasForeignKey(d => d.UsuarioCheckInId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Estadias__Usuari__2FCF1A8A");

            entity.HasOne(d => d.UsuarioCheckOut).WithMany(p => p.EstadiaUsuarioCheckOuts)
                .HasForeignKey(d => d.UsuarioCheckOutId)
                .HasConstraintName("FK__Estadias__Usuari__30C33EC3");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facturas__3214EC07D022E8E6");

            entity.HasIndex(e => e.NumeroFactura, "UQ__Facturas__CF12F9A6329203DB").IsUnique();

            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Facturas__Reserv__3D2915A8");
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Habitaci__3214EC07D8569C5B");

            entity.HasIndex(e => e.Numero, "UQ__Habitaci__7E532BC660C85BEB").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.EstadoOperativo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Disponible");
            entity.Property(e => e.FotoUrl).HasMaxLength(400);
            entity.Property(e => e.Numero)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.TipoHabitacion).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.TipoHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Habitacio__TipoH__0C85DE4D");

            entity.HasMany(d => d.Amenidads).WithMany(p => p.Habitacions)
                .UsingEntity<Dictionary<string, object>>(
                    "HabitacionAmenidade",
                    r => r.HasOne<Amenidad>().WithMany()
                        .HasForeignKey("AmenidadId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Habitacio__Ameni__1332DBDC"),
                    l => l.HasOne<Habitacion>().WithMany()
                        .HasForeignKey("HabitacionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Habitacio__Habit__123EB7A3"),
                    j =>
                    {
                        j.HasKey("HabitacionId", "AmenidadId").HasName("PK__Habitaci__433D3AEDE5BBDDA2");
                        j.ToTable("HabitacionAmenidades");
                    });
        });

        modelBuilder.Entity<HuespedReservacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Huespede__3214EC07AA78FA0B");

            entity.ToTable("HuespedesReservacion");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Reservacion).WithMany(p => p.HuespedesReservacions)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Huespedes__Reser__2A164134");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC07E2138F24");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Confirmado");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Referencia).HasMaxLength(100);

            entity.HasOne(d => d.Reservacion).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pagos__Reservaci__3493CFA7");
        });

        modelBuilder.Entity<PedidoRestaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PedidosR__3214EC0756D8EE62");

            entity.ToTable("PedidosRestaurante");

            entity.Property(e => e.EstadoPedido)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InstruccionesEspeciales).HasMaxLength(500);
            entity.Property(e => e.Modalidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidosRe__Clien__503BEA1C");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.ReservacionId)
                .HasConstraintName("FK__PedidosRe__Reser__51300E55");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permisos__3214EC07219B5E38");

            entity.HasIndex(e => e.Codigo, "UQ__Permisos__06370DACD0DD2A6F").IsUnique();

            entity.Property(e => e.Codigo)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Platillo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Platillo__3214EC0785F9C3E1");

            entity.HasIndex(e => e.Nombre, "UQ__Platillo__75E3EFCF5FF7699E").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Disponible).HasDefaultValue(true);
            entity.Property(e => e.ImagenUrl).HasMaxLength(400);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TiempoPreparacionMin).HasDefaultValue(15);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Platillos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Platillos__Categ__489AC854");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0760A97A61");

            entity.HasIndex(e => e.TokenHash, "UQ__RefreshT__BCB33F928E1261FD").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.FechaRevocacion).HasColumnType("datetime");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.HasOne(d => d.TokenReemplazo).WithMany(p => p.InverseTokenReemplazo)
                .HasForeignKey(d => d.TokenReemplazoId)
                .HasConstraintName("FK__RefreshTo__Token__7D439ABD");

            entity.HasOne(d => d.Usuario).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__Usuar__7B5B524B");
        });

        modelBuilder.Entity<ReservacionHabitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservac__3214EC072E74CA27");

            entity.HasIndex(e => new { e.ReservacionId, e.HabitacionId }, "UQ_ReservacionHabitacion").IsUnique();

            entity.Property(e => e.TarifaAplicada).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Habit__2645B050");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Reser__25518C17");
        });

        modelBuilder.Entity<Reservacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservac__3214EC07E6160C3A");

            entity.HasIndex(e => e.Codigo, "UQ__Reservac__06370DAC5034CC45").IsUnique();

            entity.Property(e => e.AnticipoRequerido).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CostoTotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PendientePago");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MotivoCancelacion).HasMaxLength(300);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Clien__17036CC0");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F8B3391D");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__75E3EFCF1E1F6833").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasMany(d => d.Permisos).WithMany(p => p.Rols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolPermis__Permi__778AC167"),
                    l => l.HasOne<Rol>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolPermis__RolId__76969D2E"),
                    j =>
                    {
                        j.HasKey("RolId", "PermisoId").HasName("PK__RolPermi__D04D0E838D7DF9BB");
                        j.ToTable("RolPermisos");
                    });
        });

        modelBuilder.Entity<TipoHabitacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TiposHab__3214EC070ABCF5E1");

            entity.ToTable("TiposHabitacion");

            entity.HasIndex(e => e.Nombre, "UQ__TiposHab__75E3EFCF775C1D65").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.PrecioBasePorNoche).HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07B76FC17D");

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__6B0F5AE04D353F87").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D10534155399BD").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Usuarios__Client__68487DD7");

            entity.HasMany(d => d.Rols).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRole",
                    r => r.HasOne<Rol>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioRo__RolId__73BA3083"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioRo__Usuar__72C60C4A"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "RolId").HasName("PK__UsuarioR__24AFD7970B1FD3C8");
                        j.ToTable("UsuarioRoles");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
