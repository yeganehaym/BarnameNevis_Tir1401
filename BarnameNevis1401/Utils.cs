namespace BarnameNevis1401;

public class Utils
{
    public string SaveFile(IFormFile file, string path)
    {
        var ext=Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString().Replace("-","").Substring(0, 10) +ext;

        using (FileStream stream = new FileStream(Path.Combine(path,fileName), FileMode.CreateNew))
        {
            file.CopyTo(stream);
            stream.Flush();
            stream.Close();
        }
        
    
        return fileName;
    }

    public bool IsValidImage(string fileName)
    {
        var array = new string[] { ".jpg", ".png", "bmp" };
        var ext = Path.GetExtension(fileName);
        if (ext == null)
            return false;

        return array.Contains(ext);

    }
}