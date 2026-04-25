using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Finance.Migrations
{
    /// <inheritdoc />
    public partial class DueAndEmis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS dues (
                    id SERIAL PRIMARY KEY NOT NULL,
                    debtor VARCHAR (50),
                    creditor VARCHAR (50),
                    description VARCHAR (300),
                    title VARCHAR (100),
                    total_amount DECIMAL,
                    due_amount DECIMAL,
                    status INT,
                    start_date DATE NOT NULL,
                    completed_date DATE,
                    remarks TEXT,
                    comments TEXT,
                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    
                    -- Amount Checks
                    CONSTRAINT CK_Due_Total_Amount CHECK (total_amount >= 0.01 AND total_amount <= 1000000),
                    CONSTRAINT CK_Due_Due_Amount CHECK ((due_amount IS NULL) OR (due_amount >= 0.01 AND due_amount <= 1000000)),
                    
                    --Logic Checks
                    CONSTRAINT CK_Due_Status CHECK (status = 1 OR status = 2)
                )
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS transactions_meta
                ALTER COLUMN created_at TYPE TIMESTAMPTZ,
                ALTER COLUMN created_at SET NOT NULL,
                ALTER COLUMN created_at SET DEFAULT NOW(),
                      
                ALTER COLUMN updated_at TYPE TIMESTAMPTZ,
                ALTER COLUMN updated_at SET NOT NULL,
                ALTER COLUMN updated_at SET DEFAULT NOW()
            ");
            
            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS transactions
                ALTER COLUMN created_at TYPE TIMESTAMPTZ,
                ALTER COLUMN created_at SET NOT NULL,
                ALTER COLUMN created_at SET DEFAULT NOW(),
                      
                ALTER COLUMN updated_at TYPE TIMESTAMPTZ,
                ALTER COLUMN updated_at SET NOT NULL,
                ALTER COLUMN updated_at SET DEFAULT NOW()
            ");
            
            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS banks
                ALTER COLUMN created_at TYPE TIMESTAMPTZ,
                ALTER COLUMN created_at SET NOT NULL,
                ALTER COLUMN created_at SET DEFAULT NOW(),
                      
                ALTER COLUMN updated_at TYPE TIMESTAMPTZ,
                ALTER COLUMN updated_at SET NOT NULL,
                ALTER COLUMN updated_at SET DEFAULT NOW()
            ");
            
            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS categories
                ALTER COLUMN created_at TYPE TIMESTAMPTZ,
                ALTER COLUMN created_at SET NOT NULL,
                ALTER COLUMN created_at SET DEFAULT NOW(),
                      
                ALTER COLUMN updated_at TYPE TIMESTAMPTZ,
                ALTER COLUMN updated_at SET NOT NULL,
                ALTER COLUMN updated_at SET DEFAULT NOW()
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
