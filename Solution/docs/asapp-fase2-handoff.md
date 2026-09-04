# SICE → Asapp — Fase 2: Modo Delegado (gestión electrónica completa)

> Este documento es la fuente de verdad para que el equipo/agente de Asapp implemente su lado
> de la Fase 2 de la integración con SICE. Está escrito para ser leído por un agente AI
> (Claude Code u otro) que trabajará en el proyecto Asapp.

---

## Contexto

**SICE** es el sistema legacy de facturación electrónica (Ecuador, SRI) desde el que se está
migrando la gestión electrónica hacia **Asapp**.

Ya existe una integración en producción ("Fase 1 — Shadow mode"): SICE sigue haciendo todo
el trabajo (firma, envío al SRI, verificación de autorización) y solo **notifica** a Asapp en
cada cambio de estado, para que Asapp tenga una copia fiel del historial. Esa fase no cambia.

**Fase 2 (lo que se acaba de implementar del lado de SICE, y que motiva este documento):**
para empresas designadas, SICE deja de firmar y enviar al SRI. En su lugar:

```
SICE recibe el comprobante de su cliente
    │
    ▼
SICE arma el XML del comprobante (SIN FIRMAR)
    │
    ▼
SICE llama a Asapp: "aquí está el XML, gestiónalo tú" ──► [Endpoint A, ver abajo]
    │
    ▼
Asapp firma el XML, lo envía al SRI, gestiona reintentos/verificación de autorización
    │
    ▼
Asapp le informa a SICE el resultado final ──► [Endpoint B, ver abajo — YA EXISTE, implementado por SICE]
    │
    ▼
SICE actualiza su BD, genera el RIDE/PDF y lo envía por correo al cliente (sin cambios en esa parte)
```

Esto se controla **por empresa** con un flag en SICE (`Empresa.emp_asapp_modo`), no es un
cambio global — algunas empresas pueden seguir en shadow mode (Fase 1) mientras otras ya
están en modo delegado (Fase 2). Ninguna empresa se activa en delegado sin coordinación previa.

---

## Endpoint A — SICE → Asapp (hand-off del XML sin firmar)

Este es el endpoint que **Asapp debe implementar/confirmar** para recibir el XML desde SICE.
El contrato propuesto abajo sigue el mismo estilo que el endpoint de ingesta ya existente de
la Fase 1 (`POST /v1/sice/comprobantes`), pero es una ruta nueva porque el significado es
distinto: en Fase 1 el XML que llega ya fue enviado al SRI por SICE (solo es notificación);
en Fase 2 el XML llega **sin firmar y sin enviar**, y Asapp es responsable de todo el ciclo
con el SRI a partir de ahí.

```
POST {baseUrl}/v1/sice/comprobantes/delegados
X-Api-Key: {api_key_de_la_empresa}     <- misma API Key por empresa que ya usa Fase 1
Content-Type: application/json
```

### Body propuesto

```json
{
  "xmlSinFirmarBase64": "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4...",
  "tipoDocumento": "01",
  "ambiente": "Produccion",
  "claveAcceso": "2406202601019300081500120010010000000011234567819",
  "numeroComprobante": "001-001-000000001",
  "fechaEmision": "2026-07-07T10:00:00-05:00",
  "emailCliente": "cliente@dominio.com"
}
```

| Campo | Tipo | Descripción |
|---|---|---|
| `xmlSinFirmarBase64` | string | XML del comprobante **sin firma digital**, codificado en Base64. Asapp debe firmarlo con su propio flujo de certificado antes de enviarlo al SRI. |
| `tipoDocumento` | string | Código SRI: `"01"`=Factura, `"03"`=Liquidación de compra, `"04"`=Nota de crédito, `"05"`=Nota de débito, `"06"`=Guía de remisión, `"07"`=Retención |
| `ambiente` | string | `"Pruebas"` o `"Produccion"` — determina si Asapp debe enviar al ambiente de pruebas u offline del SRI |
| `claveAcceso` | string | Clave de acceso de 49 dígitos generada por SICE — es el identificador único del comprobante en todo el flujo |
| `numeroComprobante` | string | Formato `"001-001-000000001"` (establecimiento-puntoEmision-secuencial) |
| `fechaEmision` | string ISO 8601 | Con offset Ecuador `-05:00` |
| `emailCliente` | string | Email del receptor del comprobante (puede venir vacío) |

### Respuesta esperada

- **200/201** — aceptado, idempotente por `claveAcceso` (si SICE reintenta el hand-off por timeout, Asapp no debe procesarlo dos veces).
- **400** — payload inválido.
- **401** — API Key inválida.
- **422** — error de negocio (XML malformado, empresa no habilitada para modo delegado en Asapp, etc).

