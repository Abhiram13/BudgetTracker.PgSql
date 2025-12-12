using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetTracker.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS categories (
                    id SERIAL PRIMARY KEY NOT NULL,
                    name VARCHAR NOT NULL,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS banks (
                    id SERIAL PRIMARY KEY NOT NULL,
                    name VARCHAR NOT NULL,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
                )
            ");

            // migrationBuilder.Sql(@"
            //     CREATE TABLE IF NOT EXISTS dues (
            //         id SERIAL PRIMARY KEY NOT NULL,
            //         debtor VARCHAR,
            //         creditor VARCHAR,
            //         description VARCHAR,
            //         title VARCHAR,
            //         total_amount DECIMAL,
            //         due_amount DECIMAL,
            //         status INT,
            //         start_date TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
            //         completed_date TIMESTAMP WITHOUT TIME ZONE,
            //         remarks TEXT,
            //         comments TEXT,
            //         created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
            //         updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
            //     )
            // ");

            // migrationBuilder.Sql(@"
            //     CREATE TABLE IF NOT EXISTS monthly_installments (
            //         id SERIAL PRIMARY KEY NOT NULL,
            //         description VARCHAR,
            //         installments INT,
            //         completed INT,
            //         principle_amount DECIMAL,
            //         installment_amount DECIMAL,
            //         start_date TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
            //         end_date TIMESTAMP WITHOUT TIME ZONE,
            //         status INT,
            //         remarks TEXT,
            //         comments TEXT,
            //         created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
            //         updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
            //     )
            // ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS transactions (
                    id SERIAL PRIMARY KEY NOT NULL,
                    amount DECIMAL,
                    actual_amount DECIMAL,
                    description VARCHAR,
                    from_bank INT,
                    to_bank INT,
                    category_id INT,
                    date TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
                    type INT,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    FOREIGN KEY (from_bank) REFERENCES banks(id),
                    FOREIGN KEY (to_bank) REFERENCES banks(id),
                    FOREIGN KEY (category_id) REFERENCES categories(id)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS transactions_meta (
	                transaction_id INT NOT NULL,
	                due_id INT,
	                emi_id INT,
	                tags VARCHAR,
	                created_at TIMESTAMP WITHOUT TIME ZONE,
	                updated_at TIMESTAMP WITHOUT TIME ZONE,
	                FOREIGN KEY (transaction_id) REFERENCES transactions(id)
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
        }
    }
}
