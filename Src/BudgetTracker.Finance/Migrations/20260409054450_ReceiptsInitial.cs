using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Finance.Migrations
{
    /// <inheritdoc />
    public partial class ReceiptsInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE receipts (    
                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    transaction_id INT NOT NULL,
                    file_name VARCHAR(255) NOT NULL,
                    object_key TEXT NOT NULL,
                    extension VARCHAR(10) NOT NULL,
                    mime_type VARCHAR(100) NOT NULL,
                    file_size_bytes BIGINT NOT NULL,
                    md5_hash VARCHAR(32),
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    CONSTRAINT fk_transaction_receipt FOREIGN KEY(transaction_id) REFERENCES transactions(id) ON DELETE CASCADE
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
