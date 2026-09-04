-- ============================================================
-- ASAPP INTEGRATION - SQL Server
-- Proyecto: SICE
-- ============================================================
-- Idempotente: se puede ejecutar más de una vez sin error.
-- ============================================================


-- ------------------------------------------------------------
-- 1. Columnas nuevas en empresa
--    emp_asapp_activo : 1 = integración habilitada para esta empresa
--    emp_asapp_apikey : API Key de Asapp asignada a esta empresa
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('empresa') AND name = 'emp_asapp_activo'
)
    ALTER TABLE empresa ADD emp_asapp_activo INT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('empresa') AND name = 'emp_asapp_apikey'
)
    ALTER TABLE empresa ADD emp_asapp_apikey VARCHAR(200) NULL;


-- ------------------------------------------------------------
-- 2. Parametro global de Asapp
--    URL base compartida por todas las empresas.
-- ------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1 FROM parametro WHERE par_empresa = 1 AND par_id = 'asapp'
)
    INSERT INTO parametro (par_empresa, par_id, par_descripcion, par_tipo, par_valor, par_estado, crea_usr, crea_fecha)
    VALUES (
        1,
        'asapp',
        'Configuracion integracion Asapp Electronic',
        'JSON',
        '{"baseUrl":"https://electronic-api-dev.asapp.com.ec/api"}',
        1,
        'SISTEMA',
        GETDATE()
    );


-- ------------------------------------------------------------
-- 3. Tabla asapp_log
--    Un registro por cada llamada realizada a Asapp
-- ------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('asapp_log') AND type = 'U')
    CREATE TABLE asapp_log (
        asl_id          INT           IDENTITY(1,1) PRIMARY KEY,
        asl_empresa     INT           NOT NULL,
        asl_claveacceso VARCHAR(49)   NOT NULL,
        asl_endpoint    VARCHAR(20)   NOT NULL,
        asl_estado      VARCHAR(20)   NULL,
        asl_httpstatus  INT           NULL,
        asl_fecha       DATETIME      DEFAULT GETDATE(),
        asl_error       VARCHAR(1000) NULL
    );

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_asl_empresa_fecha')
    CREATE INDEX idx_asl_empresa_fecha ON asapp_log (asl_empresa, asl_fecha DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_asl_claveacceso')
    CREATE INDEX idx_asl_claveacceso ON asapp_log (asl_claveacceso);
