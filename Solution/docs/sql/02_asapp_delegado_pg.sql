-- ============================================================
-- ASAPP INTEGRATION - FASE 2 (MODO DELEGADO) - PostgreSQL
-- Proyecto: SICE
-- ============================================================
-- Idempotente: se puede ejecutar más de una vez sin error.
-- ============================================================


-- ------------------------------------------------------------
-- 1. Columna nueva en empresa
--    emp_asapp_modo : NULL o 1 = shadow (comportamiento actual)
--                     2        = delegado (Asapp firma/envía/gestiona con el SRI)
-- ------------------------------------------------------------

ALTER TABLE empresa
    ADD COLUMN IF NOT EXISTS emp_asapp_modo INTEGER;

-- Backfill de claridad (no cambia comportamiento: NULL ya se trata como shadow)
UPDATE empresa SET emp_asapp_modo = 1 WHERE emp_asapp_activo = 1 AND emp_asapp_modo IS NULL;

COMMENT ON COLUMN empresa.emp_asapp_modo IS 'NULL/1=shadow (notifica), 2=delegado (Asapp firma/envia/gestiona SRI)';


-- ------------------------------------------------------------
-- 2. Columnas nuevas en asapp_log
--    asl_direccion : 'OUT' (SICE -> Asapp) | 'IN' (Asapp -> SICE). NULL historico = 'OUT'
--    asl_payload   : payload crudo (solo para llamadas entrantes), truncado por la app
-- ------------------------------------------------------------

ALTER TABLE asapp_log
    ADD COLUMN IF NOT EXISTS asl_direccion VARCHAR(10),
    ADD COLUMN IF NOT EXISTS asl_payload   TEXT;

COMMENT ON COLUMN asapp_log.asl_direccion IS 'OUT=SICE->Asapp (default historico) | IN=Asapp->SICE';
COMMENT ON COLUMN asapp_log.asl_payload   IS 'Payload crudo recibido (solo direccion IN), truncado por la app';
