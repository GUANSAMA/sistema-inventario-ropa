# Sistema de Inventario de Ropa

Aplicación de escritorio en C# Windows Forms para administrar el inventario de Tienda Moda Urbana. El repositorio separa el análisis (`docs`), los scripts SQL (`database`) y la aplicación (`src`).

## Estado actual

La documentación y los scripts de base de datos están preparados. La aplicación abre directamente, sin pedir credenciales, y permite gestionar prendas, catálogos, auditoría y reportes PDF. Para conservar la auditoría, al iniciar selecciona un usuario activo de la base de datos (preferentemente un Administrador) y registra las operaciones con esa identidad. Solo muestra la configuración inicial de Administrador si no existe ningún usuario activo.

## Requisitos de desarrollo

- Windows 10 o posterior.
- .NET SDK 10.
- SQL Server Express con una instancia local `SQLEXPRESS`.
- Visual Studio Code o Visual Studio.

## Preparar la base de datos

Ejecuta en SQL Server, en este orden, cada archivo de `database`:

1. `01_crear_base_datos.sql`
2. `02_crear_tablas.sql`
3. `03_insertar_datos_prueba.sql`
4. `04_crear_procedimientos.sql`
5. `05_crear_triggers_auditoria.sql`

Los scripts no incluyen contraseñas ni hashes de ejemplo. Si la base no tiene usuarios activos, la aplicación ofrece una configuración inicial de Administrador mediante `sp_Usuario_CrearAdministradorInicial` y luego abre directamente el inventario.

## Abrir la solución

Abre `src/InventarioRopa.sln` en Visual Studio o Visual Studio Code. También puedes iniciar la aplicación desde una terminal con `dotnet run --project src/InventarioRopa.UI/InventarioRopa.UI.csproj`. No se solicita inicio de sesión; la aplicación usa un usuario activo de `UsuarioSistema` para asociar las operaciones de auditoría. Si hay un Administrador activo, se usa esa cuenta para conservar las funciones administrativas.

La conexión predeterminada utiliza la instancia local `SQLEXPRESS` y autenticación integrada de Windows. Para conectarse a otra instancia, configura la variable de entorno `INVENTARIO_ROPA_SQLSERVER` con una cadena apropiada para tu entorno. No guardes contraseñas reales en el repositorio.

## Capas

- `InventarioRopa.UI`: formularios Windows Forms.
- `InventarioRopa.BLL`: validaciones y reglas de negocio.
- `InventarioRopa.DAL`: conexión y llamadas a procedimientos almacenados.
- `InventarioRopa.Entities`: entidades y DTO.
