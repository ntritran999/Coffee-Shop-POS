using System.Threading.Tasks;

namespace Client.Repositories
{
    public interface IImageUploadClient
    {
        Task<string> UploadAsync(string filePath);
    }
}
