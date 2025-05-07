using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.DTO;
using Tutorial9.Exceptions;

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

    public async Task CheckIfProductExists(int id)
    {
        String sql = "select 1 from product where IdProduct = @id";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return;
        }

        throw new NotFoundException("Product doesn't exist in the database!");
    }

    public async Task CheckIfWarehouseExists(int id)
    {
        String sql = "select 1 from warehouse where IdWarehouse = @id";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return;
        }

        throw new NotFoundException("Warehouse doesn't exist in the database!");
    }

    public async Task<int> GetOrderId(int idProduct, int amountPassed, DateTime datePassed)
    {
        String sql = "select top 1 IdOrder, CreatedAt from [Order] where IdProduct = @id and Amount = @amount order by CreatedAt desc";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", idProduct);
        command.Parameters.AddWithValue("amount", amountPassed);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        
        while (await reader.ReadAsync())
        {
            int idOrderOrdinal = reader.GetOrdinal("IdOrder");
            int createdAtOrdinal = reader.GetOrdinal("CreatedAt");
            int idOrder = reader.GetInt32(idOrderOrdinal);
            DateTime createdAt = reader.GetDateTime(createdAtOrdinal);

            if (createdAt < datePassed)
            {
                return idOrder;
            }
        }

        throw new NotFoundException("Order doesn't exist in the database!");
    }

    public async Task CheckIfOrderHasNotBeenFulfilled(int idOrder)
    {
        String sql = "select 1 from Product_Warehouse where IdOrder = @idOrder";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("idOrder", idOrder);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        
        while (await reader.ReadAsync())
        {
            throw new OrderFulfilledException("Order has been already fulfilled!");
        }
    }
    
    public async Task UpdateFulfilledAtColumn(int idOrder)
    {
        String sql = "update [Order] set FulfilledAt = @currentDate where IdOrder = @idOrder";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("currentDate", DateTime.Now);
        command.Parameters.AddWithValue("idOrder", idOrder);

        await connection.OpenAsync();
        int rows = command.ExecuteNonQuery();
        if (rows == 1)
            return;

        throw new NotFoundException("Error while updating the Order table: record not found!");
    }

    public async Task<Decimal> GetProductsPrice(int idProduct)
    {
        String sql = "select Price from Product where IdProduct = @idProduct";
        
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("idProduct", idProduct);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        
        while (await reader.ReadAsync())
        {
            return Convert.ToDecimal(reader.GetDecimal(0));
        }

        throw new NotFoundException("Error while fetching product price: Product doesn't exist in the database!");
    }
    
    public async Task<int> InsertDataFromPost(PostDTO post, int idOrder)
    {
 
        String sql = "insert into Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) values (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); SELECT SCOPE_IDENTITY()";
    
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("IdWarehouse", post.IdWarehouse);
        command.Parameters.AddWithValue("IdProduct", post.IdProduct);
        command.Parameters.AddWithValue("IdOrder", idOrder);
        command.Parameters.AddWithValue("Amount", post.Amount);
        command.Parameters.AddWithValue("Price", await GetProductsPrice(post.IdProduct) * post.Amount);
        command.Parameters.AddWithValue("CreatedAt", DateTime.Now);

        await connection.OpenAsync();
        int id = Convert.ToInt32(command.ExecuteScalar());
        return id;
    }
}
