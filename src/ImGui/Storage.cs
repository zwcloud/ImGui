using System.Collections.Generic;

namespace ImGui
{
    internal class Storage
    {
        public int GetInt(int key, int default_val = 0)
        {
            return IntData.GetValueOrDefault(key, default_val);
        }

        public void SetInt(int key, int val)
        {
            IntData[key] = val;
        }

        public bool GetBool(int key, bool default_val = false)
        {
            return BoolData.GetValueOrDefault(key, default_val);
        }

        public void SetBool(int key, bool val)
        {
            BoolData[key] = val;
        }

        public float GetFloat(int key, float default_val = 0.0f)
        {
            return FloatData.GetValueOrDefault(key, default_val);
        }

        public void SetFloat(int key, float val)
        {
            FloatData[key] = val;
        }
        
        private Dictionary<int, int> IntData = new Dictionary<int, int>();
        private Dictionary<int, bool> BoolData = new Dictionary<int, bool>();
        private Dictionary<int, float> FloatData = new Dictionary<int, float>();

        public int EntryCount => IntData.Count + BoolData.Count + FloatData.Count;
        public int EstimatedDataSizeInBytes =>
            EntryCount * sizeof(int)
            + IntData.Count * sizeof(int)
            + BoolData.Count * sizeof(bool)
            + FloatData.Count * sizeof(float);
    }
}