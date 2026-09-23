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
participant "AuditoriaDAL\n(DAL Audit)" as AuditDAL
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
    
    BLL -> DAL: Insertar(prendaDto)
    activate DAL
    DAL -> DB: Ejecuta SQL Parametrizado (Previene SQLi)
    DB --> DAL: Confirma inserción exitosa
    DAL --> BLL: Retorna true
    deactivate DAL

    BLL -> AuditDAL: RegistrarAccion(idPrenda, idUsuario, "INSERT")
    activate AuditDAL
    AuditDAL -> DB: Inserta registro en AuditoriaInventario
    DB --> AuditDAL: Confirma registro de auditoría
    AuditDAL --> BLL: Retorna true
    deactivate AuditDAL

    BLL --> UI: Retorna éxito general
    deactivate BLL

    UI --> User: Muestra mensaje "Prenda guardada con éxito"
end

deactivate UI
@enduml