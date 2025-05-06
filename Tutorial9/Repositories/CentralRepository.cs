using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.DTO;

namespace Tutorial9.Repositories;

public class CentralRepository : ICentralRepository
{
    private String connectionString;
    public CentralRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Default");
    }
    
    public async Task DoSomethingAsync()
    {
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        // BEGIN TRANSACTION
        try
        {
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 1);
            command.Parameters.AddWithValue("@Name", "Animal1");
        
            await command.ExecuteNonQueryAsync();
        
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 2);
            command.Parameters.AddWithValue("@Name", "Animal2");
        
            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        // END TRANSACTION
    }

    public async Task ProcedureAsync()
    {
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "NazwaProcedury";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@Id", 2);
        
        await command.ExecuteNonQueryAsync();
        
    }
    
    
    

    public async Task<int> PostData(PostDTO post)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckIfProductExists(int id)
    {
        String sql = "select 1 from product where IdProduct = @id";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return true;
        }

        return false;
    }

    public async Task<bool> CheckIfWarehouseExists(int id)
    {
        String sql = "select 1 from warehouse where IdWarehouse = @id";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return true;
        }

        return false;
    }
}