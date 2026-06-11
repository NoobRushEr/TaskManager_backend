using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'tasks' AND column_name = 'CreatedAt'
                    ) THEN
                        ALTER TABLE tasks RENAME COLUMN "CreatedAt" TO created_at;
                    ELSIF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'tasks' AND column_name = 'createdat'
                    ) THEN
                        ALTER TABLE tasks RENAME COLUMN createdat TO created_at;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'tasks' AND column_name = 'created_at'
                    ) THEN
                        ALTER TABLE tasks RENAME COLUMN created_at TO "CreatedAt";
                    END IF;
                END $$;
                """);
        }
    }
}
