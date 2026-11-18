SET QUOTED_IDENTIFIER ON;   -- necesario para los índices filtrados
GO

CREATE DATABASE HotelBD;
GO

USE HotelBD;
GO

-- 1. CATÁLOGOS
-- Los estados tienen Id fijo (no IDENTITY) porque los
-- procedimientos los usan. Las columnas BIT dicen qué
-- se permite en cada estado.


CREATE TABLE TiposIdentificacion (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,         -- Cédula, DNI, Pasaporte...
    Formato NVARCHAR(50) NULL,                   -- ayuda para validar en la API, ej: 0-0000-0000
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE EstadosUsuario (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL,
    PermiteAcceso BIT NOT NULL                   -- 1 = puede iniciar sesión / hacer reservas
);

CREATE TABLE EstadosHabitacion (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    PermiteReservar BIT NOT NULL                 -- 0 = en mantenimiento o bloqueada
);

CREATE TABLE EstadosReservacion (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    OcupaHabitacion BIT NOT NULL                 -- 1 = cuenta para la disponibilidad
);

CREATE TABLE EstadosPedido (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    Orden INT NOT NULL,                          -- el pedido solo avanza hacia un orden mayor
    PermiteModificar BIT NOT NULL,
    PermiteCancelar BIT NOT NULL,
    EsFinal BIT NOT NULL                         -- Entregado o Cancelado: ya no cambia
);

CREATE TABLE MetodosPago (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    RequiereReferencia BIT NOT NULL,             -- tarjeta y transferencia piden nº de autorización
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE EstadosPago (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE          -- Pendiente, Parcial, Pagado
);

-- Para qué es cada correo (confirmación, factura, recuperar contraseña...)
CREATE TABLE TiposCorreo (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(40) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);

CREATE TABLE EstadosCorreo (
    Id INT PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE          -- Pendiente, Enviado, Fallido, Recibido, Respondido
);

-- PARÁMETROS, TEMPORADAS Y DESCUENTOS

-- Valores del negocio que pueden cambiar sin tocar el código (IVA, horas, recargos...)
CREATE TABLE Parametros (
    Id INT IDENTITY PRIMARY KEY,
    Clave NVARCHAR(50) NOT NULL UNIQUE,
    Valor NVARCHAR(100) NOT NULL,
    TipoDato NVARCHAR(10) NOT NULL CHECK (TipoDato IN ('Decimal', 'Entero', 'Hora', 'Texto')),
    Descripcion NVARCHAR(200) NULL,
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE()
);

-- Temporadas altas: suben el precio de la noche un porcentaje
CREATE TABLE Temporadas (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(60) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,                      -- incluida
    PorcentajeRecargo DECIMAL(5,2) NOT NULL CHECK (PorcentajeRecargo >= 0),
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_Temporadas_Fechas CHECK (FechaInicio <= FechaFin)
);

CREATE TABLE Descuentos (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(60) NOT NULL,
    Codigo NVARCHAR(20) NULL,                    -- código promocional; NULL si se asigna directo al cliente
    Tipo NVARCHAR(15) NOT NULL CHECK (Tipo IN ('Porcentaje', 'MontoFijo')),
    Valor DECIMAL(10,2) NOT NULL CHECK (Valor > 0),
    AplicaA NVARCHAR(15) NOT NULL CHECK (AplicaA IN ('Hospedaje', 'Restaurante', 'Ambos')),
    FechaInicio DATE NULL,                       -- NULL = sin fecha límite
    FechaFin DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_Descuentos_Porcentaje CHECK (Tipo <> 'Porcentaje' OR Valor <= 100)
);
CREATE UNIQUE INDEX UX_Descuentos_Codigo ON Descuentos (Codigo) WHERE Codigo IS NOT NULL;

-- 3. CLIENTES, USUARIOS Y SEGURIDAD
-- Cliente = huésped del hotel (puede no tener cuenta).
-- Usuario = cuenta para entrar al sistema (empleados y huéspedes que usan la app).


CREATE TABLE Clientes (
    Id INT IDENTITY PRIMARY KEY,
    TipoIdentificacionId INT NOT NULL FOREIGN KEY REFERENCES TiposIdentificacion(Id),
    NumeroIdentificacion NVARCHAR(30) NOT NULL,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Nacionalidad NVARCHAR(50) NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Telefono NVARCHAR(20) NULL,
    FechaNacimiento DATE NULL,
    Preferencias NVARCHAR(300) NULL,             -- ej: piso alto, almohada extra
    DescuentoId INT NULL FOREIGN KEY REFERENCES Descuentos(Id),  -- descuento fijo (cliente frecuente, convenio)
    EstadoUsuarioId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosUsuario(Id),
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    RowVersion ROWVERSION,                       -- concurrencia optimista
    CONSTRAINT UQ_Clientes_Identificacion UNIQUE (TipoIdentificacionId, NumeroIdentificacion)
);

CREATE TABLE Usuarios (
    Id INT IDENTITY PRIMARY KEY,
    TipoIdentificacionId INT NOT NULL FOREIGN KEY REFERENCES TiposIdentificacion(Id),
    NumeroIdentificacion NVARCHAR(30) NOT NULL,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,         -- nunca la contraseña en texto plano
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    EstadoUsuarioId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosUsuario(Id),
    IntentosFallidos INT NOT NULL DEFAULT 0,     -- para bloquear tras varios intentos
    UltimoAcceso DATETIME NULL,
    ClienteId INT NULL FOREIGN KEY REFERENCES Clientes(Id),  -- solo si el usuario es un huésped
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    RowVersion ROWVERSION,
    CONSTRAINT UQ_Usuarios_Identificacion UNIQUE (TipoIdentificacionId, NumeroIdentificacion)
);
-- un cliente solo puede tener una cuenta
CREATE UNIQUE INDEX UX_Usuarios_Cliente ON Usuarios (ClienteId) WHERE ClienteId IS NOT NULL;

CREATE TABLE Roles (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);

CREATE TABLE Permisos (
    Id INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(100) NOT NULL UNIQUE,        -- ej: Reservaciones.Crear (nombre de la policy en la API)
    Descripcion NVARCHAR(200) NULL
);

CREATE TABLE RolPermisos (
    RolId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    PermisoId INT NOT NULL FOREIGN KEY REFERENCES Permisos(Id),
    PRIMARY KEY (RolId, PermisoId)
);

CREATE TABLE UsuarioRoles (
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    RolId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    PRIMARY KEY (UsuarioId, RolId)
);

CREATE TABLE RefreshTokens (
    Id INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    TokenHash NVARCHAR(256) NOT NULL UNIQUE,     -- se guarda el hash, no el token
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaExpiracion DATETIME NOT NULL,
    FechaRevocacion DATETIME NULL,               -- NULL = sigue vigente
    TokenReemplazoId INT NULL FOREIGN KEY REFERENCES RefreshTokens(Id)  -- rotación de tokens
);

-- Códigos de un solo uso que se mandan por correo: verificar la cuenta y recuperar la contraseña.
-- Igual que los refresh tokens, se guarda el hash y no el código.
CREATE TABLE TokensUsuario (
    Id INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Tipo NVARCHAR(20) NOT NULL CHECK (Tipo IN ('VerificacionCorreo', 'RecuperarContrasena')),
    TokenHash NVARCHAR(256) NOT NULL UNIQUE,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaExpiracion DATETIME NOT NULL,
    FechaUso DATETIME NULL                       -- NULL = todavía no se ha usado
);

CREATE TABLE Auditoria (
    Id INT IDENTITY PRIMARY KEY,
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Accion NVARCHAR(100) NOT NULL,
    Entidad NVARCHAR(100) NOT NULL,
    EntidadId INT NULL,
    Datos NVARCHAR(500) NULL,                    -- sin contraseñas ni tokens
    Resultado NVARCHAR(10) NOT NULL DEFAULT 'Exito'
        CHECK (Resultado IN ('Exito', 'Fallido', 'Denegado')),   -- Fallido = login incorrecto, Denegado = 403
    DireccionIP NVARCHAR(45) NULL,               -- la llena la API (IPv4 o IPv6)
    FechaUtc DATETIME NOT NULL DEFAULT GETUTCDATE()
);


-- 4. HABITACIONES


CREATE TABLE TiposHabitacion (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(300) NULL,
    CapacidadAdultos INT NOT NULL CHECK (CapacidadAdultos > 0),
    CapacidadNinos INT NOT NULL DEFAULT 0 CHECK (CapacidadNinos >= 0),
    PrecioBasePorNoche DECIMAL(10,2) NOT NULL CHECK (PrecioBasePorNoche > 0),
    PersonasIncluidas INT NOT NULL DEFAULT 2,    -- personas que cubre el precio base
    CargoPersonaAdicional DECIMAL(10,2) NOT NULL DEFAULT 0,  -- por persona extra y por noche
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Amenidades (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE          -- Jacuzzi, Vista al mar, Balcón...
);

CREATE TABLE Habitaciones (
    Id INT IDENTITY PRIMARY KEY,
    Numero NVARCHAR(10) NOT NULL UNIQUE,
    Piso INT NOT NULL,
    TipoHabitacionId INT NOT NULL FOREIGN KEY REFERENCES TiposHabitacion(Id),
    EstadoHabitacionId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosHabitacion(Id),
    Activo BIT NOT NULL DEFAULT 1,               -- borrado lógico
    RowVersion ROWVERSION
);

CREATE TABLE HabitacionAmenidades (
    HabitacionId INT NOT NULL FOREIGN KEY REFERENCES Habitaciones(Id),
    AmenidadId INT NOT NULL FOREIGN KEY REFERENCES Amenidades(Id),
    PRIMARY KEY (HabitacionId, AmenidadId)
);

-- Varias fotografías por habitación; una se marca como principal
CREATE TABLE HabitacionFotos (
    Id INT IDENTITY PRIMARY KEY,
    HabitacionId INT NOT NULL FOREIGN KEY REFERENCES Habitaciones(Id),
    Url NVARCHAR(300) NOT NULL,
    Descripcion NVARCHAR(100) NULL,              -- ej: vista desde el balcón
    EsPrincipal BIT NOT NULL DEFAULT 0,
    Orden INT NOT NULL DEFAULT 1
);
-- solo una foto principal por habitación
CREATE UNIQUE INDEX UX_HabitacionFotos_Principal ON HabitacionFotos (HabitacionId) WHERE EsPrincipal = 1;

-- Bitácora de cambios de estado (la llena el trigger trg_Habitaciones_Estado)
CREATE TABLE HistorialEstadoHabitacion (
    Id INT IDENTITY PRIMARY KEY,
    HabitacionId INT NOT NULL FOREIGN KEY REFERENCES Habitaciones(Id),
    EstadoAnteriorId INT NULL FOREIGN KEY REFERENCES EstadosHabitacion(Id),
    EstadoNuevoId INT NOT NULL FOREIGN KEY REFERENCES EstadosHabitacion(Id),
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Motivo NVARCHAR(200) NULL,                   -- ej: Check-in, Check-out, fuga de agua
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);


-- 5. RESERVACIONES Y ESTADÍAS

CREATE SEQUENCE SeqReservacion START WITH 1;    -- correlativo del código HOT-2026-11-00001

CREATE TABLE Reservaciones (
    Id INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(25) NOT NULL UNIQUE,
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    CantidadAdultos INT NOT NULL CHECK (CantidadAdultos > 0),
    CantidadNinos INT NOT NULL DEFAULT 0 CHECK (CantidadNinos >= 0),
    EstadoReservacionId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosReservacion(Id),
    DescuentoId INT NULL FOREIGN KEY REFERENCES Descuentos(Id),
    Subtotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    Descuento DECIMAL(10,2) NOT NULL DEFAULT 0,
    Impuestos DECIMAL(10,2) NOT NULL DEFAULT 0,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    AnticipoRequerido DECIMAL(10,2) NOT NULL DEFAULT 0,
    Observaciones NVARCHAR(300) NULL,
    MotivoCancelacion NVARCHAR(300) NULL,
    UsuarioCreacionId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    RowVersion ROWVERSION,
    CONSTRAINT CK_Reservaciones_Fechas CHECK (FechaInicio < FechaFin)   -- entrada antes que salida
);

CREATE TABLE ReservacionHabitaciones (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    HabitacionId INT NOT NULL FOREIGN KEY REFERENCES Habitaciones(Id),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL              -- suma de las noches con temporada/fin de semana
);

CREATE TABLE HuespedesReservacion (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    NombreCompleto NVARCHAR(200) NOT NULL,
    TipoIdentificacionId INT NULL FOREIGN KEY REFERENCES TiposIdentificacion(Id),
    NumeroIdentificacion NVARCHAR(30) NULL,
    EsMenor BIT NOT NULL DEFAULT 0,
    EsTitular BIT NOT NULL DEFAULT 0
);

-- Historial de cambios de la reservación (quién, cuándo y por qué)
CREATE TABLE HistorialReservacion (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    EstadoAnteriorId INT NULL FOREIGN KEY REFERENCES EstadosReservacion(Id),
    EstadoNuevoId INT NOT NULL FOREIGN KEY REFERENCES EstadosReservacion(Id),
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Motivo NVARCHAR(300) NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Estadias (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Reservaciones(Id),  -- una estadía por reserva
    CheckInReal DATETIME NOT NULL,
    CheckOutReal DATETIME NULL,                  -- NULL = el huésped sigue en el hotel
    UsuarioCheckInId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    UsuarioCheckOutId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    CodigoAcceso NVARCHAR(20) NULL               -- código de la cerradura o nº de tarjeta
);

-- Cargos durante la estadía: late check-out, lavandería, minibar, daños...
CREATE TABLE CargosAdicionales (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    Concepto NVARCHAR(150) NOT NULL,
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto > 0),
    Impuesto DECIMAL(10,2) NOT NULL DEFAULT 0,
    EsLateCheckout BIT NOT NULL DEFAULT 0,
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);

-- =============================================
-- 6. RESTAURANTE
-- =============================================

CREATE TABLE CategoriasPlatillo (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Platillos (
    Id INT IDENTITY PRIMARY KEY,
    CategoriaId INT NOT NULL FOREIGN KEY REFERENCES CategoriasPlatillo(Id),
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(300) NULL,
    Precio DECIMAL(10,2) NOT NULL CHECK (Precio > 0),
    ImagenUrl NVARCHAR(300) NULL,
    TiempoPreparacion INT NOT NULL DEFAULT 15,   -- minutos estimados
    EsBebida BIT NOT NULL DEFAULT 0,
    Disponible BIT NOT NULL DEFAULT 1,           -- 0 = agotado por hoy
    Activo BIT NOT NULL DEFAULT 1                -- 0 = ya no está en el menú (borrado lógico)
);

CREATE TABLE PedidosRestaurante (
    Id INT IDENTITY PRIMARY KEY,
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    ReservacionId INT NULL FOREIGN KEY REFERENCES Reservaciones(Id),  -- se llena si se carga a la habitación
    HabitacionId INT NULL FOREIGN KEY REFERENCES Habitaciones(Id),    -- destino del room service
    Modalidad NVARCHAR(20) NOT NULL CHECK (Modalidad IN ('RoomService', 'Mesa', 'Piscina')),
    CargoAHabitacion BIT NOT NULL DEFAULT 0,
    EstadoPedidoId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosPedido(Id),
    DescuentoId INT NULL FOREIGN KEY REFERENCES Descuentos(Id),
    FechaPedido DATETIME NOT NULL DEFAULT GETDATE(),
    FechaHoraDeseada DATETIME NULL,
    Instrucciones NVARCHAR(300) NULL,
    Subtotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    Descuento DECIMAL(10,2) NOT NULL DEFAULT 0,
    Recargo DECIMAL(10,2) NOT NULL DEFAULT 0,    -- recargo de room service
    Impuestos DECIMAL(10,2) NOT NULL DEFAULT 0,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    RowVersion ROWVERSION,
    CONSTRAINT CK_Pedidos_Cargo CHECK (CargoAHabitacion = 0 OR ReservacionId IS NOT NULL)
);

CREATE TABLE DetallePedidos (
    Id INT IDENTITY PRIMARY KEY,
    PedidoId INT NOT NULL FOREIGN KEY REFERENCES PedidosRestaurante(Id),
    PlatilloId INT NOT NULL FOREIGN KEY REFERENCES Platillos(Id),
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(10,2) NOT NULL,       -- precio guardado al momento del pedido
    Subtotal AS (Cantidad * PrecioUnitario),     -- columna calculada
    CONSTRAINT UQ_DetallePedidos UNIQUE (PedidoId, PlatilloId)
);

-- Cada cambio de estado del pedido (sirve para medir tiempos de cocina)
CREATE TABLE HistorialEstadoPedido (
    Id INT IDENTITY PRIMARY KEY,
    PedidoId INT NOT NULL FOREIGN KEY REFERENCES PedidosRestaurante(Id),
    EstadoPedidoId INT NOT NULL FOREIGN KEY REFERENCES EstadosPedido(Id),
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);

-- 7. FACTURAS, PAGOS Y CORREOS

CREATE SEQUENCE SeqFactura START WITH 1;

CREATE TABLE Facturas (
    Id INT IDENTITY PRIMARY KEY,
    NumeroFactura NVARCHAR(25) NOT NULL UNIQUE,
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    ReservacionId INT NULL FOREIGN KEY REFERENCES Reservaciones(Id),     -- factura de hospedaje
    PedidoId INT NULL FOREIGN KEY REFERENCES PedidosRestaurante(Id),     -- factura de un pedido pagado aparte
    Subtotal DECIMAL(10,2) NOT NULL,
    Descuento DECIMAL(10,2) NOT NULL,
    Impuestos DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    TotalPagado DECIMAL(10,2) NOT NULL,
    FechaEmision DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    CONSTRAINT CK_Facturas_Origen CHECK (ReservacionId IS NOT NULL OR PedidoId IS NOT NULL)
);
-- una sola factura por reservación y por pedido
CREATE UNIQUE INDEX UX_Facturas_Reservacion ON Facturas (ReservacionId) WHERE ReservacionId IS NOT NULL;
CREATE UNIQUE INDEX UX_Facturas_Pedido ON Facturas (PedidoId) WHERE PedidoId IS NOT NULL;

-- Cada línea de la factura: hospedaje, cargos, platillos, recargos
CREATE TABLE FacturaDetalles (
    Id INT IDENTITY PRIMARY KEY,
    FacturaId INT NOT NULL FOREIGN KEY REFERENCES Facturas(Id),
    TipoConcepto NVARCHAR(20) NOT NULL CHECK (TipoConcepto IN ('Hospedaje', 'CargoAdicional', 'Restaurante', 'Recargo', 'Ajuste')),
    Descripcion NVARCHAR(200) NOT NULL,
    Cantidad DECIMAL(10,2) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL
);

-- Cuenta por cobrar: UNA fila por reservación o por pedido pagado aparte.
-- Resume cuánto hay que pagar, cuánto se ha abonado y cuánto falta.
-- La mantiene al día el procedimiento sp_ActualizarPago.
CREATE TABLE Pagos (
    Id INT IDENTITY PRIMARY KEY,
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    ReservacionId INT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    PedidoId INT NULL FOREIGN KEY REFERENCES PedidosRestaurante(Id),
    MontoTotal DECIMAL(10,2) NOT NULL DEFAULT 0,     -- hospedaje + cargos + consumos (o total del pedido)
    TotalAbonado DECIMAL(10,2) NOT NULL DEFAULT 0,   -- suma de los abonos confirmados
    Saldo AS (MontoTotal - TotalAbonado),            -- lo que falta pagar (columna calculada)
    EstadoPagoId INT NOT NULL DEFAULT 1 FOREIGN KEY REFERENCES EstadosPago(Id),
    FacturaId INT NULL FOREIGN KEY REFERENCES Facturas(Id),   -- se asigna al emitir la factura
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    RowVersion ROWVERSION,
    CONSTRAINT CK_Pagos_Destino CHECK (ReservacionId IS NOT NULL OR PedidoId IS NOT NULL)
);
-- una sola cuenta por reservación y por pedido
CREATE UNIQUE INDEX UX_Pagos_Reservacion ON Pagos (ReservacionId) WHERE ReservacionId IS NOT NULL;
CREATE UNIQUE INDEX UX_Pagos_Pedido ON Pagos (PedidoId) WHERE PedidoId IS NOT NULL;

-- Cada pago parcial (anticipo, pago a mitad de la estadía, saldo final...)
CREATE TABLE Abonos (
    Id INT IDENTITY PRIMARY KEY,
    PagoId INT NOT NULL FOREIGN KEY REFERENCES Pagos(Id),
    MetodoPagoId INT NOT NULL FOREIGN KEY REFERENCES MetodosPago(Id),
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto > 0),
    Referencia NVARCHAR(100) NULL,
    UltimosDigitosTarjeta CHAR(4) NULL,          -- nunca el número completo ni el CVV
    -- Confirmado = cuenta como pagado | Reversado = error corregido | Reembolsado = devuelto al cancelar
    Estado NVARCHAR(15) NOT NULL DEFAULT 'Confirmado' CHECK (Estado IN ('Confirmado', 'Reversado', 'Reembolsado')),
    MotivoReverso NVARCHAR(300) NULL,
    FechaAbono DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id)
);
-- evita registrar dos veces la misma transacción
CREATE UNIQUE INDEX UX_Abonos_Referencia ON Abonos (Referencia) WHERE Referencia IS NOT NULL;

-- Ajustes autorizados por un supervisor sobre la cuenta de una reservación.
-- Monto positivo = cargo extra; negativo = rebaja (ej: compensación por una queja).
CREATE TABLE AjustesCuenta (
    Id INT IDENTITY PRIMARY KEY,
    ReservacionId INT NOT NULL FOREIGN KEY REFERENCES Reservaciones(Id),
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto <> 0),
    Motivo NVARCHAR(300) NOT NULL,               -- justificación obligatoria
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),        -- quien lo registra
    AutorizadoPorId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),  -- supervisor que lo aprueba
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);

