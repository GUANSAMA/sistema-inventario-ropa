USE InventarioRopaDB;
GO

-- ==========================================
-- Trigger de INSERT
-- ==========================================
CREATE OR ALTER TRIGGER trg_Prenda_Insertar
ON Prenda
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdUsuario INT = TRY_CAST(SESSION_CONTEXT(N'IdUsuario') AS INT);

    IF @IdUsuario IS NULL OR NOT EXISTS (
        SELECT 1 FROM UsuarioSistema WHERE IdUsuario = @IdUsuario AND Activo = 1
    )
    BEGIN
        ;THROW 51001, 'La auditoría requiere un IdUsuario activo en SESSION_CONTEXT.', 1;
    END;

    INSERT INTO AuditoriaInventario (IdPrenda, IdUsuario, Operacion, ValoresNuevos)
    SELECT
        i.IdPrenda,
        @IdUsuario,
        'INSERT',
        (SELECT i2.* FROM Prenda i2 WHERE i2.IdPrenda = i.IdPrenda FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted i;
END
GO

-- ==========================================
-- Trigger de UPDATE
-- Distingue una edición normal de una desactivación.
-- ==========================================
CREATE OR ALTER TRIGGER trg_Prenda_Actualizar
ON Prenda
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdUsuario INT = TRY_CAST(SESSION_CONTEXT(N'IdUsuario') AS INT);

    IF @IdUsuario IS NULL OR NOT EXISTS (
        SELECT 1 FROM UsuarioSistema WHERE IdUsuario = @IdUsuario AND Activo = 1
    )
    BEGIN
        ;THROW 51002, 'La auditoría requiere un IdUsuario activo en SESSION_CONTEXT.', 1;
    END;

    INSERT INTO AuditoriaInventario (IdPrenda, IdUsuario, Operacion, ValoresAnteriores, ValoresNuevos)
    SELECT
        d.IdPrenda,
        @IdUsuario,
        CASE WHEN d.Activo = 1 AND i.Activo = 0 THEN 'DESACTIVAR' ELSE 'UPDATE' END,
        (SELECT d2.* FROM deleted d2 WHERE d2.IdPrenda = d.IdPrenda FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT i2.* FROM inserted i2 WHERE i2.IdPrenda = d.IdPrenda FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM deleted d
    INNER JOIN inserted i ON i.IdPrenda = d.IdPrenda;
END
GO
