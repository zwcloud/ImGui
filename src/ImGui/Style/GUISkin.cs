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
            skin.InitDefaultSkin();
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
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 60);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
            }
        }

        public GUISkin(Dictionary<GUIControlName, IReadOnlyList<StyleModifier>> rules)
        {
            styles = rules;

            this.InternalStyle = new GUIStyle();
            {
                this.InternalStyle.Set<double>(GUIStyleName._FieldWidth, 200);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 60);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
            }
        }

        partial void InitDefaultSkin();

        #region short-cuts
        public double FieldWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._FieldWidth);
            set => InternalStyle.Set<double>(GUIStyleName._FieldWidth, value);
        }
        #endregion
    }
}
