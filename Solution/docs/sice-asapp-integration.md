# Integración SICE → Asapp — Guía de implementación

> Este documento es la fuente de verdad para implementar la integración entre SICE y Asapp.
> Está escrito para ser leído por un agente AI que implementará los cambios en el proyecto SICE.

---

## Contexto

**SICE** es el sistema legacy que recibe comprobantes electrónicos vía API de clientes externos.
**Asapp** es el nuevo sistema al que se está migrando.

**Fase 1 — Shadow mode (lo que se implementa aquí):**
- SICE sigue siendo responsable de todo: recibir el comprobante, firmarlo, enviarlo al SRI y gestionar la autorización.
- SICE llama a Asapp en cada cambio de estado para que Asapp tenga una copia fiel del ciclo de vida completo.
- Asapp no hace nada con esos datos todavía — los persiste, los muestra y genera reportes.
- El cliente no se entera de nada. Cero cambios en la interfaz hacia el cliente.

**Por qué por eventos y no una sola llamada al final:**
Asapp necesita el historial completo del comprobante (Enviado → Recibido → Autorizado), no solo el resultado final. Eso permite auditoria, reportes intermedios y detectar problemas antes de la migración definitiva.

---

## Autenticación

Todos los endpoints de Asapp requieren una **API Key** por empresa en el header HTTP:

```
X-Api-Key: {api_key_de_la_empresa}
```

- Cada empresa tiene su propia API Key en Asapp.
- La API Key se obtiene desde el panel de administración de Asapp (Configuración → API Keys).
- En SICE debe existir una configuración que mapee cada empresa a su API Key de Asapp.
- Si la API Key es inválida o falta, Asapp retorna `401 Unauthorized`.

### Configuración sugerida en SICE

Agregar una tabla o sección de configuración con el siguiente esquema por empresa:

| Campo | Tipo | Ejemplo |
|---|---|---|
| `RUC` | string | `"0190155722001"` |
| `AsappApiKey` | string | `"ask_live_abc123..."` |
| `AsappBaseUrl` | string | `"https://electronic.asapp.com.ec/api"` |

En desarrollo/pruebas usar la URL de staging:
- Staging: proporcionada por Cristhian al momento de las pruebas
- Producción: proporcionada por Cristhian al activar cada empresa

---

## Flujo completo de integración

```
Cliente externo
    │
    ▼
SICE recibe comprobante
    │
    ├──► [1] POST /v1/sice/comprobantes    ← crea en Asapp con estado "Enviado"
    │
    ▼
SICE firma y envía al SRI
    │
    ▼
SRI responde RECIBIDA o DEVUELTA
    │
    ├── RECIBIDA ──► [2a] PATCH /v1/sice/comprobantes/{claveAcceso}  (estado: "Recibido")
    │
    └── DEVUELTA ──► [2b] PATCH /v1/sice/comprobantes/{claveAcceso}  (estado: "Devuelto" + mensaje)
            │
            ▼ (si fue RECIBIDA, SICE consulta autorización)
        SRI responde AUTORIZADO o NO AUTORIZADO
            │
            ├── AUTORIZADO    ──► [3a] PATCH /v1/sice/comprobantes/{claveAcceso}  (estado: "Autorizado" + datos)
            └── NO AUTORIZADO ──► [3b] PATCH /v1/sice/comprobantes/{claveAcceso}  (estado: "NoAutorizado" + mensaje)
```

**Regla general:** cada vez que SICE cambia el estado de un comprobante, llama a Asapp con el nuevo estado. Las llamadas son independientes — si una falla, se reintenta esa llamada sin afectar el flujo de SICE.

---

## Endpoint 1 — Crear comprobante

### `POST /v1/sice/comprobantes`

Llamar **después de enviar el comprobante al SRI**, cuando el estado en SICE pasa a "Enviado".

#### Request

```http
POST /v1/sice/comprobantes
X-Api-Key: {api_key}
Content-Type: application/json
```

```json
{
  "xmlBase64": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4...",
  "tipoDocumento": "01",
  "ambiente": "Produccion",
  "claveAcceso": "2406202601019300081500120010010000000011234567819",
  "numeroComprobante": "001-001-000000001",
  "fechaEmision": "2026-06-19T00:00:00-05:00"
}
```

| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `xmlBase64` | string | ✅ | XML del comprobante enviado al SRI, codificado en Base64 |
| `tipoDocumento` | string | ✅ | Código SRI: `"01"`=Factura, `"03"`=LC, `"04"`=NC, `"05"`=ND, `"06"`=GR, `"07"`=Retención |
| `ambiente` | string | ✅ | `"Pruebas"` o `"Produccion"` |
| `claveAcceso` | string | ✅ | ClaveAcceso de 49 dígitos generada por SICE |
| `numeroComprobante` | string | ✅ | Formato `"001-001-000000001"` |
| `fechaEmision` | string (ISO 8601) | ✅ | Fecha de emisión con offset Ecuador `-05:00` |

#### Responses

**201 Created** — comprobante creado exitosamente:
```json
{
  "comprobanteId": 142,
  "claveAcceso": "2406202601019300081500120010010000000011234567819",
  "numeroComprobante": "001-001-000000001",
  "estadoWorkflow": "Enviado",
  "yaExistia": false
}
```

**200 OK** — comprobante ya existía (idempotencia):
```json
{
  "comprobanteId": 142,
  "claveAcceso": "2406202601019300081500120010010000000011234567819",
  "numeroComprobante": "001-001-000000001",
  "estadoWorkflow": "Enviado",
  "yaExistia": true
}
```

> La idempotencia es por `claveAcceso`. Si SICE llama dos veces con la misma clave, Asapp retorna el comprobante existente sin duplicar. Esto protege contra reintentos.

**400 Bad Request** — campos requeridos faltantes o body inválido.

**401 Unauthorized** — API Key inválida o ausente.

**422 Unprocessable Entity** — error de negocio (XML malformado, feature no habilitada):
```json
{
  "error": "XmlBase64 inválido",
  "code": "InvalidBase64"
}
```

---

## Endpoint 2 — Actualizar estado

### `PATCH /v1/sice/comprobantes/{claveAcceso}`

Llamar **cada vez que el estado del comprobante cambia** en el flujo SRI.

```http
PATCH /v1/sice/comprobantes/{claveAcceso}
X-Api-Key: {api_key}
Content-Type: application/json
```

El body varía según el estado:

---

### Estado: Recibido

SRI respondió `RECIBIDA`. El comprobante fue recibido pero aún no autorizado.

```json
{
  "estado": "Recibido"
}
```

---

### Estado: Devuelto

SRI respondió `DEVUELTA`. Hay errores formales en el comprobante.

```json
{
  "estado": "Devuelto",
  "mensaje": "ERROR: RUC del emisor no coincide con el certificado digital."
}
```

| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `estado` | string | ✅ | `"Devuelto"` |
| `mensaje` | string | ✅ | Mensaje de error retornado por el SRI |

---

### Estado: Autorizado

SRI autorizó el comprobante.

```json
{
  "estado": "Autorizado",
  "numeroAutorizacion": "2406202601019300081500120010010000000011234567819",
  "fechaAutorizacion": "2026-06-19T14:32:10-05:00",
  "xmlAutorizadoBase64": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4..."
}
```

| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `estado` | string | ✅ | `"Autorizado"` |
| `numeroAutorizacion` | string | ✅ | Número de autorización del SRI (generalmente la ClaveAcceso de 49 dígitos) |
| `fechaAutorizacion` | string (ISO 8601) | ✅ | Fecha y hora de autorización con offset Ecuador `-05:00` |
| `xmlAutorizadoBase64` | string | ✅ | XML autorizado que devuelve el SRI (con la autorización embebida), en Base64 |

---

### Estado: NoAutorizado

SRI rechazó el comprobante después del proceso de autorización.

```json
{
  "estado": "NoAutorizado",
  "mensaje": "El RUC del emisor se encuentra suspendido."
}
```

| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `estado` | string | ✅ | `"NoAutorizado"` |
| `mensaje` | string | ✅ | Motivo de rechazo retornado por el SRI |

---

### Responses del PATCH

