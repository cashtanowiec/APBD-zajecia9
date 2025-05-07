using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial9.DTO;
using Tutorial9.Exceptions;
using Tutorial9.Repositories;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly String connectionString;
    public ICentralRepository _centralRepository { get; set; }
    public DbService(IConfiguration configuration, ICentralRepository centralRepository)
    {
        connectionString = configuration.GetConnectionString("Default");
        _centralRepository = centralRepository;
    }
    
    

    // wszystkie metody zwracają NotFoundException jeśli nie znaleziono rekordu w bazie danych
    public async Task<int> PostData(PostDTO post)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            // punkt 1.
            await _centralRepository.CheckIfProductExists(post.IdProduct);
            await _centralRepository.CheckIfWarehouseExists(post.IdWarehouse);
            if (post.Amount <= 0) throw new ArgumentException("Amount can't be 0");

            // punkt 2.
            int orderId = await _centralRepository.GetOrderId(post.IdProduct, post.Amount, post.CreatedAt);

            // punkt 3.
            await _centralRepository.CheckIfOrderHasNotBeenFulfilled(orderId);

            // punkt 4.
            await _centralRepository.UpdateFulfilledAtColumn(orderId);

            // punkt 5.
            int id = await _centralRepository.InsertDataFromPost(post, orderId);

            // punkt 6.
            return id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> PostDataWithProcedure(PostDTO post)
    {
        await using SqlConnection connection = new SqlConnection(connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "AddProductToWarehouse";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@IdProduct", post.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", post.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", post.Amount);
        command.Parameters.AddWithValue("@CreatedAt", post.CreatedAt);

        int id = Convert.ToInt32(command.ExecuteScalar());
        return id;
    }
}