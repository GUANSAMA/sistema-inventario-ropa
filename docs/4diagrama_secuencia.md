# Diagrama de Secuencia - Flujo de Guardado y Auditoría

Este diagrama ilustra el flujo de control y las validaciones de seguridad que ocurren cuando un usuario registra una prenda en el sistema, mostrando la interacción entre las 4 capas y el registro automático de auditoría.

```plantuml
@startuml
!theme plain
autonumber

actor "Operador / Admin" as User
participant "FrmGestionPrendas\n(UI)" as UI
participant "PrendaBLL\n(BLL)" as BLL
participant "PrendaDAL\n(DAL)" as DAL
database "SQL Server\n(InventarioRopaDB)" as DB

User -> UI: Ingresa datos de prenda y hace clic en 'Guardar'
activate UI

UI -> UI: ValidarCamposTexto() (Verifica enteros y formato)
alt Datos Inválidos o Vacíos
    UI --> User: Muestra advertencia de validación
else Datos Válidos
    UI -> BLL: GuardarPrenda(prendaDto, idUsuario)
    activate BLL

    BLL -> BLL: ValidarPrecio(precio) (Control CLP sin decimales)
    
    BLL -> DAL: Insertar(prendaDto, idUsuario)
    activate DAL
    DAL -> DB: Ejecuta sp_Prenda_Insertar con parámetros
    activate DB
    DB -> DB: Establece SESSION_CONTEXT(IdUsuario)
    DB -> DB: Inserta Prenda
    DB -> DB: trg_Prenda_Insertar inserta AuditoriaInventario
    DB --> DAL: Devuelve IdPrendaNueva
    deactivate DB
    DAL --> BLL: Retorna true
    deactivate DAL

    BLL --> UI: Retorna éxito general
    deactivate BLL

    UI --> User: Muestra mensaje "Prenda guardada con éxito"
end

note over DAL, DB
La auditoría la escribe el trigger dentro de la misma operación
de base de datos. La aplicación no inserta una segunda fila de auditoría.
Si falta un usuario válido en el contexto de sesión, el trigger rechaza
la operación para evitar registros sin identidad.
end note

deactivate UI
@enduml
