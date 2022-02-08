using System;
using System.Collections.Generic;

namespace ImGui.Common
{
    public static class EnumHelpers
    {
        public static string ToStringExtended<T>(this Enum e) where T : struct, System.Enum
        {
            if (!(e.GetType().GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0))
                return e.ToString();

            List<string> eNames = new List<string>();
            foreach (T fish in Enum.GetValues<T>())
            {
                Enum num = fish as Enum;
                if (e.HasFlag(num) && Convert.ToInt32(fish) != 0 && Convert.ToInt32(fish) != Convert.ToInt32(e))
                    eNames.Add(fish.ToString());
            }

            return eNames.Count > 1 ? String.Join("|", eNames.ToArray()) : e.ToString();
        }
        
    }
}