using WebAPI.Data;
using WebAPI.Models.Domain;

namespace WebAPI.Services
{
    public class LocalImageUpload : IImageUpload
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly WalksDbContext dbContext;

        public LocalImageUpload(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            WalksDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<Image> Upload(Image image)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",
                image.FileName + image.FileExtension);

            await using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // https://localhost:1234/images/image.jpg
            image.FilePath = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}{httpContextAccessor.HttpContext?.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }
    }
}