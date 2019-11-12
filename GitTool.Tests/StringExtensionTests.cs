using System;
using GitTool.Services;
using Xunit;

namespace GitTool.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void Test1()
        {
            var a = "https://github.com/ILeshkevuch/Task2.git";
            var expected = "ILeshkevuch/Task2";
            Assert.Equal(expected, a.GetGitRepositoryName());
        }
        
        [Fact]
        public void Test2()
        {
            var a = "https://github.com/libgit2/libgit2sharp.git";
            var expected = "libgit2/libgit2sharp";
            Assert.Equal(expected, a.GetGitRepositoryName());
        }
        
        [Fact]
        public void Test3()
        {
            var a = "https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection.git";
            var expected = "AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection";
            Assert.Equal(expected, a.GetGitRepositoryName());
        }
        
        
    }
}