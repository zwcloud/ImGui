using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class GUISkin
    {
        public static GUISkin Current { get; set;}

        /// <summary>
        /// Default skin
        /// </summary>
        public static GUISkin Instance { get; }

        static GUISkin()
        {
            Instance = CreateDefaultSkin();
            Current = Instance;
        }

        private static GUISkin CreateDefaultSkin()
        {
            GUISkin skin = new GUISkin();

            //FIXME Maybe there is a better way to init default skin?

            // init default rule lists
            skin.InitButtonStyles();
            skin.InitSelectableStyles();
            skin.InitListBoxStyles();

            return skin;
        }

        public IReadOnlyList<StyleModifier> GetStyleModifiers(GUIControlName control)
        {
            return this.styles[control];
        }

        private readonly Dictionary<GUIControlName, IReadOnlyList<StyleModifier>> styles;

        public GUIStyle InternalStyle;

        public GUISkin()
        {
            styles = new Dictionary<GUIControlName, IReadOnlyList<StyleModifier>>();

            this.InternalStyle = new GUIStyle();
            {
                this.InternalStyle.Set<double>(GUIStyleName._FieldWidth, 200);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 80);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
            }
        }

        public GUISkin(Dictionary<GUIControlName, IReadOnlyList<StyleModifier>> rules)
        {
            styles = rules;

            this.InternalStyle = new GUIStyle();
            {
                this.InternalStyle.Set<double>(GUIStyleName._FieldWidth, 200);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 80);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
            }
        }

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
