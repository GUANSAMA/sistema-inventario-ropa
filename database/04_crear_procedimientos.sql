USE InventarioRopaDB;
GO

-- ==========================================
-- Catálogos: solo lectura desde la aplicación
-- ==========================================

CREATE OR ALTER PROCEDURE sp_Categoria_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, Nombre, Activo
    FROM Categoria
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Talla_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTalla, Nombre, Activo
    FROM Talla
    WHERE Activo = 1
    ORDER BY IdTalla;
END
GO

CREATE OR ALTER PROCEDURE sp_Color_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdColor, Nombre, Activo
    FROM Color
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

-- ==========================================
-- Prenda
-- ==========================================

CREATE OR ALTER PROCEDURE sp_Prenda_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.IdPrenda, p.CodigoSKU, p.Nombre,
        p.CategoriaId, c.Nombre AS CategoriaNombre,
        p.TallaId, t.Nombre AS TallaNombre,
        p.ColorId, col.Nombre AS ColorNombre,
        p.Marca, p.Precio, p.Stock, p.StockMinimo, p.Activo
    FROM Prenda p
    INNER JOIN Categoria c   ON c.IdCategoria = p.CategoriaId
    INNER JOIN Talla t      ON t.IdTalla = p.TallaId
    INNER JOIN Color col    ON col.IdColor = p.ColorId
    ORDER BY p.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Prenda_ObtenerPorId
    @IdPrenda INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.IdPrenda, p.CodigoSKU, p.Nombre,
        p.CategoriaId, c.Nombre AS CategoriaNombre,
        p.TallaId, t.Nombre AS TallaNombre,
        p.ColorId, col.Nombre AS ColorNombre,
        p.Marca, p.Precio, p.Stock, p.StockMinimo, p.Activo
    FROM Prenda p
    INNER JOIN Categoria c   ON c.IdCategoria = p.CategoriaId
    INNER JOIN Talla t      ON t.IdTalla = p.TallaId
    INNER JOIN Color col    ON col.IdColor = p.ColorId
    WHERE p.IdPrenda = @IdPrenda;
END
GO

-- Búsqueda + filtros combinados (RF04). Todos los parámetros son
-- opcionales: si vienen NULL, ese filtro simplemente no se aplica.
CREATE OR ALTER PROCEDURE sp_Prenda_Buscar
    @Texto        VARCHAR(100) = NULL,
    @CategoriaId  INT          = NULL,
    @TallaId      INT          = NULL,
    @ColorId      INT          = NULL,
    @Activo       BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.IdPrenda, p.CodigoSKU, p.Nombre,
        p.CategoriaId, c.Nombre AS CategoriaNombre,
        p.TallaId, t.Nombre AS TallaNombre,
        p.ColorId, col.Nombre AS ColorNombre,
        p.Marca, p.Precio, p.Stock, p.StockMinimo, p.Activo
    FROM Prenda p
    INNER JOIN Categoria c   ON c.IdCategoria = p.CategoriaId
    INNER JOIN Talla t      ON t.IdTalla = p.TallaId
    INNER JOIN Color col    ON col.IdColor = p.ColorId
    WHERE (@Texto IS NULL OR p.CodigoSKU LIKE '%' + @Texto + '%' OR p.Nombre LIKE '%' + @Texto + '%')
      AND (@CategoriaId IS NULL OR p.CategoriaId = @CategoriaId)
      AND (@TallaId IS NULL OR p.TallaId = @TallaId)
      AND (@ColorId IS NULL OR p.ColorId = @ColorId)
      AND (@Activo IS NULL OR p.Activo = @Activo)
    ORDER BY p.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Prenda_Insertar
    @CodigoSKU    VARCHAR(20),
    @Nombre       VARCHAR(100),
    @CategoriaId  INT,
    @TallaId      INT,
    @ColorId      INT,
    @Marca        VARCHAR(50) = NULL,
    @Precio       DECIMAL(10,0),
    @Stock        INT,
    @StockMinimo  INT,
    @IdUsuario    INT,
    @IdPrendaNueva INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- El trigger usa este valor para identificar al usuario de la operación.
    BEGIN TRY
        EXEC sp_set_session_context @key = N'IdUsuario', @value = @IdUsuario;

        INSERT INTO Prenda (CodigoSKU, Nombre, CategoriaId, TallaId, ColorId, Marca, Precio, Stock, StockMinimo)
        VALUES (@CodigoSKU, @Nombre, @CategoriaId, @TallaId, @ColorId, @Marca, @Precio, @Stock, @StockMinimo);

        SET @IdPrendaNueva = SCOPE_IDENTITY();
        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
    END TRY
    BEGIN CATCH
        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_Prenda_Actualizar
    @IdPrenda     INT,
    @CodigoSKU    VARCHAR(20),
    @Nombre       VARCHAR(100),
    @CategoriaId  INT,
    @TallaId      INT,
    @ColorId      INT,
    @Marca        VARCHAR(50) = NULL,
    @Precio       DECIMAL(10,0),
    @Stock        INT,
    @StockMinimo  INT,
    @IdUsuario    INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        EXEC sp_set_session_context @key = N'IdUsuario', @value = @IdUsuario;

        UPDATE Prenda
        SET CodigoSKU   = @CodigoSKU,
            Nombre      = @Nombre,
            CategoriaId = @CategoriaId,
            TallaId     = @TallaId,
            ColorId     = @ColorId,
            Marca       = @Marca,
            Precio      = @Precio,
            Stock       = @Stock,
            StockMinimo = @StockMinimo
        WHERE IdPrenda = @IdPrenda;

        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
    END TRY
    BEGIN CATCH
        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_Prenda_Desactivar
    @IdPrenda   INT,
    @IdUsuario  INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        EXEC sp_set_session_context @key = N'IdUsuario', @value = @IdUsuario;

        UPDATE Prenda
        SET Activo = 0
        WHERE IdPrenda = @IdPrenda
          AND Activo = 1;

        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
    END TRY
    BEGIN CATCH
        EXEC sp_set_session_context @key = N'IdUsuario', @value = NULL;
        THROW;
    END CATCH
