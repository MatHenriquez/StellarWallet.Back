namespace StellarWallet.Application.Utilities;

public static class Paginate
{
    public static List<T> PaginateQuery<T>(List<T> query, int pageNumber, int pageSize)
    {
        ValidatePageNumber(pageNumber, GetTotalPages(query.Count, pageSize));
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    public static int GetTotalPages(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    private static void ValidatePageNumber(int pageNumber, int totalPages)
    {
        if (pageNumber <= 0 || pageNumber > totalPages)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Invalid page number.");
        }
    }
}
