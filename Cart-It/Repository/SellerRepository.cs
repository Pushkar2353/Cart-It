﻿using Cart_It.Data;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface ISellerRepository
    {
        Task<Seller> GetSellerByIdAsync(int sellerId);
        Task<IEnumerable<Seller>> GetAllSellersAsync();
        Task<Seller> AddSellerAsync(Seller seller);
        Task<Seller> UpdateSellerAsync(Seller seller);
        Task<bool> DeleteSellerAsync(int sellerId);
    }

    public class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext _context;

        public SellerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Seller> GetSellerByIdAsync(int sellerId)
        {
            return await _context.Sellers.FindAsync(sellerId);
        }

        public async Task<IEnumerable<Seller>> GetAllSellersAsync()
        {
            return await _context.Sellers.ToListAsync();
        }

        public async Task<Seller> AddSellerAsync(Seller seller)
        {
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
            return seller;
        }

        public async Task<Seller> UpdateSellerAsync(Seller seller)
        {
            var existingSeller = await _context.Sellers.FindAsync(seller.SellerId);
            if (existingSeller == null) return null;

            // Update fields (keeping the existing ones unchanged if not provided)
            existingSeller.FirstName = string.IsNullOrEmpty(seller.FirstName) ? existingSeller.FirstName : seller.FirstName;
            existingSeller.LastName = string.IsNullOrEmpty(seller.LastName) ? existingSeller.LastName : seller.LastName;
            existingSeller.Email = string.IsNullOrEmpty(seller.Email) ? existingSeller.Email : seller.Email;
            existingSeller.Password = string.IsNullOrEmpty(seller.Password) ? existingSeller.Password : seller.Password;
            existingSeller.SellerPhoneNumber = string.IsNullOrEmpty(seller.SellerPhoneNumber) ? existingSeller.SellerPhoneNumber : seller.SellerPhoneNumber;
            existingSeller.Gender = string.IsNullOrEmpty(seller.Gender) ? existingSeller.Gender : seller.Gender;
            existingSeller.CompanyName = string.IsNullOrEmpty(seller.CompanyName) ? existingSeller.CompanyName : seller.CompanyName;
            existingSeller.AddressLine1 = string.IsNullOrEmpty(seller.AddressLine1) ? existingSeller.AddressLine1 : seller.AddressLine1;
            existingSeller.AddressLine2 = string.IsNullOrEmpty(seller.AddressLine2) ? existingSeller.AddressLine2 : seller.AddressLine2;
            existingSeller.Street = string.IsNullOrEmpty(seller.Street) ? existingSeller.Street : seller.Street;
            existingSeller.City = string.IsNullOrEmpty(seller.City) ? existingSeller.City : seller.City;
            existingSeller.State = string.IsNullOrEmpty(seller.State) ? existingSeller.State : seller.State;
            existingSeller.Country = string.IsNullOrEmpty(seller.Country) ? existingSeller.Country : seller.Country;
            existingSeller.PinCode = string.IsNullOrEmpty(seller.PinCode) ? existingSeller.PinCode : seller.PinCode;
            existingSeller.GSTIN = string.IsNullOrEmpty(seller.GSTIN) ? existingSeller.GSTIN : seller.GSTIN;
            existingSeller.BankAccountNumber = string.IsNullOrEmpty(seller.BankAccountNumber) ? existingSeller.BankAccountNumber : seller.BankAccountNumber;

            await _context.SaveChangesAsync();
            return existingSeller;
        }

        public async Task<bool> DeleteSellerAsync(int sellerId)
        {
            var seller = await _context.Sellers.FindAsync(sellerId);
            if (seller == null) return false;

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
