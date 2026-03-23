# SICE - Sistema de Comprobantes Electrónicos

## Descripción

Sistema de **facturación electrónica para Ecuador**, integrado con el **SRI** (Servicio de Rentas Internas).

**Flujo principal:** generar XML → firmar digitalmente (certificado `.p12`) → enviar al SRI → recibir autorización → enviar PDF/XML por correo al cliente.

**Comprobantes soportados:** Facturas, Notas de crédito, Retenciones, Guías de remisión.

**Multi-empresa:** cada `Empresa` tiene su propio certificado digital. Una instalación gestiona múltiples emisores.

**Ambientes SRI:**
- Producción: `ec.gob.sri.cel` (recepción) y `ec.gob.sri.celcer` (autorización)
- Pruebas: variantes `Off` (`ec.gob.sri.celOff`, `ec.gob.sri.celcerOff`)

---

## Arquitectura

Solución .NET Framework 4.8 con arquitectura N-capas:

```
WebUI / WebAPI / WinServiceSICE
        |
BusinessLogicLayer (BLL)
        |
DataAccessLayer (DAL)
        |
SqlDataBase / PostgreSQLDataBase
        |
SQL Server / PostgreSQL
```

### Proyectos

| Proyecto | Namespace | Rol |
|---|---|---|
| `BusinessObjects` | `BusinessObjects` | Entidades / DTOs |
| `DataAccessLayer` | `DataAccessLayer` | Acceso a datos (DAL) |
| `BusinessLogicLayer` | `BusinessLogicLayer` | Lógica de negocio (BLL) |
| `SqlDataBase` | `SqlDataBase` | Motor SQL Server (DB, Sentences, TransactionManager) |
| `PostgreSQLDataBase` | `SqlDataBasePG` | Motor PostgreSQL (misma interfaz que SqlDataBase) |
| `Functions` | `Functions` | Utilidades (Conversiones, etc.) |
| `ExceptionHandling` | `ExceptionHandling` | Manejo de excepciones |
| `HtmlObjects` | `HtmlObjects` | Elementos HTML |
| `Services` | `Services` | Servicios auxiliares |
| `WebUI` | — | ASP.NET Web Forms (UI principal) |
| `WebAPI` | — | ASP.NET Web API REST |
| `WinServiceSICE` | — | Servicio de Windows |

---

## Configuración (Web.config / App.config)

```xml
<appSettings>
  <add key="connection" value="Server=...;Database=...;..." />
  <add key="provider" value="SqlServer" />  <!-- SqlServer | PostgreSQL | Oracle -->
</appSettings>
```

El `TransactionManager` y el `DAL` leen estas claves en tiempo de ejecución.

---

## Entidades (BusinessObjects)

Cada entidad sigue este patrón estricto:

### Atributo `[Data]`

```csharp
public class Data : System.Attribute
{
    public bool key { get; set; }          // Es llave primaria (usada en WHERE de UPDATE/DELETE)
    public bool auto { get; set; }         // Autonumérico: excluido de INSERT
    public bool originalkey { get; set; }  // Copia de la PK para UPDATE (campo_key)
    public bool noupdate { get; set; }     // Excluido del SET en UPDATE
    public bool nosql { get; set; }        // No es columna propia; viene de JOIN (ver tablaref/camporef)
    public bool noprop { get; set; }       // Propiedad de solo uso en memoria, nunca va a SQL
    public string tablaref { get; set; }   // Tabla del JOIN cuando nosql=true
    public string camporef { get; set; }   // Campo de la tabla referenciada
    public string foreign { get; set; }    // Columna(s) FK en la tabla actual (coma-separado)
    public string keyref { get; set; }     // Columna(s) PK en la tabla referenciada (coma-separado)
    public string join { get; set; }       // Tipo de JOIN: "INNER" | "left" (default INNER)
}
```

### Constructores obligatorios

Cada entidad tiene **4 constructores**:

```csharp
// 1. Vacío
public Entidad() { }

// 2. Parametrizado (campos de la tabla)
public Entidad(tipo campo1, tipo campo2, ...) { ... }

// 3. Desde IDataReader (columnas leídas de la BD)
public Entidad(IDataReader reader) { ... }

// 4. Desde Dictionary<string, object> (usado por la WebAPI / JSON)
public Entidad(object objeto) {
    Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
    // TryGetValue + Conversiones.GetValueByType(valor, typeof(Tipo))
}
```

### Método obligatorio

```csharp
public PropertyInfo[] GetProperties()
{
    return this.GetType().GetProperties();
}
```

### Convención de nombres de campos

- PK: `{prefijo}_campo` con `[Data(key = true)]`
- Copia de PK para UPDATE: `{prefijo}_campo_key` con `[Data(originalkey = true)]`
- Auditoría: `crea_usr`, `crea_fecha`, `mod_usr`, `mod_fecha`

Ejemplo:
```csharp
[Data(key = true)]
public String usr_id { get; set; }
[Data(originalkey = true)]
public String usr_id_key { get; set; }
[Data(noupdate = true)]
public String crea_usr { get; set; }
```

---

## DataAccessLayer (DAL)

### DAL base (`DAL.cs`)

Gestiona transacciones multi-proveedor:

```csharp
DAL dal = new DAL();
dal.CreateTransaction();
dal.BeginTransaction();
// ... operaciones con dal como parámetro
dal.Commit();
// o dal.Rollback() en el catch
```

