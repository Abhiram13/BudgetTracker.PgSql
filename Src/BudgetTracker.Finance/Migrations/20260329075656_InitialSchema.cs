using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetTracker.Finance.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS categories (
                    id SERIAL PRIMARY KEY NOT NULL,
                    name VARCHAR(20) NOT NULL,
                    created_at DATE NOT NULL,
                    updated_at DATE NOT NULL,
                    CONSTRAINT CK_Categories_Name_Regex CHECK (name ~ '^(?=.*[a-zA-Z])[a-zA-Z0-9 ,]*$'),
                    CONSTRAINT CK_Categories_Name_MinLength CHECK (LENGTH(TRIM(name)) >= 3)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS banks (
                    id SERIAL PRIMARY KEY NOT NULL,
                    name VARCHAR(25) NOT NULL,
                    created_at DATE NOT NULL,
                    updated_at DATE NOT NULL,
                    CONSTRAINT CK_Banks_Name_Regex CHECK (name ~ '^(?=.*[a-zA-Z])[a-zA-Z0-9 ,]*$'),
                    CONSTRAINT CK_Banks_Name_MinLength CHECK (LENGTH(TRIM(name)) >= 3)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS transactions (
                    id SERIAL PRIMARY KEY NOT NULL,
                    amount DECIMAL NOT NULL,
                    actual_amount DECIMAL,
                    description VARCHAR(50) NOT NULL,
                    from_bank INT,
                    to_bank INT,
                    category_id INT NOT NULL,
                    date DATE,
                    type INT NOT NULL,
                    created_at DATE NOT NULL,
                    updated_at DATE NOT NULL,
                    
                    -- Foreign Keys
                    FOREIGN KEY (from_bank) REFERENCES banks(id),
                    FOREIGN KEY (to_bank) REFERENCES banks(id),
                    FOREIGN KEY (category_id) REFERENCES categories(id),
                    
                    -- Amount Checks
                    CONSTRAINT CK_Transaction_Amount CHECK (amount >= 0.01 AND amount <= 1000000),
                    CONSTRAINT CK_Transaction_Actual_Amount CHECK (actual_amount IS NULL OR (actual_amount >= 0.01 AND actual_amount <= 1000000)),
                    
                    -- Description Checks
                    CONSTRAINT CK_Transaction_Description_Regex CHECK (description ~ '^(?=.*[a-zA-Z])[a-zA-Z0-9# ,]*$'),
                    CONSTRAINT CK_Transactions_Description_Length CHECK (LENGTH(TRIM(description)) >= 3),
        
                    -- Logic Checks
                    CONSTRAINT CK_Transactions_Type CHECK (type IN (1, 2)),
                    CONSTRAINT CK_Transactions_Bank_Requirements CHECK (
                        (type = 1 AND from_bank IS NOT NULL) OR 
                        (type = 2 AND to_bank IS NOT NULL)
                    ),
                    CONSTRAINT CK_Transactions_BanksNotSame CHECK (from_bank <> to_bank)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS transactions_meta (
	                transaction_id INT NOT NULL,
	                due_id INT,
	                emi_id INT,
	                tags VARCHAR,
	                created_at DATE NOT NULL,
	                updated_at DATE NOT NULL,
	                PRIMARY KEY (transaction_id),
	                FOREIGN KEY (transaction_id) REFERENCES transactions(id) ON DELETE CASCADE
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
