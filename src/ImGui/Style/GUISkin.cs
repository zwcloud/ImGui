using System;
using System.Collections.Generic;

namespace ImGui
{
    internal partial class GUISkin
    {
        /// <summary>
        /// Current Skin
        /// </summary>
        public static GUISkin Current { get; set;}

        /// <summary>
        /// Default skin
        /// </summary>
        public static GUISkin Default { get; }

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

            return skin;
        }

        private readonly Dictionary<GUIControlName, StyleRuleSet> styles;

        public GUIStyle InternalStyle;

        private GUISkin()
        {
            styles = new Dictionary<GUIControlName, StyleRuleSet>();

            this.InternalStyle = new GUIStyle();
            {
                this.InternalStyle.Set<double>(GUIStyleName._FieldWidth, 200);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 80);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
            }
        }

        public StyleRuleSet this[GUIControlName name] => this.styles[name];

        #region short-cuts

        /*
         * # Field and label design
         *
         * single-line:
         * +-----------+         +---------+
         * | ~ field ~ | spacing |  label  |
         * +-----------+         +---------+
         *
         * multiple-line:
         * +-----------+         +---------+
         * | ~ field ~ | spacing |  label  |
         * |           |         +---------+
         * |           |
         * |           |
         * |           |
         * +-----------+
         *
         * Field is horizontally stretched. Spacing and label is fix-sized.
         */

        public double FieldWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._FieldWidth);
            set => InternalStyle.Set<double>(GUIStyleName._FieldWidth, value);
        }

        public double FieldSpacing
        {
            get => InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            set => InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, value);
        }

        public double LabelWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._LabelWidth);
            set => InternalStyle.Set<double>(GUIStyleName._LabelWidth, value);
        }

        #endregion
    }
}
