-- ============================================================
-- AUTOMATA SRI - Script de base de datos
-- Motor: PostgreSQL
-- Proyecto: SICE
-- ============================================================
-- Ejecutar en orden. Es idempotente (IF NOT EXISTS / ON CONFLICT).
-- ============================================================


-- ------------------------------------------------------------
-- 1. Columnas nuevas en comprobante
-- ------------------------------------------------------------

ALTER TABLE comprobante
    ADD COLUMN IF NOT EXISTS com_reintentos            INTEGER   DEFAULT 0,
    ADD COLUMN IF NOT EXISTS com_fechaultimointento    TIMESTAMP;

COMMENT ON COLUMN comprobante.com_reintentos         IS 'Contador de intentos del automata para este comprobante';
COMMENT ON COLUMN comprobante.com_fechaultimointento IS 'Ultima vez que el automata proceso este comprobante';


-- ------------------------------------------------------------
-- 2. Tabla sri_regla_mensaje
--    Reglas para clasificar mensajes de error del SRI
--    El clasificador evalua com_mensaje contra srm_patron
--    en orden ascendente de srm_orden (menor = primero)
-- ------------------------------------------------------------

CREATE TABLE IF NOT EXISTS sri_regla_mensaje (
    srm_id          SERIAL        PRIMARY KEY,
    srm_patron      VARCHAR(500)  NOT NULL,
    srm_accion      VARCHAR(20)   NOT NULL,    -- REENVIAR | VERIFICAR | ALERTAR | IGNORAR
    srm_estado      INTEGER,                   -- estado del comprobante al que aplica (NULL = todos)
    srm_orden       INTEGER       DEFAULT 0,   -- orden de evaluacion (menor = primero)
    srm_descripcion VARCHAR(500),
    srm_activo      BOOLEAN       DEFAULT TRUE
);

CREATE INDEX IF NOT EXISTS idx_srm_estado_orden
    ON sri_regla_mensaje (srm_estado, srm_orden ASC);

COMMENT ON TABLE  sri_regla_mensaje           IS 'Reglas de clasificacion de mensajes de error del SRI para el automata';
COMMENT ON COLUMN sri_regla_mensaje.srm_accion IS 'REENVIAR=error transitorio reenviar XML | VERIFICAR=consultar autorizacion | ALERTAR=requiere intervencion manual | IGNORAR=omitir';
COMMENT ON COLUMN sri_regla_mensaje.srm_estado IS 'Estado del comprobante al que aplica la regla. NULL = aplica a todos los estados';


-- ------------------------------------------------------------
-- 3. Tabla automata_ejecucion
--    Un registro por empresa por cada corrida del automata
-- ------------------------------------------------------------

CREATE TABLE IF NOT EXISTS automata_ejecucion (
    ae_id           SERIAL        PRIMARY KEY,
    ae_empresa      INTEGER       NOT NULL,
    ae_fecha_inicio TIMESTAMP     NOT NULL,
    ae_fecha_fin    TIMESTAMP,
    ae_procesados   INTEGER       DEFAULT 0,
    ae_autorizados  INTEGER       DEFAULT 0,
    ae_alertas      INTEGER       DEFAULT 0,
    ae_simulacion   BOOLEAN       DEFAULT FALSE
);

CREATE INDEX IF NOT EXISTS idx_ae_empresa_fecha
    ON automata_ejecucion (ae_empresa, ae_fecha_inicio DESC);

COMMENT ON TABLE automata_ejecucion IS 'Resumen de cada ejecucion del automata SRI por empresa';


-- ------------------------------------------------------------
-- 4. Tabla automata_log
--    Un registro por comprobante procesado en cada ejecucion
--    NOTA: los resultados ALERTA no se graban aqui,
--    solo se cuentan en ae_alertas de automata_ejecucion
-- ------------------------------------------------------------

