-- ============================================================
-- ASAPP INTEGRATION - FASE 2 (MODO DELEGADO) - SQL Server
-- Proyecto: SICE
-- ============================================================
-- Idempotente: se puede ejecutar más de una vez sin error.
-- ============================================================


-- ------------------------------------------------------------
-- 1. Columna nueva en empresa
--    emp_asapp_modo : NULL o 1 = shadow (comportamiento actual)
--                     2        = delegado (Asapp firma/envía/gestiona con el SRI)
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('empresa') AND name = 'emp_asapp_modo'
)
    ALTER TABLE empresa ADD emp_asapp_modo INT NULL;

-- Backfill de claridad (no cambia comportamiento: NULL ya se trata como shadow)
UPDATE empresa SET emp_asapp_modo = 1 WHERE emp_asapp_activo = 1 AND emp_asapp_modo IS NULL;


-- ------------------------------------------------------------
-- 2. Columnas nuevas en asapp_log
--    asl_direccion : 'OUT' (SICE -> Asapp) | 'IN' (Asapp -> SICE). NULL historico = 'OUT'
--    asl_payload   : payload crudo (solo para llamadas entrantes), truncado por la app
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('asapp_log') AND name = 'asl_direccion'
)
    ALTER TABLE asapp_log ADD asl_direccion VARCHAR(10) NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('asapp_log') AND name = 'asl_payload'
)
    ALTER TABLE asapp_log ADD asl_payload VARCHAR(MAX) NULL;
