using System;
using System.Collections.Generic;

namespace ImGui
{
    internal partial class GUISkin
    {
        /// <summary>
        /// Current Skin
        /// </summary>
        internal static GUISkin Current { get; set;}

        /// <summary>
        /// Default skin
        /// </summary>
        internal static GUISkin Default { get; }

        internal static GUISkin Custom { get; set;}

        static GUISkin()
        {
            Default = CreateDefaultSkin();
            Current = Default;
        }

        private static GUISkin CreateDefaultSkin()
        {
            GUISkin skin = new GUISkin();

            // init default rule lists
            StyleRuleSet button = new StyleRuleSet();
            skin.InitButtonStyles(button);
            skin.styles[GUIControlName.Button] = button;

            StyleRuleSet label = new StyleRuleSet();
            skin.InitLabelStyles(label);
            skin.styles[GUIControlName.Label] = label;

            StyleRuleSet box = new StyleRuleSet();
            skin.InitBoxStyles(box);
            skin.styles[GUIControlName.Box] = box;

            skin.InitCollapsingHeaderStyles(button, out var collapsingHeader);
            skin.styles[GUIControlName.CollapsingHeader] = collapsingHeader;

            StyleRuleSet textBox = new StyleRuleSet();
            skin.InitTextBoxStyles(textBox);
            skin.styles[GUIControlName.TextBox] = textBox;

            StyleRuleSet image = new StyleRuleSet();
            skin.InitImageStyles(image);
            skin.styles[GUIControlName.Image] = image;

            StyleRuleSet toggle = new StyleRuleSet();
            skin.InitToggleStyles(toggle);
            skin.styles[GUIControlName.Toggle] = toggle;

            StyleRuleSet selectable = new StyleRuleSet();
            skin.InitSelectableStyles(selectable);
            skin.styles[GUIControlName.Selectable] = selectable;

            StyleRuleSet separator = new StyleRuleSet();
            skin.InitSeparatorStyle(separator);
            skin.styles[GUIControlName.Separator] = separator;

            StyleRuleSet progressBar = new StyleRuleSet();
            skin.InitProgressBarStyles(progressBar);
            skin.styles[GUIControlName.ProgressBar] = progressBar;

            StyleRuleSet slider = new StyleRuleSet();
            skin.InitSliderStyles(slider);
            skin.styles[GUIControlName.Slider] = slider;

            StyleRuleSet listbox = new StyleRuleSet();
            skin.InitListBoxStyles(listbox);
            skin.styles[GUIControlName.ListBox] = listbox;

            skin.InitTreeNodeStyles(button, out var treeNode);
            skin.styles[GUIControlName.TreeNode] = treeNode;

            StyleRuleSet colorField = new StyleRuleSet();
            skin.InitColorFieldStyles(colorField);
            skin.styles[GUIControlName.ColorField] = colorField;

            var comboBox = new StyleRuleSet();
            skin.InitComboBoxStyles(comboBox);
            skin.styles[GUIControlName.ComboBox] = comboBox;

            return skin;
        }

        private readonly Dictionary<GUIControlName, StyleRuleSet> styles;

        private GUISkin()
        {
            styles = new Dictionary<GUIControlName, StyleRuleSet>();
        }

        public GUISkin(Dictionary<GUIControlName, StyleRuleSet> styles)
        {
            this.styles = new Dictionary<GUIControlName, StyleRuleSet>(styles);
        }

        internal StyleRuleSet this[GUIControlName name]
        {
            get
            {
                if (Custom != null && Custom.styles.TryGetValue(name, out var value))
                {
                    return value;
                }
                return this.styles[name];
            }
        }
    }
}
