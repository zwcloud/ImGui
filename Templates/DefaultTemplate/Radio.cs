using ImGui;

namespace DefaultTemplate
{
    class Radio
    {
        private readonly string[] labels;
        private readonly bool[] toggleValues;
        private readonly bool[] newToggleValues;
        private int activeToggleIndex = -1;
        
        public Radio(string[] labels)
        {
            this.labels = labels;
            toggleValues = new bool[labels.Length];
            newToggleValues = new bool[labels.Length];
        }

        public int DoGUI()
        {
            for (int i = 0; i < this.labels.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(this.labels[i], this.labels[i]);
                this.newToggleValues[i] = GUILayout.Toggle(this.toggleValues[i], this.labels[i]);
                GUILayout.EndHorizontal();
            }

            for (var i = 0; i < this.labels.Length; ++i)
            {
                var newValue = this.newToggleValues[i];
                if (newValue != this.toggleValues[i] & newValue)
                {
                    this.activeToggleIndex = i;
                }
                this.toggleValues[i] = false;
            }

            if (this.activeToggleIndex != -1)
            {
                this.toggleValues[this.activeToggleIndex] = true;
            }

            return activeToggleIndex;
        }

    }
}