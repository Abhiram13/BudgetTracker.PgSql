using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Finance.Migrations
{
    /// <inheritdoc />
    public partial class OutBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE finance_outbox_events (
                    id UUID PRIMARY KEY NOT NULL,
                    entity_type VARCHAR(30) NOT NULL, --Transaction, Categories, banks
                    entity_id INT NOT NULL,
                    event_type VARCHAR(100) NOT NULL, --TransactionCreated, TransactionUpdated ...
                    payload JSONB NOT NULL,
                    status VARCHAR(20) NOT NULL DEFAULT 'Pending', --Pending, Processing, Published, Failed
                    processed_at TIMESTAMP WITHOUT TIME ZONE NULL,
                    retry_count INT NOT NULL DEFAULT 0,
                    error TEXT NULL,                    
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()                  
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
