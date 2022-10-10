using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BarnameNevis1401.TagHelpers;

public class CustomTagHelper:TagHelper
{
    [HtmlAttributeName(name:"asp-items")]
    public List<string> Items { get; set; }
    
    [HtmlAttributeNotBound]
    public int MyPrivate { get; set; }
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        
        if(Items==null || Items.Count==0)
            return;
        
        output.TagName = "ul";
        output.TagMode = TagMode.StartTagAndEndTag;

        foreach (var item in Items)
        {
            //============ A Tag ====================
            var aTag = new TagBuilder("a");
            aTag.TagRenderMode = TagRenderMode.Normal;
            aTag.Attributes.Add("href","https://www.google.com/search?q="+item);
            aTag.Attributes.Add("target","_blank");
            aTag.InnerHtml.Append(item);
            //========== Li Tag =================================
            var liTag = new TagBuilder("li");
            liTag.TagRenderMode = TagRenderMode.Normal;
            liTag.InnerHtml.AppendHtml(aTag);
          
            //====== Final Tag (ul) ==========
            output.Content.AppendHtml(liTag);
            

        }
        
        base.Process(context, output);
    }
    
    
}
