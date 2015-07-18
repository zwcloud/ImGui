using System;
using System.Linq;
using Cairo;

namespace IMGUI
{
    public partial class Style
    {
#if false
        public static bool BuildFrom(out Style style, ExCSS.StyleRule rule)
        {
            try
            {
                Func<string, Length> numberQueryExactly = delegate(string name)
                {
                    var query = rule.Declarations
                        .Where(p => p.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => p.Term);
                    var primativeTerm = query.First() as ExCSS.PrimitiveTerm;
                    if (primativeTerm != null)
                        return new Length(primativeTerm.GetFloatValue(ExCSS.UnitType.Pixel)??0.0f, Unit.Pixel);
                    return Length.Undefined;
                };

                Func<string, Color> colorQueryExactly = delegate(string name)
                {
                    var query = rule.Declarations
                        .Where(p => p.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => p.Term);
                    var primativeTerm = query.First() as ExCSS.PrimitiveTerm;
                    if(primativeTerm != null)
                    {
                        var exCSSColor = primativeTerm.Value as ExCSS.HtmlColor;
                        if(exCSSColor != null)
                        {
                            var result = CairoEx.ColorArgb(exCSSColor.A, exCSSColor.R, exCSSColor.G, exCSSColor.B);
                            return result;
                        }
                    }
                    return CairoEx.ColorBlack;
                };

                style = new Style
                {
                    BorderTop = numberQueryExactly("border-top"),
                    BorderRight = numberQueryExactly("border-right"),
                    BorderBottom = numberQueryExactly("border-bottom"),
                    BorderLeft = numberQueryExactly("border-left"),
                    BorderTopColor = colorQueryExactly("border-top-color"),
                    BorderRightColor = colorQueryExactly("border-right-color"),
                    BorderBottomColor = colorQueryExactly("border-bottom-color"),
                    BorderLeftColor = colorQueryExactly("border-left-color"),
                    MarginTop = numberQueryExactly("margin-top"),
                    MarginRight = numberQueryExactly("margin-right"),
                    MarginBottom = numberQueryExactly("margin-bottom"),
                    MarginLeft = numberQueryExactly("margin-left"),
                    PaddingTop = numberQueryExactly("padding-top"),
                    PaddingRight = numberQueryExactly("padding-right"),
                    PaddingBottom = numberQueryExactly("padding-bottom"),
                    PaddingLeft = numberQueryExactly("padding-left")
                };

                //foreach (var property in rule.Declarations)
                //{
                //    Debug.WriteLine("Name: {0}, Term: {1}", property.Name, property.Term.ToString());
                //}
            }
            catch (Exception)
            {
                style = null;
                return false;
            }

            return true;
        }
#endif
    }
}
