using Tutorial9.DTO;

namespace Tutorial9.Repositories;

public interface ICentralRepository
{
    Task DoSomethingAsync();
    Task ProcedureAsync();
    public Task CheckIfProductExists(int id);
    public Task CheckIfWarehouseExists(int id);
    public Task<int> GetOrderId(int idProduct, int amountPassed, DateTime datePassed);
    public Task CheckIfOrderHasNotBeenFulfilled(int idOrder);
    public Task UpdateFulfilledAtColumn(int idOrder);
    public Task<int> InsertDataFromPost(PostDTO post, int idOrder);
}