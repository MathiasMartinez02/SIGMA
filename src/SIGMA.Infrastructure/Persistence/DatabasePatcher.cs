using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SIGMA.Infrastructure.Persistence;

public static class DatabasePatcher
{
    public static async Task ApplyAsync(ApplicationDbContext context, ILogger logger)
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            await using var cmd = connection.CreateCommand();

            cmd.CommandText = """
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'PasswordResetToken')
                BEGIN
                    ALTER TABLE [Users] ADD [PasswordResetToken] nvarchar(128) NULL;
                END

                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'PasswordResetTokenExpiry')
                BEGIN
                    ALTER TABLE [Users] ADD [PasswordResetTokenExpiry] datetime2 NULL;
                END

                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'TechnicalDocuments')
                BEGIN
                    CREATE TABLE [TechnicalDocuments] (
                        [Id]            uniqueidentifier NOT NULL,
                        [Type]          nvarchar(30)     NOT NULL,
                        [Title]         nvarchar(200)    NOT NULL,
                        [ReferenceCode] nvarchar(100)    NULL,
                        [FileUrl]       nvarchar(500)    NOT NULL,
                        [Description]   nvarchar(1000)   NULL,
                        [IssueDate]     datetime2        NULL,
                        [ExpiryDate]    datetime2        NULL,
                        [CreatedAt]     datetime2        NOT NULL,
                        [UpdatedAt]     datetime2        NOT NULL,
                        [CreatedById]   uniqueidentifier NULL,
                        [IsDeleted]     bit              NOT NULL,
                        CONSTRAINT [PK_TechnicalDocuments] PRIMARY KEY ([Id])
                    );

                    CREATE INDEX [IX_TechnicalDocuments_Type]
                        ON [TechnicalDocuments] ([Type]);

                    CREATE INDEX [IX_TechnicalDocuments_ExpiryDate]
                        ON [TechnicalDocuments] ([ExpiryDate]);
                END

                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'InventoryItems' AND COLUMN_NAME = 'MaximumStock')
                BEGIN
                    ALTER TABLE [InventoryItems] ADD [MaximumStock] decimal(12,3) NULL;
                END

                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Notifications')
                BEGIN
                    CREATE TABLE [Notifications] (
                        [Id]              uniqueidentifier NOT NULL,
                        [UserId]          uniqueidentifier NOT NULL,
                        [Type]            nvarchar(30)     NOT NULL,
                        [Title]           nvarchar(200)    NOT NULL,
                        [Message]         nvarchar(1000)   NOT NULL,
                        [RelatedEntityId] uniqueidentifier NULL,
                        [IsRead]          bit              NOT NULL,
                        [ReadAt]          datetime2        NULL,
                        [CreatedAt]       datetime2        NOT NULL,
                        [UpdatedAt]       datetime2        NOT NULL,
                        [CreatedById]     uniqueidentifier NULL,
                        [IsDeleted]       bit              NOT NULL,
                        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [Users] ([Id]) ON DELETE CASCADE
                    );

                    CREATE INDEX [IX_Notifications_UserId]
                        ON [Notifications] ([UserId]);

                    CREATE INDEX [IX_Notifications_IsRead]
                        ON [Notifications] ([IsRead]);
                END

                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'WorkOrders' AND COLUMN_NAME = 'IntakeDate')
                BEGIN
                    ALTER TABLE [WorkOrders] ADD [IntakeDate] datetime2 NOT NULL DEFAULT '0001-01-01';
                END
                """;

            await cmd.ExecuteNonQueryAsync();
            logger.LogInformation("Schema patch applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Schema patch failed.");
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
