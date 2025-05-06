using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Tutorial9.DTO;
using Tutorial9.Exceptions;
using Tutorial9.Repositories;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;
    public ICentralRepository _centralRepository { get; set; }
    public DbService(IConfiguration configuration, ICentralRepository centralRepository)
    {
        _configuration = configuration;
        _centralRepository = centralRepository;
    }
    
    

    public async Task<int> PostData(PostDTO post)
    {
        throw new NotImplementedException();
        
        bool isValid = await _centralRepository.CheckIfProductExists(post.IdProduct) &&
                       await _centralRepository.CheckIfWarehouseExists(post.IdWarehouse) && post.Amount > 0;

        if (isValid)
        {
            
        }
        else
        {
            throw new NotFoundException("Database entry not found!");
        }
    }
}