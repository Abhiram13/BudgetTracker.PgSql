using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetTracker.Dues.Migrations
{
    /// <inheritdoc />
    public partial class InitialDuesSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS dues (
                    id SERIAL PRIMARY KEY NOT NULL,
                    debtor VARCHAR,
                    creditor VARCHAR,
                    description VARCHAR,
                    title VARCHAR,
                    total_amount DECIMAL,
                    due_amount DECIMAL,
                    status INT,
                    start_date DATE,
                    completed_date DATE,
                    remarks TEXT,
                    comments TEXT,
                    created_at DATE,
                    updated_at DATE
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