-- ---------- Historial de correos ----------
-- Guarda TODOS los correos: los que el hotel envía (confirmaciones, facturas...)
-- y los que recibe de los clientes (consultas). La API registra el correo,
-- lo envía por SMTP y luego actualiza su estado con sp_ActualizarEstadoCorreo.
CREATE TABLE Correos (
    Id INT IDENTITY PRIMARY KEY,
    Direccion NVARCHAR(10) NOT NULL CHECK (Direccion IN ('Enviado', 'Recibido')),
    TipoCorreoId INT NOT NULL FOREIGN KEY REFERENCES TiposCorreo(Id),
    EstadoCorreoId INT NOT NULL FOREIGN KEY REFERENCES EstadosCorreo(Id),
    Remitente NVARCHAR(150) NOT NULL,
    Destinatario NVARCHAR(150) NOT NULL,
    Asunto NVARCHAR(200) NOT NULL,
    Cuerpo NVARCHAR(MAX) NOT NULL,
    ClienteId INT NULL FOREIGN KEY REFERENCES Clientes(Id),            -- a quién se le envió / quién escribió
    ReservacionId INT NULL FOREIGN KEY REFERENCES Reservaciones(Id),   -- si el correo es de una reserva
    FacturaId INT NULL FOREIGN KEY REFERENCES Facturas(Id),            -- si se envió una factura
    CorreoRespuestaId INT NULL FOREIGN KEY REFERENCES Correos(Id),     -- respuesta a un correo recibido
    Intentos INT NOT NULL DEFAULT 0,                                   -- intentos de envío por SMTP
    MensajeError NVARCHAR(500) NULL,                                   -- último error del servidor de correo
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),            -- NULL = lo envió el sistema
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    FechaEnvio DATETIME NULL                                           -- cuándo salió o cuándo llegó
);
GO

-- 8. ÍNDICES

CREATE INDEX IX_Clientes_Apellidos ON Clientes (Apellidos, Nombres);
CREATE INDEX IX_Reservaciones_Cliente ON Reservaciones (ClienteId, EstadoReservacionId);
CREATE INDEX IX_Reservaciones_Fechas ON Reservaciones (FechaInicio, FechaFin);
CREATE INDEX IX_ResHab_Disponibilidad ON ReservacionHabitaciones (HabitacionId, FechaInicio, FechaFin);  -- búsqueda de traslapes
CREATE INDEX IX_Pagos_Estado ON Pagos (EstadoPagoId);
CREATE INDEX IX_Abonos_Pago ON Abonos (PagoId);
CREATE INDEX IX_Abonos_Fecha ON Abonos (FechaAbono);
CREATE INDEX IX_Pedidos_Estado ON PedidosRestaurante (EstadoPedidoId, FechaPedido);
CREATE INDEX IX_Pedidos_Reservacion ON PedidosRestaurante (ReservacionId);
CREATE INDEX IX_Platillos_Categoria ON Platillos (CategoriaId);
CREATE INDEX IX_Auditoria_Fecha ON Auditoria (FechaUtc);
CREATE INDEX IX_Correos_Estado ON Correos (EstadoCorreoId, FechaRegistro);    -- la API busca los pendientes de enviar
CREATE INDEX IX_Correos_Cliente ON Correos (ClienteId);
CREATE INDEX IX_Ajustes_Reservacion ON AjustesCuenta (ReservacionId);
CREATE INDEX IX_TokensUsuario_Usuario ON TokensUsuario (UsuarioId, Tipo);
CREATE INDEX IX_HabitacionFotos_Habitacion ON HabitacionFotos (HabitacionId, Orden);
GO


-- 9. FUNCIONES

-- Lee un valor de la tabla Parametros
CREATE FUNCTION fn_Parametro (@Clave NVARCHAR(50))
RETURNS NVARCHAR(100)
AS
BEGIN
    RETURN (SELECT Valor FROM Parametros WHERE Clave = @Clave);
END
GO

-- Precio de UNA noche: precio base + temporada alta + fin de semana
CREATE FUNCTION fn_TarifaNoche (@TipoHabitacionId INT, @Fecha DATE)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Precio DECIMAL(10,2) = (SELECT PrecioBasePorNoche FROM TiposHabitacion WHERE Id = @TipoHabitacionId);

    DECLARE @Temporada DECIMAL(10,2) = ISNULL((SELECT MAX(PorcentajeRecargo) FROM Temporadas
                                               WHERE Activo = 1 AND @Fecha BETWEEN FechaInicio AND FechaFin), 0);
    DECLARE @FinDeSemana DECIMAL(10,2) = 0;

    -- viernes o sábado (el 05/01/1900 fue viernes; así no depende del idioma del servidor)
    IF DATEDIFF(DAY, '19000105', @Fecha) % 7 IN (0, 1)
        SET @FinDeSemana = CAST(dbo.fn_Parametro('RecargoFinDeSemana') AS DECIMAL(10,2));

    RETURN ROUND(@Precio * (1 + @Temporada / 100) * (1 + @FinDeSemana / 100), 2);
END
GO

-- Costo del hospedaje: suma noche por noche + personas adicionales
CREATE FUNCTION fn_CostoHospedaje (@TipoHabitacionId INT, @Inicio DATE, @Fin DATE, @Personas INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Total DECIMAL(10,2) = 0, @Dia DATE = @Inicio;

    WHILE @Dia < @Fin                            -- la noche del día de salida no se cobra
    BEGIN
        SET @Total = @Total + dbo.fn_TarifaNoche(@TipoHabitacionId, @Dia);
        SET @Dia = DATEADD(DAY, 1, @Dia);
    END

    DECLARE @Incluidas INT, @CargoExtra DECIMAL(10,2);
    SELECT @Incluidas = PersonasIncluidas, @CargoExtra = CargoPersonaAdicional
    FROM TiposHabitacion WHERE Id = @TipoHabitacionId;

    IF @Personas > @Incluidas
        SET @Total = @Total + (@Personas - @Incluidas) * @CargoExtra * DATEDIFF(DAY, @Inicio, @Fin);

    RETURN @Total;
END
GO

-- Cuánto se descuenta de un monto. Devuelve 0 si el descuento no aplica
-- (inactivo, vencido o es para otra área: Hospedaje / Restaurante)
CREATE FUNCTION fn_MontoDescuento (@DescuentoId INT, @Monto DECIMAL(10,2), @Area NVARCHAR(15))
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Tipo NVARCHAR(15), @Valor DECIMAL(10,2), @Hoy DATE = CAST(GETDATE() AS DATE);

    SELECT @Tipo = Tipo, @Valor = Valor
    FROM Descuentos
    WHERE Id = @DescuentoId AND Activo = 1
      AND AplicaA IN (@Area, 'Ambos')
      AND (FechaInicio IS NULL OR @Hoy >= FechaInicio)
      AND (FechaFin IS NULL OR @Hoy <= FechaFin);

    IF @Tipo IS NULL RETURN 0;
    IF @Tipo = 'Porcentaje' RETURN ROUND(@Monto * @Valor / 100, 2);
    RETURN CASE WHEN @Valor > @Monto THEN @Monto ELSE @Valor END;   -- monto fijo, sin pasar el total
END
GO

-- Id del descuento de un código promocional, o NULL si no existe, está vencido o es de otra área
CREATE FUNCTION fn_DescuentoValido (@Codigo NVARCHAR(20), @Area NVARCHAR(15))
RETURNS INT
AS
BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    RETURN (SELECT Id FROM Descuentos
            WHERE Codigo = @Codigo AND Activo = 1
              AND AplicaA IN (@Area, 'Ambos')
              AND (FechaInicio IS NULL OR @Hoy >= FechaInicio)
              AND (FechaFin IS NULL OR @Hoy <= FechaFin));
END
GO

-- 1 si la habitación está libre en esas fechas.
-- Hay traslape cuando: nuevaEntrada < salidaExistente Y nuevaSalida > entradaExistente
-- @ExcluirReservacionId: al modificar una reserva, no choca consigo misma (NULL si es nueva)
CREATE FUNCTION fn_HabitacionDisponible (@HabitacionId INT, @Inicio DATE, @Fin DATE, @ExcluirReservacionId INT)
RETURNS BIT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Habitaciones h
                   INNER JOIN EstadosHabitacion e ON e.Id = h.EstadoHabitacionId
                   WHERE h.Id = @HabitacionId AND h.Activo = 1 AND e.PermiteReservar = 1)
        RETURN 0;

    IF EXISTS (SELECT 1
               FROM ReservacionHabitaciones rh
               INNER JOIN Reservaciones r ON r.Id = rh.ReservacionId
               INNER JOIN EstadosReservacion er ON er.Id = r.EstadoReservacionId
               WHERE rh.HabitacionId = @HabitacionId
                 AND er.OcupaHabitacion = 1
                 AND (@ExcluirReservacionId IS NULL OR r.Id <> @ExcluirReservacionId)
                 AND @Inicio < rh.FechaFin
                 AND @Fin > rh.FechaInicio)
        RETURN 0;

    RETURN 1;
END
GO

