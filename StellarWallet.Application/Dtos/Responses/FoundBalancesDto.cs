using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Dtos.Responses;

public class FoundBalancesDto(List<AccountBalances> balances, int totalPages)
{
    public List<AccountBalances> Balances { get; set; } = balances;
    public int TotalPages { get; set; } = totalPages;
}
