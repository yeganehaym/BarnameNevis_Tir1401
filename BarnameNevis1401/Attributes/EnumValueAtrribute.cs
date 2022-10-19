namespace BarnameNevis1401.Attributes;

[AttributeUsage(AttributeTargets.Field,Inherited =true)]
public class EnumValueAttribute:Attribute
{
    public string Name { get; set; }

   
}

public static class AttributeUtils
{
    public static string GetValue(this Enum en)
    {
        var type = en.GetType().GetMember(en.ToString()).First();
        
        foreach (var attr in type.GetCustomAttributes(true))
        {
            var attribute = attr as EnumValueAttribute;

            if (attribute != null)
            {
                return attribute.Name;
            }
            
        }

        return en.ToString();
    }
}

public enum UserStatus
{
    [EnumValue( Name="غیرفعال")]
    None,
    [EnumValue( Name="فعال")]
    Active,
    [EnumValue(Name="مسدود")]
    Ban,
    NoComment,
    NoUpload
}