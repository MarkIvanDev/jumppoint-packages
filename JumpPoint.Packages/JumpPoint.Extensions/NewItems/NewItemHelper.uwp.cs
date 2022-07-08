using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace JumpPoint.Extensions.NewItems
{
    public static partial class NewItemHelper
    {
        public static async Task<byte[]> GetBytes(string contentToken)
        {
            try
            {
                var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(contentToken);
                if (file != null)
                {
                    return await IOHelper.ReadBytes(file);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
