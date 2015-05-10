using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMGUI
{
    class Scalar
    {
        const float SPEED = 2.0f;

	    public void lerp()
        {
	        float DIFF = myTarget - myCurrent;

	        if(DIFF < 1.0f)
		        myCurrent = myTarget;
	        else
		        myCurrent += DIFF * Timer.elapsed() * SPEED;
        }

        public void set(int aValue)
        {
            myTarget = (float)aValue;
        }

        public int get()
        {
            //return (int)myCurrent;
            return (int)myTarget;
        }

        private float myCurrent = 0.0f, myTarget = 0.0f;
    }
}
