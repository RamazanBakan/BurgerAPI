using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication10.Models;
using WebApplication10.DTOS;

namespace WebApplication10.Service
{
    public interface IAddressService
    {
        /// <summary>
        /// Kullanıcıya yeni bir adres ekler.
        /// </summary>
        /// <param name="addressDto">Adres bilgileri</param>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Eklenen adres</returns>
        Task<Address> AddAddressAsync(CreateAddressDto addressDto, int userId);

        /// <summary>
        /// Kullanıcının tüm adreslerini getirir.
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Kullanıcının adres listesi</returns>
        Task<List<Address>> GetAddressesByUserIdAsync(int userId);

        /// <summary>
        /// Belirli bir adresi getirir.
        /// </summary>
        /// <param name="addressId">Adres ID</param>
        /// <returns>Adres</returns>
        Task<Address> GetAddressByIdAsync(int addressId);

        /// <summary>
        /// Belirli bir adresi günceller.
        /// </summary>
        /// <param name="addressId">Adres ID</param>
        /// <param name="addressDto">Adres bilgileri</param>
        /// <returns>Güncellenmiş adres</returns>
        Task<Address> UpdateAddressAsync(int addressId, CreateAddressDto addressDto);

        /// <summary>
        /// Belirli bir adresi siler.
        /// </summary>
        /// <param name="addressId">Adres ID</param>
        /// <returns>Hiçbir şey döndürmez</returns>
        Task DeleteAddressAsync(int addressId);
    }
}