**⚠️ Este contrato (ruta, nombres de campos, formato de idempotencia) está PROPUESTO por
SICE y no ha sido validado ni implementado por Asapp todavía.** Si Asapp prefiere otra ruta,
otro nombre de campo, u otro mecanismo de confirmación de recepción, se debe coordinar el
cambio y actualizar el código de `AsappClient.EnviarDelegado` en SICE en consecuencia.

---

## Endpoint B — Asapp → SICE (resultado final) — **YA IMPLEMENTADO por SICE**

Este es el endpoint que **Asapp debe consumir** cuando termine de gestionar el comprobante
con el SRI (autorizado, rechazado, o devuelto por errores formales). Ya está en el código de
SICE y compilado; falta solo que Asapp lo integre y coordinar el despliegue a producción.

### Autenticación

Es el **mismo mecanismo de login JWT** que Asapp ya usa hoy para el endpoint de consulta de
estado de la Fase 1 (`GET /api/comprobante/{claveAcceso}/estado`). No hay que implementar
nada nuevo de autenticación:

```
POST https://siceapi.tao.com.ec/api/login
Content-Type: application/json

{ "username": "api", "password": "{se comparte por canal seguro, no está en este documento}" }

→ 200 OK
{ "token": "eyJ...", "expireMinutes": 30 }
```

El token expira en 30 minutos — Asapp debe volver a loguearse cuando expire (o antes de cada
lote de callbacks si no quiere manejar expiración fina).

### Endpoint de resultado

```
POST https://siceapi.tao.com.ec/api/comprobante/{claveAcceso}/resultado
Authorization: Bearer {token}
Content-Type: application/json
```

`{claveAcceso}` es la clave de acceso de 49 dígitos del comprobante (la misma que SICE le
mandó en el Endpoint A).

### Body

```json
{
  "resultado": "AUTORIZADO",
  "numeroAutorizacion": "2406202601019300081500120010010000000011234567819",
  "fechaAutorizacion": "2026-07-07T14:32:10-05:00",
  "xmlAutorizadoBase64": "PGF1dG9yaXphY2lvbmVzPjxhdXRvcml6YWNpb24+...",
  "mensaje": null,
  "referenciaAsapp": "asapp-job-98213"
}
```

| Campo | Tipo | Requerido | Descripción |
|---|---|---|---|
| `resultado` | string | ✅ siempre | Únicos valores válidos: `"AUTORIZADO"` \| `"NOAUTORIZADO"` \| `"DEVUELTO"` |
| `numeroAutorizacion` | string | ✅ si `resultado="AUTORIZADO"` | Número de autorización del SRI (normalmente igual a la clave de acceso) |
| `fechaAutorizacion` | string ISO 8601 | ✅ si `resultado="AUTORIZADO"` | Con offset Ecuador `-05:00` |
| `xmlAutorizadoBase64` | string | ✅ si `resultado="AUTORIZADO"` | Ver **contrato crítico del XML** más abajo — es obligatorio |
| `mensaje` | string | ✅ si `resultado="NOAUTORIZADO"` o `"DEVUELTO"` | Motivo del rechazo/devolución que dio el SRI |
| `referenciaAsapp` | string | opcional | Solo trazabilidad, se guarda en el log de auditoría de SICE, no afecta la lógica |

**Distinción `NOAUTORIZADO` vs `DEVUELTO`:**
- `DEVUELTO` = el SRI rechazó el comprobante en la etapa de **recepción** (errores formales, ej. RUC del emisor no coincide con el certificado).
- `NOAUTORIZADO` = el SRI recibió el comprobante correctamente pero lo **rechazó en la etapa de autorización** (ej. RUC del emisor suspendido).

Si Asapp solo maneja un estado de rechazo internamente, mapear a `NOAUTORIZADO` por defecto,
pero se prefiere la distinción si Asapp ya la tiene disponible.

### ⚠️ Contrato crítico del XML autorizado — LEER CON ATENCIÓN

`xmlAutorizadoBase64` (una vez decodificado de Base64) **debe ser un XML completo con esta
forma exacta** — el mismo formato que el SRI devuelve nativamente en su respuesta de
autorización:

```xml
<autorizaciones>
  <autorizacion>
    <estado>AUTORIZADO</estado>
    <numeroAutorizacion>...</numeroAutorizacion>
    <fechaAutorizacion>...</fechaAutorizacion>
    <ambiente>...</ambiente>
    <comprobante><![CDATA[<?xml version="1.0" encoding="UTF-8"?><factura>...</factura>]]></comprobante>
  </autorizacion>
</autorizaciones>
```

