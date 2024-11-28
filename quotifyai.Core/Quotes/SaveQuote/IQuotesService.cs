namespace quotifyai.Core.Quotes.SaveQuote;

public interface IQuotesService
{
    Task SaveQuoteAsync(string quoteData);
}