-- Total a pagar de una reservación = hospedaje + cargos + pedidos cargados a la habitación + ajustes.
-- Si está cancelada, el total es lo que el hotel se quedó como penalización (los abonos no reembolsados).
CREATE FUNCTION fn_TotalReservacion (@ReservacionId INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Hospedaje DECIMAL(10,2), @Cargos DECIMAL(10,2), @Pedidos DECIMAL(10,2), @Ajustes DECIMAL(10,2);

    IF EXISTS (SELECT 1 FROM Reservaciones WHERE Id = @ReservacionId AND EstadoReservacionId = 5)   -- 5 = Cancelada
        RETURN ISNULL((SELECT SUM(a.Monto) FROM Abonos a INNER JOIN Pagos p ON p.Id = a.PagoId
                       WHERE p.ReservacionId = @ReservacionId AND a.Estado = 'Confirmado'), 0);

    SELECT @Hospedaje = Total FROM Reservaciones WHERE Id = @ReservacionId;

    SELECT @Ajustes = ISNULL(SUM(Monto), 0) FROM AjustesCuenta WHERE ReservacionId = @ReservacionId;

    SELECT @Cargos = ISNULL(SUM(Monto + Impuesto), 0) FROM CargosAdicionales WHERE ReservacionId = @ReservacionId;

    SELECT @Pedidos = ISNULL(SUM(p.Total), 0)
    FROM PedidosRestaurante p
    INNER JOIN EstadosPedido e ON e.Id = p.EstadoPedidoId
    WHERE p.ReservacionId = @ReservacionId AND p.CargoAHabitacion = 1 AND e.Nombre <> 'Cancelado';

    RETURN ISNULL(@Hospedaje, 0) + @Cargos + @Pedidos + @Ajustes;
END
GO

-- 1 si el pedido ya está en una factura (la suya o la de la reservación): ya no se puede tocar
CREATE FUNCTION fn_PedidoFacturado (@PedidoId INT)
RETURNS BIT
AS
BEGIN
    IF EXISTS (SELECT 1
               FROM PedidosRestaurante p
               INNER JOIN Pagos pg ON pg.PedidoId = p.Id
                                   OR (p.CargoAHabitacion = 1 AND pg.ReservacionId = p.ReservacionId)
               WHERE p.Id = @PedidoId AND pg.FacturaId IS NOT NULL)
        RETURN 1;
    RETURN 0;
END
GO

-- Lo que falta pagar = total - abonos confirmados
CREATE FUNCTION fn_SaldoReservacion (@ReservacionId INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Abonado DECIMAL(10,2) = ISNULL((SELECT SUM(a.Monto)
                                             FROM Abonos a INNER JOIN Pagos p ON p.Id = a.PagoId
                                             WHERE p.ReservacionId = @ReservacionId AND a.Estado = 'Confirmado'), 0);

    RETURN dbo.fn_TotalReservacion(@ReservacionId) - @Abonado;
END
GO


-- 10. VISTAS

-- Roles y permisos de cada usuario activo (para armar el token JWT)
CREATE VIEW vw_UsuarioPermisos
AS
SELECT u.Id AS UsuarioId, u.NombreUsuario, u.Email, r.Nombre AS Rol, p.Codigo AS Permiso
FROM Usuarios u
INNER JOIN EstadosUsuario eu ON eu.Id = u.EstadoUsuarioId AND eu.PermiteAcceso = 1
INNER JOIN UsuarioRoles ur ON ur.UsuarioId = u.Id
INNER JOIN Roles r ON r.Id = ur.RolId
INNER JOIN RolPermisos rp ON rp.RolId = r.Id
INNER JOIN Permisos p ON p.Id = rp.PermisoId;
GO

-- Todos los usuarios y clientes con su estado
CREATE VIEW vw_EstadoUsuarios
AS
SELECT 'Usuario' AS TipoRegistro, u.Id, u.NombreUsuario AS Identificador,
       u.Nombres + ' ' + u.Apellidos AS NombreCompleto, ti.Nombre AS TipoIdentificacion,
       u.NumeroIdentificacion, e.Nombre AS Estado, e.PermiteAcceso
FROM Usuarios u
INNER JOIN EstadosUsuario e ON e.Id = u.EstadoUsuarioId
INNER JOIN TiposIdentificacion ti ON ti.Id = u.TipoIdentificacionId
UNION ALL
SELECT 'Cliente', c.Id, c.Email, c.Nombres + ' ' + c.Apellidos, ti.Nombre,
       c.NumeroIdentificacion, e.Nombre, e.PermiteAcceso
FROM Clientes c
INNER JOIN EstadosUsuario e ON e.Id = c.EstadoUsuarioId
INNER JOIN TiposIdentificacion ti ON ti.Id = c.TipoIdentificacionId;
GO

CREATE VIEW vw_ReservacionesDetalle
AS
SELECT r.Id, r.Codigo, er.Nombre AS Estado, r.FechaInicio, r.FechaFin,
       DATEDIFF(DAY, r.FechaInicio, r.FechaFin) AS Noches,
       c.Nombres + ' ' + c.Apellidos AS Cliente,
       h.Numero AS Habitacion, t.Nombre AS TipoHabitacion,
       r.Subtotal, r.Descuento, r.Impuestos, r.Total, r.AnticipoRequerido,
       dbo.fn_SaldoReservacion(r.Id) AS Saldo
FROM Reservaciones r
INNER JOIN EstadosReservacion er ON er.Id = r.EstadoReservacionId
INNER JOIN Clientes c ON c.Id = r.ClienteId
INNER JOIN ReservacionHabitaciones rh ON rh.ReservacionId = r.Id
INNER JOIN Habitaciones h ON h.Id = rh.HabitacionId
INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId;
GO

-- Cantidad de habitaciones por estado (dashboard)
CREATE VIEW vw_OcupacionActual
AS
SELECT e.Nombre AS Estado, COUNT(h.Id) AS Cantidad
FROM EstadosHabitacion e
LEFT JOIN Habitaciones h ON h.EstadoHabitacionId = e.Id AND h.Activo = 1
GROUP BY e.Id, e.Nombre;
GO

CREATE VIEW vw_IngresosPorMes
AS
SELECT YEAR(a.FechaAbono) AS Anio, MONTH(a.FechaAbono) AS Mes, m.Nombre AS MetodoPago,
       CASE WHEN p.PedidoId IS NOT NULL THEN 'Restaurante' ELSE 'Hospedaje' END AS Origen,
       SUM(a.Monto) AS Total
FROM Abonos a
INNER JOIN Pagos p ON p.Id = a.PagoId
INNER JOIN MetodosPago m ON m.Id = a.MetodoPagoId
WHERE a.Estado = 'Confirmado'
GROUP BY YEAR(a.FechaAbono), MONTH(a.FechaAbono), m.Nombre,
         CASE WHEN p.PedidoId IS NOT NULL THEN 'Restaurante' ELSE 'Hospedaje' END;
GO

-- Estado de cuenta de cada reservación o pedido: total, abonado, saldo y estado del pago
CREATE VIEW vw_EstadoCuentas
AS
SELECT p.Id AS PagoId, c.Nombres + ' ' + c.Apellidos AS Cliente,
       CASE WHEN p.ReservacionId IS NOT NULL THEN 'Reservación ' + r.Codigo
            ELSE 'Pedido #' + CAST(p.PedidoId AS VARCHAR) END AS Concepto,
       p.MontoTotal, p.TotalAbonado, p.Saldo, e.Nombre AS EstadoPago,
       (SELECT COUNT(*) FROM Abonos WHERE PagoId = p.Id AND Estado = 'Confirmado') AS CantidadAbonos,
       p.FechaActualizacion
FROM Pagos p
INNER JOIN Clientes c ON c.Id = p.ClienteId
INNER JOIN EstadosPago e ON e.Id = p.EstadoPagoId
LEFT JOIN Reservaciones r ON r.Id = p.ReservacionId;
GO

CREATE VIEW vw_PlatillosMasVendidos
AS
SELECT pl.Id, pl.Nombre, c.Nombre AS Categoria, SUM(d.Cantidad) AS CantidadVendida, SUM(d.Subtotal) AS TotalVendido
FROM DetallePedidos d
INNER JOIN Platillos pl ON pl.Id = d.PlatilloId
INNER JOIN CategoriasPlatillo c ON c.Id = pl.CategoriaId
INNER JOIN PedidosRestaurante p ON p.Id = d.PedidoId
INNER JOIN EstadosPedido e ON e.Id = p.EstadoPedidoId
WHERE e.Nombre <> 'Cancelado'
GROUP BY pl.Id, pl.Nombre, c.Nombre;
GO

-- Minutos reales entre "En Preparación" y "Listo" de cada pedido
CREATE VIEW vw_TiemposPreparacion
AS
SELECT p.Id AS PedidoId, ini.Fecha AS InicioPreparacion, fin.Fecha AS Listo,
       DATEDIFF(MINUTE, ini.Fecha, fin.Fecha) AS MinutosReales,
       (SELECT MAX(pl.TiempoPreparacion) FROM DetallePedidos d
        INNER JOIN Platillos pl ON pl.Id = d.PlatilloId WHERE d.PedidoId = p.Id) AS MinutosEstimados
FROM PedidosRestaurante p
INNER JOIN HistorialEstadoPedido ini ON ini.PedidoId = p.Id
INNER JOIN EstadosPedido eIni ON eIni.Id = ini.EstadoPedidoId AND eIni.Nombre = 'EnPreparacion'
INNER JOIN HistorialEstadoPedido fin ON fin.PedidoId = p.Id
INNER JOIN EstadosPedido eFin ON eFin.Id = fin.EstadoPedidoId AND eFin.Nombre = 'Listo';
GO

-- Encabezado y líneas de cada factura
CREATE VIEW vw_FacturaConceptos
AS
SELECT f.Id AS FacturaId, f.NumeroFactura, f.FechaEmision,
       c.Nombres + ' ' + c.Apellidos AS Cliente, r.Codigo AS Reservacion,
       d.TipoConcepto, d.Descripcion, d.Cantidad, d.PrecioUnitario, d.Subtotal,
       f.Subtotal AS SubtotalFactura, f.Descuento, f.Impuestos, f.Total
FROM Facturas f
INNER JOIN Clientes c ON c.Id = f.ClienteId
LEFT JOIN Reservaciones r ON r.Id = f.ReservacionId
INNER JOIN FacturaDetalles d ON d.FacturaId = f.Id;
GO

-- Abonos con los que se canceló cada factura (anticipo, saldo final, etc.)
CREATE VIEW vw_FacturaAbonos
AS
SELECT f.Id AS FacturaId, f.NumeroFactura, a.Id AS AbonoId, a.FechaAbono, m.Nombre AS MetodoPago,
       a.Referencia, a.UltimosDigitosTarjeta, a.Monto, a.Estado,
       f.Total AS TotalFactura, p.TotalAbonado, p.Saldo, ep.Nombre AS EstadoPago
FROM Facturas f
INNER JOIN Pagos p ON p.FacturaId = f.Id
INNER JOIN EstadosPago ep ON ep.Id = p.EstadoPagoId
INNER JOIN Abonos a ON a.PagoId = p.Id
INNER JOIN MetodosPago m ON m.Id = a.MetodoPagoId;
GO

-- Historial de correos enviados y recibidos, con sus datos relacionados
CREATE VIEW vw_HistorialCorreos
AS
SELECT co.Id, co.Direccion, tc.Nombre AS Tipo, ec.Nombre AS Estado,
       co.Remitente, co.Destinatario, co.Asunto,
       c.Nombres + ' ' + c.Apellidos AS Cliente,
       r.Codigo AS Reservacion, f.NumeroFactura, co.CorreoRespuestaId,
       co.Intentos, co.MensajeError, u.NombreUsuario AS EnviadoPor,
       co.FechaRegistro, co.FechaEnvio
FROM Correos co
INNER JOIN TiposCorreo tc ON tc.Id = co.TipoCorreoId
INNER JOIN EstadosCorreo ec ON ec.Id = co.EstadoCorreoId
LEFT JOIN Clientes c ON c.Id = co.ClienteId
LEFT JOIN Reservaciones r ON r.Id = co.ReservacionId
LEFT JOIN Facturas f ON f.Id = co.FacturaId
LEFT JOIN Usuarios u ON u.Id = co.UsuarioId;
GO


-- 11. TRIGGER: bitácora de estados de habitación
-- El usuario responsable se toma de SESSION_CONTEXT,
-- que lo fijan los procedimientos (o la API).

CREATE TRIGGER trg_Habitaciones_Estado
ON Habitaciones
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HistorialEstadoHabitacion (HabitacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
    SELECT i.Id, d.EstadoHabitacionId, i.EstadoHabitacionId, CAST(SESSION_CONTEXT(N'UsuarioId') AS INT),
           CAST(SESSION_CONTEXT(N'Motivo') AS NVARCHAR(200))
    FROM inserted i
    LEFT JOIN deleted d ON d.Id = i.Id
    WHERE d.Id IS NULL OR i.EstadoHabitacionId <> d.EstadoHabitacionId;
END
GO


-- 12. PROCEDIMIENTOS ALMACENADOS
-- Errores: 50001 = validación (400), 50002 = conflicto (409),
--          50003 = saldo pendiente (400), 50004 = no existe (404)
-- XACT_ABORT ON: si algo falla dentro de una transacción, se deshace completa.


--  Usuarios 

-- Activa, bloquea o suspende una cuenta. Si ya no permite acceso, cierra sus sesiones
CREATE PROCEDURE sp_CambiarEstadoUsuario
    @UsuarioId INT,
    @EstadoUsuarioId INT,
    @ResponsableId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Id = @UsuarioId)
        THROW 50004, 'El usuario no existe.', 1;
    IF NOT EXISTS (SELECT 1 FROM EstadosUsuario WHERE Id = @EstadoUsuarioId)
        THROW 50001, 'El estado no existe.', 1;

    BEGIN TRANSACTION;
        UPDATE Usuarios SET EstadoUsuarioId = @EstadoUsuarioId, IntentosFallidos = 0 WHERE Id = @UsuarioId;

        -- revoca los refresh tokens vigentes
        IF (SELECT PermiteAcceso FROM EstadosUsuario WHERE Id = @EstadoUsuarioId) = 0
            UPDATE RefreshTokens SET FechaRevocacion = GETDATE()
            WHERE UsuarioId = @UsuarioId AND FechaRevocacion IS NULL;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@ResponsableId, 'CambiarEstadoUsuario', 'Usuario', @UsuarioId,
                (SELECT Nombre FROM EstadosUsuario WHERE Id = @EstadoUsuarioId));
    COMMIT;
END
GO

--  Habitaciones 

CREATE PROCEDURE sp_BuscarDisponibilidad
    @Inicio DATE,
    @Fin DATE,
    @Adultos INT = 1,
    @Ninos INT = 0,
    @TipoHabitacionId INT = NULL,
    @AmenidadId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Inicio >= @Fin
        THROW 50001, 'La fecha de entrada debe ser menor que la de salida.', 1;

    SELECT h.Id, h.Numero, h.Piso, t.Nombre AS Tipo, t.PrecioBasePorNoche,
           DATEDIFF(DAY, @Inicio, @Fin) AS Noches,
           dbo.fn_CostoHospedaje(t.Id, @Inicio, @Fin, @Adultos + @Ninos) AS CostoSinImpuestos,
           (SELECT STRING_AGG(a.Nombre, ', ') FROM HabitacionAmenidades ha
            INNER JOIN Amenidades a ON a.Id = ha.AmenidadId WHERE ha.HabitacionId = h.Id) AS Amenidades,
           (SELECT Url FROM HabitacionFotos WHERE HabitacionId = h.Id AND EsPrincipal = 1) AS FotoPrincipal
    FROM Habitaciones h
    INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
    WHERE t.Activo = 1
      AND t.CapacidadAdultos >= @Adultos
      AND t.CapacidadAdultos + t.CapacidadNinos >= @Adultos + @Ninos
      AND (@TipoHabitacionId IS NULL OR t.Id = @TipoHabitacionId)
      AND (@AmenidadId IS NULL OR EXISTS (SELECT 1 FROM HabitacionAmenidades
                                          WHERE HabitacionId = h.Id AND AmenidadId = @AmenidadId))
      AND dbo.fn_HabitacionDisponible(h.Id, @Inicio, @Fin, NULL) = 1
    ORDER BY t.PrecioBasePorNoche, h.Numero;
END
GO

--Pagos y abonos

-- Recalcula la cuenta (Pagos): total a pagar, total abonado y estado del pago.
-- Se llama cada vez que cambia algo que afecta lo que se debe o lo que se pagó.
CREATE PROCEDURE sp_ActualizarPago
    @ReservacionId INT = NULL,
    @PedidoId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @PagoId INT, @Total DECIMAL(10,2), @Abonado DECIMAL(10,2), @Anulada BIT = 0;

    SELECT @PagoId = Id FROM Pagos
    WHERE (@ReservacionId IS NOT NULL AND ReservacionId = @ReservacionId)
       OR (@PedidoId IS NOT NULL AND PedidoId = @PedidoId);

    IF @PagoId IS NULL RETURN;

    IF @ReservacionId IS NOT NULL
    BEGIN
        SET @Total = dbo.fn_TotalReservacion(@ReservacionId);
        IF EXISTS (SELECT 1 FROM Reservaciones WHERE Id = @ReservacionId AND EstadoReservacionId = 5)   -- 5 = Cancelada
            SET @Anulada = 1;
    END
    ELSE
        SELECT @Total = CASE WHEN EstadoPedidoId = 7 THEN 0 ELSE Total END,   -- 7 = Cancelado
               @Anulada = CASE WHEN EstadoPedidoId = 7 THEN 1 ELSE 0 END
        FROM PedidosRestaurante WHERE Id = @PedidoId;

    SELECT @Abonado = ISNULL(SUM(Monto), 0) FROM Abonos WHERE PagoId = @PagoId AND Estado = 'Confirmado';

    UPDATE Pagos
    SET MontoTotal = @Total,
        TotalAbonado = @Abonado,
        EstadoPagoId = CASE WHEN @Anulada = 1 THEN 4           -- Anulada (reserva o pedido cancelado)
                            WHEN @Abonado = 0 THEN 1           -- Pendiente
                            WHEN @Abonado < @Total THEN 2      -- Parcial
                            ELSE 3 END,                        -- Pagado
        FechaActualizacion = GETDATE()
    WHERE Id = @PagoId;
END
GO

--  Reservaciones 

CREATE PROCEDURE sp_CrearReservacion
    @ClienteId INT,
    @HabitacionId INT,
    @Inicio DATE,
    @Fin DATE,
    @Adultos INT,
    @Ninos INT = 0,
    @CodigoDescuento NVARCHAR(20) = NULL,       -- código promocional opcional
    @Observaciones NVARCHAR(300) = NULL,
    @UsuarioId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- validaciones
    IF @Inicio >= @Fin
        THROW 50001, 'La fecha de entrada debe ser menor que la de salida.', 1;
    IF @Inicio < CAST(GETDATE() AS DATE)
        THROW 50001, 'No se puede reservar en fechas pasadas.', 1;
    IF DATEDIFF(DAY, @Inicio, @Fin) > CAST(dbo.fn_Parametro('MaxNochesPorReserva') AS INT)
        THROW 50001, 'La reservación supera el máximo de noches permitido.', 1;
    IF NOT EXISTS (SELECT 1 FROM Clientes c INNER JOIN EstadosUsuario e ON e.Id = c.EstadoUsuarioId
                   WHERE c.Id = @ClienteId AND e.PermiteAcceso = 1)
        THROW 50001, 'El cliente no existe o no está activo.', 1;

    DECLARE @TipoId INT, @CapAdultos INT, @CapTotal INT;
    SELECT @TipoId = t.Id, @CapAdultos = t.CapacidadAdultos, @CapTotal = t.CapacidadAdultos + t.CapacidadNinos
    FROM Habitaciones h INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
    WHERE h.Id = @HabitacionId;

    IF @TipoId IS NULL
        THROW 50004, 'La habitación no existe.', 1;
    IF @Adultos > @CapAdultos OR @Adultos + @Ninos > @CapTotal
        THROW 50001, 'La cantidad de personas supera la capacidad de la habitación.', 1;

    -- el código promocional tiene prioridad sobre el descuento fijo del cliente
    DECLARE @DescuentoId INT;
    IF @CodigoDescuento IS NOT NULL
    BEGIN
        SET @DescuentoId = dbo.fn_DescuentoValido(@CodigoDescuento, 'Hospedaje');
        IF @DescuentoId IS NULL
            THROW 50001, 'El código de descuento no existe, está vencido o no aplica a hospedaje.', 1;
    END
    ELSE
        SELECT @DescuentoId = DescuentoId FROM Clientes WHERE Id = @ClienteId;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- bloquea la habitación: si dos usuarios reservan a la vez, el segundo espera
        DECLARE @Bloqueo INT;
        SELECT @Bloqueo = Id FROM Habitaciones WITH (UPDLOCK, HOLDLOCK) WHERE Id = @HabitacionId;

        -- la disponibilidad se revisa DENTRO de la transacción
        IF dbo.fn_HabitacionDisponible(@HabitacionId, @Inicio, @Fin, NULL) = 0
            THROW 50002, 'La habitación no está disponible en esas fechas.', 1;

        DECLARE @Subtotal DECIMAL(10,2) = dbo.fn_CostoHospedaje(@TipoId, @Inicio, @Fin, @Adultos + @Ninos);
        DECLARE @Descuento DECIMAL(10,2) = dbo.fn_MontoDescuento(@DescuentoId, @Subtotal, 'Hospedaje');
        DECLARE @Impuestos DECIMAL(10,2) = ROUND((@Subtotal - @Descuento) * CAST(dbo.fn_Parametro('IVA') AS DECIMAL(10,2)) / 100, 2);
        DECLARE @Total DECIMAL(10,2) = @Subtotal - @Descuento + @Impuestos;
        DECLARE @Anticipo DECIMAL(10,2) = ROUND(@Total * CAST(dbo.fn_Parametro('PorcentajeAnticipo') AS DECIMAL(10,2)) / 100, 2);

        -- código único: HOT-AAAA-MM-00001
        DECLARE @Codigo NVARCHAR(25) = dbo.fn_Parametro('PrefijoReservacion') + '-' + FORMAT(GETDATE(), 'yyyy-MM') + '-'
                                     + RIGHT('00000' + CAST(NEXT VALUE FOR SeqReservacion AS VARCHAR), 5);
        DECLARE @Id INT;

        INSERT INTO Reservaciones (Codigo, ClienteId, FechaInicio, FechaFin, CantidadAdultos, CantidadNinos, EstadoReservacionId,
                                   DescuentoId, Subtotal, Descuento, Impuestos, Total, AnticipoRequerido, Observaciones, UsuarioCreacionId)
        VALUES (@Codigo, @ClienteId, @Inicio, @Fin, @Adultos, @Ninos, 1,   -- 1 = PendientePago
                CASE WHEN @Descuento > 0 THEN @DescuentoId END, @Subtotal, @Descuento, @Impuestos, @Total, @Anticipo, @Observaciones, @UsuarioId);
        SET @Id = SCOPE_IDENTITY();

        INSERT INTO ReservacionHabitaciones (ReservacionId, HabitacionId, FechaInicio, FechaFin, Subtotal)
        VALUES (@Id, @HabitacionId, @Inicio, @Fin, @Subtotal);

        -- el titular queda como primer huésped
        INSERT INTO HuespedesReservacion (ReservacionId, NombreCompleto, TipoIdentificacionId, NumeroIdentificacion, EsTitular)
        SELECT @Id, Nombres + ' ' + Apellidos, TipoIdentificacionId, NumeroIdentificacion, 1 FROM Clientes WHERE Id = @ClienteId;

        -- se abre la cuenta de la reservación, todavía sin abonos
        INSERT INTO Pagos (ClienteId, ReservacionId, MontoTotal) VALUES (@ClienteId, @Id, @Total);

        INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
        VALUES (@Id, NULL, 1, @UsuarioId, 'Reservación creada');

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'CrearReservacion', 'Reservacion', @Id, @Codigo);

        COMMIT;
        SELECT @Id AS ReservacionId, @Codigo AS Codigo, @Total AS Total, @Anticipo AS AnticipoRequerido;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Registra un abono a la cuenta de la reservación.
