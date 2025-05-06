using Tutorial9.DTO;

namespace Tutorial9.Repositories;

public interface ICentralRepository
{
    Task DoSomethingAsync();
    Task ProcedureAsync();
    public Task<int> PostData(PostDTO post);
    public Task<bool> CheckIfProductExists(int id);
    public Task<bool> CheckIfWarehouseExists(int id);
}