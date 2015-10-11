using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Application.MainForm.Controls.Keys.Contains(value))
                    throw new ArgumentException("Specified Control name is already used.");
                _name = value;
            }
        }

        public string State { get; set; }

        /// <summary>
        /// Does this control need repaint?
        /// </summary>
        public bool NeedRepaint { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public abstract void OnUpdate();

        public abstract void OnRender(Context g);

        protected Control()
        {
            Name = String.Empty;
            Params = new Dictionary<string, object>();
            State = "Normal";
        }

        protected Control(string name)
        {
            Name = name;
            Params = new Dictionary<string, object>();
            State = "Normal";
        }

        #region Implementation of IDisposable

        public abstract void Dispose();

        #endregion
    }
}
