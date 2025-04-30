
namespace dotNIES.API.Core.Repositories.Internal;

public interface IBaseRepository
{
    Task<bool> DeleteRecordAsync<T>(T model) where T : class;
    Task<IEnumerable<T>> GetAllAsync<T>(string tableName);
    Task<IEnumerable<T>> GetDataAsync<T>(string sqlStatement);
    Task<T?> GetRecordAsync<T>(string sqlStatement);
    Task<int> InsertAsync<T>(T model) where T : class;
    Task<Guid> InsertAsync<T>(T model, bool userGuidAsId) where T : class;
    Task<bool> SoftDeleteRecordAsync<T>(T model) where T : class;
    Task<bool> UpdateRecordAsync<T>(T model) where T : class;
}