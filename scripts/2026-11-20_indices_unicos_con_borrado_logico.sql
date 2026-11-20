/* =====================================================================
   SIGMA - Indices unicos que ignoran registros dados de baja (IsDeleted = 1)
   Aplicar una sola vez sobre una base creada antes del 20/11/2026.
   Las bases nuevas ya se crean asi (configuracion EF con HasFilter).
   Permite volver a registrar una matricula, CUIT, numero de parte o email
   que pertenecia a un registro dado de baja.
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DROP INDEX [IX_Aircraft_Registration] ON [Aircraft];
CREATE UNIQUE INDEX [IX_Aircraft_Registration] ON [Aircraft] ([Registration]) WHERE [IsDeleted] = 0;

DROP INDEX [IX_Clients_TaxId] ON [Clients];
CREATE UNIQUE INDEX [IX_Clients_TaxId] ON [Clients] ([TaxId]) WHERE [IsDeleted] = 0;

DROP INDEX [IX_InventoryItems_PartNumber] ON [InventoryItems];
CREATE UNIQUE INDEX [IX_InventoryItems_PartNumber] ON [InventoryItems] ([PartNumber]) WHERE [IsDeleted] = 0;

DROP INDEX [IX_Users_Email] ON [Users];
CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [IsDeleted] = 0;

COMMIT TRANSACTION;

/* Verificacion: las 4 filas deben mostrar has_filter = 1 */
SELECT t.name AS tabla, i.name AS indice, i.has_filter, i.filter_definition
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
WHERE i.name IN ('IX_Aircraft_Registration', 'IX_Clients_TaxId', 'IX_InventoryItems_PartNumber', 'IX_Users_Email');