Proveedor detectado con `DAL.GetProvider()` → lee `AppSettings["provider"]`.

### Patrón XxxDAL

Cada entidad tiene una clase `{Entidad}DAL` con métodos estáticos:

| Método | Descripción |
|---|---|
| `Insert(obj)` | INSERT sin transacción externa |
| `Insert(dal, obj)` | INSERT con transacción compartida |
| `InsertIdentity(obj)` | INSERT retornando ID generado |
| `InsertIdentity(dal, obj)` | Ídem con transacción compartida |
| `Update(obj)` | UPDATE por PK |
| `Update(dal, obj)` | UPDATE con transacción compartida |
| `Delete(obj)` | DELETE por PK |
| `Delete(dal, obj)` | DELETE con transacción compartida |
| `GetByPK(obj)` | SELECT por llave primaria |
| `GetById(obj)` | SELECT por campos `*_id` |
| `GetAll(whereClause, orderBy)` | SELECT con WHERE string literal |
| `GetAll(whereParams, orderBy)` | SELECT con WHERE parametrizado |
| `GetAllTop(whereParams, orderBy, top)` | SELECT TOP N |
| `GetAllbyPage(where, orderBy, desde, hasta)` | Paginación con ROW_NUMBER |
| `GetRecordCount(where, orderBy)` | COUNT(*) |
| `GetMax(campo)` | MAX(campo) |
| `GetSum(campo, where, orderBy)` | SUM(campo) |

Cada método tiene variante con `string WhereClause` y con `WhereParams`.

### WhereParams

```csharp
// WHERE campo = {0} AND otro = {1}
var p = new WhereParams("campo = {0} AND otro = {1}", valor0, valor1);
```

Los `{0}`, `{1}`... se reemplazan por `@par0`, `@par1`... como parámetros SQL.

---

## BusinessLogicLayer (BLL)

### BLL base (`BLL.cs`)

Envuelve el `DAL` para gestión de transacciones:

```csharp
BLL bll = new BLL();
bll.CreateTransaction();
bll.BeginTransaction();
EntidadBLL.Insert(bll, obj);
bll.Commit();
```

### Patrón XxxBLL

Cada `{Entidad}BLL` delega directamente a `{Entidad}DAL`. No agrega lógica adicional salvo que sea necesario. La firma es idéntica al DAL pero recibiendo `BLL bll` en lugar de `DAL dal`.

---

## Motor SQL (SqlDataBase)

### Sentences

Genera SQL dinámicamente a partir de `PropertyInfo[]` y `SentenceType`:

- `Insert` → `INSERT INTO tabla (campos) VALUES (@campos)`
- `InsertIdentity` → igual + `; SELECT SCOPE_IDENTITY()`
- `Update` → `UPDATE tabla SET campo=@campo WHERE pk=@pk_key`
- `Delete` → `DELETE FROM tabla WHERE pk=@pk`
- `GetAll` → `SELECT tabla.campos [, joins] FROM tabla [INNER/LEFT JOIN ...]`
- `GetAllByPage` → `SELECT ... FROM (SELECT ROW_NUMBER() OVER(ORDER BY %orderby%) RowNr, ... %whereclause%) t WHERE RowNr BETWEEN %desde% AND %hasta%`
- `GetRecordCount` → `SELECT COUNT(*) FROM tabla`
- `GetMax` → `SELECT MAX(%campo%) FROM tabla`
- `GetSum` → `SELECT SUM(%campo%) FROM tabla`

Los JOINs se construyen automáticamente desde las propiedades con `[Data(nosql=true, tablaref=..., camporef=..., foreign=..., keyref=..., join=...)]`.

### TransactionManager

```csharp
TransactionManager tm = new TransactionManager(); // lee AppSettings["connection"]
tm.BeginTransaction();
// usar tm.connection y tm.transaction en SqlCommand
tm.Commit();   // cierra conexión
tm.Rollback(); // cierra conexión
```

---

## WebAPI

Endpoints en `Controllers/`:

- `LoginController` — autenticación, genera token JWT
- `ComprobanteController` — operaciones sobre comprobantes
- `EnvioController` — envío de comprobantes al SRI

Token validado por `TokenValidationHandler` (DelegatingHandler).

---

## WebUI

ASP.NET Web Forms. Clases auxiliares en `clases/`:

- `Firma.cs` — firma digital XML (certificado `.p12`)
- `Proceso.cs` — procesamiento de comprobantes electrónicos
- `Enums.cs` — enumeraciones del dominio

Tiene Web References al SRI (Ecuador):
- `ec.gob.sri.cel` / `ec.gob.sri.celOff` — recepción comprobantes (producción / offline)
- `ec.gob.sri.celcer` / `ec.gob.sri.celcerOff` — autorización comprobantes

---

## Agregar una nueva entidad

Al agregar una nueva tabla `foo` seguir estos pasos:

1. **`BusinessObjects/Foo.cs`** — entidad con `[Data]`, 4 constructores, `GetProperties()`
2. **`DataAccessLayer/FooDAL.cs`** — clase `FooDAL` con todos los métodos estáticos estándar, con soporte SqlServer y PostgreSQL
3. **`BusinessLogicLayer/FooBLL.cs`** — clase `FooBLL` que delega a `FooDAL`

Seguir exactamente el patrón de `Usuario` / `UsuarioDAL` / `UsuarioBLL` como referencia.
