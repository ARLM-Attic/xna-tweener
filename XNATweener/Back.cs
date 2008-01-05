using System;
using System.Collections.Generic;
using System.Text;

namespace XNATweener
{
    public static class Back
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * ((1.70158f + 1) * t - 1.70158f) + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1) + b;
        }

        /// NOTE: This method is not really working...
        public static float EaseInOut(float t, float b, float c, float d)
        {
            float s = 1.70158f * 1.525f;
            if ((t /= d / 2) < 1)
            {
                return c / 2 * (t * t * ((s + 1) * t - 1.70158f)) + b;
            }
            t -= 2;
            return c / 2 * (t * t * ((s + 1) * t + 1.70158f) + 2) + b;
        }
    }
}