CREATE TABLE IF NOT EXISTS automata_log (
    al_id             SERIAL        PRIMARY KEY,
    al_ejecucion      INTEGER       REFERENCES automata_ejecucion(ae_id),
    al_empresa        INTEGER       NOT NULL,
    al_numero         VARCHAR(50)   NOT NULL,   -- com_numero (clave de acceso SRI)
    al_numero_legible VARCHAR(100),             -- ej: "FAC 001-001-000000001"
    al_estado_inicial INTEGER,
    al_estado_final   INTEGER,
    al_accion         VARCHAR(20),              -- REENVIAR | VERIFICAR | IGNORAR
    al_resultado      VARCHAR(20),              -- OK | ERROR | LIMITE | SIMULACION
    al_mensaje        VARCHAR(500),
    al_fecha          TIMESTAMP     DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_al_empresa_fecha
    ON automata_log (al_empresa, al_fecha DESC);

COMMENT ON TABLE  automata_log                IS 'Detalle de comprobantes procesados por el automata SRI';
COMMENT ON COLUMN automata_log.al_numero      IS 'com_numero: clave de acceso SRI del comprobante';
COMMENT ON COLUMN automata_log.al_numero_legible IS 'Numero legible ej: FAC 001-001-000000001';
COMMENT ON COLUMN automata_log.al_resultado   IS 'OK=exitoso | ERROR=fallo | LIMITE=max reintentos alcanzado | SIMULACION=dry run';


-- ------------------------------------------------------------
-- 5. Reglas iniciales en sri_regla_mensaje
--    Estado 4 = DEVUELTO
--    Estado 6 = NOAUTORIZADO
-- ------------------------------------------------------------

-- Limpiar reglas existentes para reinsertar (idempotente)
DELETE FROM sri_regla_mensaje;

-- Estado DEVUELTO (4): errores transitorios -> REENVIAR
INSERT INTO sri_regla_mensaje (srm_patron, srm_accion, srm_estado, srm_orden, srm_descripcion) VALUES
    ('CLAVE ACCESO REGISTRADA',   'VERIFICAR',  4,  10, 'Ya existe en SRI, consultar autorizacion'),
    ('SERVICIO NO DISPONIBLE',    'REENVIAR',   4,  20, 'Servicio SRI no disponible, reintentar'),
    ('SISTEMA EN MANTENIMIENTO',  'REENVIAR',   4,  30, 'SRI en mantenimiento, reintentar'),
    ('TIMEOUT',                   'REENVIAR',   4,  40, 'Timeout de conexion, reintentar'),
    ('CLAVE DE ACCESO NO VALIDA', 'ALERTAR',    4,  50, 'Clave de acceso invalida, revision manual'),
    ('FECHA DE EMISION',          'ALERTAR',    4,  60, 'Problema con fecha de emision'),
    ('RUC NO EXISTE',             'ALERTAR',    4,  70, 'RUC del emisor no existe en SRI'),
    ('RUC INVALIDO',              'ALERTAR',    4,  80, 'RUC invalido'),
    ('NUMERO DE COMPROBANTE',     'ALERTAR',    4,  90, 'Problema con numero de comprobante'),
    ('ERROR DE FIRMA',            'ALERTAR',    4, 100, 'Error en firma digital'),
    ('CERTIFICADO CADUCADO',      'ALERTAR',    4, 110, 'Certificado digital caducado'),
    ('CERTIFICADO REVOCADO',      'ALERTAR',    4, 120, 'Certificado digital revocado'),
    ('TIPO DE COMPROBANTE',       'ALERTAR',    4, 130, 'Tipo de comprobante incorrecto'),
    ('AMBIENTE',                  'ALERTAR',    4, 140, 'Ambiente incorrecto (pruebas/produccion)');

-- Estado NOAUTORIZADO (6): posibles duplicados -> VERIFICAR
INSERT INTO sri_regla_mensaje (srm_patron, srm_accion, srm_estado, srm_orden, srm_descripcion) VALUES
    ('NUMERO DE AUTORIZACION YA EXISTE',        'VERIFICAR', 6,  10, 'Autorizacion ya existe, consultar'),
    ('COMPROBANTE DUPLICADO',                   'VERIFICAR', 6,  20, 'Comprobante duplicado, consultar autorizacion'),
    ('RUC EMISOR NO EXISTE',                    'ALERTAR',   6,  30, 'RUC del emisor no existe'),
    ('RUC EMISOR INACTIVO',                     'ALERTAR',   6,  40, 'RUC del emisor inactivo'),
    ('ESTABLECIMIENTO NO EXISTE',               'ALERTAR',   6,  50, 'Establecimiento no registrado en SRI'),
    ('PUNTO DE EMISION NO EXISTE',              'ALERTAR',   6,  60, 'Punto de emision no registrado'),
    ('VALOR TOTAL INCORRECTO',                  'ALERTAR',   6,  70, 'Valor total del comprobante incorrecto'),
    ('IMPUESTO INVALIDO',                       'ALERTAR',   6,  80, 'Impuesto invalido en el comprobante'),
    ('FECHA DE EMISION NO CORRESPONDE',         'ALERTAR',   6,  90, 'Fecha de emision fuera de rango permitido'),
    ('ERROR EN LA IDENTIFICACION DEL RECEPTOR', 'ALERTAR',   6, 100, 'Identificacion del receptor incorrecta');


-- ------------------------------------------------------------
-- 6. Parametro automatasri - unico, en empresa 1
--    La config se lee siempre desde par_empresa=1.
--    simulacion: true -> arrancar en modo dry run
--    Cambiar a false cuando este validado en produccion
-- ------------------------------------------------------------

INSERT INTO parametro (par_empresa, par_id, par_descripcion, par_tipo, par_valor, par_estado, crea_usr, crea_fecha)
VALUES (
    1,
    'automatasri',
    'Configuracion del Automata SRI',
    'JSON',
    '{"activo":true,"simulacion":true,"dias_atras":30,"dias_retencion_log":90,"max_reintentos_enviado":5,"max_reintentos_devuelto":3,"minutos_espera_enviado":30,"minutos_espera_recibido":15,"horas_alerta_recibido":48}',
    1,
    'SISTEMA',
    NOW()
)
ON CONFLICT (par_empresa, par_id) DO NOTHING;