-- Cuando lo abonado cubre el anticipo, la reservación pasa a Confirmada
CREATE PROCEDURE sp_RegistrarAbono
    @ReservacionId INT,
    @Monto DECIMAL(10,2),
    @MetodoPagoId INT,
    @Referencia NVARCHAR(100) = NULL,
    @UltimosDigitos CHAR(4) = NULL,
    @UsuarioId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Estado INT, @Anticipo DECIMAL(10,2), @PagoId INT;
    SELECT @Estado = EstadoReservacionId, @Anticipo = AnticipoRequerido FROM Reservaciones WHERE Id = @ReservacionId;
    SELECT @PagoId = Id FROM Pagos WHERE ReservacionId = @ReservacionId;

    IF @Estado IS NULL
        THROW 50004, 'La reservación no existe.', 1;
    IF @Estado NOT IN (1, 2, 3)                  -- PendientePago, Confirmada, EnCurso
        THROW 50001, 'No se pueden registrar abonos en una reservación cancelada o completada.', 1;
    IF @Monto <= 0
        THROW 50001, 'El monto debe ser mayor que cero.', 1;
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = @MetodoPagoId AND Activo = 1)
        THROW 50001, 'Método de pago no válido.', 1;
    IF @Referencia IS NULL AND (SELECT RequiereReferencia FROM MetodosPago WHERE Id = @MetodoPagoId) = 1
        THROW 50001, 'Este método de pago requiere número de referencia.', 1;
    IF @Referencia IS NOT NULL AND EXISTS (SELECT 1 FROM Abonos WHERE Referencia = @Referencia)
        THROW 50002, 'Ya existe un abono con esa referencia.', 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- bloquea la cuenta: si llegan dos abonos a la vez, el segundo espera y ve el saldo ya actualizado
        DECLARE @Bloqueo INT;
        SELECT @Bloqueo = Id FROM Pagos WITH (UPDLOCK, HOLDLOCK) WHERE Id = @PagoId;

        IF @Monto > dbo.fn_SaldoReservacion(@ReservacionId)
            THROW 50001, 'El abono es mayor que el saldo pendiente.', 1;

        INSERT INTO Abonos (PagoId, MetodoPagoId, Monto, Referencia, UltimosDigitosTarjeta, UsuarioId)
        VALUES (@PagoId, @MetodoPagoId, @Monto, @Referencia, @UltimosDigitos, @UsuarioId);

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;

        -- política de anticipo (las reservas pendientes ya apartan la habitación, no hay traslape posible)
        IF @Estado = 1 AND (SELECT TotalAbonado FROM Pagos WHERE Id = @PagoId) >= @Anticipo
        BEGIN
            UPDATE Reservaciones SET EstadoReservacionId = 2 WHERE Id = @ReservacionId;   -- 2 = Confirmada

            INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
            VALUES (@ReservacionId, 1, 2, @UsuarioId, 'Anticipo cubierto');
        END

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'RegistrarAbono', 'Reservacion', @ReservacionId, CAST(@Monto AS VARCHAR(20)));

        COMMIT;

        SELECT MontoTotal, TotalAbonado, Saldo FROM Pagos WHERE Id = @PagoId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Los abonos no se borran: se marcan como Reversados con su motivo y la cuenta se recalcula
CREATE PROCEDURE sp_ReversarAbono
    @AbonoId INT,
    @Motivo NVARCHAR(300),
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ReservacionId INT, @PedidoId INT, @FacturaId INT;
    SELECT @ReservacionId = p.ReservacionId, @PedidoId = p.PedidoId, @FacturaId = p.FacturaId
    FROM Abonos a INNER JOIN Pagos p ON p.Id = a.PagoId
    WHERE a.Id = @AbonoId AND a.Estado = 'Confirmado';

    IF @ReservacionId IS NULL AND @PedidoId IS NULL
        THROW 50001, 'El abono no existe o ya fue reversado.', 1;
    IF @FacturaId IS NOT NULL
        THROW 50001, 'El abono ya pertenece a una factura emitida.', 1;
    IF @Motivo IS NULL OR @Motivo = ''
        THROW 50001, 'Debe indicar el motivo del reverso.', 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE Abonos SET Estado = 'Reversado', MotivoReverso = @Motivo WHERE Id = @AbonoId;

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId, @PedidoId = @PedidoId;

        -- si la reserva estaba Confirmada y ya no cubre el anticipo, vuelve a PendientePago
        IF EXISTS (SELECT 1 FROM Reservaciones r INNER JOIN Pagos p ON p.ReservacionId = r.Id
                   WHERE r.Id = @ReservacionId AND r.EstadoReservacionId = 2 AND p.TotalAbonado < r.AnticipoRequerido)
        BEGIN
            UPDATE Reservaciones SET EstadoReservacionId = 1 WHERE Id = @ReservacionId;
            INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
            VALUES (@ReservacionId, 2, 1, @UsuarioId, 'Abono reversado: el anticipo ya no está cubierto');
        END

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'ReversarAbono', 'Abono', @AbonoId, @Motivo);
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Política de cancelación (parámetro HorasCancelacionSinCargo):
--   * con suficiente anticipación: se reembolsa todo lo abonado
--   * tarde (o el mismo día): lo abonado se queda como penalización
-- La cuenta queda en estado Anulada con saldo cero.
CREATE PROCEDURE sp_CancelarReservacion
    @ReservacionId INT,
    @Motivo NVARCHAR(300),
    @UsuarioId INT = NULL,
    @ClienteId INT = NULL                        -- si lo pide un huésped: solo su propia reserva
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Estado INT, @Dueno INT, @Inicio DATE;
    SELECT @Estado = EstadoReservacionId, @Dueno = ClienteId, @Inicio = FechaInicio
    FROM Reservaciones WHERE Id = @ReservacionId;

    IF @Estado IS NULL
        THROW 50004, 'La reservación no existe.', 1;
    IF @ClienteId IS NOT NULL AND @ClienteId <> @Dueno
        THROW 50001, 'No puede cancelar reservaciones de otro cliente.', 1;
    IF @Estado NOT IN (1, 2)
        THROW 50001, 'Solo se pueden cancelar reservaciones pendientes o confirmadas.', 1;
    IF @Motivo IS NULL OR @Motivo = ''
        THROW 50001, 'Debe indicar el motivo de la cancelación.', 1;

    -- fecha y hora límite para cancelar sin cargo = llegada (día + hora de check-in) menos X horas
    DECLARE @Llegada DATETIME = CAST(@Inicio AS DATETIME) + CAST(CAST(dbo.fn_Parametro('HoraCheckIn') AS TIME) AS DATETIME);
    DECLARE @Limite DATETIME = DATEADD(HOUR, -CAST(dbo.fn_Parametro('HorasCancelacionSinCargo') AS INT), @Llegada);
    DECLARE @Reembolsa BIT = CASE WHEN GETDATE() <= @Limite THEN 1 ELSE 0 END;
    DECLARE @MontoReembolso DECIMAL(10,2) = 0;

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE Reservaciones SET EstadoReservacionId = 5, MotivoCancelacion = @Motivo WHERE Id = @ReservacionId;  -- 5 = Cancelada

        IF @Reembolsa = 1
        BEGIN
            SELECT @MontoReembolso = ISNULL(SUM(a.Monto), 0)
            FROM Abonos a INNER JOIN Pagos p ON p.Id = a.PagoId
            WHERE p.ReservacionId = @ReservacionId AND a.Estado = 'Confirmado';

            UPDATE a SET Estado = 'Reembolsado', MotivoReverso = 'Cancelación sin cargo: ' + @Motivo
            FROM Abonos a INNER JOIN Pagos p ON p.Id = a.PagoId
            WHERE p.ReservacionId = @ReservacionId AND a.Estado = 'Confirmado';
        END

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;   -- la cuenta pasa a Anulada

        INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
        VALUES (@ReservacionId, @Estado, 5, @UsuarioId, @Motivo);

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'CancelarReservacion', 'Reservacion', @ReservacionId,
                @Motivo + CASE WHEN @Reembolsa = 1 THEN ' (con reembolso)' ELSE ' (con penalización)' END);
        COMMIT;

        SELECT @Reembolsa AS ConReembolso, @MontoReembolso AS MontoReembolsado,
               (SELECT MontoTotal FROM Pagos WHERE ReservacionId = @ReservacionId) AS MontoRetenido;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Cambia fechas o cantidad de personas de una reserva Pendiente o Confirmada.
