using GitApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace GitApp.Tests.Services
{
    public class HtmlHelperXTests
    {
        private readonly Mock<IHtmlHelper> mockHtml = new Mock<IHtmlHelper>();
        [Fact]
        public void ShortString()
        {
            // Assert
            Assert.Equal("<td>qwe</td>", mockHtml.Object.ShortTd("qwe").Value);
        }
        
        [Fact]
        public void LongString()
        {
            //Act
            var text = "qweqweqweaqweqweqweaqweqweqweaqweqweqweaqweqweqweaqweqweqweaqweqweqweaqweqweqwea";
            var result = $"<td title='{text}'>{text.Remove(50)}...</td>";
            // Assert
            Assert.Equal(result, mockHtml.Object.ShortTd(text).Value);
        }
        
        [Fact]
        public void ShortStringWithStringLengthParameter()
        {
            //Act
            var text = "qwe";
            var len = 1;
            var result = $"<td title='{text}'>{text.Remove(len)}...</td>";
            // Assert
            Assert.Equal(result, mockHtml.Object.ShortTd("qwe",len).Value);
        }
        
        [Fact]
        public void LongStringWithStringLengthParameter()
        {
            //Act
            var text = "123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.";
            var result = $"<td>{text}</td>";
            // Assert
            Assert.Equal(result, mockHtml.Object.ShortTd(text,100).Value);
        }
    }
}