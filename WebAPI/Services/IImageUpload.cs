using WebAPI.Models.Domain;

namespace WebAPI.Services
{
    public interface IImageUpload
    {
        Task<Image> Upload(Image image);
    }
}