**200 OK** — estado actualizado correctamente. Body vacío.

**400 Bad Request** — campo `estado` faltante o body inválido.

**401 Unauthorized** — API Key inválida o ausente.

**404 Not Found** — no existe un comprobante con esa `claveAcceso` para esta empresa.
```json
{
  "error": "Comprobante no encontrado: 2406202601019300081500120010010000000011234567819",
  "code": "NotFound"
}
```

**422 Unprocessable Entity** — estado inválido o transición no permitida:
```json
{
  "error": "Estado inválido: EstadoDesconocido",
  "code": "EstadoInvalido"
}
```

---

## Mapeo de estados

| Estado en SICE | Estado en Asapp | Endpoint |
|---|---|---|
| Enviado al SRI | `"Enviado"` | POST (al crear) |
| SRI responde RECIBIDA | `"Recibido"` | PATCH |
| SRI responde DEVUELTA | `"Devuelto"` | PATCH |
| SRI autoriza | `"Autorizado"` | PATCH |
| SRI no autoriza | `"NoAutorizado"` | PATCH |

Los únicos valores válidos para el campo `estado` en el PATCH son:
`"Recibido"`, `"Devuelto"`, `"Autorizado"`, `"NoAutorizado"`

---

## Ejemplos de implementación en C#

### Cliente HTTP reutilizable

```csharp
public class AsappSiceClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AsappSiceClient(HttpClient http, string apiKey)
    {
        _http   = http;
        _apiKey = apiKey;
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, object? body = null)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("X-Api-Key", _apiKey);

        if (body is not null)
            request.Content = JsonContent.Create(body);

        return request;
    }

    public async Task<AsappIngestResult> IngestComprobanteAsync(SiceAsappIngestDto dto)
    {
        var request  = BuildRequest(HttpMethod.Post, "v1/sice/comprobantes", dto);
        var response = await _http.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new AsappAuthException("API Key inválida o no configurada para esta empresa.");

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // 400 o 422 — loggear y continuar; no bloquear el flujo de SICE
            return new AsappIngestResult { Exito = false, Error = json };
        }

        var result = JsonSerializer.Deserialize<AsappIngestResponse>(json, JsonOptions);
        return new AsappIngestResult { Exito = true, ComprobanteId = result!.ComprobanteId };
    }

    public async Task<bool> UpdateEstadoAsync(string claveAcceso, object estadoBody)
    {
        var request  = BuildRequest(HttpMethod.Patch, $"v1/sice/comprobantes/{claveAcceso}", estadoBody);
        var response = await _http.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new AsappAuthException("API Key inválida o no configurada para esta empresa.");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            // El comprobante no existe en Asapp — loggear, no es un error crítico
            return false;
        }

        return response.IsSuccessStatusCode;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
```

### DTOs del lado SICE

```csharp
public class SiceAsappIngestDto
{
    [JsonPropertyName("xmlBase64")]
    public string XmlBase64 { get; set; } = string.Empty;

    [JsonPropertyName("tipoDocumento")]
    public string TipoDocumento { get; set; } = string.Empty;

    [JsonPropertyName("ambiente")]
    public string Ambiente { get; set; } = string.Empty;

    [JsonPropertyName("claveAcceso")]
    public string ClaveAcceso { get; set; } = string.Empty;

    [JsonPropertyName("numeroComprobante")]
    public string NumeroComprobante { get; set; } = string.Empty;

    [JsonPropertyName("fechaEmision")]
    public DateTimeOffset FechaEmision { get; set; }
}

public class SiceAsappUpdateRecibidoDto
{
    [JsonPropertyName("estado")]
    public string Estado => "Recibido";
}

public class SiceAsappUpdateDevueltoDto
{
    [JsonPropertyName("estado")]
    public string Estado => "Devuelto";

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;
}

public class SiceAsappUpdateAutorizadoDto
{
    [JsonPropertyName("estado")]
    public string Estado => "Autorizado";

    [JsonPropertyName("numeroAutorizacion")]
    public string NumeroAutorizacion { get; set; } = string.Empty;

    [JsonPropertyName("fechaAutorizacion")]
    public DateTimeOffset FechaAutorizacion { get; set; }

    [JsonPropertyName("xmlAutorizadoBase64")]
    public string XmlAutorizadoBase64 { get; set; } = string.Empty;
}

public class SiceAsappUpdateNoAutorizadoDto
{
    [JsonPropertyName("estado")]
    public string Estado => "NoAutorizado";

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;
}
```

