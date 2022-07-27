using System.ComponentModel.DataAnnotations;

namespace BarnameNevis1401.Models;

public class ImageUploadModel
{
    [Required]
    public IFormFile Picture { get; set; }
}