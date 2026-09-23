# Informe Técnico y Requerimientos — Fase 1

## Sistema de Inventario de Ropa para Tienda Moda Urbana

**Asignatura:** Programación Segura  
**Estudiante:** Guillermo Sanchez  
**Docente:** Ricardo Arturo Zamorano Pontiggia  
**Institución y sede:** AIEP San Joaquín  
**Fecha:** 23-09-2026  
**Repositorio:** [sistema-inventario-ropa](https://github.com/GUANSAMA/sistema-inventario-ropa )

---

## 1. Descripción y contexto del problema

La tienda ficticia **Tienda Moda Urbana** necesita administrar de manera centralizada el inventario de sus prendas de vestir. El catálogo contiene productos que se diferencian por categoría, talla, color, marca, precio y cantidad disponible.

La administración manual o desorganizada de estos datos puede provocar registros duplicados, precios incorrectos, cantidades negativas, referencias inválidas y pérdida de información histórica. Además, si no existe un registro de auditoría, resulta difícil identificar quién modificó una prenda, cuándo realizó el cambio y cuáles eran los valores anteriores.

Para resolver esta situación se desarrollará una aplicación de escritorio en C# Windows Forms, conectada a SQL Server Express. El sistema se organizará mediante una arquitectura de cuatro capas: UI, BLL, DAL y Entities. Las operaciones principales de la base de datos se ejecutarán mediante procedimientos almacenados y los cambios críticos se registrarán mediante triggers de auditoría.

## 2. Delimitación de la problemática

El sistema se enfocará en la gestión interna del inventario de ropa. Permitirá administrar prendas, consultar catálogos relacionados, controlar el stock actual, aplicar validaciones, registrar trazabilidad y generar reportes PDF.

El proyecto no incluirá punto de venta, facturación electrónica, pagos, gestión de clientes, comercio electrónico, remuneraciones, compras a proveedores ni administración de múltiples sucursales.

El alcance inicial no contempla una tabla independiente de movimientos de stock. El sistema mantendrá el stock actual en `Prenda` y registrará sus modificaciones mediante `AuditoriaInventario`. Las entradas, salidas, devoluciones y transferencias podrán ser una ampliación futura.

## 3. Justificación

La solución permitirá reducir errores en la administración del inventario y mejorar la trazabilidad de las operaciones. La separación en capas facilitará el mantenimiento del código y evitará que la interfaz acceda directamente a la base de datos.

El uso de procedimientos almacenados y consultas parametrizadas contribuirá a disminuir el riesgo de inyección SQL. Las validaciones en la interfaz, en la lógica de negocio y en la base de datos permitirán mantener la integridad de la información.

La auditoría mediante triggers permitirá registrar automáticamente las operaciones realizadas sobre las prendas. Los reportes PDF facilitarán la revisión del estado del inventario y del historial de cambios.

## 4. Objetivo general

Desarrollar e implementar una aplicación de escritorio segura en C# .NET 10 Windows Forms, conectada a SQL Server Express, que permita gestionar y auditar el inventario de ropa de Tienda Moda Urbana mediante una arquitectura de cuatro capas.

## 5. Objetivos específicos

1. Diseñar una base de datos relacional normalizada para almacenar prendas, catálogos, usuarios y auditoría.
2. Implementar un mantenedor CRUD para la entidad `Prenda`.
3. Permitir la consulta de categorías, tallas y colores activos para clasificar las prendas.
4. Incorporar autenticación de usuarios y autorización básica mediante los roles Administrador y Operador de Bodega.
5. Aplicar validaciones para impedir SKU duplicados, precios inválidos, stock negativo y relaciones inexistentes.
6. Utilizar procedimientos almacenados y parámetros tipados desde la capa DAL.
7. Registrar automáticamente las operaciones `INSERT`, `UPDATE` y `DESACTIVAR` mediante triggers.
8. Permitir la consulta del historial de auditoría a los usuarios autorizados.
9. Generar reportes PDF del inventario y del historial de auditoría.
10. Mantener la documentación, scripts y código en un repositorio GitHub con commits incrementales.

## 6. Alcance funcional

El sistema permitirá iniciar sesión, consultar el inventario, registrar prendas, modificar sus datos, desactivarlas, buscarlas y filtrarlas. Cada prenda tendrá un código SKU único, nombre, categoría, talla, color, marca, precio, stock, stock mínimo y estado.

Las categorías, tallas y colores se utilizarán como catálogos relacionados. En la primera versión se podrán consultar desde el mantenedor de prendas. El CRUD completo de estos catálogos no forma parte del mantenedor principal.

El stock actual se almacenará en la tabla `Prenda`. Cuando se modifique, la operación quedará registrada en la tabla `AuditoriaInventario`. No se implementará una tabla propia de movimientos de stock en esta versión.

El sistema generará dos tipos de reporte PDF: un reporte del inventario y un reporte del historial de auditoría. El usuario podrá seleccionar la ruta de destino mediante `SaveFileDialog`.

## 7. Limitaciones

La aplicación funcionará en sistemas Windows debido al uso de Windows Forms y SQL Server Express. La conexión se realizará mediante autenticación integrada de Windows, utilizando la instancia local `SQLEXPRESS`.

La primera versión estará pensada para una tienda pequeña, con un inventario aproximado de hasta 200 prendas. El sistema no tendrá funcionamiento web ni aplicación móvil.

Los valores monetarios se expresarán en pesos chilenos sin decimales. El precio se almacenará en SQL Server como `DECIMAL(10,0)` y se representará en C# mediante `decimal`.

La auditoría registrará cambios sobre la entidad `Prenda`, pero no constituirá un sistema general de auditoría para todas las tablas de la base de datos.

## 8. Actores del sistema

### 8.1 Administrador

El Administrador podrá iniciar sesión, consultar el inventario, registrar y modificar prendas, desactivarlas, consultar la auditoría, gestionar usuarios y generar reportes PDF.

### 8.2 Operador de Bodega

El Operador de Bodega podrá iniciar sesión, consultar el inventario y realizar las operaciones de inventario autorizadas por su rol. No podrá gestionar usuarios ni consultar la auditoría si no posee ese permiso.

### 8.3 Sistema

El Sistema ejecutará validaciones, procedimientos almacenados, triggers de auditoría y generación de reportes. Se considerará un actor secundario en los casos de uso automáticos.

## 9. Requerimientos funcionales

### RF01 — Autenticación y autorización

El sistema debe permitir que un usuario registrado inicie sesión con sus credenciales y acceda a las funciones autorizadas según su rol. Las contraseñas no deben almacenarse en texto plano.

### RF02 — Gestión de prendas

El sistema debe permitir registrar, consultar, modificar y desactivar prendas. Cada prenda debe relacionarse con una categoría, talla y color existentes y activos.

La desactivación será lógica mediante el campo `Activo`. No se eliminará físicamente la prenda desde la aplicación, para conservar la trazabilidad histórica.

### RF03 — Consulta de catálogos

El sistema debe cargar categorías, tallas y colores activos para utilizarlos en los controles de selección del mantenedor de prendas.

### RF04 — Búsqueda y filtrado

El sistema debe permitir buscar prendas por código SKU o nombre, y filtrar el inventario por categoría, talla, color y estado activo.

### RF05 — Validación de precio y stock

El sistema debe validar que el precio sea mayor que cero y que el stock y el stock mínimo sean números enteros mayores o iguales a cero. El precio deberá almacenarse como `DECIMAL(10,0)`.

### RF06 — Auditoría automática

El sistema debe registrar automáticamente las operaciones `INSERT`, `UPDATE` y `DESACTIVAR` realizadas sobre la entidad `Prenda`.

El registro debe incluir la operación, fecha y hora, usuario, identificador de la prenda y valores anteriores y nuevos cuando corresponda.

La auditoría se implementará principalmente mediante triggers en SQL Server y deberá ejecutarse dentro de la misma transacción de la operación de inventario.

### RF07 — Consulta de auditoría

El Administrador debe poder consultar el historial de auditoría mediante una pantalla de solo lectura. La consulta debe mostrar la operación, usuario, fecha, prenda, valores anteriores y valores nuevos.

### RF08 — Generación de reportes PDF

El sistema debe permitir generar un reporte PDF del inventario y un reporte PDF del historial de auditoría.

El usuario autorizado podrá seleccionar la ubicación del archivo mediante `SaveFileDialog`.

El documento generado debe incluir título, fecha y hora, datos consultados, numeración de páginas y mensajes de error comprensibles cuando la generación no sea posible.

### RF09 — Gestión de usuarios

El Administrador debe poder registrar, consultar y deshabilitar usuarios del sistema, asignándoles uno de los roles definidos. La gestión de usuarios debe respetar las reglas de seguridad de las credenciales.

## 10. Requerimientos no funcionales

### RNF01 — Arquitectura

La solución debe separar responsabilidades en las capas `UI`, `BLL`, `DAL` y `Entities`. La UI no debe crear conexiones ni ejecutar comandos SQL directamente.

### RNF02 — Plataforma y base de datos

La aplicación debe utilizar C# con Windows Forms sobre `net10.0-windows` y SQL Server Express en la instancia `SQLEXPRESS`, utilizando la base de datos `InventarioRopaDB`.

### RNF03 — Consultas parametrizadas

Las operaciones de la capa DAL deben utilizar procedimientos almacenados y parámetros tipados. No se deben concatenar valores ingresados por el usuario en consultas SQL.

### RNF04 — Integridad de datos

La base de datos debe utilizar claves primarias, claves foráneas, restricciones `NOT NULL`, `UNIQUE`, `CHECK` y valores predeterminados cuando corresponda.

### RNF05 — Seguridad de credenciales

Las contraseñas de `UsuarioSistema` no deben almacenarse en texto plano. La autenticación de la aplicación debe diferenciarse de la autenticación integrada de Windows utilizada por la conexión a SQL Server.

### RNF06 — Usabilidad

Los formularios deben utilizar etiquetas claras, controles adecuados, mensajes comprensibles y confirmación antes de desactivar una prenda o usuario.

### RNF07 — Manejo de errores

Los errores técnicos no deben mostrarse directamente al usuario. La aplicación debe capturar excepciones y presentar mensajes amigables.

### RNF08 — Trazabilidad

Las operaciones `INSERT`, `UPDATE` y `DESACTIVAR` de `Prenda` deben generar un registro de auditoría asociado a la misma transacción.

### RNF09 — Reportes

Los reportes PDF deben ser legibles, incluir fecha de generación, presentar los datos solicitados y mostrar numeración de páginas.

### RNF10 — Mantenibilidad

El código debe utilizar nombres descriptivos, clases con responsabilidades acotadas y una estructura de carpetas coherente con la arquitectura definida.

### RNF11 — Capacidad inicial

El sistema debe responder adecuadamente para un inventario local de hasta 200 prendas y sus registros de auditoría asociados.

## 11. Tríada de requerimientos

La tríada se define mediante **Rol o Usuario + Necesidad o Acción + Criterio de aceptación y seguridad**.

| ID | Rol o usuario | Necesidad o acción | Criterio de aceptación y seguridad |
|---|---|---|---|
| TR01 | Administrador y Operador de Bodega | Iniciar sesión para acceder al sistema según el rol asignado. | Las credenciales válidas permiten el acceso; las inválidas son rechazadas; la contraseña no se almacena en texto plano y las funciones se limitan según el rol. |
| TR02 | Administrador y Operador autorizado | Registrar, consultar, modificar y desactivar prendas. | El SKU es único; las claves foráneas existen; los datos válidos se guardan mediante procedimientos almacenados; la desactivación cambia `Activo` y conserva el historial. |
| TR03 | Administrador y Operador autorizado | Seleccionar categorías, tallas y colores activos para clasificar una prenda. | Los controles muestran solo catálogos válidos y la base rechaza referencias inexistentes o inactivas. |
| TR04 | Administrador y Operador de Bodega | Buscar y filtrar el inventario. | La grilla muestra únicamente los registros que coinciden con los filtros y la búsqueda utiliza parámetros sin concatenar entradas SQL. |
| TR05 | Administrador y Operador autorizado | Mantener precios y cantidades de stock válidas. | Se rechazan precios menores o iguales a cero, stock negativo y stock mínimo negativo; el precio se almacena como `DECIMAL(10,0)`. |
| TR06 | Sistema | Registrar automáticamente los cambios realizados sobre las prendas. | Cada `INSERT`, `UPDATE` y `DESACTIVAR` crea una fila de auditoría con operación, usuario, fecha, identificador y valores de cambio dentro de la misma transacción. |
| TR07 | Administrador | Consultar el historial de auditoría. | La grilla es de solo lectura y muestra operación, usuario, fecha, prenda, valores anteriores y valores nuevos. Los usuarios no autorizados no pueden acceder. |
| TR08 | Administrador | Generar reportes PDF del inventario y de la auditoría. | El usuario selecciona una ruta, el archivo incluye título, fecha, datos y páginas numeradas; si falla la operación se muestra un mensaje comprensible. |
| TR09 | Administrador | Gestionar usuarios y asignar roles. | El Administrador puede crear o deshabilitar usuarios; los roles válidos son Administrador y Operador de Bodega; las contraseñas se almacenan mediante hash. |

## 12. Relación con ISO/IEC 27001:2022

Los controles siguientes se utilizan como **referencia académica** para relacionar los requisitos del sistema con prácticas de seguridad. Esta relación no constituye una certificación ni demuestra conformidad formal con ISO/IEC 27001.

| Requisito | Control de referencia | Aplicación en el sistema |
|---|---|---|
| RF01, RF09, RNF05 | A.5.15 Control de acceso | El acceso a las funciones se limita según autenticación y rol. |
| RF01, RF09 | A.5.16 Gestión de identidad | Cada usuario posee una identidad propia en `UsuarioSistema`. |
| RF01, RF09, RNF05 | A.5.17 Información de autenticación | Las credenciales se protegen y las contraseñas no se almacenan en texto plano. |
| RF06, RF07, RNF08 | A.8.15 Registro de actividad | Las operaciones sobre prendas se registran mediante auditoría. |
| RF07, RNF08 | A.8.16 Actividades de monitoreo | El Administrador puede revisar los registros generados. |
| RNF01, RNF03 | A.8.25 Ciclo de vida de desarrollo seguro | La arquitectura por capas, la parametrización y las validaciones se consideran durante el desarrollo. |
| RNF03, RNF04, RNF05 | A.8.26 Requisitos de seguridad de aplicaciones | Se definen requisitos de autenticación, integridad, validación y protección de datos. |

## 13. Criterios generales de aceptación

1. La solución debe compilar sin errores.
2. La UI no debe contener objetos `SqlConnection`, `SqlCommand` ni consultas SQL directas.
3. La UI debe comunicarse con la BLL, la BLL con la DAL y la DAL con los procedimientos almacenados.
4. Las entidades y DTO deben ubicarse en la capa `Entities`.
5. El sistema debe impedir SKU duplicados, precios inválidos, stock negativo y referencias inexistentes.
6. La desactivación debe ser lógica y no debe borrar físicamente el historial.
7. Los triggers deben generar auditoría para `INSERT`, `UPDATE` y `DESACTIVAR`.
8. Los usuarios sin autorización no deben acceder a la gestión de usuarios ni a la consulta de auditoría.
9. Los reportes PDF deben generarse con título, fecha, datos y numeración de páginas.
10. Los mensajes mostrados al usuario no deben exponer contraseñas, cadenas de conexión ni detalles técnicos sensibles.
11. El repositorio debe contener documentación, scripts y código organizados en `/docs`, `/database` y `/src`.
12. El historial de Git debe demostrar avances incrementales mediante commits descriptivos.

## 14. Plan inicial de pruebas

| ID | Tipo | Caso de prueba | Entrada o acción | Resultado esperado |
|---|---|---|---|---|
| CP01 | Funcional | Login válido | Usuario y contraseña correctos | El sistema permite el acceso según el rol. |
| CP02 | Seguridad | Login inválido | Contraseña incorrecta | El acceso es rechazado y no se revela información sensible. |
| CP03 | Integridad | SKU duplicado | Registrar un SKU existente | La operación es rechazada por validación o restricción `UNIQUE`. |
| CP04 | Validación | Precio inválido | Precio igual o menor que cero | El sistema no permite guardar la prenda. |
| CP05 | Validación | Stock negativo | `Stock = -1` | UI, BLL o BD rechazan el valor. |
| CP06 | Integración | Registro de prenda | Completar datos válidos y guardar | La prenda se persiste mediante un procedimiento almacenado. |
| CP07 | Auditoría | Insertar prenda | Registrar una nueva prenda | Se crea una fila `INSERT` en `AuditoriaInventario`. |
| CP08 | Auditoría | Modificar precio | Cambiar el precio de una prenda | Se crea una fila `UPDATE` con valor anterior y nuevo. |
| CP09 | Auditoría | Desactivar prenda | Desactivar una prenda | `Activo` cambia a falso y se crea una fila `DESACTIVAR`. |
| CP10 | Autorización | Operador consulta auditoría | Intentar abrir el historial sin permiso | El sistema impide el acceso. |
| CP11 | Seguridad | Entrada SQL | Ingresar `' OR 1=1 --` en una búsqueda | La entrada se trata como texto y no altera la consulta. |
| CP12 | Filtros | Filtrar inventario | Seleccionar categoría o talla | La grilla muestra solo registros coincidentes. |
| CP13 | Reporte | Generar inventario PDF | Seleccionar ruta de guardado | Se crea un PDF legible con fecha, datos y páginas numeradas. |
| CP14 | Reporte | Generar auditoría PDF | Seleccionar ruta de guardado | Se crea un PDF con operación, usuario, fecha y valores de cambio. |
| CP15 | Errores | SQL Server no disponible | Usar una conexión inválida | La aplicación muestra un mensaje amigable y no se cierra inesperadamente. |

## 15. Trazabilidad hacia las fases siguientes

Los requerimientos `RF01` a `RF09` deberán reflejarse en los diagramas de casos de uso.

Las clases principales serán:

- `Prenda`
- `Categoria`
- `Talla`
- `Color`
- `UsuarioSistema`
- `AuditoriaInventario`

La base de datos deberá incluir procedimientos almacenados para listar, buscar, insertar, actualizar y desactivar prendas, además de consultar catálogos, usuarios y auditoría.

Los triggers de `Prenda` deberán generar los registros de trazabilidad.

La aplicación Windows Forms deberá implementar las funciones de UI, mientras que las validaciones estarán en BLL y las operaciones de persistencia en DAL. La generación de reportes será invocada desde la UI mediante una capa de negocio o servicio especializado.

## 16. Próxima fase

La siguiente fase será el modelado UML y el diseño de la base de datos. Antes de crear los diagramas se debe verificar que los nombres de requerimientos, entidades, roles y operaciones sean los mismos en toda la documentación.

## Referencias

[1]: /home/ubuntu/upload/Guia_Trabajo_Practico_y_Rubrica_Evaluacion_v21(2).pdf "Guía de Trabajo Práctico Integrador y Rúbrica de Evaluación"

[2]: https://github.com/GUANSAMA/sistema-inventario-ropa "Repositorio GitHub del proyecto"