### Ejemplo de uso en el flujo SRI de SICE

```csharp
// Después de enviar al SRI
public async Task ProcesarEnvioSriAsync(Comprobante comprobante, AsappSiceClient asapp)
{
    // [1] Informar a Asapp que el comprobante fue enviado
    await asapp.IngestComprobanteAsync(new SiceAsappIngestDto
    {
        XmlBase64         = Convert.ToBase64String(comprobante.XmlBytes),
        TipoDocumento     = comprobante.CodigoSri,          // "01", "04", etc.
        Ambiente          = comprobante.Ambiente,            // "Produccion"
        ClaveAcceso       = comprobante.ClaveAcceso,
        NumeroComprobante = comprobante.NumeroComprobante,
        FechaEmision      = comprobante.FechaEmision
    });
}

// Cuando SRI responde a la recepción
public async Task ProcesarRespuestaSriAsync(string claveAcceso, string estadoSri,
    string? mensaje, AsappSiceClient asapp)
{
    if (estadoSri == "RECIBIDA")
    {
        // [2a] Recibido
        await asapp.UpdateEstadoAsync(claveAcceso, new SiceAsappUpdateRecibidoDto());
    }
    else if (estadoSri == "DEVUELTA")
    {
        // [2b] Devuelto con mensaje de error
        await asapp.UpdateEstadoAsync(claveAcceso, new SiceAsappUpdateDevueltoDto
        {
            Mensaje = mensaje ?? string.Empty
        });
    }
}

// Cuando SRI responde a la consulta de autorización
public async Task ProcesarAutorizacionAsync(string claveAcceso, string estadoAutorizacion,
    string? numeroAutorizacion, DateTimeOffset? fechaAutorizacion,
    byte[]? xmlAutorizadoBytes, string? mensajeRechazo,
    AsappSiceClient asapp)
{
    if (estadoAutorizacion == "AUTORIZADO")
    {
        // [3a] Autorizado
        await asapp.UpdateEstadoAsync(claveAcceso, new SiceAsappUpdateAutorizadoDto
        {
            NumeroAutorizacion  = numeroAutorizacion!,
            FechaAutorizacion   = fechaAutorizacion!.Value,
            XmlAutorizadoBase64 = Convert.ToBase64String(xmlAutorizadoBytes!)
        });
    }
    else if (estadoAutorizacion == "NO AUTORIZADO")
    {
        // [3b] No autorizado
        await asapp.UpdateEstadoAsync(claveAcceso, new SiceAsappUpdateNoAutorizadoDto
        {
            Mensaje = mensajeRechazo ?? string.Empty
        });
    }
}
```

---

## Manejo de errores y reintentos

### Principio fundamental

**Las llamadas a Asapp nunca deben bloquear ni interrumpir el flujo normal de SICE.**
Si Asapp no está disponible o retorna error, SICE continúa normalmente y registra el fallo.

### Estrategia por código de respuesta

| Código | Acción en SICE |
|---|---|
| `200` / `201` | Éxito — continuar normalmente |
| `400` | Error en los datos enviados — loggear con detalle, no reintentar |
| `401` | API Key inválida — loggear como error crítico de configuración, alertar al administrador |
| `404` | Comprobante no existe en Asapp (solo en PATCH) — loggear, no es crítico |
| `422` | Error de negocio — loggear con el campo `error` de la respuesta, no reintentar |
| `500` / timeout | Error temporal — reintentar máximo 3 veces con backoff exponencial (1s, 3s, 9s) |

### Patrón de reintento recomendado

