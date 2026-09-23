USE InventarioRopaDB;
GO

-- ==========================================
-- Catálogos
-- ==========================================

INSERT INTO Categoria (Nombre) VALUES
    ('Poleras'), ('Pantalones'), ('Chaquetas'),
    ('Polerones'), ('Vestidos'), ('Accesorios');
GO

INSERT INTO Talla (Nombre) VALUES
    ('XS'), ('S'), ('M'), ('L'), ('XL'), ('XXL'), ('Única');
GO

INSERT INTO Color (Nombre) VALUES
    ('Negro'), ('Blanco'), ('Gris'), ('Azul'),
    ('Rojo'), ('Verde'), ('Beige'), ('Café');
GO

-- ==========================================
-- Alta inicial del Administrador
-- ==========================================
-- No se insertan hashes de relleno ni credenciales compartidas en este
-- script. En el primer inicio, la aplicación debe detectar que no existen
-- usuarios y ofrecer un alta inicial de Administrador. La aplicación genera
-- hash y salt con PBKDF2 y llama a sp_Usuario_CrearAdministradorInicial. Después de crear
-- el Administrador, el alta inicial debe quedar deshabilitada; las cuentas
-- siguientes se crean desde la función de gestión de usuarios.
-- El IdUsuario creado debe enviarse a los procedimientos de prendas para
-- que los triggers puedan atribuir correctamente cada cambio.
GO
-- ==========================================
-- Prendas de ejemplo (18 filas)
-- Usa subconsultas por Nombre para no depender de que los IDENTITY
-- de los catálogos empiecen exactamente en 1.
-- ==========================================

INSERT INTO Prenda (CodigoSKU, Nombre, CategoriaId, TallaId, ColorId, Marca, Precio, Stock, StockMinimo)
VALUES
('POL-NEG-S',  'Polera básica cuello redondo',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Poleras'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'S'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'Urbana Co.', 8990, 25, 5),

('POL-BLA-M',  'Polera básica cuello redondo',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Poleras'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'M'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Blanco'),
    'Urbana Co.', 8990, 30, 5),

('POL-GRI-L',  'Polera oversize',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Poleras'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'L'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Gris'),
    'StreetLab', 10990, 18, 4),

('PAN-NEG-M',  'Pantalón jogger',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Pantalones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'M'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'StreetLab', 15990, 20, 4),

('PAN-AZU-L',  'Pantalón cargo',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Pantalones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'L'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Azul'),
    'Urbana Co.', 17990, 15, 3),

('PAN-BEI-XL', 'Pantalón cargo',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Pantalones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'XL'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Beige'),
    'Urbana Co.', 17990, 10, 3),

('CHA-NEG-M',  'Chaqueta bomber',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Chaquetas'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'M'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'NorthWear', 29990, 12, 3),

('CHA-VER-L',  'Chaqueta cortavientos',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Chaquetas'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'L'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Verde'),
    'NorthWear', 24990, 9, 2),

('CHA-GRI-XL', 'Chaqueta de jean',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Chaquetas'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'XL'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Gris'),
    'StreetLab', 26990, 7, 2),

('POR-NEG-M',  'Polerón con capucha',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Polerones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'M'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'Urbana Co.', 19990, 22, 5),

('POR-GRI-L',  'Polerón con capucha',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Polerones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'L'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Gris'),
    'Urbana Co.', 19990, 20, 5),

('POR-AZU-XL', 'Polerón oversize',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Polerones'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'XL'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Azul'),
    'StreetLab', 21990, 14, 3),

('VES-NEG-S',  'Vestido corto casual',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Vestidos'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'S'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'Flora', 22990, 11, 3),

('VES-ROJ-M',  'Vestido midi',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Vestidos'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'M'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Rojo'),
    'Flora', 25990, 8, 2),

('VES-BLA-L',  'Vestido de verano',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Vestidos'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'L'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Blanco'),
    'Flora', 20990, 13, 3),

('ACC-NEG-UN', 'Gorro de lana',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Accesorios'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'Única'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Negro'),
    'NorthWear', 5990, 40, 8),

('ACC-CAF-UN', 'Cinturón de cuero',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Accesorios'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'Única'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Café'),
    'StreetLab', 7990, 16, 4),

('ACC-GRI-UN', 'Bufanda de punto',
    (SELECT IdCategoria FROM Categoria WHERE Nombre = 'Accesorios'),
    (SELECT IdTalla FROM Talla WHERE Nombre = 'Única'),
    (SELECT IdColor FROM Color WHERE Nombre = 'Gris'),
    'NorthWear', 6990, 2, 5);
GO
