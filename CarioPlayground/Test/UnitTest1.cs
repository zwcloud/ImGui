using System;
using System.Collections.Generic;
using ExCSS;
using ImGui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class IMGUITest
    {
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
