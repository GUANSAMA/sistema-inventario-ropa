# Diagrama de Clases - Arquitectura de 4 Capas

El siguiente diagrama ilustra la arquitectura de software del Sistema de Inventario de Ropa, separando las responsabilidades en las capas UI, BLL, DAL y Entities, asegurando que la interfaz no acceda directamente a la base de datos.

```plantuml
@startuml
!theme plain
skinparam classBackgroundColor #f9f9f9
skinparam classBorderColor #333333

package "Entities (Entidades del Dominio)" {
  class Prenda {
    + IdPrenda : int
    + IdCategoria : int
    + IdTalla : int
    + IdColor : int
    + NombreProducto : string
    + Precio : int
    + Stock : int
  }
  class Categoria {
    + IdCategoria : int
    + Descripcion : string
  }
}

package "DAL (Acceso a Datos)" {
  class PrendaDAL {
    - conexion : SqlConnection
    + Insertar(prenda: Prenda) : bool
    + Actualizar(prenda: Prenda) : bool
    + Eliminar(id: int) : bool
    + ListarTodas() : List<Prenda>
  }
  class AuditoriaDAL {
    + RegistrarAccion(idPrenda: int, idUsuario: int, accion: string) : bool
  }
}

package "BLL (Lógica de Negocio)" {
  class PrendaBLL {
    - dal : PrendaDAL
    - auditoriaDal : AuditoriaDAL
    + GuardarPrenda(prenda: Prenda, idUsuario: int) : bool
    + EliminarPrenda(id: int, idUsuario: int) : bool
    + ObtenerPrendas() : List<Prenda>
    - ValidarPrecio(precio: int) : bool
  }
}

package "UI (Interfaz de Usuario - WinForms)" {
  class FrmGestionPrendas {
    - bll : PrendaBLL
    - usuarioActual : int
    + btnGuardar_Click()
    + btnEliminar_Click()
    + CargarGrilla()
    - ValidarCamposTexto() : bool
  }
}

' Relaciones de dependencia
FrmGestionPrendas ..> PrendaBLL : " Llama a"
PrendaBLL ..> PrendaDAL : " Usa"
PrendaBLL ..> AuditoriaDAL : " Registra"

' Relación transversal (Todas las capas usan las entidades)
DAL ..> Entities : " Usa"
BLL ..> Entities : " Usa"
UI ..> Entities : " Usa"

@enduml