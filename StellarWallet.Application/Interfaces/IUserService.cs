﻿using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id);
        Task Add(UserCreationDto user);
        Task Update(UserUpdateDto user);
        Task Delete(int id);
    }
}
