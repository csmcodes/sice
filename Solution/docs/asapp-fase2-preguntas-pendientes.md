# Respuesta a Asapp — Fase 2 (modo delegado)

## 1. Formato de `xmlAutorizadoBase64`

Confirmado, gracias. El wrapper que describen (`<autorizaciones><autorizacion>...<comprobante><![CDATA[...]]></comprobante>...`)
coincide exactamente con lo que `wfGetXML.aspx` espera. No se requiere ningún ajuste de
nuestro lado. Este punto queda cerrado.

## 2. Empresa/RUC piloto

Confirmado: **Transportes Ortiz, RUC 0190050858001**. Vamos a activar
`emp_asapp_modo=2` para esa empresa del lado de SICE cuando estemos listos para probar
end-to-end.

## 3. URL del callback para pruebas

Aclaración: SICE **no usa Azure App Configuration**. La WebAPI está publicada
manualmente en un VPS, con un `Web.config` estático — no hay un config service externo
ni "labels" de ambiente. La URL es fija y es la misma tanto para pruebas como para
producción:

```
https://siceapi.tao.com.ec/api/comprobante/{claveAcceso}/resultado
```

Confirmado entonces: **sí, usen `https://siceapi.tao.com.ec` para todo** (pruebas y
producción), autenticándose con el usuario `api` que ya les compartimos.

---

## Próximos pasos de nuestro lado

- Aplicar las migraciones SQL (`docs/sql/02_asapp_delegado_*.sql`) en el ambiente de
  Pruebas.
- Activar `emp_asapp_modo=2` para Transportes Ortiz (0190050858001) solo en Pruebas
  (`ambiente="Pruebas"` en el payload) hasta validar el flujo completo.
- Avisarles cuando esté activo para coordinar la primera prueba end-to-end.

*Respuesta a `docs/asapp-fase2-preguntas-pendientes.md` — preguntas originales del
2026-07-08.*
