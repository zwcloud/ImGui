using System;
using System.Collections.Generic;
using System.Linq;
using Cairo;

namespace ImGui
{
    /// <summary>
    /// Base class of all controls
    /// </summary>
    /// <remarks>
    /// Every control must implement a DoControl method to call by GUI.
    /// </remarks>
    internal abstract class Control : IDisposable, IUpdatable, IRenderable
    {
        private string name;

        protected Control(string name, BaseForm form)
        {
            Form = form;
            Name = name;
            State = "Normal";
            Active = false;
            NeedRepaint = true;

            Form.Controls[Name] = this;
        }

        public string Name
        {
            get { return name; }
            set
            {
                if(Form.Controls.Keys.Contains(value))
                    throw new ArgumentException("Specified Control name is already used.");
                name = value;
            }
        }

        public string State { get; set; }
        internal BaseForm Form { get; set; }
        public bool Active { get; set; }
        public Rect Rect { get; protected set; }

        /// <summary>
        /// Does this control need repaint? TODO expand this into render tree
        /// </summary>
        public bool NeedRepaint { get; set; }
        public abstract void Dispose();
        public abstract void OnRender(Context g);
        public abstract void OnClear(Context g);
        public abstract void OnUpdate();
    }
}