-- ============================================================
-- RUC PROVEEDOR DE FACTURACION ELECTRONICA - PostgreSQL
-- Proyecto: SICE
-- Resolucion SRI NAC-DGERCGC26-00000027 / Ficha Tecnica Offline v2.34, Anexo 26
-- ============================================================
-- Idempotente: se puede ejecutar mas de una vez sin error.
-- ============================================================


-- ------------------------------------------------------------
-- Parametro global: RUC de SICE como proveedor de sistema
-- informatico de facturacion electronica. Se inyecta como
-- <campoAdicional nombre="RUC Proveedor"> en infoAdicional de
-- todos los comprobantes emitidos.
-- ------------------------------------------------------------

INSERT INTO parametro (par_empresa, par_id, par_descripcion, par_tipo, par_valor, par_estado, crea_usr, crea_fecha)
VALUES (
    1,
    'rucProveedorFacturacion',
    'RUC de SICE como proveedor de sistema de facturacion electronica (Resolucion NAC-DGERCGC26-00000027)',
    'TEXT',
    '0103567665001',
    1,
    'SISTEMA',
    NOW()
)
ON CONFLICT (par_empresa, par_id) DO NOTHING;
