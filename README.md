# Sistema de Inventario de Ropa

Aplicación de escritorio en C# Windows Forms para administrar el inventario de Tienda Moda Urbana. El repositorio separa el análisis (`docs`), los scripts SQL (`database`) y la aplicación (`src`).

## Estado actual

La documentación y los scripts de base de datos están preparados. La aplicación incluye el alta inicial segura del Administrador, inicio de sesión, gestión de prendas, catálogos, auditoría y reportes PDF. La conexión real y la compilación todavía deben verificarse en el equipo de desarrollo.

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

En el primer inicio, la aplicación deberá crear el Administrador mediante `sp_Usuario_CrearAdministradorInicial`; los scripts no incluyen contraseñas ni hashes de ejemplo.

## Abrir la solución

Abre `src/InventarioRopa.sln` en Visual Studio o Visual Studio Code. También puedes iniciar la aplicación desde una terminal con `dotnet run --project src/InventarioRopa.UI/InventarioRopa.UI.csproj`. La primera ejecución solicitará crear la cuenta Administrador.

La conexión predeterminada utiliza la instancia local `SQLEXPRESS` y autenticación integrada de Windows. Para conectarse a otra instancia, configura la variable de entorno `INVENTARIO_ROPA_SQLSERVER` con una cadena apropiada para tu entorno. No guardes contraseñas reales en el repositorio.

## Capas

- `InventarioRopa.UI`: formularios Windows Forms.
- `InventarioRopa.BLL`: validaciones y reglas de negocio.
- `InventarioRopa.DAL`: conexión y llamadas a procedimientos almacenados.
- `InventarioRopa.Entities`: entidades y DTO.
