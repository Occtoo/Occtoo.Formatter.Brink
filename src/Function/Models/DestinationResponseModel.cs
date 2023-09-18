using System.Collections.Generic;

namespace Occtoo.Formatter.Brink.Models
{
    public class DestinationResponseModel<T>
    {
        public string Language { get; set; }

        public IEnumerable<T> Results { get; set; }
    }
}
