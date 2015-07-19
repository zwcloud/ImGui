using System.Collections.Generic;
using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class Radio : Control
    {
        internal static Dictionary<string, HashSet<string>> Groups;

        static Radio()
        {
            Groups = new Dictionary<string, HashSet<string>>();
        }

        private string groupName;
        internal string GroupName
        {
            get { return groupName; }
            private set
            {
                if(groupName!=null)
                {
                    if(value == groupName) return;
#if DEBUG
                    Debug.Assert(!Groups.ContainsKey(groupName),
                        "Older group is not recorded.");
                    var removed = Groups[groupName].Remove(Name);
                    Debug.Assert(removed, "This radio is not in the Group <{0}>.", groupName);
#else
                    Groups[_groupName].Remove(Name);
#endif
                }
                if(!Groups.ContainsKey(value))
                    Groups[value] = new HashSet<string>();
                Groups[value].Add(Name);
                groupName = value;
            }
        }

        private bool Actived { get; set; }

        internal Radio(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        internal static bool DoControl(Context g, Rect rect, string text, string groupName, bool value, string name)
        {
            #region Get control reference
            Radio radio;
            if(!Controls.ContainsKey(name))
            {
                radio = new Radio(name);
            }
            else
            {
                radio = Controls[name] as Radio;
            }

            Debug.Assert(radio != null);
            #endregion

            #region Set control data
            radio.GroupName = groupName;
            #endregion

            #region Logic
            if(rect.Contains(Input.MousePos))
            {
                if(Input.LeftButtonState == InputState.Up)
                {
                    radio.State = "Hover";
                }
                else if(Input.LeftButtonState == InputState.Down)
                {
                    radio.State = "Active";
                }

                if(Input.LeftButtonClicked)
                {
                    radio.Actived = true;
                }
            }
            else
            {
                radio.State = "Normal";
            }
            if(radio.Actived)
            {
                var group = Groups[groupName];
                foreach (var radioName in group)
                {
#if DEBUG
                    Debug.Assert(Controls[radioName] is Radio);
#endif
                    ((Radio) Controls[radioName]).Actived = false;
                }
                radio.Actived = true;
            }
            #endregion

            #region Draw
            var radioBoxRect = new Rect(rect.TopLeft, new Size(20, 20));
            var radioBoxCenter = radioBoxRect.Center;
            g.StrokeCircle(radioBoxCenter, 10, CairoEx.ColorBlack);
            if(!radio.Actived)
            {
                if(radio.State == "Hover")
                    g.FillCircle(radioBoxCenter, 5, CairoEx.ColorRgb(46, 167, 224));
                else if(radio.State == "Active")
                    g.FillCircle(radioBoxCenter, 5, CairoEx.ColorBlack);
            }
            else
            {
                if(radio.State == "Normal")
                    g.FillCircle(radioBoxCenter, 5, CairoEx.ColorBlack);
                else if(radio.State == "Hover")
                {
                    g.FillCircle(radioBoxCenter, 5, CairoEx.ColorRgb(46, 167, 224));
                    g.StrokeCircle(radioBoxCenter, 5, CairoEx.ColorBlack);
                }
                else if(radio.State == "Active")
                    g.StrokeCircle(radioBoxCenter, 5, CairoEx.ColorBlack);
            }

            var radioTextRect = new Rect(radioBoxRect.TopRight + new Vector(5,0),
                rect.BottomRight);
            g.DrawBoxModel(radioTextRect, new Content(text), Skin.current.Radio[radio.State]);
            #endregion


            return radio.Actived;
        }
    }
}