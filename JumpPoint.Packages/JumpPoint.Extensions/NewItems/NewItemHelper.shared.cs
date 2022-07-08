using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions.NewItems
{
    public static partial class NewItemHelper
    {
        public static IDictionary<string, object> GetData(NewItemPayload payload)
        {
            var data = new Dictionary<string, object>();
            data.Add(nameof(NewItemPayload.Destination), payload.Destination);
            return data;
        }

        public static NewItemResultPayload GetResult(IDictionary<string, object> data)
        {
            if (data.TryGetValue(nameof(NewItemResultPayload.FileName), out var fileName))
            {
                return new NewItemResultPayload
                {
                    FileName = fileName?.ToString(),
                    ContentToken = data.TryGetValue(nameof(NewItemResultPayload.ContentToken), out var contentToken) ? contentToken?.ToString() : null
                };
            }
            else
            {
                return null;
            }
        }

    }
}
