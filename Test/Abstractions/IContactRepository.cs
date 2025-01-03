using Test.Data.Models;

namespace Test.Abstractions
{
    public interface IContactRepository
    {
        Task Add(Contact contact);
        Task<List<Contact>> GetAll();
        Task Update(Guid id, Contact Contact);
        Task Delete(Guid id);
        Task Save();
        Task Clear();
    }
}
