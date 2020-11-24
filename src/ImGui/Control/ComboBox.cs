using System.Collections.Generic;
using System.Linq;
using ImGui.Rendering;

//BUG Hover state persists when move from mainRect to outside.
//BUG Abnormal representation when drag from mainRect to outside.
//TODO GUILayout version

namespace ImGui
{
    class ComboBoxContext
    {
        public string[] Texts { get; set; }
        public Form ItemsContainer { get; set; }
        public int SelectedIndex { get; set; }
        public string Text { get; set; }
    }

    public partial class GUI
    {
        internal static ComboBoxContext comboBoxContext;//TODO move this to Window.storage

        public static int ComboBox(Rect rect, string[] texts)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return comboBoxContext.SelectedIndex;
            
            //get or create the root node
            var id = window.GetID(texts);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"Combobox<{texts[0]}..>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.ComboBox]);
                //set initial states TODO move to shared storage
                GUI.comboBoxContext = new ComboBoxContext();
                GUI.comboBoxContext.Texts = texts;
                GUI.comboBoxContext.Text = texts[0];
                GUI.comboBoxContext.SelectedIndex = 0;
            }
            node.RuleSet.ApplyStack();
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);
            if(pressed)
            {
                Application.AddFrom(()=>
                {
                    var form = ComboBoxItemsForm.Create(
                        comboBoxContext.Texts,
                        node,
                        i => comboBoxContext.SelectedIndex = i);
                    comboBoxContext.ItemsContainer = form;
                    return form;
                });
                node.State = GUIState.Active;
            }

            if (node.State == Active)
            {
                comboBoxContext.ItemsContainer?.Show();
            }

            comboBoxContext.Text = comboBoxContext.Texts[comboBoxContext.SelectedIndex];
            
            // last item state
            window.TempData.LastItemState = node.State;

            //render
            using var g = node.RenderOpen();
            g.DrawBoxModel(comboBoxContext.Text, node.RuleSet, node.Rect);
            g.RenderArrow(node.Rect.Min + new Vector(node.PaddingLeft, 0), node.Height,
                node.RuleSet.FontColor, Internal.Direcion.Down, 1.0);

            return comboBoxContext.SelectedIndex;
        }
    }

    internal partial class GUISkin
    {
        private void InitComboBoxStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Border(1.0, GUIState.Normal)
                .Border(1.0, GUIState.Hover)
                .Border(1.0, GUIState.Active)
                .Padding(5.0, GUIState.Normal)
                .Padding(5.0, GUIState.Hover)
                .Padding(5.0, GUIState.Active)
                .BorderColor(new Color(0.26f, 0.59f, 0.98f, 0.40f), GUIState.Normal)
                .BorderColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Hover)
                .BorderColor(new Color(0.06f, 0.53f, 0.98f, 1.00f), GUIState.Active)
                .BackgroundColor(Color.Rgb(0x65a9d7), GUIState.Normal)
                .BackgroundColor(Color.Rgb(0x28597a), GUIState.Hover)
                .BackgroundColor(Color.Rgb(0x1b435e), GUIState.Active)
                .BackgroundGradient(Gradient.TopBottom)
                .FontColor(Color.Black)
                .AlignmentVertical(Alignment.Center, GUIState.Normal)
                .AlignmentVertical(Alignment.Center, GUIState.Hover)
                .AlignmentVertical(Alignment.Center, GUIState.Active)
                .AlignmentHorizontal(Alignment.Center, GUIState.Normal)
                .AlignmentHorizontal(Alignment.Center, GUIState.Hover)
                .AlignmentHorizontal(Alignment.Center, GUIState.Active)
                .GradientTopDownColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Normal)
                .GradientTopDownColor(Color.Rgb(247, 247, 247), Color.Rgb(221, 221, 221), GUIState.Hover)
                .GradientTopDownColor(Color.Rgb(222, 222, 222), Color.Rgb(248, 248, 248), GUIState.Active);
        }
    }

    internal sealed class ComboBoxItemsForm : Form
    {
        private readonly List<string> textList;
        private System.Action<int> CallBack { get; set; }

        public static ComboBoxItemsForm Create(string[] texts, Node attachedNode,
            System.Action<int> callBack)
        {
            var maxLength = 0;
            var maxLengthString = string.Empty;
            foreach (var s in texts)
            {
                if (s.Length <= maxLength) continue;
                maxLength = s.Length;
                maxLengthString = s;
            }

            var size = attachedNode.RuleSet.CalcSize(maxLengthString);
            size.Height *= texts.Length;

            size += new Vector(100, 100);
            
            var clientPos = attachedNode.Rect.BottomLeft;
            var clientRect = new Rect(clientPos, size);

            var form = new ComboBoxItemsForm(clientRect, texts, callBack);
            
            form.BackgroundColor = Color.Peru;
            return form;
        }

        private ComboBoxItemsForm(Rect rect, string[] texts, System.Action<int> callBack)
            : base(rect, "comboBoxForm"/*acts as debug name*/, WindowTypes.ClientAreaOnly)
        {
            textList = new List<string>(texts);
            CallBack = callBack;
        }

        protected override void OnGUI()
        {
            GUI.Begin("ComboBoxWindow", (0, 0), ClientSize,
                WindowFlags.ShowBorders | WindowFlags.NoTitleBar | WindowFlags.NoCollapse 
                | WindowFlags.NoMove | WindowFlags.NoResize | WindowFlags.NoScrollbar );
            GUILayout.BeginVertical("CombolBox");
            var clickedIndx = -1;
            for (var i = 0; i < textList.Count; i++)
            {
                if(GUILayout.Button(textList[i]))
                {
                    clickedIndx = i;
                }
            }
            GUILayout.EndVertical();
            GUI.End();

            if (clickedIndx >= 0)
            {
                CallBack?.Invoke(clickedIndx);
                this.Close();
                Application.RemoveForm(this);
            }
        }
    }
}
