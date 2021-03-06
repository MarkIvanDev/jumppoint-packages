using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace JumpPoint.Extensions
{
    public static partial class IOHelper
    {
        private static readonly List<string> appUriSchemes = new List<string> { "ms-appx", "ms-appx-web", "ms-appdata" };

        public static async Task WriteText(this StorageFile file, string text)
        {
            int retryAttempts = 3;
            const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
            const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);
            const int ERROR_UNABLE_TO_REMOVE_REPLACED = unchecked((int)0x80070497);

            // Application now has read/write access to the picked file.
            while (retryAttempts > 0)
            {
                try
                {
                    retryAttempts--;
                    await FileIO.WriteTextAsync(file, text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                    break;
                }
                catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) ||
                                           (ex.HResult == ERROR_SHARING_VIOLATION) ||
                                           (ex.HResult == ERROR_UNABLE_TO_REMOVE_REPLACED))
                {
                    // This might be recovered by retrying, otherwise let the exception be raised.
                    // The app can decide to wait before retrying.
                }
            }
        }

        public static async Task WriteBytes(this StorageFile file, byte[] buffer)
        {
            int retryAttempts = 3;
            const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
            const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);
            const int ERROR_UNABLE_TO_REMOVE_REPLACED = unchecked((int)0x80070497);

            // Application now has read/write access to the picked file.
            while (retryAttempts > 0)
            {
                try
                {
                    retryAttempts--;
                    await FileIO.WriteBytesAsync(file, buffer);
                    break;
                }
                catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) ||
                                           (ex.HResult == ERROR_SHARING_VIOLATION) ||
                                           (ex.HResult == ERROR_UNABLE_TO_REMOVE_REPLACED))
                {
                    // This might be recovered by retrying, otherwise let the exception be raised.
                    // The app can decide to wait before retrying.
                }
            }
        }

        public static async Task<string> ReadText(this StorageFile file)
        {
            try
            {
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<byte[]> ReadBytes(this StorageFile file)
        {
            try
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                return buffer.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        static async Task<IList<T>> PlatformReadItems<T>(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                var text = await file.ReadText();
                return JsonConvert.DeserializeObject<List<T>>(text);
            }
            else
            {
                return new List<T>();
            }
        }

        static async Task<byte[]> PlatformReadBytes(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                return await file.ReadBytes();
            }
            else
            {
                return null;
            }
        }

        static async Task<string> PlatformWriteItems<T>(IList<T> items)
        {
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
            var json = JsonConvert.SerializeObject(items);
            await file.WriteText(json);
            return SharedStorageAccessManager.AddFile(file);
        }

        static async Task<string> PlatformWriteBytes(byte[] bytes)
        {
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
            await file.WriteBytes(bytes);
            return SharedStorageAccessManager.AddFile(file);
        }

        static async Task<Stream> PlatformGetStream(Uri uri)
        {
            try
            {
                if (appUriSchemes.Contains(uri.Scheme))
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    var stream = await file.OpenReadAsync();
                    return stream.AsStream();
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
