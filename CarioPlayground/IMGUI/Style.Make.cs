using System.Diagnostics;
using System.Reflection;

namespace ImGui
{
    public partial class Style
    {
        /// <summary>
        /// the default style
        /// </summary>
        private readonly static Style s_prototype = new Style();

        /// <summary>
        /// Make a default style object
        /// </summary>
        /// <returns>a default style</returns>
        public static Style Make()
        {
            return s_prototype.MemberwiseClone() as Style;
        }

        /// <summary>
        /// Make a default style and modify it according to some modifiers
        /// </summary>
        /// <param name="modifiers">modifiers</param>
        /// <returns>a style made from the prototype modified by modifiers</returns>
        public static Style Make(StyleModifier[] modifiers)
        {
            var result = Make();
#if DEBUG
            Debug.Assert(result != null, "result != null");
#endif
            var type = result.GetType();
            foreach (var m in modifiers)
            {
                var property = type.GetTypeInfo().GetProperty(m.Name);
#if DEBUG
                Debug.Assert(property.CanWrite, "Specified property is readonly");
#endif
                property.SetValue(result, m.Value);
            }

            return result;
        }

        /// <summary>
        /// Make a cloned style
        /// </summary>
        /// <param name="srcStyle">the source style</param>
        public static Style Make(Style srcStyle)
        {
            return srcStyle.MemberwiseClone() as Style;
        }

        /// <summary>
        /// Make a cloned style and modify it according to some modifiers
        /// </summary>
        /// <param name="srcStyle">the source style</param>
        /// <param name="modifiers">modifiers</param>
        /// <returns>a style made from the source style modified by modifiers</returns>
        public static Style Make(Style srcStyle, StyleModifier[] modifiers)
        {
            var result = srcStyle.MemberwiseClone() as Style;
#if DEBUG
            Debug.Assert(result != null, "result != null");
#endif
            var type = result.GetType();
            foreach (var m in modifiers)
            {
                var property = type.GetProperty(m.Name);
#if DEBUG
                Debug.Assert(property.CanWrite, "Specified property is readonly");
#endif
                property.SetValue(result, m.Value);
            }

            return result;
        }

    }
}
