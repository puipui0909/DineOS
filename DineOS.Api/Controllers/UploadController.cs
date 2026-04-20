using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/menu");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = "/images/menu/" + fileName;

        return Ok(new { imageUrl });
    }
}