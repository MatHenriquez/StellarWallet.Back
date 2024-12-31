using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Mappers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserCreationDto, User>();
        CreateMap<UserUpdateDto, User>();
        CreateMap<UserContact, UserContactsDto>();
        CreateMap<BlockchainAccount, BlockchainAccountDto>();
    }
}