-- Recalcula el precio y vuelve a revisar la disponibilidad (sin chocar consigo misma).
CREATE PROCEDURE sp_ModificarReservacion
    @ReservacionId INT,
    @Inicio DATE,
    @Fin DATE,
    @Adultos INT,
    @Ninos INT = 0,
    @UsuarioId INT = NULL,
    @ClienteId INT = NULL                        -- si lo pide un huésped: solo su propia reserva
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Estado INT, @Dueno INT, @DescuentoId INT;
    SELECT @Estado = EstadoReservacionId, @Dueno = ClienteId, @DescuentoId = DescuentoId
    FROM Reservaciones WHERE Id = @ReservacionId;

    IF @Estado IS NULL
        THROW 50004, 'La reservación no existe.', 1;
    IF @ClienteId IS NOT NULL AND @ClienteId <> @Dueno
        THROW 50001, 'No puede modificar reservaciones de otro cliente.', 1;
    IF @Estado NOT IN (1, 2)
        THROW 50001, 'Solo se pueden modificar reservaciones pendientes o confirmadas.', 1;
    IF @Inicio >= @Fin
        THROW 50001, 'La fecha de entrada debe ser menor que la de salida.', 1;
    IF @Inicio < CAST(GETDATE() AS DATE)
        THROW 50001, 'No se puede reservar en fechas pasadas.', 1;
    IF DATEDIFF(DAY, @Inicio, @Fin) > CAST(dbo.fn_Parametro('MaxNochesPorReserva') AS INT)
        THROW 50001, 'La reservación supera el máximo de noches permitido.', 1;

    DECLARE @HabitacionId INT, @TipoId INT, @CapAdultos INT, @CapTotal INT;
    SELECT TOP 1 @HabitacionId = rh.HabitacionId, @TipoId = t.Id,
                 @CapAdultos = t.CapacidadAdultos, @CapTotal = t.CapacidadAdultos + t.CapacidadNinos
    FROM ReservacionHabitaciones rh
    INNER JOIN Habitaciones h ON h.Id = rh.HabitacionId
    INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
    WHERE rh.ReservacionId = @ReservacionId;

    IF @Adultos <= 0 OR @Ninos < 0 OR @Adultos > @CapAdultos OR @Adultos + @Ninos > @CapTotal
        THROW 50001, 'La cantidad de personas no es válida para la capacidad de la habitación.', 1;

    -- conserva el descuento que ya tenía; si no tenía, usa el del cliente
    IF @DescuentoId IS NULL
        SELECT @DescuentoId = DescuentoId FROM Clientes WHERE Id = @Dueno;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Bloqueo INT;
        SELECT @Bloqueo = Id FROM Habitaciones WITH (UPDLOCK, HOLDLOCK) WHERE Id = @HabitacionId;

        IF dbo.fn_HabitacionDisponible(@HabitacionId, @Inicio, @Fin, @ReservacionId) = 0
            THROW 50002, 'La habitación no está disponible en las nuevas fechas.', 1;

        DECLARE @Subtotal DECIMAL(10,2) = dbo.fn_CostoHospedaje(@TipoId, @Inicio, @Fin, @Adultos + @Ninos);
        DECLARE @Descuento DECIMAL(10,2) = dbo.fn_MontoDescuento(@DescuentoId, @Subtotal, 'Hospedaje');
        DECLARE @Impuestos DECIMAL(10,2) = ROUND((@Subtotal - @Descuento) * CAST(dbo.fn_Parametro('IVA') AS DECIMAL(10,2)) / 100, 2);
        DECLARE @Total DECIMAL(10,2) = @Subtotal - @Descuento + @Impuestos;
        DECLARE @Anticipo DECIMAL(10,2) = ROUND(@Total * CAST(dbo.fn_Parametro('PorcentajeAnticipo') AS DECIMAL(10,2)) / 100, 2);
        DECLARE @Abonado DECIMAL(10,2) = (SELECT TotalAbonado FROM Pagos WHERE ReservacionId = @ReservacionId);

        IF @Abonado > @Total
            THROW 50001, 'Lo abonado supera el nuevo total. Reverse un abono antes de modificar.', 1;

        -- el estado depende de si lo abonado cubre el NUEVO anticipo
        DECLARE @NuevoEstado INT = CASE WHEN @Abonado >= @Anticipo THEN 2 ELSE 1 END;

        UPDATE Reservaciones
        SET FechaInicio = @Inicio, FechaFin = @Fin, CantidadAdultos = @Adultos, CantidadNinos = @Ninos,
            DescuentoId = CASE WHEN @Descuento > 0 THEN @DescuentoId END,
            Subtotal = @Subtotal, Descuento = @Descuento, Impuestos = @Impuestos, Total = @Total,
            AnticipoRequerido = @Anticipo, EstadoReservacionId = @NuevoEstado
        WHERE Id = @ReservacionId;

        UPDATE ReservacionHabitaciones SET FechaInicio = @Inicio, FechaFin = @Fin, Subtotal = @Subtotal
        WHERE ReservacionId = @ReservacionId;

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;

        INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
        VALUES (@ReservacionId, @Estado, @NuevoEstado, @UsuarioId,
                'Modificada: ' + CONVERT(VARCHAR(10), @Inicio, 23) + ' a ' + CONVERT(VARCHAR(10), @Fin, 23)
                + ', ' + CAST(@Adultos AS VARCHAR) + ' adultos y ' + CAST(@Ninos AS VARCHAR) + ' niños');

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'ModificarReservacion', 'Reservacion', @ReservacionId, CAST(@Total AS VARCHAR(20)));
        COMMIT;

        SELECT r.Id AS ReservacionId, er.Nombre AS Estado, r.Total, r.AnticipoRequerido, p.TotalAbonado, p.Saldo
        FROM Reservaciones r
        INNER JOIN EstadosReservacion er ON er.Id = r.EstadoReservacionId
        INNER JOIN Pagos p ON p.ReservacionId = r.Id
        WHERE r.Id = @ReservacionId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Ajuste autorizado sobre la cuenta (cargo extra o rebaja), con justificación y supervisor que lo aprueba
CREATE PROCEDURE sp_RegistrarAjuste
    @ReservacionId INT,
    @Monto DECIMAL(10,2),                        -- positivo = cargo, negativo = rebaja
    @Motivo NVARCHAR(300),
    @UsuarioId INT,
    @AutorizadoPorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Reservaciones WHERE Id = @ReservacionId AND EstadoReservacionId IN (1, 2, 3))
        THROW 50001, 'Solo se pueden ajustar reservaciones activas (pendientes, confirmadas o en curso).', 1;
    IF @Monto = 0
        THROW 50001, 'El monto del ajuste no puede ser cero.', 1;
    IF @Motivo IS NULL OR @Motivo = ''
        THROW 50001, 'Debe indicar la justificación del ajuste.', 1;
    IF NOT EXISTS (SELECT 1 FROM vw_UsuarioPermisos WHERE UsuarioId = @AutorizadoPorId AND Permiso = 'Ajustes.Autorizar')
        THROW 50001, 'El usuario que autoriza no tiene permiso para autorizar ajustes.', 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Bloqueo INT;
        SELECT @Bloqueo = Id FROM Pagos WITH (UPDLOCK, HOLDLOCK) WHERE ReservacionId = @ReservacionId;

        IF dbo.fn_SaldoReservacion(@ReservacionId) + @Monto < 0
            THROW 50001, 'La rebaja es mayor que el saldo pendiente.', 1;

        INSERT INTO AjustesCuenta (ReservacionId, Monto, Motivo, UsuarioId, AutorizadoPorId)
        VALUES (@ReservacionId, @Monto, @Motivo, @UsuarioId, @AutorizadoPorId);

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'RegistrarAjuste', 'Reservacion', @ReservacionId,
                CAST(@Monto AS VARCHAR(20)) + ' autorizado por usuario ' + CAST(@AutorizadoPorId AS VARCHAR));
        COMMIT;

        SELECT MontoTotal, TotalAbonado, Saldo FROM Pagos WHERE ReservacionId = @ReservacionId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

--  Check-in / Check-out 

-- Cambia el estado de una habitación a mano (limpieza terminada, mantenimiento, bloqueo).
-- Reservada y Ocupada no se ponen a mano: las asigna el sistema.
CREATE PROCEDURE sp_CambiarEstadoHabitacion
    @HabitacionId INT,
    @EstadoHabitacionId INT,
    @Motivo NVARCHAR(200) = NULL,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Habitaciones WHERE Id = @HabitacionId AND Activo = 1)
        THROW 50004, 'La habitación no existe o está desactivada.', 1;
    IF @EstadoHabitacionId NOT IN (1, 4, 5, 6)   -- Disponible, EnLimpieza, EnMantenimiento, Bloqueada
        THROW 50001, 'Los estados Reservada y Ocupada los asigna el sistema.', 1;
    IF EXISTS (SELECT 1 FROM ReservacionHabitaciones rh INNER JOIN Reservaciones r ON r.Id = rh.ReservacionId
               WHERE rh.HabitacionId = @HabitacionId AND r.EstadoReservacionId = 3)   -- 3 = EnCurso
        THROW 50002, 'La habitación tiene un huésped hospedado; primero debe hacerse el check-out.', 1;
    IF @EstadoHabitacionId IN (5, 6) AND (@Motivo IS NULL OR @Motivo = '')
        THROW 50001, 'Debe indicar el motivo del mantenimiento o bloqueo.', 1;

    BEGIN TRANSACTION;
        EXEC sp_set_session_context N'UsuarioId', @UsuarioId;   -- los usa el trigger de la bitácora
        EXEC sp_set_session_context N'Motivo', @Motivo;

        UPDATE Habitaciones SET EstadoHabitacionId = @EstadoHabitacionId WHERE Id = @HabitacionId;

        EXEC sp_set_session_context N'Motivo', NULL;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'CambiarEstadoHabitacion', 'Habitacion', @HabitacionId,
                (SELECT Nombre FROM EstadosHabitacion WHERE Id = @EstadoHabitacionId));
    COMMIT;

    -- si queda fuera de servicio, devuelve las reservas futuras afectadas para que recepción las reubique
    IF @EstadoHabitacionId IN (5, 6)
        SELECT r.Id AS ReservacionId, r.Codigo, r.FechaInicio, r.FechaFin
        FROM ReservacionHabitaciones rh
        INNER JOIN Reservaciones r ON r.Id = rh.ReservacionId
        WHERE rh.HabitacionId = @HabitacionId AND r.EstadoReservacionId IN (1, 2)
          AND rh.FechaFin > CAST(GETDATE() AS DATE);
END
GO

-- @AutorizarAnticipado = 1 permite entrar antes de la HoraCheckIn (lo decide un supervisor)
CREATE PROCEDURE sp_CheckIn
    @ReservacionId INT,
    @UsuarioId INT,
    @AutorizarAnticipado BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Estado INT, @Inicio DATE, @Fin DATE, @Hoy DATE = CAST(GETDATE() AS DATE);
    SELECT @Estado = EstadoReservacionId, @Inicio = FechaInicio, @Fin = FechaFin FROM Reservaciones WHERE Id = @ReservacionId;

    IF @Estado IS NULL
        THROW 50004, 'La reservación no existe.', 1;
    IF @Estado IN (4, 5)
        THROW 50001, 'No se puede hacer check-in de una reservación completada o cancelada.', 1;
    IF @Estado = 3
        THROW 50002, 'La reservación ya tiene check-in.', 1;
    IF @Estado = 1
        THROW 50001, 'La reservación no está confirmada: falta el anticipo.', 1;
    IF @Hoy < @Inicio OR @Hoy >= @Fin
        THROW 50001, 'El check-in solo se puede hacer dentro de las fechas de la reservación.', 1;

    DECLARE @HoraCheckIn TIME = CAST(dbo.fn_Parametro('HoraCheckIn') AS TIME);
    IF @Hoy = @Inicio AND CAST(GETDATE() AS TIME) < @HoraCheckIn AND @AutorizarAnticipado = 0
    BEGIN
        DECLARE @MsgHora NVARCHAR(200) = 'El check-in es a partir de las ' + LEFT(CAST(@HoraCheckIn AS VARCHAR), 5)
                                       + '. Un supervisor puede autorizar un check-in anticipado.';
        THROW 50001, @MsgHora, 1;
    END

    IF EXISTS (SELECT 1 FROM ReservacionHabitaciones rh INNER JOIN Habitaciones h ON h.Id = rh.HabitacionId
               WHERE rh.ReservacionId = @ReservacionId AND h.EstadoHabitacionId NOT IN (1, 2))  -- Disponible o Reservada
        THROW 50002, 'La habitación todavía no está lista (ocupada, en limpieza o fuera de servicio).', 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        EXEC sp_set_session_context N'UsuarioId', @UsuarioId;   -- para la bitácora de la habitación
        EXEC sp_set_session_context N'Motivo', N'Check-in';

        INSERT INTO Estadias (ReservacionId, CheckInReal, UsuarioCheckInId, CodigoAcceso)
        VALUES (@ReservacionId, GETDATE(), @UsuarioId,
                RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR), 6));  -- código de 6 dígitos

        UPDATE Reservaciones SET EstadoReservacionId = 3 WHERE Id = @ReservacionId;   -- 3 = EnCurso

        UPDATE Habitaciones SET EstadoHabitacionId = 3                               -- 3 = Ocupada
        WHERE Id IN (SELECT HabitacionId FROM ReservacionHabitaciones WHERE ReservacionId = @ReservacionId);

        INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
        VALUES (@ReservacionId, 2, 3, @UsuarioId, 'Check-in');

        EXEC sp_set_session_context N'Motivo', NULL;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'CheckIn', 'Reservacion', @ReservacionId,
                CASE WHEN @Hoy = @Inicio AND CAST(GETDATE() AS TIME) < @HoraCheckIn THEN 'Check-in anticipado autorizado' END);
        COMMIT;

        SELECT CodigoAcceso FROM Estadias WHERE ReservacionId = @ReservacionId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_RegistrarCargoAdicional
    @ReservacionId INT,
    @Concepto NVARCHAR(150),
    @Monto DECIMAL(10,2),
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Reservaciones WHERE Id = @ReservacionId AND EstadoReservacionId = 3)
        THROW 50001, 'Solo se registran cargos a huéspedes con estadía en curso.', 1;
    IF @Monto <= 0
        THROW 50001, 'El monto debe ser mayor que cero.', 1;

    BEGIN TRANSACTION;
        INSERT INTO CargosAdicionales (ReservacionId, Concepto, Monto, Impuesto, UsuarioId)
        VALUES (@ReservacionId, @Concepto, @Monto,
                ROUND(@Monto * CAST(dbo.fn_Parametro('IVA') AS DECIMAL(10,2)) / 100, 2), @UsuarioId);

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;   -- sube el total de la cuenta
    COMMIT;
