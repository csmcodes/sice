-- ============================================================
-- ASAPP INTEGRATION - PostgreSQL
-- Proyecto: SICE
-- ============================================================
-- Idempotente: se puede ejecutar más de una vez sin error.
-- ============================================================


-- ------------------------------------------------------------
-- 1. Columnas nuevas en empresa
--    emp_asapp_activo : 1 = integración habilitada para esta empresa
--    emp_asapp_apikey : API Key de Asapp asignada a esta empresa
-- ------------------------------------------------------------

ALTER TABLE empresa
    ADD COLUMN IF NOT EXISTS emp_asapp_activo  INTEGER      DEFAULT 0,
    ADD COLUMN IF NOT EXISTS emp_asapp_apikey  VARCHAR(200);

COMMENT ON COLUMN empresa.emp_asapp_activo IS '1 = integracion Asapp activa para esta empresa';
COMMENT ON COLUMN empresa.emp_asapp_apikey IS 'API Key de Asapp asignada a esta empresa (X-Api-Key)';


-- ------------------------------------------------------------
-- 2. Parametro global de Asapp
--    URL base compartida por todas las empresas.
-- ------------------------------------------------------------

INSERT INTO parametro (par_empresa, par_id, par_descripcion, par_tipo, par_valor, par_estado, crea_usr, crea_fecha)
VALUES (
    1,
    'asapp',
    'Configuracion integracion Asapp Electronic',
    'JSON',
    '{"baseUrl":"https://electronic-api-dev.asapp.com.ec/api"}',
    1,
    'SISTEMA',
    NOW()
)
ON CONFLICT (par_empresa, par_id) DO NOTHING;


-- ------------------------------------------------------------
-- 3. Tabla asapp_log
--    Un registro por cada llamada realizada a Asapp
-- ------------------------------------------------------------

CREATE TABLE IF NOT EXISTS asapp_log (
    asl_id          SERIAL        PRIMARY KEY,
    asl_empresa     INTEGER       NOT NULL,
    asl_claveacceso VARCHAR(49)   NOT NULL,
    asl_endpoint    VARCHAR(20)   NOT NULL,   -- Ingest | UpdateEstado
    asl_estado      VARCHAR(20),              -- Enviado | Recibido | Devuelto | Autorizado | NoAutorizado
    asl_httpstatus  INTEGER,
    asl_fecha       TIMESTAMP     DEFAULT NOW(),
    asl_error       VARCHAR(1000)
);

CREATE INDEX IF NOT EXISTS idx_asl_empresa_fecha
    ON asapp_log (asl_empresa, asl_fecha DESC);

CREATE INDEX IF NOT EXISTS idx_asl_claveacceso
    ON asapp_log (asl_claveacceso);

COMMENT ON TABLE  asapp_log              IS 'Auditoria de llamadas HTTP realizadas a Asapp por SICE';
COMMENT ON COLUMN asapp_log.asl_endpoint IS 'Ingest=POST crear comprobante | UpdateEstado=PATCH cambio de estado';