Esto **no es opcional ni cosmético**: SICE tiene una pantalla de descarga de XML para el
cliente final (`wfGetXML.aspx`) que parsea `xmlAutorizadoBase64` esperando exactamente este
wrapper (`<autorizaciones><autorizacion>...<comprobante><![CDATA[...]]></comprobante>...`).
Si Asapp entrega el XML en otra forma (por ejemplo, solo el `<factura>` interno sin el
wrapper, o sin el CDATA), la descarga de XML del cliente se rompe. Si Asapp no puede replicar
esta forma exacta, avisar ANTES de activar cualquier empresa en producción para adaptar el
parser del lado de SICE.

### Respuesta de SICE

**200 OK** — procesado correctamente:
```json
{ "claveAcceso": "...", "estado": "AUTORIZADO", "yaProcesado": false }
```

Si Asapp reintenta el mismo callback (por timeout de red, por ejemplo) con el mismo
`resultado`, SICE responde igual **200** pero con `"yaProcesado": true` — no vuelve a mutar el
comprobante ni reenvía el correo/RIDE al cliente. Es seguro reintentar.

**400 Bad Request** — datos inválidos (falta `resultado`, o falta un campo requerido según el
`resultado`, o el XML en `xmlAutorizadoBase64` no es Base64/XML válido):
```json
{ "error": "...", "code": "DatosInvalidos" }
```

**404 Not Found** — no existe en SICE un comprobante con esa `claveAcceso`:
```json
{ "error": "Comprobante no encontrado: ...", "code": "NotFound" }
```

**409 Conflict** — dos casos posibles:
```json
{ "error": "...", "code": "EmpresaNoDelegada" }
```
la empresa de ese comprobante ya no está en modo delegado del lado de SICE (se revirtió a
shadow o se desactivó) — Asapp no debería seguir gestionando ese comprobante, hay que
coordinar manualmente.
```json
{ "error": "...", "code": "ConflictoEstado" }
```
el comprobante ya tiene en SICE un estado final **distinto** al que Asapp está reportando
(por ejemplo Asapp manda `NOAUTORIZADO` pero SICE ya tiene `AUTORIZADO` registrado) — señal de
un problema a investigar, no reintentar automáticamente.

**500** — error interno de SICE, reintentar con backoff.

### Estrategia de reintentos recomendada para Asapp

Igual que la que SICE ya usa para llamar a Asapp en Fase 1: máximo 3 intentos con backoff
exponencial (1s, 3s, 9s) ante `500`/timeout/sin respuesta. Ante `400`/`409` no reintentar
automáticamente — son errores de datos o de estado que requieren revisión manual. Ante `404`
tampoco reintentar (el comprobante no existe en SICE, algo está desincronizado).

---

## Checklist para el equipo/agente de Asapp

- [ ] Confirmar o renegociar el contrato del **Endpoint A** (`POST /v1/sice/comprobantes/delegados`) — ¿la ruta y los campos propuestos funcionan, o Asapp prefiere otra forma?
- [ ] Implementar la recepción del Endpoint A: al recibir el XML sin firmar, encolar/gestionar la firma + envío al SRI con el flujo propio de Asapp (igual que ya hacen para sus propios clientes, pero por cuenta de SICE)
- [ ] Implementar el login contra `POST https://siceapi.tao.com.ec/api/login` (credenciales se comparten por canal seguro aparte de este documento) y manejo de expiración del token (30 min)
- [ ] Implementar la llamada al **Endpoint B** (`POST /api/comprobante/{claveAcceso}/resultado`) en los 3 casos: `AUTORIZADO`, `NOAUTORIZADO`, `DEVUELTO`
- [ ] Verificar que el XML que arma Asapp para `xmlAutorizadoBase64` respeta el wrapper `<autorizaciones><autorizacion>...<comprobante><![CDATA[...]]></comprobante></autorizacion></autorizaciones>` exacto
- [ ] Implementar reintentos con backoff ante 500/timeout, sin reintento automático ante 400/404/409
- [ ] Probar el flujo completo en el ambiente de **Pruebas** del SRI (`ambiente="Pruebas"`) con una empresa piloto acordada entre ambos equipos, antes de cualquier activación en producción
- [ ] Confirmar con el equipo de SICE (Cristhian) la URL exacta de `baseUrl` de staging/producción de SICE si difiere de `https://siceapi.tao.com.ec`, y las credenciales del usuario `api`

---

*Documento generado el 2026-07-07 desde el proyecto SICE, como contraparte del documento
`docs/sice-asapp-integration.md` (Fase 1, ya implementada). Fase 2 implementada en branch
`develop` del lado de SICE — pendiente de merge a `main` y de coordinación de contrato con
Asapp antes de activar cualquier empresa en modo delegado en producción.*
