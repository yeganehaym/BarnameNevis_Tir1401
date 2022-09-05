using BarnameNevis1401.Core;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Mvc;

namespace BarnameNevis1401.Components;

public class SpaceReportComponent:ViewComponent
{
    private IUserService _userService;
    private IImageService _imageService;

    public SpaceReportComponent(IUserService userService, IImageService imageService)
    {
        _userService = userService;
        _imageService = imageService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int defaultPercent)
    {
        var userId = UserClaimsPrincipal.GetUserId();
        var user = await _userService.FindUserAsync(userId);
        var sum = await _imageService.GetSumOfBytes(userId);

        var model = new SpaceReportModel()
        {
            UsedSpace = sum,
            TotalSpace = user.Space,
            DefaultPercent=defaultPercent
        };
        return View("Default",model);
    }
}