END
GO

-- 1) cobra late check-out si sale tarde  2) exige saldo en cero
-- 3) cierra la estadía, la habitación pasa a En Limpieza y se emite la factura
CREATE PROCEDURE sp_CheckOut
    @ReservacionId INT,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @FechaFin DATE, @Subtotal DECIMAL(10,2);
    SELECT @FechaFin = FechaFin, @Subtotal = Subtotal FROM Reservaciones
    WHERE Id = @ReservacionId AND EstadoReservacionId = 3;

    IF @FechaFin IS NULL
        THROW 50001, 'No se puede hacer check-out: la reservación no está En Curso (no tiene check-in).', 1;

    -- todos los pedidos cargados a la habitación deben estar entregados o cancelados
    IF EXISTS (SELECT 1 FROM PedidosRestaurante p INNER JOIN EstadosPedido e ON e.Id = p.EstadoPedidoId
               WHERE p.ReservacionId = @ReservacionId AND p.CargoAHabitacion = 1 AND e.EsFinal = 0)
        THROW 50001, 'El huésped tiene pedidos cargados a la habitación sin entregar o cancelar.', 1;

    -- late check-out: después de la hora límite se cobra un % de una noche (una sola vez)
    DECLARE @Ahora DATETIME = GETDATE();
    IF (CAST(@Ahora AS DATE) > @FechaFin
        OR (CAST(@Ahora AS DATE) = @FechaFin AND CAST(@Ahora AS TIME) > CAST(dbo.fn_Parametro('HoraCheckOut') AS TIME)))
       AND NOT EXISTS (SELECT 1 FROM CargosAdicionales WHERE ReservacionId = @ReservacionId AND EsLateCheckout = 1)
    BEGIN
        DECLARE @MontoLate DECIMAL(10,2) = ROUND(@Subtotal / (SELECT DATEDIFF(DAY, FechaInicio, FechaFin) FROM Reservaciones WHERE Id = @ReservacionId)
                                                 * CAST(dbo.fn_Parametro('PorcentajeLateCheckout') AS DECIMAL(10,2)) / 100, 2);
        INSERT INTO CargosAdicionales (ReservacionId, Concepto, Monto, Impuesto, EsLateCheckout, UsuarioId)
        VALUES (@ReservacionId, 'Salida tardía (late check-out)', @MontoLate,
                ROUND(@MontoLate * CAST(dbo.fn_Parametro('IVA') AS DECIMAL(10,2)) / 100, 2), 1, @UsuarioId);

        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;
    END

    -- si queda saldo no se libera la cuenta (el cargo de late check-out sí queda guardado)
    DECLARE @Saldo DECIMAL(10,2) = dbo.fn_SaldoReservacion(@ReservacionId);
    IF @Saldo > 0
    BEGIN
        DECLARE @Msg NVARCHAR(200) = 'El huésped tiene un saldo pendiente de ' + CAST(@Saldo AS VARCHAR(20)) + '. Debe pagarlo antes del check-out.';
        THROW 50003, @Msg, 1;
    END
    -- saldo a favor (ej: se canceló un pedido ya pagado): hay que reversar el abono sobrante primero
    IF @Saldo < 0
    BEGIN
        DECLARE @MsgFavor NVARCHAR(200) = 'El huésped tiene un saldo a favor de ' + CAST(-@Saldo AS VARCHAR(20)) + '. Reverse el abono sobrante antes del check-out.';
        THROW 50003, @MsgFavor, 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        EXEC sp_set_session_context N'UsuarioId', @UsuarioId;
        EXEC sp_set_session_context N'Motivo', N'Check-out';

        UPDATE Estadias SET CheckOutReal = GETDATE(), UsuarioCheckOutId = @UsuarioId WHERE ReservacionId = @ReservacionId;

        UPDATE Reservaciones SET EstadoReservacionId = 4 WHERE Id = @ReservacionId;   -- 4 = Completada

        UPDATE Habitaciones SET EstadoHabitacionId = 4                               -- 4 = EnLimpieza
        WHERE Id IN (SELECT HabitacionId FROM ReservacionHabitaciones WHERE ReservacionId = @ReservacionId);

        EXEC sp_set_session_context N'Motivo', NULL;

        INSERT INTO HistorialReservacion (ReservacionId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, Motivo)
        VALUES (@ReservacionId, 3, 4, @UsuarioId, 'Check-out');

        --  Factura: encabezado 
        DECLARE @FacturaId INT;
        DECLARE @Numero NVARCHAR(25) = 'FAC-' + CAST(YEAR(GETDATE()) AS VARCHAR) + '-'
                                     + RIGHT('000000' + CAST(NEXT VALUE FOR SeqFactura AS VARCHAR), 6);

        INSERT INTO Facturas (NumeroFactura, ClienteId, ReservacionId, Subtotal, Descuento, Impuestos, Total, TotalPagado, UsuarioId)
        SELECT @Numero, r.ClienteId, r.Id,
               r.Subtotal + ISNULL(ca.Monto, 0) + ISNULL(pe.Subtotal, 0) + ISNULL(pe.Recargo, 0) + ISNULL(aj.Cargos, 0),
               r.Descuento + ISNULL(pe.Descuento, 0) + ISNULL(aj.Rebajas, 0),
               r.Impuestos + ISNULL(ca.Impuesto, 0) + ISNULL(pe.Impuestos, 0),
               r.Total + ISNULL(ca.Monto + ca.Impuesto, 0) + ISNULL(pe.Total, 0) + ISNULL(aj.Cargos, 0) - ISNULL(aj.Rebajas, 0),
               (SELECT TotalAbonado FROM Pagos WHERE ReservacionId = r.Id),
               @UsuarioId
        FROM Reservaciones r
        OUTER APPLY (SELECT SUM(Monto) AS Monto, SUM(Impuesto) AS Impuesto
                     FROM CargosAdicionales WHERE ReservacionId = r.Id) ca
        OUTER APPLY (SELECT SUM(Subtotal) AS Subtotal, SUM(Recargo) AS Recargo, SUM(Descuento) AS Descuento,
                            SUM(Impuestos) AS Impuestos, SUM(Total) AS Total
                     FROM PedidosRestaurante
                     WHERE ReservacionId = r.Id AND CargoAHabitacion = 1 AND EstadoPedidoId <> 7) pe   -- 7 = Cancelado
        OUTER APPLY (SELECT SUM(CASE WHEN Monto > 0 THEN Monto ELSE 0 END) AS Cargos,
                            SUM(CASE WHEN Monto < 0 THEN -Monto ELSE 0 END) AS Rebajas
                     FROM AjustesCuenta WHERE ReservacionId = r.Id) aj
        WHERE r.Id = @ReservacionId;

        SET @FacturaId = SCOPE_IDENTITY();

        --  Factura: detalle 
        -- hospedaje
        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Hospedaje', 'Habitación ' + h.Numero + ' - ' + t.Nombre,
               DATEDIFF(DAY, rh.FechaInicio, rh.FechaFin),
               ROUND(rh.Subtotal / DATEDIFF(DAY, rh.FechaInicio, rh.FechaFin), 2), rh.Subtotal
        FROM ReservacionHabitaciones rh
        INNER JOIN Habitaciones h ON h.Id = rh.HabitacionId
        INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
        WHERE rh.ReservacionId = @ReservacionId;

        -- cargos adicionales
        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'CargoAdicional', Concepto, 1, Monto, Monto
        FROM CargosAdicionales WHERE ReservacionId = @ReservacionId;

        -- cada platillo consumido y cargado a la habitación
        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Restaurante', pl.Nombre + ' (pedido #' + CAST(p.Id AS VARCHAR) + ', ' + p.Modalidad + ')',
               d.Cantidad, d.PrecioUnitario, d.Subtotal
        FROM PedidosRestaurante p
        INNER JOIN DetallePedidos d ON d.PedidoId = p.Id
        INNER JOIN Platillos pl ON pl.Id = d.PlatilloId
        WHERE p.ReservacionId = @ReservacionId AND p.CargoAHabitacion = 1 AND p.EstadoPedidoId <> 7;

        -- recargo de room service
        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Recargo', 'Recargo por room service', 1, SUM(Recargo), SUM(Recargo)
        FROM PedidosRestaurante
        WHERE ReservacionId = @ReservacionId AND CargoAHabitacion = 1 AND EstadoPedidoId <> 7
        HAVING SUM(Recargo) > 0;

        -- ajustes autorizados (las rebajas salen en negativo)
        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Ajuste', Motivo, 1, Monto, Monto
        FROM AjustesCuenta WHERE ReservacionId = @ReservacionId;

        -- la cuenta (y con ella todos sus abonos) queda ligada a esta factura
        UPDATE Pagos SET FacturaId = @FacturaId WHERE ReservacionId = @ReservacionId;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'CheckOut', 'Reservacion', @ReservacionId, @Numero);

        COMMIT;
        SELECT @FacturaId AS FacturaId, @Numero AS NumeroFactura;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

--  Restaurante 

-- Recalcula los montos del pedido (uso interno)
CREATE PROCEDURE sp_RecalcularPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Subtotal DECIMAL(10,2), @DescuentoId INT, @Modalidad NVARCHAR(20);
    SELECT @Subtotal = ISNULL(SUM(Subtotal), 0) FROM DetallePedidos WHERE PedidoId = @PedidoId;
    SELECT @DescuentoId = DescuentoId, @Modalidad = Modalidad FROM PedidosRestaurante WHERE Id = @PedidoId;

    DECLARE @Descuento DECIMAL(10,2) = dbo.fn_MontoDescuento(@DescuentoId, @Subtotal, 'Restaurante');
    DECLARE @Recargo DECIMAL(10,2) = 0;

    IF @Modalidad = 'RoomService'
        SET @Recargo = ROUND((@Subtotal - @Descuento) * CAST(dbo.fn_Parametro('RecargoRoomService') AS DECIMAL(10,2)) / 100, 2);

    DECLARE @Impuestos DECIMAL(10,2) = ROUND((@Subtotal - @Descuento + @Recargo) * CAST(dbo.fn_Parametro('IVA') AS DECIMAL(10,2)) / 100, 2);

    UPDATE PedidosRestaurante
    SET Subtotal = @Subtotal, Descuento = @Descuento, Recargo = @Recargo, Impuestos = @Impuestos,
        Total = @Subtotal - @Descuento + @Recargo + @Impuestos
    WHERE Id = @PedidoId;

    -- actualiza la cuenta donde se cobra el pedido: la de la reservación o la del propio pedido
    DECLARE @ReservacionId INT = (SELECT ReservacionId FROM PedidosRestaurante WHERE Id = @PedidoId AND CargoAHabitacion = 1);
    IF @ReservacionId IS NOT NULL
        EXEC sp_ActualizarPago @ReservacionId = @ReservacionId;
    ELSE
        EXEC sp_ActualizarPago @PedidoId = @PedidoId;
END
GO

-- Crea el pedido (vacío). Room service o cargo a habitación exigen estadía activa
CREATE PROCEDURE sp_CrearPedido
    @ClienteId INT,
    @Modalidad NVARCHAR(20),
    @CargarAHabitacion BIT = 0,
    @FechaHoraDeseada DATETIME = NULL,
    @Instrucciones NVARCHAR(300) = NULL,
    @UsuarioId INT = NULL,
    @CodigoDescuento NVARCHAR(20) = NULL         -- código promocional opcional (ej: HAPPY20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Clientes c INNER JOIN EstadosUsuario e ON e.Id = c.EstadoUsuarioId
                   WHERE c.Id = @ClienteId AND e.PermiteAcceso = 1)
        THROW 50001, 'El cliente no existe o no está activo.', 1;
    IF @Modalidad NOT IN ('RoomService', 'Mesa', 'Piscina')
        THROW 50001, 'Modalidad no válida (RoomService, Mesa o Piscina).', 1;

    -- el código promocional tiene prioridad sobre el descuento fijo del cliente
    DECLARE @DescuentoId INT;
    IF @CodigoDescuento IS NOT NULL
    BEGIN
        SET @DescuentoId = dbo.fn_DescuentoValido(@CodigoDescuento, 'Restaurante');
        IF @DescuentoId IS NULL
            THROW 50001, 'El código de descuento no existe, está vencido o no aplica al restaurante.', 1;
    END
    ELSE
        SELECT @DescuentoId = DescuentoId FROM Clientes WHERE Id = @ClienteId;

    DECLARE @ReservacionId INT = NULL, @HabitacionId INT = NULL;

    IF @CargarAHabitacion = 1 OR @Modalidad = 'RoomService'
    BEGIN
        SELECT TOP 1 @ReservacionId = r.Id, @HabitacionId = rh.HabitacionId
        FROM Reservaciones r
        INNER JOIN ReservacionHabitaciones rh ON rh.ReservacionId = r.Id
        WHERE r.ClienteId = @ClienteId AND r.EstadoReservacionId = 3;   -- 3 = EnCurso

        IF @ReservacionId IS NULL
            THROW 50001, 'El cliente no tiene una estadía activa: no se puede cargar a la habitación ni pedir room service.', 1;
    END

    BEGIN TRANSACTION;
        INSERT INTO PedidosRestaurante (ClienteId, ReservacionId, HabitacionId, Modalidad, CargoAHabitacion,
                                        DescuentoId, FechaHoraDeseada, Instrucciones, UsuarioId)
        VALUES (@ClienteId, @ReservacionId, @HabitacionId, @Modalidad, @CargarAHabitacion,
                @DescuentoId, @FechaHoraDeseada, @Instrucciones, @UsuarioId);

        DECLARE @PedidoId INT = SCOPE_IDENTITY();

        INSERT INTO HistorialEstadoPedido (PedidoId, EstadoPedidoId, UsuarioId) VALUES (@PedidoId, 1, @UsuarioId);

        -- si no se carga a la habitación, el pedido tiene su propia cuenta
        IF @CargarAHabitacion = 0
            INSERT INTO Pagos (ClienteId, PedidoId) VALUES (@ClienteId, @PedidoId);
    COMMIT;

    SELECT @PedidoId AS PedidoId;
END
GO

-- Agrega un platillo; el precio se toma de la BD, nunca del cliente
CREATE PROCEDURE sp_AgregarPlatilloPedido
    @PedidoId INT,
    @PlatilloId INT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM PedidosRestaurante p INNER JOIN EstadosPedido e ON e.Id = p.EstadoPedidoId
                   WHERE p.Id = @PedidoId AND e.PermiteModificar = 1)
        THROW 50001, 'El pedido no existe o ya no se puede modificar.', 1;
    IF dbo.fn_PedidoFacturado(@PedidoId) = 1
        THROW 50001, 'El pedido ya fue pagado o facturado y no se puede modificar.', 1;
    IF @Cantidad <= 0
        THROW 50001, 'La cantidad debe ser mayor que cero.', 1;
    IF NOT EXISTS (SELECT 1 FROM Platillos pl INNER JOIN CategoriasPlatillo c ON c.Id = pl.CategoriaId
                   WHERE pl.Id = @PlatilloId AND pl.Activo = 1 AND pl.Disponible = 1 AND c.Activo = 1)
        THROW 50001, 'El platillo no está disponible.', 1;

    BEGIN TRANSACTION;
        -- si el platillo ya estaba en el pedido se suma la cantidad
        IF EXISTS (SELECT 1 FROM DetallePedidos WHERE PedidoId = @PedidoId AND PlatilloId = @PlatilloId)
            UPDATE DetallePedidos SET Cantidad = Cantidad + @Cantidad WHERE PedidoId = @PedidoId AND PlatilloId = @PlatilloId;
        ELSE
            INSERT INTO DetallePedidos (PedidoId, PlatilloId, Cantidad, PrecioUnitario)
            SELECT @PedidoId, Id, @Cantidad, Precio FROM Platillos WHERE Id = @PlatilloId;

        EXEC sp_RecalcularPedido @PedidoId;
    COMMIT;
