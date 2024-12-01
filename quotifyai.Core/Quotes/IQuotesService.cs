namespace quotifyai.Core.Quotes;

public interface IQuotesService
{
    Task SaveQuoteAsync(string quoteData);
}