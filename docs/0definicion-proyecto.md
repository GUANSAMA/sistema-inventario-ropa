# Fase 0: Definición inicial del proyecto

## Sistema de Inventario de Ropa para Tienda Moda Urbana

**Asignatura:** Programación Segura  
**Estudiante:** Guillermo Sanchez  
**Docente:** Ricardo Arturo Zamorano Pontiggia  
**Institución y sede:** AIEP San Joaquín  
**Proyecto:** Sistema de Inventario de Ropa  
**Fecha:** 23-09-2026  
**Repositorio:** https://github.com/GUANSAMA/sistema-inventario-ropa  
**Versión del documento:** 0.1  

## 1. Propósito del proyecto

El proyecto consiste en diseñar e implementar un sistema de escritorio para administrar el inventario de una tienda ficticia denominada **Tienda Moda Urbana**. La aplicación permitirá registrar, consultar, modificar y desactivar prendas de vestir. También conservará una trazabilidad de los cambios realizados y generará reportes en formato PDF.

La solución se desarrollará con **C# y Windows Forms** sobre **.NET 10**, utilizará **SQL Server Express** como sistema gestor de base de datos y organizará el código en cuatro capas: UI, BLL, DAL y Entities.

## 2. Problemática preliminar

Tienda Moda Urbana necesita controlar prendas que varían por categoría, talla, color, marca y cantidad disponible. Si esta información se administra de forma manual o en registros poco estructurados, aumentan los riesgos de duplicar productos, ingresar precios inválidos, perder el control del stock y no saber quién modificó un registro.

El sistema propuesto centralizará la información en una base de datos relacional. La aplicación aplicará validaciones antes de guardar los datos, utilizará procedimientos almacenados para las operaciones principales y conservará un historial de auditoría para respaldar la trazabilidad.

## 3. Alcance

El sistema incluirá el mantenimiento de prendas, la gestión de catálogos básicos y la consulta del inventario. Una prenda tendrá un código SKU único, una categoría, una talla, un color, una marca, un precio en pesos chilenos, un stock actual, un stock mínimo y un estado activo o inactivo.

La aplicación permitirá registrar, listar, buscar, actualizar y desactivar prendas. También permitirá filtrar el inventario por categoría, talla, color y estado. Cuando el stock sea igual o inferior al stock mínimo, el sistema podrá mostrar una alerta de bajo inventario.

La trazabilidad registrará las operaciones relevantes sobre las prendas. Se generarán dos reportes PDF independientes: uno para el inventario y otro para el historial de auditoría.

## 4. Exclusiones iniciales

La primera versión no incluirá ventas, compras a proveedores, pagos, facturación electrónica, integración con tiendas en línea ni control de bodegas múltiples. Tampoco se implementará un sistema contable.

El control de usuarios incluirá inicio de sesión con los roles **Administrador** y **Operador de Bodega**. El Administrador podrá consultar auditoría y gestionar usuarios; ambos roles podrán consultar, registrar y actualizar prendas, mientras que solo el Administrador podrá desactivarlas. La tabla `UsuarioSistema` identificará al usuario que ejecuta cada operación.

## 5. Actores

### Administrador

El Administrador podrá iniciar sesión, consultar el inventario, registrar nuevas prendas, modificar datos, desactivar prendas, consultar la auditoría y generar reportes.

### Operador de Bodega

El Operador de Bodega podrá iniciar sesión, consultar, registrar y actualizar prendas. No podrá gestionar usuarios ni consultar la auditoría.

### Sistema

El Sistema validará los datos, ejecutará los procedimientos almacenados, registrará la auditoría y generará los reportes PDF. No es una persona, pero se modelará como actor secundario en los casos de uso donde corresponda.

## 6. Entidades preliminares

| Entidad | Propósito |
|---|---|
| `Prenda` | Almacena la información principal de cada artículo de ropa. |
| `Categoria` | Clasifica prendas como polera, pantalón, chaqueta o vestido. |
| `Talla` | Define tallas disponibles, por ejemplo S, M, L o XL. |
| `Color` | Mantiene el catálogo de colores. |
| `UsuarioSistema` | Identifica a los usuarios y sus roles. |
| `AuditoriaInventario` | Registra operaciones y cambios sobre las prendas. |

## 7. Relaciones preliminares

Una categoría puede estar asociada a muchas prendas, mientras que cada prenda pertenece a una categoría. La misma regla se aplica a las entidades `Talla` y `Color`.

Un usuario puede generar muchos registros de auditoría. Cada registro de auditoría identificará la prenda afectada, el tipo de operación, la fecha y hora, el usuario y los valores anteriores y nuevos cuando correspondan.

La relación preliminar puede expresarse así:

```text
Categoria       1 ──── N Prenda
Talla           1 ──── N Prenda
Color           1 ──── N Prenda
UsuarioSistema  1 ──── N AuditoriaInventario
Prenda          1 ──── N AuditoriaInventario
```

## 8. Datos técnicos confirmados

| Elemento | Decisión |
|---|---|
| Sistema operativo | Windows |
| Editor | Visual Studio Code |
| Lenguaje | C# |
| SDK disponible | .NET SDK 10.0.400 |
| Framework objetivo | `net10.0-windows` |
| Interfaz | Windows Forms |
| Base de datos | SQL Server Express |
| Instancia | `SQLEXPRESS` |
| Autenticación | Windows / integrada |
| Base propuesta | `InventarioRopaDB` |
| UML | PlantUML |
| Moneda | Peso chileno sin decimales |
| Tipo de precio | `DECIMAL(10,0)` |
| Control de versiones | Git y GitHub |
| Repositorio | `https://github.com/GUANSAMA/sistema-inventario-ropa` |

