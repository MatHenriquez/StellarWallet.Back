using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Dtos.Responses;

public class TransactionsDto(List<BlockchainPayment> payments, int totalPages)
{
    public List<BlockchainPayment> Payments { get; set; } = payments;
    public int TotalPages { get; set; } = totalPages;
}