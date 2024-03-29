﻿using Microsoft.AspNetCore.Http;

namespace BarnameNevis1401.Infrastructure;

public class Utils
{
    public string SaveFile(IFormFile file, string path)
    {
        var ext=Path.GetExtension((string?)file.FileName);
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
    
    private static Random random = new Random();

    public static string RandomString(int length,RandomType randomType)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
         string chars = "0123456789";

        switch (randomType)
        {
            case RandomType.OnlyLetters:
                chars = letters;
                break;
            case RandomType.All:
                chars = letters + numbers;
                break;
        }
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public enum RandomType
{
    OnlyNumbers,
    OnlyLetters,
    All
}