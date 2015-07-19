using System;
using Cairo;

using FontDescription = Pango.FontDescription;
using Weight = Pango.Weight;
using Slant = Pango.Style;
using Color = Cairo.Color;

namespace IMGUI
{
    public struct Font : ICloneable
    {
        private FontDescription description;

        public string Family
        {
            get
            {
                return Description.Family;
            }
            set
            {
                Description.Family = value ?? "SimHei";
                //Description = FontDescription.FromString(string.Format("{0} {1} {2} {3}", family, weight, slant, size));
            }
        }

        public Slant Slant
        {
            get
            {
                return Description.Style;
            }
            set
            {
                Description.Style = value;
                //Description = FontDescription.FromString(string.Format("{0} {1} {2} {3}", family, weight, slant, size));
            }
        }

        public Weight Weight
        {
            get
            {
                return Description.Weight;
            }
            set
            {
                Description.Weight = value;
                //Description = FontDescription.FromString(string.Format("{0} {1} {2} {3}", family, weight, slant, size));
            }
        }


        public int Size
        {
            get
            {
                return (int)(Description.Size / Pango.Scale.PangoScale);
            }
            set
            {
                Description.Size = (int)(value * Pango.Scale.PangoScale);
                //Description = FontDescription.FromString(string.Format("{0} {1} {2} {3}", family, weight, slant, size));
            }
        }

        public Color Color { get; set; }

        public FontDescription Description
        {
            get
            {
                if(description == null)
                {
                    description = new FontDescription();
                }
                return description;
            }
            private set { description = value; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Font
            {
                Description = description.Copy()
            };
        }

        #endregion
    }
}