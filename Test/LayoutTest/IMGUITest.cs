using System;
using Xunit;

namespace Test
{
    public class ImGuiTest
    {
        [Fact]
        public void TestUnicodeTextSize()
        {
            {
                var str = "123456";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.Equal(6, byteCount);
            }

            {
                var str = "123456你好";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.Equal(12, byteCount);
            }

            {
                var str = "123456你";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.Equal(9, byteCount);
            }
        }

#if false
        [TestMethod]
        public void TestRenderObject_Build()
        {
            List<RenderObject> vR;
            RenderObject.BuildFrom(out vR, "layout.html", "style.css");
        }
        
        [TestMethod]
        public void TestStyle_BuildFrom()
        {
            Style style;
            var parser = new ExCSS.Parser();
            var stylesheet = parser.Parse(
@"#Header
{
	padding-left: 20px;
	padding-top: 15px;
    padding-bottom: 15px;
}");
            var result = Style.BuildFrom(out style, stylesheet.Rules[0] as StyleRule);
            Assert.IsTrue(result, "Style.BuildFrom Failed!");
        }
#endif

    }
}
