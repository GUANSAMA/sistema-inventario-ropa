# Modelo de Base de Datos - Tienda Moda Urbana

El siguiente diagrama representa la estructura relacional para la base de datos `InventarioRopaDB` en SQL Server Express, cumpliendo con los requerimientos de la Fase 1.

```plantuml
@startuml
!theme plain
hide circle
skinparam linetype ortho
skinparam EntityBackgroundColor #f9f9f9
skinparam EntityBorderColor #333333

entity "UsuarioSistema" as us {
  * id_usuario : int <<PK>>
  --
  * username : varchar(50)
  * password_hash : varchar(255)
  * rol : varchar(20)
  * activo : bit
}

entity "Categoria" as cat {
  * id_categoria : int <<PK>>
  --
  * descripcion : varchar(100)
}

entity "Talla" as tal {
  * id_talla : int <<PK>>
  --
  * descripcion : varchar(20)
}

entity "Color" as col {
  * id_color : int <<PK>>
  --
  * descripcion : varchar(50)
}

entity "Prenda" as pre {
  * id_prenda : int <<PK>>
  --
  * id_categoria : int <<FK>>
  * id_talla : int <<FK>>
  * id_color : int <<FK>>
  * nombre_producto : varchar(100)
  * precio : int
  * stock : int
}

entity "AuditoriaInventario" as aud {
  * id_auditoria : int <<PK>>
  --
  * id_prenda : int
  * id_usuario : int <<FK>>
  * accion : varchar(20)
  * fecha_movimiento : datetime
}

' Relaciones obligatorias de la Prenda
cat ||--o{ pre
tal ||--o{ pre
col ||--o{ pre

' Relación de Auditoría con el Usuario que realiza la acción
us ||--o{ aud

@enduml