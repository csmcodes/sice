-- ============================================================
-- AUTOMATA SRI - Regla adicional: glitch transitorio de ambiente
-- Motor: PostgreSQL
-- Proyecto: SICE
-- ============================================================
-- Contexto: comprobantes DEVUELTOS con mensaje "El ambiente de la
-- solicitud PRODUCCIÓN no coincide con el de ejecución PRUEBAS"
-- quedaban sin resolver porque matcheaban la regla generica
-- 'AMBIENTE' (orden 140, accion ALERTAR) antes de llegar a
-- cualquier regla de reenvio. En la practica este mensaje especifico
-- es un glitch transitorio de enrutamiento del SRI: reenviar el
-- mismo comprobante sin cambiar nada lo resuelve.
--
-- Esta regla se evalua ANTES que la generica 'AMBIENTE' (orden 15
-- vs 140) y solo matchea el mensaje especifico, sin tocar el
-- comportamiento de otros mensajes de ambiente (ej. certificado
-- apuntando al ambiente incorrecto), que siguen yendo a ALERTAR.
--
-- Patron sin tilde ('EJECUCION' en vez de 'EJECUCIÓN') a proposito:
-- el clasificador hace mensajeUpper.Contains(patron.ToUpper()), y
-- una tilde distinta entre el patron y el mensaje real rompe el
-- match. 'NO COINCIDE CON EL DE' evita el caracter acentuado y
-- sigue siendo suficientemente especifico.
--
-- Idempotente: no inserta si ya existe una regla con este patron.
-- ============================================================

INSERT INTO sri_regla_mensaje (srm_patron, srm_accion, srm_estado, srm_orden, srm_descripcion, srm_activo)
SELECT 'NO COINCIDE CON EL DE', 'REENVIAR', 4, 15,
       'Glitch transitorio de enrutamiento del SRI (ambiente), se resuelve con reenvio simple', true
WHERE NOT EXISTS (
    SELECT 1 FROM sri_regla_mensaje WHERE srm_patron = 'NO COINCIDE CON EL DE' AND srm_estado = 4
);
