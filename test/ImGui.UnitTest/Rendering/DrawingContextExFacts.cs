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

            [Fact]
            public void DrawABoxModelWithImageContent()
            {
                Node node = new Node(1, "imageNode", new Rect(10, 10, 300, 200));
                StyleRuleSetBuilder ruleSetBuilder = new StyleRuleSetBuilder(node.RuleSet);
                ruleSetBuilder
                    .Border((5, 10, 5, 10))
                    .BorderColor(Color.HotPink)
                    .Padding((4, 2, 4, 2));
                node.UseBoxModel = true;

                var texture = new FakeTexture();
                texture.LoadImage(@"assets\images\logo.png");

                void Populate(DrawingContext dc, ImGui.OSAbstraction.Graphics.ITexture t, StyleRuleSet rule, Rect r)
                {
                    dc.DrawBoxModel(t, rule, r);
                }

                var context = node.RenderOpen();
                Populate(context, texture, node.RuleSet, node.Rect);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker, texture, node.RuleSet, node.Rect);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

            [Fact]
            public void DrawABoxModelWithDifferentBorder()
            {
                var node = new Node(1, new Rect(20, 20, 200, 80));
                node.RuleSet.Border = (10, 20, 30, 40);
                node.RuleSet.BorderColor = (Color.Red, Color.DarkGreen, Color.DeepSkyBlue, Color.YellowGreen);

                var context = node.RenderOpen();
                context.DrawBoxModel(node.RuleSet, node.Rect);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                checker.DrawBoxModel(node.RuleSet, node.Rect);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

            [Fact]
            public void DrawABoxModelWithRoundBorder()
            {
                var node = new Node(1, new Rect(20, 20, 200, 200));
                node.RuleSet.Border = (top:20, right: 30, bottom: 20, left:40);
                node.RuleSet.BorderColor = (Color.Red, Color.DarkGreen, Color.DeepSkyBlue, Color.Black);
                node.RuleSet.BorderRadius = (TopLeft: 50, TopRight: 40, BottomRight: 20, BottomLeft: 30);
                node.RuleSet.BackgroundColor = Color.AliceBlue;

                var context = node.RenderOpen();
                context.DrawBoxModel(node.RuleSet, node.Rect);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                checker.DrawBoxModel(node.RuleSet, node.Rect);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

            [Fact]
            public void DrawWithClipRect()
            {
                var node = new Node(1, new Rect(20, 20, 200, 200));
                
                void Populate(DrawingContext dc)
                {
                    dc.PushClip(new RectangleGeometry(new Rect(0, 0, 200, 200)));
                    dc.DrawLine(node.RuleSet, new Point(1,1), new Point(1000, 1000));
                    dc.PushClip(new RectangleGeometry(new Rect(0, 0, 100, 100)));
                    dc.DrawLine(node.RuleSet, new Point(1000,1), new Point(1, 1000));
                    dc.Pop();
                    dc.Pop();
                }
                
                var context = node.RenderOpen();
                Populate(context);
                context.Close();
                
                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker);

                //read records from visual to checker and compare with node's content
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

        }
    }
}