using System;
using System.Collections.Generic;
using System.Diagnostics;
//using Ivony.Html;

namespace IMGUI
{
    public partial class RenderObject
    {
#if false
    /// <summary>
    /// Build RenderObjects form a HTML(layout) file and a CSS(style) file
    /// </summary>
    /// <param name="vRenderObject">result</param>
    /// <param name="layoutHTML">HTML(layout) file content</param>
    /// <param name="styleCSS">CSS(style) file content</param>
    /// <returns>true: successful, false: failed</returns>
        public static bool BuildFrom(out List<RenderObject> vRenderObject, string layoutHTML, string styleCSS)
        {
            vRenderObject = new List<RenderObject>();
            try
            {
                var htmlParser = new Ivony.Html.Parser.JumonyParser();
                var htmlDocument =
                    htmlParser.LoadDocument(System.IO.Path.Combine(Environment.CurrentDirectory, layoutHTML));
                var nodes = htmlDocument.Nodes();

                var cssPaser = new ExCSS.Parser();
                var cssContent =
                    System.IO.File.ReadAllText(System.IO.Path.Combine(Environment.CurrentDirectory, styleCSS));
                var styleSheet = cssPaser.Parse(cssContent);

                foreach (var rule in styleSheet.StyleRules)
                {
                    Debug.WriteLine("Declarations: {0}, Selector: {1}, Value: {2}", rule.Declarations, rule.Selector,
                        rule.Value);

                    //RenderObject
                    var node = htmlDocument.FindSingle(rule.Value);
                    var renderObject = new RenderObject();
                    renderObject.DOMObject = node;

                    //StyleObject
                    Style style = null;
                    //Style.BuildFrom(out style, rule);
                    renderObject.StyleObject = style;

                    vRenderObject.Add(renderObject);
                }
            }
            catch (Exception)
            {
                vRenderObject = null;
                return false;
            }

            return true;
        }

#endif
    }
}
