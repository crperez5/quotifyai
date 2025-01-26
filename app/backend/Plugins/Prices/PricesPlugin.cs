using System.ComponentModel;

namespace MinimalApi.Plugins.Prices;

internal sealed class PricesPlugin
{
    private readonly List<PriceModel> _priceTableByCodes =
    [
      new PriceModel("1", "aluminio alucobond", 1.5m),
      new PriceModel("2", "chapa dibond", 0.5m),
    ];

    [KernelFunction("get_price")]
    [Description("Returns the price of a material per square meter")]
    public Task<decimal?> GetPriceAsync(
        [Description("The code of the material to get a price for.")] string code)
    {
        var priceTableModel = _priceTableByCodes.FirstOrDefault(s => s.Code == code);
        if (priceTableModel == null)
        {
            return Task.FromResult<decimal?>(null);
        }

        return Task.FromResult<decimal?>(priceTableModel.Price);
    }        
}