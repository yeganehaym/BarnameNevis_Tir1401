using System.Security.Cryptography;
using System.Text;

namespace BarnameNevis1401;

public static class Converter
{
    public static int ToInt(this string value)
    {
        return int.Parse(value);
    }

    public static string ToUnit(this long bytes)
    {
        var units = new[] { "Bytes", "KB", "MB", "GB" };

        var i = 0;

        decimal size = bytes;
        string unit = units[0];
        while (size >= 1024)
        {
            size /= 1024;
            i++;
            unit = units[i];

        } 

        return $"{size.ToString("F")} {unit}";

    }

    public static string Hash(this string value)
    {
        var password = Encoding.ASCII.GetBytes(value);
        var sha = new HMACSHA256();
        var hash = sha.ComputeHash(password);
        var newpassword = Encoding.ASCII.GetString(hash);
        return newpassword;
    }
}