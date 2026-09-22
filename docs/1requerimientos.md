# Informe Técnico y Requerimientos - Fase 1

**Proyecto:** Sistema de Inventario de Ropa
**Tienda:** Tienda Moda Urbana
**Asignatura:** Programación Segura
**Estudiante:** Guillermo Sanchez
**Docente:** Ricardo Arturo Zamorano Pontiggia
**Institución:** AIEP San Joaquín
**Fecha:** 23-09-2026
**Repositorio:** [sistema-inventario-ropa](https://github.com/GUANSAMA/sistema-inventario-ropa)

---

## 1. Descripción y contexto del problema
La "Tienda Moda Urbana" requiere modernizar y asegurar su gestión de stock. Actualmente, el manejo del inventario de ropa puede presentar vulnerabilidades, pérdida de información o inconsistencias debido a la falta de un sistema centralizado y auditado. Se necesita una aplicación de escritorio robusta que permita gestionar de forma segura los productos (prendas) y sus atributos (categorías, tallas, colores), garantizando la integridad de los datos y registrando quién realiza cada acción.

## 2. Delimitación de la problemática
El sistema se enfocará **exclusivamente en la gestión del inventario interno y la auditoría de dichas operaciones**. 
Quedan fuera de este alcance:
- Módulo de punto de venta (POS) o facturación.
- Integración con pasarelas de pago electrónico.
- Gestión de clientes o comercio electrónico.
- Recursos humanos (pago de sueldos o turnos).

## 3. Justificación
Desarrollar este sistema bajo el enfoque de **Programación Segura** y arquitectura de 4 capas (UI, BLL, DAL, Entities) permite proteger la información contra vulnerabilidades comunes (como inyección SQL o escalado de privilegios). La inclusión de la entidad `AuditoriaInventario` justifica la necesidad crítica de trazabilidad en sistemas modernos, cumpliendo con los estándares académicos y de la industria para el manejo seguro de bases de datos relacionales locales.

## 4. Objetivo general
Desarrollar e implementar una aplicación de escritorio segura en C# y .NET 10 (Windows Forms) conectada a SQL Server Express, para gestionar y auditar el inventario de la "Tienda Moda Urbana" aplicando principios de arquitectura por capas.

## 5. Objetivos específicos
- Estructurar el proyecto en 4 capas lógicas: `UI` (Interfaz gráfica), `BLL` (Lógica de negocio), `DAL` (Acceso a datos) y `Entities` (Entidades del dominio).
- Crear los mantenedores (CRUD) completos para las entidades: `Categoria`, `Talla`, `Color` y `Prenda`.
- Implementar un módulo de gestión y autenticación de `UsuarioSistema`.
- Configurar un mecanismo de registro automático en la entidad `AuditoriaInventario` para rastrear cambios críticos.
- Aplicar validaciones de seguridad en todas las capas para evitar ingresos de datos maliciosos y asegurar el formato de moneda (pesos chilenos sin decimales).

## 6. Alcance
El sistema será una aplicación de escritorio de uso interno (on-premise) en entorno Windows. Permitirá a los usuarios autenticados gestionar el catálogo de ropa. Utilizará autenticación de Windows para la conexión a la base de datos `InventarioRopaDB` (instancia `SQLEXPRESS`), asegurando que las credenciales de la DB no queden expuestas en la interfaz.

## 7. Limitaciones
- **Plataforma:** Exclusivo para sistemas operativos Windows (debido al uso de .NET 10 Windows Forms y autenticación integrada de Windows).
- **Red:** El sistema funcionará en red local o en el mismo equipo donde resida el motor SQLEXPRESS.
- **Moneda:** Los valores monetarios estarán restringidos estrictamente a Pesos Chilenos (CLP) sin uso de decimales.
- **Roles específicos:** Pendiente por definir si existirán múltiples niveles de usuario detallados (ej. Cajero vs Administrador). Por defecto se asumirá un control basado en usuarios registrados en el sistema.

## 8. Actores del sistema
1. **Administrador de Sistema:** Usuario con permisos totales, capaz de gestionar otros usuarios, revisar auditorías y manipular todo el inventario.
2. **Operador de Bodega:** Usuario estándar encargado de la gestión diaria del inventario (crear, editar, listar prendas y sus atributos).

## 9. Requerimientos funcionales (RF)
- **RF01 - Gestión de Atributos:** El sistema debe permitir crear, leer, actualizar y eliminar (CRUD) registros en las entidades `Categoria`, `Talla` y `Color`.
- **RF02 - Gestión de Prendas:** El sistema debe permitir el CRUD de la entidad `Prenda`, relacionándola obligatoriamente con una Categoría, una Talla y un Color.
- **RF03 - Manejo de Precios:** El sistema debe asegurar que el ingreso y visualización de precios en `Prenda` sea numérico entero (Pesos Chilenos, sin decimales).
- **RF04 - Autenticación:** El sistema debe exigir un inicio de sesión mediante credenciales de `UsuarioSistema` antes de otorgar acceso a los formularios principales.
- **RF05 - Trazabilidad:** El sistema debe registrar automáticamente en `AuditoriaInventario` (acción, fecha, usuario involucrado) cada vez que se modifique o elimine una prenda.

## 10. Requerimientos no funcionales (RNF)
- **RNF01 - Arquitectura:** El código debe separar responsabilidades estrictamente en las capas: `UI`, `BLL`, `DAL` y `Entities`.
- **RNF02 - Base de Datos:** Se debe utilizar SQL Server Express (`SQLEXPRESS`) con la base de datos `InventarioRopaDB`.
- **RNF03 - Interfaz:** La interfaz gráfica debe desarrollarse utilizando Windows Forms bajo el framework `net10.0-windows`.
- **RNF04 - Seguridad de Datos:** Todas las consultas a la base de datos desde la capa `DAL` deben utilizar parámetros tipados (Parametrización) para mitigar riesgos de Inyección SQL.
- **RNF05 - Documentación:** Se debe emplear PlantUML para la generación de diagramas arquitectónicos y de base de datos en fases posteriores.

## 11. Tríada de Requerimientos Funcionales
*Formato: [Rol/Usuario] - [Necesidad/Acción] - [Criterio de Aceptación y Seguridad]*

| ID | Rol / Usuario | Necesidad o Acción (Qué y Para qué) | Criterio de Aceptación y Seguridad |
|---|---|---|---|
| **TR01** | Operador / Administrador | Necesito registrar, editar y listar `Prenda`s con su respectivo precio y características. | **Aceptación:** Los datos se guardan correctamente en DB respetando FKs. El precio es `int` (sin decimales).<br>**Seguridad:** Los inputs en WinForms validan caracteres no numéricos y previenen inyección SQL desde el DAL. |
| **TR02** | Administrador | Necesito gestionar los `UsuarioSistema` para controlar quién accede. | **Aceptación:** Permite crear y deshabilitar usuarios.<br>**Seguridad:** Las contraseñas (si se almacenan) no deben guardarse en texto plano. La conexión a BD usa Seguridad Integrada de Windows. |
| **TR03** | Administrador | Necesito visualizar el registro de `AuditoriaInventario` para rastrear anomalías. | **Aceptación:** El formulario muestra una grilla de solo lectura con el historial.<br>**Seguridad:** La tabla de auditoría no permite modificaciones ni borrados (Delete/Update bloqueados a nivel de sistema/DB). |
| **TR04** | Sistema (Automático) | Necesito interceptar las acciones en la capa BLL para grabar en `AuditoriaInventario`. | **Aceptación:** Cada vez que el BLL ejecuta un Update o Delete de Prenda, llama al método de auditoría.<br>**Seguridad:** Si la auditoría falla, la transacción de inventario debería manejarse de forma segura (Control de Excepciones). |

## 12. Relación de los requerimientos con controles ISO/IEC 27001
El enfoque del proyecto se alinea con los siguientes controles (versión 2022 / 2013):
- **Control de Acceso (A.9 / 5.15):** Cumplido mediante el **RF04** y **TR02**, garantizando que solo `UsuarioSistema` registrados puedan acceder a la información, apoyado por la autenticación de Windows a nivel de motor SQL.
- **Seguridad en las Operaciones / Desarrollo Seguro (A.14 / 8.25):** Cumplido mediante el **RNF04**, que exige consultas parametrizadas, separación de capas lógicas, y validación de tipos de datos en UI/BLL para prevenir vulnerabilidades OWASP (ej. SQLi).
- **Registro y Supervisión (A.12.4 / 8.15):** Cumplido directamente por el **RF05** y la entidad `AuditoriaInventario`, estableciendo logs inmutables desde la interfaz de usuario de las acciones críticas.

## 13. Criterios generales de aceptación
1. **Compilación Limpia:** El código C# debe compilar sin errores ni advertencias de seguridad críticas.
2. **Independencia de Capas:** La capa `UI` bajo ninguna circunstancia debe contener código de acceso a datos (objetos `SqlConnection`, `SqlCommand`, etc.). Estos pertenecen exclusivamente a `DAL`.
3. **Flujo de Entidades:** Las capas deben comunicarse utilizando únicamente objetos instanciados de la biblioteca de clases `Entities`.
4. **Manejo de Errores:** Las excepciones de SQL no deben mostrarse crudas (raw) al usuario en el WinForms; deben ser capturadas en `BLL` y mostrar mensajes amigables en `UI`.

## 14. Plan inicial de pruebas
- **Pruebas de Componente (Manuales):** Validación de que los TextBox de precios no admitan letras ni caracteres especiales.
- **Pruebas de Integración:** Verificar que al guardar una `Prenda` en el UI, pase exitosamente por BLL, DAL y persista en `InventarioRopaDB` en SQL Server.
- **Pruebas de Seguridad:** 
  - Intentar insertar código SQL (ej. `' OR 1=1 --`) en los campos de texto para confirmar que la parametrización bloquea el ataque.
  - Verificar que intentar saltarse el Login cierre la aplicación.
- **Pruebas de Trazabilidad:** Modificar el precio de una Prenda y constatar mediante consulta que se insertó una nueva fila automática en `AuditoriaInventario`.

---
*Documento generado para la Fase 1. Pendiente paso a Fase 2 (Diagramación UML).*