END
GO

-- El pedido solo avanza (según el Orden de EstadosPedido).
-- Solo se cancela en los estados con PermiteCancelar = 1 (Pendiente, Confirmado)
CREATE PROCEDURE sp_CambiarEstadoPedido
    @PedidoId INT,
    @NuevoEstadoId INT,
    @UsuarioId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OrdenActual INT, @EsFinal BIT, @PermiteCancelar BIT;
    SELECT @OrdenActual = e.Orden, @EsFinal = e.EsFinal, @PermiteCancelar = e.PermiteCancelar
    FROM PedidosRestaurante p INNER JOIN EstadosPedido e ON e.Id = p.EstadoPedidoId
    WHERE p.Id = @PedidoId;

    DECLARE @NuevoNombre NVARCHAR(30), @NuevoOrden INT;
    SELECT @NuevoNombre = Nombre, @NuevoOrden = Orden FROM EstadosPedido WHERE Id = @NuevoEstadoId;

    IF @OrdenActual IS NULL
        THROW 50004, 'El pedido no existe.', 1;
    IF @NuevoNombre IS NULL
        THROW 50001, 'El estado no existe.', 1;
    IF @EsFinal = 1
        THROW 50001, 'El pedido ya fue entregado o cancelado.', 1;
    IF @NuevoNombre = 'Cancelado' AND @PermiteCancelar = 0
        THROW 50001, 'El pedido ya está en preparación y no se puede cancelar.', 1;
    IF @NuevoNombre = 'Cancelado' AND dbo.fn_PedidoFacturado(@PedidoId) = 1
        THROW 50001, 'El pedido ya fue pagado o facturado y no se puede cancelar.', 1;
    IF @NuevoNombre <> 'Cancelado' AND @NuevoOrden <= @OrdenActual
        THROW 50001, 'El pedido no puede regresar a un estado anterior.', 1;
    IF @NuevoNombre <> 'Cancelado' AND NOT EXISTS (SELECT 1 FROM DetallePedidos WHERE PedidoId = @PedidoId)
        THROW 50001, 'El pedido no tiene platillos.', 1;

    BEGIN TRANSACTION;
        UPDATE PedidosRestaurante SET EstadoPedidoId = @NuevoEstadoId WHERE Id = @PedidoId;

        INSERT INTO HistorialEstadoPedido (PedidoId, EstadoPedidoId, UsuarioId) VALUES (@PedidoId, @NuevoEstadoId, @UsuarioId);

        -- un pedido cancelado ya no se cobra: se recalcula la cuenta
        IF @NuevoNombre = 'Cancelado'
            EXEC sp_RecalcularPedido @PedidoId;
    COMMIT;
END
GO

-- Pago de un pedido que NO se cargó a la habitación: registra el abono por el total y emite su factura
CREATE PROCEDURE sp_PagarPedido
    @PedidoId INT,
    @MetodoPagoId INT,
    @Referencia NVARCHAR(100) = NULL,
    @UltimosDigitos CHAR(4) = NULL,
    @UsuarioId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Total DECIMAL(10,2), @ClienteId INT, @CargoHab BIT, @Estado INT;
    SELECT @Total = Total, @ClienteId = ClienteId, @CargoHab = CargoAHabitacion, @Estado = EstadoPedidoId
    FROM PedidosRestaurante WHERE Id = @PedidoId;

    IF @Total IS NULL
        THROW 50004, 'El pedido no existe.', 1;
    IF @CargoHab = 1
        THROW 50001, 'Este pedido se cargó a la habitación; se paga en el check-out.', 1;
    IF @Estado = 7
        THROW 50001, 'El pedido está cancelado.', 1;
    IF @Total = 0
        THROW 50001, 'El pedido no tiene platillos.', 1;
    IF EXISTS (SELECT 1 FROM Facturas WHERE PedidoId = @PedidoId)
        THROW 50002, 'El pedido ya fue pagado.', 1;
    IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = @MetodoPagoId AND Activo = 1)
        THROW 50001, 'Método de pago no válido.', 1;
    IF @Referencia IS NULL AND (SELECT RequiereReferencia FROM MetodosPago WHERE Id = @MetodoPagoId) = 1
        THROW 50001, 'Este método de pago requiere número de referencia.', 1;
    IF @Referencia IS NOT NULL AND EXISTS (SELECT 1 FROM Abonos WHERE Referencia = @Referencia)
        THROW 50002, 'Ya existe un abono con esa referencia.', 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Numero NVARCHAR(25) = 'FAC-' + CAST(YEAR(GETDATE()) AS VARCHAR) + '-'
                                     + RIGHT('000000' + CAST(NEXT VALUE FOR SeqFactura AS VARCHAR), 6);
        DECLARE @FacturaId INT, @PagoId INT = (SELECT Id FROM Pagos WHERE PedidoId = @PedidoId);

        INSERT INTO Facturas (NumeroFactura, ClienteId, PedidoId, Subtotal, Descuento, Impuestos, Total, TotalPagado, UsuarioId)
        SELECT @Numero, ClienteId, Id, Subtotal + Recargo, Descuento, Impuestos, Total, Total, @UsuarioId
        FROM PedidosRestaurante WHERE Id = @PedidoId;
        SET @FacturaId = SCOPE_IDENTITY();

        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Restaurante', pl.Nombre, d.Cantidad, d.PrecioUnitario, d.Subtotal
        FROM DetallePedidos d INNER JOIN Platillos pl ON pl.Id = d.PlatilloId
        WHERE d.PedidoId = @PedidoId;

        INSERT INTO FacturaDetalles (FacturaId, TipoConcepto, Descripcion, Cantidad, PrecioUnitario, Subtotal)
        SELECT @FacturaId, 'Recargo', 'Recargo por room service', 1, Recargo, Recargo
        FROM PedidosRestaurante WHERE Id = @PedidoId AND Recargo > 0;

        INSERT INTO Abonos (PagoId, MetodoPagoId, Monto, Referencia, UltimosDigitosTarjeta, UsuarioId)
        VALUES (@PagoId, @MetodoPagoId, @Total, @Referencia, @UltimosDigitos, @UsuarioId);

        UPDATE Pagos SET FacturaId = @FacturaId WHERE Id = @PagoId;
        EXEC sp_ActualizarPago @PedidoId = @PedidoId;

        INSERT INTO Auditoria (UsuarioId, Accion, Entidad, EntidadId, Datos)
        VALUES (@UsuarioId, 'PagarPedido', 'Pedido', @PedidoId, @Numero);

        COMMIT;
        SELECT @FacturaId AS FacturaId, @Numero AS NumeroFactura;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

--  Correos 

-- Registra un correo en el historial.
--   Enviado:  queda Pendiente hasta que la API lo mande (remitente = correo del hotel).
--   Recibido: queda Recibido; si el remitente es un cliente se liga solo por su email.
--   Si @CorreoRespuestaId trae un correo recibido, ese pasa a Respondido.
CREATE PROCEDURE sp_RegistrarCorreo
    @Direccion NVARCHAR(10),
    @TipoCorreoId INT,
    @Asunto NVARCHAR(200),
    @Cuerpo NVARCHAR(MAX),
    @Destinatario NVARCHAR(150) = NULL,
    @Remitente NVARCHAR(150) = NULL,
    @ClienteId INT = NULL,
    @ReservacionId INT = NULL,
    @FacturaId INT = NULL,
    @CorreoRespuestaId INT = NULL,
    @UsuarioId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @CorreoHotel NVARCHAR(150) = dbo.fn_Parametro('CorreoHotel');

    IF @Direccion = 'Enviado'
    BEGIN
        SET @Remitente = ISNULL(@Remitente, @CorreoHotel);
        -- si no mandan destinatario se usa el email del cliente
        IF @Destinatario IS NULL SELECT @Destinatario = Email FROM Clientes WHERE Id = @ClienteId;
        IF @Destinatario IS NULL THROW 50001, 'El correo enviado necesita un destinatario.', 1;
    END
    ELSE IF @Direccion = 'Recibido'
    BEGIN
        SET @Destinatario = ISNULL(@Destinatario, @CorreoHotel);
        IF @Remitente IS NULL THROW 50001, 'El correo recibido necesita un remitente.', 1;
        IF @ClienteId IS NULL SELECT @ClienteId = Id FROM Clientes WHERE Email = @Remitente;
    END
    ELSE
        THROW 50001, 'La dirección debe ser Enviado o Recibido.', 1;

    IF @CorreoRespuestaId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM Correos WHERE Id = @CorreoRespuestaId AND Direccion = 'Recibido')
        THROW 50001, 'Solo se puede responder a un correo recibido.', 1;

    BEGIN TRANSACTION;
        INSERT INTO Correos (Direccion, TipoCorreoId, EstadoCorreoId, Remitente, Destinatario, Asunto, Cuerpo,
                             ClienteId, ReservacionId, FacturaId, CorreoRespuestaId, UsuarioId, FechaEnvio)
        VALUES (@Direccion, @TipoCorreoId,
                CASE WHEN @Direccion = 'Enviado' THEN 1 ELSE 4 END,     -- Pendiente / Recibido
                @Remitente, @Destinatario, @Asunto, @Cuerpo,
                @ClienteId, @ReservacionId, @FacturaId, @CorreoRespuestaId, @UsuarioId,
                CASE WHEN @Direccion = 'Recibido' THEN GETDATE() END);

        DECLARE @CorreoId INT = SCOPE_IDENTITY();

        IF @CorreoRespuestaId IS NOT NULL
            UPDATE Correos SET EstadoCorreoId = 5 WHERE Id = @CorreoRespuestaId;   -- Respondido
    COMMIT;

    SELECT @CorreoId AS CorreoId;
END
GO

-- La API lo llama después de intentar enviar por SMTP.
-- Si falla se queda Pendiente para reintentar; al llegar al máximo de intentos pasa a Fallido.
CREATE PROCEDURE sp_ActualizarEstadoCorreo
    @CorreoId INT,
    @Exito BIT,
    @MensajeError NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM Correos WHERE Id = @CorreoId AND Direccion = 'Enviado' AND EstadoCorreoId = 1)
        THROW 50001, 'El correo no existe o no está pendiente de envío.', 1;

    DECLARE @MaxIntentos INT = CAST(dbo.fn_Parametro('MaxIntentosCorreo') AS INT);

    UPDATE Correos
    SET Intentos = Intentos + 1,
        EstadoCorreoId = CASE WHEN @Exito = 1 THEN 2                              -- Enviado
                              WHEN Intentos + 1 >= @MaxIntentos THEN 3            -- Fallido
                              ELSE 1 END,                                         -- sigue Pendiente
        FechaEnvio = CASE WHEN @Exito = 1 THEN GETDATE() END,
        MensajeError = CASE WHEN @Exito = 1 THEN NULL ELSE @MensajeError END
    WHERE Id = @CorreoId;

    SELECT co.Id, ec.Nombre AS Estado, co.Intentos
    FROM Correos co INNER JOIN EstadosCorreo ec ON ec.Id = co.EstadoCorreoId
    WHERE co.Id = @CorreoId;
END
GO

 
-- 13. DATOS INICIALES


INSERT INTO TiposIdentificacion (Nombre, Formato) VALUES
('Cédula de identidad', '0-0000-0000'),
('DNI', '00000000'),
('Pasaporte', NULL);

INSERT INTO EstadosUsuario (Id, Nombre, Descripcion, PermiteAcceso) VALUES
(1, 'Activo', 'Puede usar el sistema con normalidad', 1),
(2, 'Inactivo', 'Dado de baja (borrado lógico)', 0),
(3, 'Bloqueado', 'Bloqueado por intentos fallidos o por seguridad', 0),
(4, 'Suspendido', 'Suspendido temporalmente por un administrador', 0),
(5, 'PendienteVerificacion', 'Aún no confirma su correo', 0);

INSERT INTO EstadosHabitacion (Id, Nombre, PermiteReservar) VALUES
(1, 'Disponible', 1),
(2, 'Reservada', 1),
(3, 'Ocupada', 1),
(4, 'EnLimpieza', 1),
(5, 'EnMantenimiento', 0),
(6, 'Bloqueada', 0);

INSERT INTO EstadosReservacion (Id, Nombre, OcupaHabitacion) VALUES
(1, 'PendientePago', 1),
(2, 'Confirmada', 1),
(3, 'EnCurso', 1),
(4, 'Completada', 0),
(5, 'Cancelada', 0);

INSERT INTO EstadosPedido (Id, Nombre, Orden, PermiteModificar, PermiteCancelar, EsFinal) VALUES
(1, 'Pendiente', 1, 1, 1, 0),
(2, 'Confirmado', 2, 1, 1, 0),
(3, 'EnPreparacion', 3, 0, 0, 0),
(4, 'Listo', 4, 0, 0, 0),
(5, 'EnCamino', 5, 0, 0, 0),
(6, 'Entregado', 6, 0, 0, 1),
(7, 'Cancelado', 7, 0, 0, 1);

INSERT INTO MetodosPago (Nombre, RequiereReferencia) VALUES
('Efectivo', 0), ('Tarjeta', 1), ('Transferencia', 1);

INSERT INTO EstadosPago (Id, Nombre) VALUES
(1, 'Pendiente'),        -- sin abonos
(2, 'Parcial'),          -- tiene abonos pero queda saldo
(3, 'Pagado'),          -- saldo en cero
(4, 'Anulada');          -- reserva o pedido cancelado (cuenta cerrada)

INSERT INTO TiposCorreo (Id, Nombre, Descripcion) VALUES
(1, 'ConfirmacionReservacion', 'Se envía al crear o confirmar una reservación'),
(2, 'CancelacionReservacion', 'Aviso de reservación cancelada'),
(3, 'Factura', 'Factura electrónica después del check-out o del pago'),
(4, 'RecuperarContrasena', 'Enlace para cambiar la contraseña'),
(5, 'VerificacionCuenta', 'Enlace para confirmar el correo de una cuenta nueva'),
(6, 'RecordatorioLlegada', 'Recordatorio un día antes del check-in'),
(7, 'ConsultaCliente', 'Correo que escribe un cliente al hotel'),
(8, 'RespuestaConsulta', 'Respuesta del hotel a una consulta');

INSERT INTO EstadosCorreo (Id, Nombre) VALUES
(1, 'Pendiente'),        -- registrado, falta enviarlo
(2, 'Enviado'),
(3, 'Fallido'),          -- se agotaron los intentos
(4, 'Recibido'),         -- correo que llegó al hotel
(5, 'Respondido');       -- correo recibido que ya se contestó

