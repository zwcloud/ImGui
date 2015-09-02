using System.Diagnostics;
using Cairo;

//BUG Hover state persists when move from mainRect to outside.
//BUG Abnormal representation when drag from mainRect to outside.

namespace IMGUI
{
    internal class ComboBox : Control
    {
        public int SelectedIndex { get; private set; }//TODO consider remove this property

        private int HoverIndex { get; set; }

        private int ActiveIndex { get; set; }

        internal ComboBox(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        internal static int DoControl(Context g, Context gTop, Rect rect, string[] texts, int selectedIndex, string name)
        {
            #region Get control reference
            ComboBox comboBox;
            if(!Controls.ContainsKey(name))
            {
                comboBox = new ComboBox(name);
            }
            else
            {
                comboBox = Controls[name] as ComboBox;
            }

            Debug.Assert(comboBox != null);
            #endregion

            #region Set control data
            comboBox.SelectedIndex = selectedIndex;
            #endregion

            #region Logic
            int textCount = texts.Length;
            Rect extendRect = new Rect(
                rect.BottomLeft + new Vector(1, 1),
                rect.BottomRight + textCount*(new Vector(rect.Height, rect.Height)));

            Debug.Assert(!rect.IntersectsWith(extendRect));

            bool inMainRect = rect.Contains(Input.Mouse.MousePos);
            bool inExtendRect = extendRect.Contains(Input.Mouse.MousePos);

            if(!inMainRect && !inExtendRect)
            {
                if(comboBox.State == "Hover")
                {
                    comboBox.State = "Normal";
                }
                else if(comboBox.State == "Active")
                {
                    if(Input.Mouse.LeftButtonClicked)
                    {
                        comboBox.State = "Normal";
                    }
                }
            }
            else if(inMainRect)
            {
                if(comboBox.State == "Normal")
                {
                    comboBox.State = "Hover";
                }
                if(comboBox.State == "Hover")
                {
                    if(Input.Mouse.LeftButtonClicked)
                    {
                        comboBox.State = "Active";
                    }
                }
                else if(comboBox.State == "Active")
                {
                    if(Input.Mouse.LeftButtonClicked)
                    {
                        comboBox.State = "Hover";
                    }
                }
            }
            else
            {
                if(comboBox.State == "Active")
                {
                    for(int i = 0; i < textCount; i++)
                    {
                        var itemRect = rect;
                        itemRect.Y += (i + 1) * rect.Height;
                        bool inItemRect = itemRect.Contains(Input.Mouse.MousePos);
                        if(inItemRect)
                        {
                            if(Input.Mouse.LeftButtonState == InputState.Up)
                            {
                                comboBox.HoverIndex = i;
                            }
                            else if(Input.Mouse.LeftButtonState == InputState.Down)
                            {
                                comboBox.ActiveIndex = i;
                            }

                            if(Input.Mouse.LeftButtonClicked)
                            {
                                comboBox.SelectedIndex = i;
                                comboBox.State = "Normal";
                            }
                            break;
                        }
                    }
                }
            }

            #endregion

            #region Draw
            g.DrawBoxModel(rect, new Content(texts[comboBox.SelectedIndex]), Skin.current.ComboBox[comboBox.State]);
            g.LineWidth = 1;
            /* TODO Draw this trangle as a content */
            var trianglePoints = new Point[3];
            trianglePoints[0].X = rect.TopRight.X - 5;
            trianglePoints[0].Y = rect.TopRight.Y + 0.2 * rect.Height;
            trianglePoints[1].X = trianglePoints[0].X - 0.6 * rect.Height;
            trianglePoints[1].Y = trianglePoints[0].Y;
            trianglePoints[2].X = trianglePoints[0].X - 0.3 * rect.Height;
            trianglePoints[2].Y = trianglePoints[0].Y + 0.6 * rect.Height;
            g.StrokePolygon(trianglePoints, CairoEx.ColorBlack);
            #endregion
            if(comboBox.State == "Active")
            {
                for(int i = 0; i < textCount; i++)
                {
                    var itemRect = rect;
                    itemRect.Y += (i + 1)*rect.Height;
                    if( i == comboBox.ActiveIndex)
                    {
                        gTop.DrawBoxModel(itemRect, new Content(texts[i]), Skin.current.ComboBox["Item:Active"]);
                    }
                    else if(i == comboBox.HoverIndex)
                    {
                        gTop.DrawBoxModel(itemRect, new Content(texts[i]), Skin.current.ComboBox["Item:Hover"]);
                    }
                    else
                    {
                        gTop.DrawBoxModel(itemRect, new Content(texts[i]), Skin.current.ComboBox["Item"]);
                    }
                }
            }
            return comboBox.SelectedIndex;
        }
    }
}
