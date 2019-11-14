using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GitApp.Services
{
    public static class HtmlHelperX
    {
        private const int DefaultTextLength = 50;
        
        public static HtmlString ShortTd(this IHtmlHelper helper, string text, int textLength = DefaultTextLength)
        {
            var result = $"<td>{text}</td>";
            if (text.Length > textLength)
            {
                result = $"<td title='{text}'>{text.Remove(textLength)}...</td>";
            }
            
            return new HtmlString(result);
        }
    }
}