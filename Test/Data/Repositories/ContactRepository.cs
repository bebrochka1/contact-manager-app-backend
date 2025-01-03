using Microsoft.EntityFrameworkCore;
using Test.Abstractions;
using Test.Data.Models;

namespace Test.Data.Repositories
{
    public class ContactRepository : IContactRepository, IDisposable
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Contact contact)
        {
            if (contact != null) await _context.Contacts.AddAsync(contact);
        }

        public async Task Clear()
        {
            var contacts = await _context.Contacts.ToListAsync();
            _context.Contacts.RemoveRange(contacts);
        }

        public async Task<List<Contact>> GetAll() => await _context.Contacts
            .AsNoTracking()
            .ToListAsync();

        public async Task Update(Guid id, Contact contact)
        {
            var contactInDb = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contactInDb != null) 
            {
                contactInDb.Name = contact.Name;
                contactInDb.DateOfBirth = contact.DateOfBirth;
                contactInDb.Salary = contact.Salary;
                contactInDb.Married = contact.Married;
                contactInDb.Phone = contact.Phone;
            }
            else
            {
                throw new ArgumentException($"Contact with id {id} does not exist");
            }
        }

        public async Task Delete(Guid Id)
        {
            var contactInDb = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == Id);
            if(contactInDb != null) _context.Remove(contactInDb);
        }

        public async Task Save() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if(_disposed) return;
            
            if(disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }

        ~ContactRepository()
        {
            Dispose(false);
        }
    }
}
