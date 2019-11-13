using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GitApp.Services
{
    public static class CustomHtmlHelper
    {
        private const int DefaultTextLength = 50;
        
        public static IHtmlContent ShortTd(this HtmlHelper helper, string text, int textLength = DefaultTextLength)
        {
            var result = $"<td>{text}</td>";
            if (text.Length > 50)
            {
                
                result = $" <td title='{text}'>{text.Remove(textLength)}...</td>";
            }

            return new HtmlContentBuilder().Append(result);
        }
    }
}