END
GO

-- ==========================================
-- Usuarios
-- ==========================================

CREATE OR ALTER PROCEDURE sp_Usuario_ObtenerPorNombre
    @NombreUsuario VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, NombreUsuario, NombreCompleto, PasswordHash, PasswordSalt, Rol, Activo
    FROM UsuarioSistema
    WHERE NombreUsuario = @NombreUsuario;
END
GO

CREATE OR ALTER PROCEDURE sp_Usuario_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, NombreUsuario, NombreCompleto, Rol, Activo
    FROM UsuarioSistema
    ORDER BY NombreCompleto;
END
GO

-- Alta única del primer Administrador. El bloqueo de tabla dentro de la
-- transacción impide que dos primeras configuraciones creen cuentas a la vez.
CREATE OR ALTER PROCEDURE sp_Usuario_CrearAdministradorInicial
    @NombreUsuario  VARCHAR(50),
    @NombreCompleto VARCHAR(100),
    @PasswordHash   VARBINARY(64),
    @PasswordSalt   VARBINARY(32)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM UsuarioSistema WITH (TABLOCKX, HOLDLOCK))
    BEGIN
        ROLLBACK TRANSACTION;
        ;THROW 51003, 'El Administrador inicial ya fue creado.', 1;
    END;

    INSERT INTO UsuarioSistema (NombreUsuario, NombreCompleto, PasswordHash, PasswordSalt, Rol)
    VALUES (@NombreUsuario, @NombreCompleto, @PasswordHash, @PasswordSalt, 'Administrador');

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE sp_Usuario_Registrar
    @NombreUsuario  VARCHAR(50),
    @NombreCompleto VARCHAR(100),
    @PasswordHash   VARBINARY(64),
    @PasswordSalt   VARBINARY(32),
    @Rol            VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO UsuarioSistema (NombreUsuario, NombreCompleto, PasswordHash, PasswordSalt, Rol)
    VALUES (@NombreUsuario, @NombreCompleto, @PasswordHash, @PasswordSalt, @Rol);
END
GO

CREATE OR ALTER PROCEDURE sp_Usuario_Desactivar
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE UsuarioSistema
    SET Activo = 0
    WHERE IdUsuario = @IdUsuario
      AND Activo = 1;
END
GO

-- ==========================================
-- Auditoría (solo lectura desde la aplicación)
-- ==========================================

CREATE OR ALTER PROCEDURE sp_Auditoria_Listar
    @FechaInicio DATETIME2 = NULL,
    @FechaFin    DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        a.IdAuditoria, a.IdPrenda, p.CodigoSKU, p.Nombre AS PrendaNombre,
        a.IdUsuario, u.NombreUsuario, a.Operacion, a.FechaHora,
        a.ValoresAnteriores, a.ValoresNuevos
    FROM AuditoriaInventario a
    INNER JOIN Prenda p         ON p.IdPrenda = a.IdPrenda
    INNER JOIN UsuarioSistema u ON u.IdUsuario = a.IdUsuario
    WHERE (@FechaInicio IS NULL OR a.FechaHora >= @FechaInicio)
      AND (@FechaFin IS NULL OR a.FechaHora <= @FechaFin)
    ORDER BY a.FechaHora DESC;
END
GO
