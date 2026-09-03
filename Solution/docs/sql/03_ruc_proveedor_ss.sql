-- ============================================================
-- RUC PROVEEDOR DE FACTURACION ELECTRONICA - SQL Server
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

IF NOT EXISTS (
    SELECT 1 FROM parametro WHERE par_empresa = 1 AND par_id = 'rucProveedorFacturacion'
)
    INSERT INTO parametro (par_empresa, par_id, par_descripcion, par_tipo, par_valor, par_estado, crea_usr, crea_fecha)
    VALUES (
        1,
        'rucProveedorFacturacion',
        'RUC de SICE como proveedor de sistema de facturacion electronica (Resolucion NAC-DGERCGC26-00000027)',
        'TEXT',
        '0103567665001',
        1,
        'SISTEMA',
        GETDATE()
    );
