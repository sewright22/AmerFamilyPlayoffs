namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SqliteInMemoryItemsControllerTest : ApiContextTest, IDisposable
    {
        private readonly DbConnection connection;

        public SqliteInMemoryItemsControllerTest() : base (new DbContextOptionsBuilder<AmerFamilyPlayoffContext>()
                                                              .UseSqlite(CreateInMemoryDatabase())
                                                              .Options)
        {
            this.connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => this.connection.Dispose();
    }
}
