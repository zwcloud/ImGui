using System;
using System.Collections.Generic;
using System.Linq;
using Cairo;

namespace IMGUI
{
    /// <summary>
    /// Base class of all controls
    /// </summary>
    /// <remarks>
    /// Must implement a functional method to call by user.
    /// </remarks>
    abstract class Control : IDisposable
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (Form.Controls.Keys.Contains(value))
                    throw new ArgumentException("Specified Control name is already used.");
                name = value;
            }
        }

        public string State { get; set; }

        internal BaseForm Form { get; set; }

        /// <summary>
        /// Does this control need repaint?
        /// </summary>
        public bool NeedRepaint { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public abstract void OnUpdate();

        public abstract void OnRender(Context g);

        protected Control(string name, BaseForm form)
        {
            Form = form;
            Name = name;
            Params = new Dictionary<string, object>();
            State = "Normal";
            NeedRepaint = true;

            Form.Controls[Name] = this;
        }

        #region Implementation of IDisposable

        public abstract void Dispose();

        #endregion
    }
}
