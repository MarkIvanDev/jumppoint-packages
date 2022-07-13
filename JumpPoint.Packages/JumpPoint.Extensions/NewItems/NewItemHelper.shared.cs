using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions.NewItems
{
    public static partial class NewItemHelper
    {

        public static NewItemPayload GetPayload(IDictionary<string, object> data)
        {
            if (data.TryGetValue(nameof(NewItemPayload.Destination), out var d))
            {
                return new NewItemPayload
                {
                    Destination = d?.ToString()
                };
            }
            else
            {
                return null;
            }
        }

        public static async Task<IList<NewItemResultPayload>> GetResults(IDictionary<string, object> data)
        {
            var payloads = new List<NewItemResultPayload>();
            try
            {
                if (data.TryGetValue(nameof(NewItemResultPayload), out var nirp))
                {
                    var items = await IOHelper.ReadItems<NewItemResultPayload>(nirp?.ToString());
                    payloads.AddRange(items);
                }
                else if (data.TryGetValue(nameof(NewItemResultPayload.FileName), out var fn))
                {
                    payloads.Add(new NewItemResultPayload
                    {
                        FileName = fn?.ToString(),
                        ContentToken = data.TryGetValue(nameof(NewItemResultPayload.ContentToken), out var contentToken) ? contentToken?.ToString() : null
                    });
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static IDictionary<string, object> GetData(NewItemPayload payload)
        {
            var data = new Dictionary<string, object>();
            data.Add(nameof(NewItemPayload.Destination), payload.Destination);
            return data;
        }

        public static async Task<IDictionary<string, object>> GetData(IList<NewItemResultPayload> results)
        {
            var data = new Dictionary<string, object>();
            try
            {
                if (results.Count == 1)
                {
                    data.Add(nameof(NewItemResultPayload.FileName), results[0].FileName);
                    data.Add(nameof(NewItemResultPayload.ContentToken), results[0].ContentToken);
                }
                else if (results.Count > 1)
                {
                    data.Add(nameof(NewItemResultPayload), await IOHelper.WriteItems(results));
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }
    }
}
