using System.Collections.Generic;

namespace Occtoo.Formatter.Brink.Models
{
    public class BrinkPriceModel
    {
        public List<ProductVariantPrice> productVariantPrices { get; set; }
    }

    public class ProductVariantPrice
    {
        public int basePriceAmount { get; set; }
        public string countryCode { get; set; }
        public int salePriceAmount { get; set; }
        public ReferencePriceAmount referencePriceAmount { get; set; }
    }

    public class ReferencePriceAmount
    {
    }
}