La aplicación usa por defecto la instancia local `SQLEXPRESS` con autenticación integrada de Windows. La cadena se puede reemplazar mediante la variable de entorno `INVENTARIO_ROPA_SQLSERVER`; no se guardan contraseñas en el repositorio.

## 9. Arquitectura propuesta

La aplicación seguirá una arquitectura de cuatro capas:

```text
[ UI - Windows Forms ]
          │
          ▼
[ BLL - Reglas de negocio y validaciones ]
          │
          ▼
[ DAL - Conexión y procedimientos almacenados ]
          │
          ▼
[ SQL Server - Tablas, integridad y auditoría ]

[ Entities - Entidades y DTO ]
```

La capa UI recibirá las acciones del usuario y mostrará los resultados. La capa BLL verificará reglas como SKU obligatorio, precio positivo y stock no negativo. La capa DAL abrirá conexiones y ejecutará procedimientos almacenados. La capa Entities contendrá las clases que representan los datos del dominio y los objetos de transferencia.

## 10. Estructura inicial del repositorio

```text
sistema-inventario-ropa/
├── docs/
│   ├── 0definicion-proyecto.md
│   ├── 1requerimientos.md
│   ├── 2casos_usos.md
│   ├── 3diagrama_clases.md
│   ├── 4diagrama_secuencia.md
│   └── (documentos y diagramas del proyecto)
├── database/
│   ├── 01_crear_base_datos.sql
│   ├── 02_crear_tablas.sql
│   ├── 03_insertar_datos_prueba.sql
│   ├── 04_crear_procedimientos.sql
│   └── 05_crear_triggers_auditoria.sql
├── src/
│   ├── InventarioRopa.sln
│   ├── InventarioRopa.UI/
│   ├── InventarioRopa.BLL/
│   ├── InventarioRopa.DAL/
│   └── InventarioRopa.Entities/
├── README.md
└── .gitignore
```

## 11. Plan de trabajo

### Fase 1: Análisis y documentación (borrador preparado; revisar consistencia)

El borrador documenta la problemática, el alcance, los objetivos, los requisitos funcionales y no funcionales, la tríada, los controles de seguridad y criterios iniciales de aceptación.

### Fase 2: Modelado UML y diseño relacional (diagramas y modelo inicial preparados)

Ya hay diagramas de casos de uso, clases y secuencia, además del modelo relacional y su normalización. Falta revisar la alineación final entre estos artefactos.

### Fase 3: Base de datos (scripts iniciales preparados; validar en SQL Server)

Los scripts iniciales de tablas, restricciones, datos de prueba, procedimientos y triggers están preparados. Falta ejecutarlos y verificarlos en SQL Server.

### Fase 4: Aplicación por capas (primera implementación preparada)

La solución ya contiene cuatro proyectos para UI, BLL, DAL y Entities. Incluye entidades, conexión configurable, llamadas a procedimientos de prendas, validaciones, inicio de sesión, gestión de usuarios, consulta de auditoría y reportes PDF. Falta compilar y comprobar los flujos con SQL Server.

### Fase 5: Auditoría y reportes (primera implementación preparada)

La aplicación ya incluye consulta de auditoría y reportes PDF independientes de inventario e historial. Falta verificarlos con datos de SQL Server.

### Fase 6: Validación y entrega (pendiente)

Se verificarán los casos de uso, las validaciones, los errores de conexión, la ejecución de procedimientos, la auditoría, los reportes y la coherencia general. Finalmente se actualizarán el README y el historial de commits.

## 12. Primeros commits recomendados

```text
docs: definir alcance inicial del sistema
docs: documentar actores y entidades preliminares
docs: agregar estructura del repositorio
docs: agregar problemática y objetivos
docs: agregar requerimientos funcionales e ISO 27001
docs: agregar diagramas UML
database: crear modelo relacional y tablas
database: agregar procedimientos almacenados
database: agregar auditoría y triggers
src: crear solución Windows Forms por capas
src: implementar mantenedor de prendas
src: agregar validaciones y manejo de errores
src: agregar reportes PDF
docs: agregar pruebas y actualizar README
```

## 13. Información pendiente de confirmación

Antes de cerrar la Fase 1 se debe confirmar con el profesor el formato final del informe, la plantilla institucional, la norma de citación, la edición de ISO/IEC 27001 que debe utilizarse, el alcance obligatorio del login, la obligatoriedad de los reportes PDF y el tratamiento de los movimientos de inventario.

También deben completarse los datos de portada, como el nombre completo del estudiante, el docente, la institución, la sede y la fecha oficial de entrega.

## 14. Estado y próximos pasos

La definición, los requerimientos, los diagramas, el modelo relacional, los scripts y una primera versión de la aplicación están preparados. Queda pendiente ejecutar la base de datos, compilar y verificar los flujos antes de dar el proyecto por terminado.

## Referencias

[1]: Guía de Trabajo Práctico Integrador y Rúbrica de Evaluación, PDF entregado por el docente.
[2]: https://github.com/GUANSAMA/sistema-inventario-ropa "Repositorio GitHub del proyecto"
