using System;
using System.Collections.Generic;
using System.Text;

namespace XNATweener
{
    public class Elastic
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
		    if (t==0) 
            {
                return b;
            }
            if ((t /= d) == 1) 
            {
                return b+c;
            }
            float p = d * .3f;
            float s = p / 4;
            return -(float)(c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
		    if (t==0) 
            {
                return b;
            }
            if ((t /= d) == 1) 
            {
                return b+c;
            }
            float p = d * .3f;
            float s = p / 4;
		    return (float)(c * Math.Pow(2,-10*t) * Math.Sin((t*d-s)*(2*Math.PI)/p ) + c + b);
	    }

        /// NOTE: This method is not really working...
        public static float EaseInOut(float t, float b, float c, float d)
        {
		    if (t==0) 
            {
                return b;
            }
            if ((t /= d/2) == 2) 
            {
                return b+c; 
            }
            float p = d * (.3f * 1.5f);
            float s = p / 4;
            if (t < 1)
            {
                return -.5f * (float)(c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            }
		    return c * (float)(Math.Pow(2,-10*(t-=1)) * Math.Sin((t*d-s) * (2*Math.PI) / p) * .5 + c + b);
	    }
    }
}
