using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WereldbouwerAPI
{
    public class WereldBouwerRepository : IWereldBouwerRepository
    {
        private readonly string sqlConnectionString;

        public WereldBouwerRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<WereldBouwer>> GetAllAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<WereldBouwer>("SELECT * FROM [Environment2D]");
            }
        }

        public async Task<IEnumerable<WereldBouwer>> GetByUserIdAsync(string id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var result = await sqlConnection.QueryAsync<WereldBouwer>("SELECT * FROM [Environment2D] WHERE OwnerUserId = @Id", new { id });
                return result;
            }
        }

        public async Task<WereldBouwer> GetByWereldbouwerIdAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<WereldBouwer>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { id });
            }
        }

        public async Task<WereldBouwer> AddAsync(WereldBouwer wereldBouwer)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, OwnerUserId, MaxHeight, MaxLength) VALUES (@Id, @Name, @OwnerUserId, @MaxHeight, @MaxLength)", wereldBouwer);
                return wereldBouwer;
            }
        }

        public async Task UpdateAsync(WereldBouwer wereldBouwer)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                                 "Name = @Name, " +
                                                 "OwnerUserId = @OwnerUserId, " +
                                                 "MaxHeight = @MaxHeight, " +
                                                 "MaxLength = @MaxLength " +
                                                 "WHERE Id = @Id", wereldBouwer);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
            }
        }


    }
}
