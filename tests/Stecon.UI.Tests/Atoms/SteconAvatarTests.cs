using Bunit;

namespace Stecon.UI.Tests;

public class SteconAvatarTests : BunitContext
{
    [Fact]
    public void RendersImage_WhenImageUrlSupplied()
    {
        var cut = Render<SteconAvatar>(p => p
            .Add(x => x.ImageUrl, "avatar.jpg")
            .Add(x => x.AltText, "Jane Doe"));

        var img = cut.Find("img.stecon-avatar__image");
        Assert.Equal("avatar.jpg", img.GetAttribute("src"));
        Assert.Equal("Jane Doe", cut.Find("span.stecon-avatar").GetAttribute("aria-label"));
    }

    [Fact]
    public void RendersInitials_WhenNoImageUrlSupplied()
    {
        var cut = Render<SteconAvatar>(p => p.Add(x => x.InitialsText, "Jane Doe"));

        Assert.Empty(cut.FindAll("img"));
        Assert.Equal("JA", cut.Find(".stecon-avatar__initials").TextContent);
    }
}
