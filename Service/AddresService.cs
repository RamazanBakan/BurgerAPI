
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;
using WebApplication10.DTOS;
using WebApplication10.Models;

namespace WebApplication10.Service
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanıcıya yeni adres ekle
        public async Task<Address> AddAddressAsync(CreateAddressDto addressDto, int userId)
        {
            if (addressDto == null)
            {
                throw new ArgumentNullException(nameof(addressDto));
            }

            var newAddress = new Address
            {
                UserId = userId,
                AddressLine1 = addressDto.AddressLine1,
                AddressLine2 = addressDto.AddressLine2,
                City = addressDto.City,
                State = addressDto.State,
                Country = addressDto.Country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            return newAddress;
        }

        // Kullanıcının tüm adreslerini getir
        public async Task<List<Address>> GetAddressesByUserIdAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        // Belirli bir adresi ID'ye göre getir
        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");
            }
            return address;
        }

        // Adresi güncelle
        public async Task<Address> UpdateAddressAsync(int addressId, CreateAddressDto addressDto)
        {
            var existingAddress = await _context.Addresses.FindAsync(addressId);
            if (existingAddress == null)
            {
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");
            }

            existingAddress.AddressLine1 = addressDto.AddressLine1;
            existingAddress.AddressLine2 = addressDto.AddressLine2;
            existingAddress.City = addressDto.City;
            existingAddress.State = addressDto.State;
            existingAddress.Country = addressDto.Country;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();

            return existingAddress;
        }

        // Adresi sil
        public async Task DeleteAddressAsync(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
