using System.Security.Claims;

namespace BarnameNevis1401;

public static class UserUtility
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var strId=user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (strId == null)
            return 0;
        
        var userId = int.Parse(strId);
        return userId;
    }
}