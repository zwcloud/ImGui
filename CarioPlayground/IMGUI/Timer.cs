using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32;

namespace IMGUI
{
    class Timer
    {
        static float locTime = 0.0f, locElapsed = 0.0f;

        public static void update()
        {
	        float old = locTime;
	        locTime = (float)Native.GetTickCount() * 0.001f; 
	        locElapsed = locTime - old;
	        if(locElapsed > 1.0f)
		        locElapsed = 0.0f;
        }

	    public static float now()
        {
            return locTime;
        }

        public static float elapsed()
        {
            return locElapsed;
        }

        public static bool isFuture(float aTime)
        {
            return aTime > now();
        }
    }
}