```csharp
public async Task<bool> CallAsappWithRetryAsync(Func<Task<bool>> call, string operacion, string claveAcceso)
{
    var intentos = 0;
    var maxIntentos = 3;

    while (intentos < maxIntentos)
    {
        try
        {
            return await call();
        }
        catch (HttpRequestException ex) when (intentos < maxIntentos - 1)
        {
            intentos++;
            var delay = TimeSpan.FromSeconds(Math.Pow(3, intentos));
            _logger.LogWarning("Asapp no disponible ({Op}, {Clave}). Reintento {N}/{Max} en {Delay}s",
                operacion, claveAcceso, intentos, maxIntentos, delay.TotalSeconds);
            await Task.Delay(delay);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error llamando a Asapp ({Op}, {Clave}). Se omite sin afectar SICE.",
                operacion, claveAcceso);
            return false;
        }
    }

    _logger.LogError("Asapp no disponible después de {Max} intentos ({Op}, {Clave}). Se omite.",
        maxIntentos, operacion, claveAcceso);
    return false;
}
```

---

## Registro de configuración en SICE

Para implementar correctamente el multi-empresa, SICE necesita una forma de resolver la API Key de Asapp por empresa. La implementación exacta depende de cómo SICE gestiona su configuración hoy, pero el dato que necesita es:

```
empresa.RUC → AsappApiKey + AsappBaseUrl
```

Opciones de implementación:
- Columna `AsappApiKey` y `AsappBaseUrl` en la tabla de empresas de SICE
- Sección en el archivo de configuración (`appsettings.json`) por empresa
- Variable de entorno por empresa si SICE corre instancias separadas

**Importante:** La API Key es por empresa en Asapp. Dos empresas distintas tienen API Keys distintas.

---

## Registro de llamadas (auditoría)

Se recomienda que SICE mantenga una tabla de log de llamadas a Asapp:

| Campo | Descripción |
|---|---|
| `ClaveAcceso` | Identificador del comprobante |
| `Endpoint` | `"Ingest"` o `"UpdateEstado"` |
| `EstadoEnviado` | Estado que se comunicó (Enviado, Recibido, etc.) |
| `HttpStatus` | Código de respuesta de Asapp |
| `FechaHora` | Timestamp de la llamada |
| `Error` | Mensaje de error si hubo fallo |

Esto permite diagnosticar problemas durante el período de pruebas sin depender de los logs de Asapp.

---

## Checklist de implementación

- [ ] Agregar campos `AsappApiKey` y `AsappBaseUrl` a la configuración por empresa en SICE
- [ ] Crear clase `AsappSiceClient` con `HttpClient` inyectado vía DI
- [ ] Registrar `AsappSiceClient` en el contenedor DI (uno por empresa o con factory)
- [ ] Implementar DTOs: `SiceAsappIngestDto`, `SiceAsappUpdateRecibidoDto`, `SiceAsappUpdateDevueltoDto`, `SiceAsappUpdateAutorizadoDto`, `SiceAsappUpdateNoAutorizadoDto`
- [ ] En el paso de envío al SRI: llamar `IngestComprobanteAsync` antes o después del envío (recomendado: después, cuando el estado ya es "Enviado")
- [ ] En el callback/poll de recepción SRI: llamar `UpdateEstadoAsync` con `"Recibido"` o `"Devuelto"`
- [ ] En el callback/poll de autorización SRI: llamar `UpdateEstadoAsync` con `"Autorizado"` o `"NoAutorizado"`
- [ ] Implementar manejo de errores: las llamadas a Asapp no bloquean el flujo de SICE
- [ ] Implementar reintentos para errores 5xx/timeout (máx. 3 intentos, backoff exponencial)
- [ ] Crear tabla de auditoría de llamadas a Asapp
- [ ] Configurar API Key de prueba para la empresa de test
- [ ] Probar el flujo completo en staging con un comprobante real
- [ ] Verificar en Asapp que el comprobante aparece con historial completo

---

## Contacto y acceso

- **API Key de staging:** solicitarla a Cristhian antes de las pruebas
- **URL de staging:** proporcionada por Cristhian
- **Empresa de prueba:** la primera empresa que se conecte en modo shadow

---

*Documento generado el 2026-06-19. Versión de la API: Asapp Electronic v1 — endpoints `/v1/sice/*`.*
