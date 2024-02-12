using AutoMapper;
using Occtoo.Formatter.Brink.Models;
using System.Collections.Generic;
using System.Linq;

namespace Occtoo.Formatter.Brink.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<List<OcctooProductResponse>, IEnumerable<ProductModel>>().ConvertUsing<CleanProductModelConverter>();
        }

        class CleanProductModelConverter : ITypeConverter<List<OcctooProductResponse>, IEnumerable<ProductModel>>
        {
            public IEnumerable<ProductModel> Convert(List<OcctooProductResponse> source, IEnumerable<ProductModel> destination, ResolutionContext context)
            {
                foreach (var product in source)
                {
                    var displayDescriptions = new Dictionary<string, string>
                    {
                        { "sv", product.DescriptionSe }
                    };

                    var customAttributes = new Dictionary<string, string>
                    {
                        { "category", product.Category },
                        { "color", product.Color },
                        { "subcategory", product.SubCategory }
                    };

                    var displayNames = new Dictionary<string, string>
                    {
                        { "sv", product.Title }
                    };

                    ProductModel result = new ProductModel
                    {
                        Description = product.DescriptionSe,
                        IsActive = true,
                        Name = product.Title,
                        ProductParentId = product.Id,
                        ImageUrl = product.Media.FirstOrDefault()?.Url,
                        DisplayDescriptions = displayDescriptions,
                        CustomAttributes = customAttributes,
                        Weight = product.Weight ?? 0,
                        TaxGroupId = "000",
                        DisplayNames = displayNames



                    };

                    yield return result;
                }
            }
        }
    }
}
