using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace optima_tracking.Data
{
    public class SqlUnitDataStore : IUnitDatastore
    {
        private readonly string _connString;

        public SqlUnitDataStore(string connString)
        {
            _connString = connString;
        }
        public async Task<IReadOnlyList<UnitData>> ReadAllDataAsync(CancellationToken token = default)
        {
            var result = new List<UnitData>();
            using (var con = new SqlConnection(_connString))
            {
                await con.OpenAsync(token);
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @" select * from [dbo].[Unit]";
                    var reader = await cmd.ExecuteReaderAsync(token);
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new UnitData
                            {
                                ApartmentNumber = (int)reader["UnitNumber"],
                                DateRecorded = (DateTime)reader["DateCollected"],
                                Rooms = (int)reader["BedRooms"],
                                Baths = (int)reader["Bathrooms"],
                                SquareFootage = (int)reader["SqFt"],
                                Rent = (int)reader["Price"],
                                DateAvailable = reader["DateAvailable"] == DBNull.Value ? null : (DateTime?)reader["DateAvailable"]
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task SaveAsync(UnitData unitData, CancellationToken token = default)
        {
            using (var con = new SqlConnection(_connString))
            {
                await con.OpenAsync(token);
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
IF NOT EXISTS(select * from dbo.Unit where DateCollected = Cast(GETUTCDATE() as date) and UnitNumber = @unit)
BEGIN
INSERT INTO [dbo].[Unit]
(DateCollected, UnitNumber, BedRooms, Bathrooms, SqFt, Price, DateAvailable)
values
(GETUTCDATE(), @unit, @beds, @baths, @sqft, @price, @da)
END
";
                    cmd.Parameters.AddWithValue("@date", unitData.DateRecorded);
                    cmd.Parameters.AddWithValue("@unit", unitData.ApartmentNumber);
                    cmd.Parameters.AddWithValue("@beds", unitData.Rooms);
                    cmd.Parameters.AddWithValue("@baths", unitData.Baths);
                    cmd.Parameters.AddWithValue("@sqft", unitData.SquareFootage);
                    cmd.Parameters.AddWithValue("@price", unitData.Rent);
                    cmd.Parameters.AddWithValue("@da", unitData.DateAvailable);
                    await cmd.ExecuteNonQueryAsync(token);
                }
            }
        }
    }
}