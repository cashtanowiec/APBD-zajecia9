using Tutorial9.DTO;

namespace Tutorial9.Services;

public interface IDbService
{
    Task<int> PostData(PostDTO post);
}