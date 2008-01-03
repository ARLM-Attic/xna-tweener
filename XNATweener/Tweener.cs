using System;
using Microsoft.Xna.Framework;

namespace XNATweener
{
    public delegate float TweeningFunction(float timeElapsed, float start, float change, float duration);

    public class Tweener
    {
        public Tweener(float from, float to, TimeSpan duration, TweeningFunction tweeningFunction)
        {
            _from = from;
            _position = from;
            _change = to - from;
            _tweeningFunction = tweeningFunction;
            _duration = duration;
        }

        #region Properties
        private float _position;
        public float position
        {
            get
            {
                return _position;
            }
            protected set
            {
                _position = value;
            }
        }

        private float _from;
        protected float from
        {
            get
            {
                return _from;
            }
        }

        private float _change;
        protected float change
        {
            get
            {
                return _change;
            }
        }

        private TimeSpan _duration;
        protected TimeSpan duration
        {
            get
            {
                return _duration;
            }
        }

        private TimeSpan _elapsed = TimeSpan.Zero;
        protected TimeSpan elapsed
        {
            get
            {
                return _elapsed;
            }
            set
            {
                _elapsed = value;
            }
        }

        private TweeningFunction _tweeningFunction;
        protected TweeningFunction tweeningFunction
        {
            get
            {
                return _tweeningFunction;
            }
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            position = tweeningFunction((float)elapsed.TotalSeconds, from, change, (float)duration.TotalSeconds);
            elapsed += gameTime.ElapsedGameTime;
            if (elapsed > duration)
            {
                elapsed = duration;
            }
        }

        public void Reset()
        {
            elapsed = TimeSpan.Zero;
            position = from;
        }
        #endregion
    }
}