INSERT INTO Parametros (Clave, Valor, TipoDato, Descripcion) VALUES
('NombreHotel', 'Hotel BD', 'Texto', 'Nombre comercial del hotel'),
('IVA', '16', 'Decimal', 'Porcentaje de impuesto (16 o 21)'),
('PorcentajeAnticipo', '30', 'Decimal', '% mínimo pagado para confirmar una reservación'),
('HoraCheckIn', '15:00', 'Hora', 'Hora desde la que se puede hacer check-in'),
('HoraCheckOut', '12:00', 'Hora', 'Hora límite de salida'),
('PorcentajeLateCheckout', '50', 'Decimal', '% de una noche que se cobra por salir tarde'),
('RecargoFinDeSemana', '15', 'Decimal', '% extra en las noches de viernes y sábado'),
('RecargoRoomService', '10', 'Decimal', '% extra por llevar el pedido a la habitación'),
('MaxNochesPorReserva', '30', 'Entero', 'Máximo de noches en una sola reservación'),
('PrefijoReservacion', 'HOT', 'Texto', 'Prefijo del código de reservación'),
('CorreoHotel', 'reservas@hotelbd.com', 'Texto', 'Correo desde el que se envían y reciben los mensajes'),
('MaxIntentosCorreo', '3', 'Entero', 'Intentos de envío antes de marcar un correo como Fallido'),
('HorasCancelacionSinCargo', '48', 'Entero', 'Cancelando con más horas de anticipación se reembolsa lo abonado'),
('MinutosTokenRecuperacion', '30', 'Entero', 'Vigencia del código para recuperar la contraseña'),
('HorasTokenVerificacion', '24', 'Entero', 'Vigencia del enlace para verificar el correo');

INSERT INTO Temporadas (Nombre, FechaInicio, FechaFin, PorcentajeRecargo) VALUES
('Navidad y Año Nuevo', '2026-12-15', '2027-01-06', 25),
('Semana Santa', '2027-03-21', '2027-03-28', 20),
('Verano', '2027-06-15', '2027-08-31', 15);

INSERT INTO Descuentos (Nombre, Codigo, Tipo, Valor, AplicaA, FechaInicio, FechaFin) VALUES
('Cliente frecuente', NULL, 'Porcentaje', 10, 'Ambos', NULL, NULL),
('Convenio empresarial', NULL, 'Porcentaje', 15, 'Hospedaje', NULL, NULL),
('Promoción de bienvenida', 'BIENVENIDA', 'MontoFijo', 20, 'Hospedaje', '2026-01-01', '2027-12-31'),
('Happy hour restaurante', 'HAPPY20', 'Porcentaje', 20, 'Restaurante', '2026-01-01', '2027-12-31');

-- ---------- Roles y permisos ----------
INSERT INTO Roles (Nombre, Descripcion) VALUES
('Admin', 'Usuarios, roles, catálogos, parámetros, reportes y auditoría'),
('Recepcionista', 'Clientes, habitaciones, reservaciones, check-in/out y pagos'),
('Cajero', 'Pagos, saldos y facturas'),
('PersonalCocina', 'Menú y estados de los pedidos'),
('Supervisor', 'Reportes operativos y reversos de pagos'),
('Auditor', 'Consulta de auditoría y reportes'),
('Huesped', 'Su perfil, sus reservaciones y sus pedidos');

INSERT INTO Permisos (Codigo) VALUES
('Usuarios.Gestionar'), ('Parametros.Editar'), ('Auditoria.Ver'),
('Clientes.Leer'), ('Clientes.Editar'),
('Habitaciones.Leer'), ('Habitaciones.Editar'), ('Habitaciones.CambiarEstado'),
('Reservaciones.Leer'), ('Reservaciones.Crear'), ('Reservaciones.Modificar'), ('Reservaciones.Cancelar'),
('Ajustes.Autorizar'),
('Estadias.CheckIn'), ('Estadias.CheckOut'), ('Cargos.Registrar'),
('Pagos.Leer'), ('Pagos.Registrar'), ('Pagos.Reversar'), ('Facturas.Ver'),
('Menu.Editar'), ('Pedidos.Gestionar'), ('Pedidos.Crear'),
('Reportes.Ver'), ('Descuentos.Gestionar'),
('Perfil.Propio'), ('Reservaciones.Propias'), ('Pedidos.Propios');

-- Admin tiene todos los permisos
INSERT INTO RolPermisos (RolId, PermisoId)
SELECT r.Id, p.Id FROM Roles r CROSS JOIN Permisos p WHERE r.Nombre = 'Admin';

INSERT INTO RolPermisos (RolId, PermisoId)
SELECT r.Id, p.Id
FROM (VALUES
    ('Recepcionista', 'Clientes.Leer'), ('Recepcionista', 'Clientes.Editar'), ('Recepcionista', 'Habitaciones.Leer'),
    ('Recepcionista', 'Reservaciones.Leer'), ('Recepcionista', 'Reservaciones.Crear'), ('Recepcionista', 'Reservaciones.Cancelar'),
    ('Recepcionista', 'Reservaciones.Modificar'), ('Recepcionista', 'Habitaciones.CambiarEstado'),
    ('Recepcionista', 'Estadias.CheckIn'), ('Recepcionista', 'Estadias.CheckOut'), ('Recepcionista', 'Cargos.Registrar'),
    ('Recepcionista', 'Pagos.Leer'), ('Recepcionista', 'Pagos.Registrar'), ('Recepcionista', 'Facturas.Ver'),
    ('Recepcionista', 'Pedidos.Crear'),
    ('Cajero', 'Pagos.Leer'), ('Cajero', 'Pagos.Registrar'), ('Cajero', 'Facturas.Ver'), ('Cajero', 'Reservaciones.Leer'),
    ('PersonalCocina', 'Menu.Editar'), ('PersonalCocina', 'Pedidos.Gestionar'),
    ('Supervisor', 'Reportes.Ver'), ('Supervisor', 'Pagos.Leer'), ('Supervisor', 'Pagos.Reversar'),
    ('Supervisor', 'Reservaciones.Leer'), ('Supervisor', 'Descuentos.Gestionar'),
    ('Supervisor', 'Ajustes.Autorizar'), ('Supervisor', 'Habitaciones.CambiarEstado'),
    ('Auditor', 'Auditoria.Ver'), ('Auditor', 'Reportes.Ver'), ('Auditor', 'Pagos.Leer'),
    ('Huesped', 'Perfil.Propio'), ('Huesped', 'Habitaciones.Leer'), ('Huesped', 'Reservaciones.Propias'), ('Huesped', 'Pedidos.Propios')
) m(Rol, Permiso)
INNER JOIN Roles r ON r.Nombre = m.Rol
INNER JOIN Permisos p ON p.Codigo = m.Permiso;

--  Clientes 
INSERT INTO Clientes (TipoIdentificacionId, NumeroIdentificacion, Nombres, Apellidos, Nacionalidad, Email, Telefono, FechaNacimiento, Preferencias, DescuentoId) VALUES
(1, '1-1234-0567', 'María', 'López Rivas', 'Costarricense', 'maria.lopez@correo.com', '8888-1111', '1992-04-18', 'Piso alto', NULL),
(3, 'X1234567', 'John', 'Smith', 'Estadounidense', 'john.smith@correo.com', '305-555-0101', '1985-11-11', NULL, NULL),
(2, '45879632', 'Ana', 'Herrera Castro', 'Peruana', 'ana.herrera@correo.com', '987-654-321', '1995-01-30', 'Cliente frecuente', 1);

--  Usuarios (uno por rol) 
-- Contraseña de todos: Hotel2026*  (hash BCrypt, se verifica con BCrypt.Net-Next)
DECLARE @Hash NVARCHAR(256) = '$2a$11$q6byd8XF0gGrri5HBQjUluPgzwiHKyySjijjxVhEjV.bk81J55g9C';

INSERT INTO Usuarios (TipoIdentificacionId, NumeroIdentificacion, NombreUsuario, Email, PasswordHash, Nombres, Apellidos, Telefono, ClienteId) VALUES
(1, '1-0901-0001', 'admin',       'admin@hotelbd.com',       @Hash, 'Administrador', 'General',   '2222-0001', NULL),
(1, '1-0901-0002', 'recepcion',   'recepcion@hotelbd.com',   @Hash, 'Laura',         'Martínez',  '2222-0002', NULL),
(1, '1-0901-0003', 'cajero',      'cajero@hotelbd.com',      @Hash, 'Pedro',         'Gómez',     '2222-0003', NULL),
(1, '1-0901-0004', 'cocina',      'cocina@hotelbd.com',      @Hash, 'Rosa',          'Hernández', '2222-0004', NULL),
(1, '1-0901-0005', 'supervisor',  'supervisor@hotelbd.com',  @Hash, 'Juan',         'Castillo',  '2222-0005', NULL),
(1, '1-0901-0006', 'auditor',    'auditor@hotelbd.com',     @Hash, 'Elena',         'Ramírez',   '2222-0006', NULL),
(1, '1-1234-0567', 'maria.lopez', 'maria.lopez@correo.com',  @Hash, 'María',         'López Rivas', '8888-1111', 1);

INSERT INTO UsuarioRoles (UsuarioId, RolId)
SELECT u.Id, r.Id
FROM (VALUES ('admin', 'Admin'), ('recepcion', 'Recepcionista'), ('cajero', 'Cajero'), ('cocina', 'PersonalCocina'),
             ('supervisor', 'Supervisor'), ('auditor', 'Auditor'), ('maria.lopez', 'Huesped')) m(Usuario, Rol)
INNER JOIN Usuarios u ON u.NombreUsuario = m.Usuario
INNER JOIN Roles r ON r.Nombre = m.Rol;

--  Habitaciones 
INSERT INTO TiposHabitacion (Nombre, Descripcion, CapacidadAdultos, CapacidadNinos, PrecioBasePorNoche, PersonasIncluidas, CargoPersonaAdicional) VALUES
('Simple', 'Una cama matrimonial', 1, 1, 55.00, 1, 15.00),
('Doble Estándar', 'Dos camas dobles', 2, 2, 85.00, 2, 20.00),
('Suite Ejecutiva', 'Sala de estar y cama king', 2, 1, 160.00, 2, 30.00),
('Suite Presidencial', 'Jacuzzi, comedor y vista al mar', 4, 2, 350.00, 2, 40.00),
('Familiar', 'Dos ambientes con cocineta', 4, 3, 140.00, 4, 18.00);

INSERT INTO Amenidades (Nombre) VALUES
('Aire acondicionado'), ('Smart TV'), ('WiFi'), ('Minibar'), ('Balcón'), ('Vista al mar'), ('Cama king'), ('Jacuzzi'), ('Cocineta');

-- el trigger guarda el estado inicial de cada habitación en la bitácora
INSERT INTO Habitaciones (Numero, Piso, TipoHabitacionId, EstadoHabitacionId) VALUES
('101', 1, 1, 1),
('102', 1, 1, 1),
('103', 1, 1, 5),             -- en mantenimiento
('201', 2, 2, 1),
('202', 2, 2, 1),
('203', 2, 2, 1),
('204', 2, 2, 6),             -- bloqueada
('301', 3, 5, 1),
('302', 3, 5, 4),             -- en limpieza
('401', 4, 3, 1),
('402', 4, 3, 1),
('501', 5, 4, 1);

-- fotos según el tipo: la principal (habitación) y la del baño
INSERT INTO HabitacionFotos (HabitacionId, Url, Descripcion, EsPrincipal, Orden)
SELECT h.Id, '/assets/habitaciones/' + f.Archivo + '.jpg', 'Habitación', 1, 1
FROM Habitaciones h
INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
INNER JOIN (VALUES ('Simple', 'simple'), ('Doble Estándar', 'doble'), ('Familiar', 'familiar'),
                   ('Suite Ejecutiva', 'suite-ejecutiva'), ('Suite Presidencial', 'suite-presidencial')) f(Tipo, Archivo)
        ON f.Tipo = t.Nombre
UNION ALL
SELECT h.Id, '/assets/habitaciones/' + f.Archivo + '-bano.jpg', 'Baño', 0, 2
FROM Habitaciones h
INNER JOIN TiposHabitacion t ON t.Id = h.TipoHabitacionId
INNER JOIN (VALUES ('Simple', 'simple'), ('Doble Estándar', 'doble'), ('Familiar', 'familiar'),
                   ('Suite Ejecutiva', 'suite-ejecutiva'), ('Suite Presidencial', 'suite-presidencial')) f(Tipo, Archivo)
        ON f.Tipo = t.Nombre;

-- amenidades según el tipo de habitación
INSERT INTO HabitacionAmenidades (HabitacionId, AmenidadId)
SELECT h.Id, a.Id
FROM (VALUES
    ('Simple', 'Aire acondicionado'), ('Simple', 'Smart TV'), ('Simple', 'WiFi'),
    ('Doble Estándar', 'Aire acondicionado'), ('Doble Estándar', 'Smart TV'), ('Doble Estándar', 'WiFi'), ('Doble Estándar', 'Minibar'),
    ('Familiar', 'Aire acondicionado'), ('Familiar', 'Smart TV'), ('Familiar', 'WiFi'), ('Familiar', 'Minibar'), ('Familiar', 'Cocineta'),
    ('Suite Ejecutiva', 'Aire acondicionado'), ('Suite Ejecutiva', 'Smart TV'), ('Suite Ejecutiva', 'WiFi'), ('Suite Ejecutiva', 'Minibar'),
    ('Suite Ejecutiva', 'Balcón'), ('Suite Ejecutiva', 'Vista al mar'), ('Suite Ejecutiva', 'Cama king'),
    ('Suite Presidencial', 'Aire acondicionado'), ('Suite Presidencial', 'Smart TV'), ('Suite Presidencial', 'WiFi'), ('Suite Presidencial', 'Minibar'),
    ('Suite Presidencial', 'Balcón'), ('Suite Presidencial', 'Vista al mar'), ('Suite Presidencial', 'Cama king'), ('Suite Presidencial', 'Jacuzzi')
) m(Tipo, Amenidad)
INNER JOIN TiposHabitacion t ON t.Nombre = m.Tipo
INNER JOIN Habitaciones h ON h.TipoHabitacionId = t.Id
INNER JOIN Amenidades a ON a.Nombre = m.Amenidad;

--  Restaurante 
INSERT INTO CategoriasPlatillo (Nombre) VALUES ('Desayunos'), ('Entradas'), ('Platos Fuertes'), ('Postres'), ('Bebidas'), ('Bar');

INSERT INTO Platillos (CategoriaId, Nombre, Descripcion, Precio, ImagenUrl, TiempoPreparacion, EsBebida) VALUES
(1, 'Desayuno Típico', 'Huevos, frijoles, plátano maduro y queso', 9.50, '/assets/menu/desayuno.jpg', 15, 0),
(1, 'Pancakes con Frutas', 'Con miel de maple y fruta fresca', 8.00, '/assets/menu/pancakes.jpg', 12, 0),
(2, 'Ceviche de Pescado', 'Pescado marinado en limón', 11.00, '/assets/menu/ceviche.jpg', 10, 0),
(2, 'Nachos Supremos', 'Con queso, carne y guacamole', 10.00, '/assets/menu/nachos.jpg', 12, 0),
(3, 'Filete a la Parrilla', 'Con papas y vegetales', 24.00, '/assets/menu/filete.jpg', 25, 0),
(3, 'Pasta Alfredo', 'Con pollo a la plancha', 15.00, '/assets/menu/pasta.jpg', 18, 0),
(3, 'Hamburguesa de la Casa', 'Con papas fritas', 13.00, '/assets/menu/hamburguesa.jpg', 15, 0),
(4, 'Tres Leches', 'Postre de la casa', 6.00, '/assets/menu/tres-leches.jpg', 5, 0),
(4, 'Brownie con Helado', 'Brownie tibio con vainilla', 6.50, '/assets/menu/brownie.jpg', 7, 0),
(5, 'Jugo Natural', 'Naranja, piña o sandía', 3.50, '/assets/menu/jugo.jpg', 5, 1),
(5, 'Café Americano', 'Café recién hecho', 2.50, '/assets/menu/cafe.jpg', 3, 1),
(6, 'Piña Colada', 'Ron, coco y piña', 8.00, '/assets/menu/pina-colada.jpg', 5, 1),
(6, 'Cerveza Nacional', 'Botella 355 ml', 3.00, '/assets/menu/cerveza.jpg', 1, 1);
GO