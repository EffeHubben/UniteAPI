using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace WereldbouwerAPI
{
    public class Object2DRepository : IObject2DRepository
    {
        private readonly string sqlConnectionString;

        public Object2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<Object2D>> GetByEnvironmentIdAsync(Guid environmentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Object2D>(
                    "SELECT * FROM Object2D WHERE EnvironmentId = @EnvironmentId",
                    new { EnvironmentId = environmentId });
            }
        }

        public async Task<Object2D> AddObject2DAsync(Object2D object2D)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync(
                    "INSERT INTO Object2D (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) " +
                    "VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)",
                    object2D);
                return object2D;
            }
        }

        public async Task<Object2D> UpdateObject2DAsync(Object2D object2D)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                // Valideer EnvironmentId
                var environmentExists = await sqlConnection.QueryFirstOrDefaultAsync<int>(
                    "SELECT 1 FROM Environment2D WHERE Id = @EnvironmentId",
                    new { object2D.environmentId });

                if (environmentExists == 0) // Of environmentExists == null als je liever null controleert
                {
                    throw new InvalidOperationException($"Environment2D with Id {object2D.environmentId} does not exist.");
                }

                // Voer de update uit als de EnvironmentId bestaat
                await sqlConnection.ExecuteAsync(
                    "UPDATE Object2D SET EnvironmentId = @EnvironmentId, PrefabId = @PrefabId, PositionX = @PositionX, " +
                    "PositionY = @PositionY, ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer " +
                    "WHERE id = @id",
                    object2D);

                return await sqlConnection.QueryFirstOrDefaultAsync<Object2D>(
                    "SELECT * FROM Object2D WHERE id = @id",
                    new { object2D.id });
            }
        }

        public async Task DeleteObjectAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM Object2D WHERE Id = @Id", new { id });
            }
        }

        public async Task DeleteAllByEnvironmentIdAsync(Guid environmentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM Object2D WHERE EnvironmentId = @EnvironmentId", new { EnvironmentId = environmentId });
            }
        }
    }
}
