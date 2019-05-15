using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class DrawingContextExFacts
    {
        public class DrawBoxModel
        {
            [Fact]
            public void DrawAnEmptyBoxModel()
            {
                void Populate(DrawingContext dc, StyleRuleSet rule, Rect r)
                {
                    dc.DrawBoxModel(rule, r);
                }

                var node = new Node(1, new Rect(10, 10, 300, 60));
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5));

                var context = node.RenderOpen();
                Populate(context, styleRuleSet, node.Rect);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker, styleRuleSet, node.Rect);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

            [Fact]
            public void DrawABoxModelWithTextContent()
            {
                void Populate(DrawingContext dc, string t, StyleRuleSet rule, Rect r)
                {
                    dc.DrawBoxModel(t, rule, r);
                }

                var node = new Node(1, new Rect(10, 10, 300, 60));
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5));
                const string text = "啊rABC";

                var context = node.RenderOpen();
                Populate(context, text, styleRuleSet, node.Rect);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker, text, styleRuleSet, node.Rect);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

        }
    }
}