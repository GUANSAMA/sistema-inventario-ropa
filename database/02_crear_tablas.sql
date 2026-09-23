USE InventarioRopaDB;
GO

-- ==========================================
-- Catálogos (sin dependencias)
-- ==========================================

CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) NOT NULL,
    Nombre      VARCHAR(50)       NOT NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT (1),
    CONSTRAINT PK_Categoria PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE Talla (
    IdTalla INT IDENTITY(1,1) NOT NULL,
    Nombre  VARCHAR(10)       NOT NULL,
    Activo  BIT               NOT NULL CONSTRAINT DF_Talla_Activo DEFAULT (1),
    CONSTRAINT PK_Talla PRIMARY KEY (IdTalla),
    CONSTRAINT UQ_Talla_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE Color (
    IdColor INT IDENTITY(1,1) NOT NULL,
    Nombre  VARCHAR(30)       NOT NULL,
    Activo  BIT               NOT NULL CONSTRAINT DF_Color_Activo DEFAULT (1),
    CONSTRAINT PK_Color PRIMARY KEY (IdColor),
    CONSTRAINT UQ_Color_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE UsuarioSistema (
    IdUsuario      INT IDENTITY(1,1) NOT NULL,
    NombreUsuario  VARCHAR(50)       NOT NULL,
    NombreCompleto VARCHAR(100)      NOT NULL,
    PasswordHash   VARBINARY(64)     NOT NULL,
    PasswordSalt   VARBINARY(32)     NOT NULL,
    Rol            VARCHAR(20)       NOT NULL,
    Activo         BIT               NOT NULL CONSTRAINT DF_UsuarioSistema_Activo DEFAULT (1),
    CONSTRAINT PK_UsuarioSistema PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_UsuarioSistema_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT CK_UsuarioSistema_Rol CHECK (Rol IN ('Administrador', 'Operador de Bodega'))
);
GO

-- ==========================================
-- Prenda (depende de Categoria, Talla, Color)
-- ==========================================

CREATE TABLE Prenda (
    IdPrenda     INT IDENTITY(1,1) NOT NULL,
    CodigoSKU    VARCHAR(20)       NOT NULL,
    Nombre       VARCHAR(100)      NOT NULL,
    CategoriaId  INT               NOT NULL,
    TallaId      INT               NOT NULL,
    ColorId      INT               NOT NULL,
    Marca        VARCHAR(50)       NULL,
    Precio       DECIMAL(10,0)     NOT NULL,
    Stock        INT               NOT NULL CONSTRAINT DF_Prenda_Stock DEFAULT (0),
    StockMinimo  INT               NOT NULL CONSTRAINT DF_Prenda_StockMinimo DEFAULT (0),
    Activo       BIT               NOT NULL CONSTRAINT DF_Prenda_Activo DEFAULT (1),
    CONSTRAINT PK_Prenda PRIMARY KEY (IdPrenda),
    CONSTRAINT UQ_Prenda_CodigoSKU UNIQUE (CodigoSKU),
    CONSTRAINT FK_Prenda_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categoria (IdCategoria) ON DELETE NO ACTION,
    CONSTRAINT FK_Prenda_Talla FOREIGN KEY (TallaId) REFERENCES Talla (IdTalla) ON DELETE NO ACTION,
    CONSTRAINT FK_Prenda_Color FOREIGN KEY (ColorId) REFERENCES Color (IdColor) ON DELETE NO ACTION,
    CONSTRAINT CK_Prenda_Precio CHECK (Precio > 0),
    CONSTRAINT CK_Prenda_Stock CHECK (Stock >= 0),
    CONSTRAINT CK_Prenda_StockMinimo CHECK (StockMinimo >= 0)
);
GO

-- ==========================================
-- AuditoriaInventario (depende de Prenda y UsuarioSistema)
-- ==========================================

CREATE TABLE AuditoriaInventario (
    IdAuditoria        BIGINT IDENTITY(1,1) NOT NULL,
    IdPrenda           INT                  NOT NULL,
    IdUsuario          INT                  NOT NULL,
    Operacion          VARCHAR(15)          NOT NULL,
    FechaHora          DATETIME2            NOT NULL CONSTRAINT DF_AuditoriaInventario_FechaHora DEFAULT (SYSDATETIME()),
    ValoresAnteriores  NVARCHAR(MAX)        NULL,
    ValoresNuevos      NVARCHAR(MAX)        NULL,
    CONSTRAINT PK_AuditoriaInventario PRIMARY KEY (IdAuditoria),
    CONSTRAINT FK_AuditoriaInventario_Prenda FOREIGN KEY (IdPrenda) REFERENCES Prenda (IdPrenda) ON DELETE NO ACTION,
    CONSTRAINT FK_AuditoriaInventario_Usuario FOREIGN KEY (IdUsuario) REFERENCES UsuarioSistema (IdUsuario) ON DELETE NO ACTION,
    CONSTRAINT CK_AuditoriaInventario_Operacion CHECK (Operacion IN ('INSERT', 'UPDATE', 'DESACTIVAR'))
);
GO