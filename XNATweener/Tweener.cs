using System;
using Microsoft.Xna.Framework;

namespace XNATweener
{
    /// <summary>
    /// This is the delegate of the tweening functions.
    /// All functions must calculate the current position of the tweener based on how long has elapsed,
    /// where to start, the total amount to move and the total duration.
    /// See the library classes for several useful tweening functions.
    /// </summary>
    /// <param name="timeElapsed">The time that has elapsed since the beginning of the tweener.</param>
    /// <param name="start">Where did the tweener start</param>
    /// <param name="change">How much will the tweener move from start to end</param>
    /// <param name="duration">The total duration of tweening.</param>
    /// <returns></returns>
    public delegate float TweeningFunction(float timeElapsed, float start, float change, float duration);

    /// <summary>
    /// The Tweener class handles moving a Position from start to end in the specified time using a specific function.
    /// Whenever the Tweener is updated, which is done by a call to Update, it will move the Position further along 
    /// the path to completion. On each update an updated event is called so you can respond to the Position change.
    /// When the Tweener has reached the end it will stop and signal that is it finished using the Ended event.
    /// It is possible to stop the Tweener, pausing it until it is started again.
    /// You can also reset the Tweener to repeat the same movement, reset it with new parameters or even reverse the
    /// direction of the tweener.
    /// </summary>
    public class Tweener
    {
        #region Constructors
        /// <summary>
        /// Create a Tweener with info on where to move from and to, how long it should take and the function to use.
        /// </summary>
        /// <param name="from">The starting position</param>
        /// <param name="to">The position reached at the end</param>
        /// <param name="duration">How long befor we reach the end?</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        public Tweener(float from, float to, float duration, TweeningFunction tweeningFunction)
        {
            _from = from;
            _position = from;
            _change = to - from;
            _tweeningFunction = tweeningFunction;
            _duration = duration;
        }

        /// <summary>
        /// Create a Tweener with info on where to move from and to, how long it should take and the function to use.
        /// </summary>
        /// <param name="from">The starting position</param>
        /// <param name="to">The position reached at the end</param>
        /// <param name="duration">How long befor we reach the end?</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        public Tweener(float from, float to, TimeSpan duration, TweeningFunction tweeningFunction)
            : this(from, to, (float)duration.TotalSeconds, tweeningFunction)
        {
        }

        /// <summary>
        /// Create a stopped tweener with no information on where to move from and to.
        /// Useful in conjunction with the Reset(from, to) call to ready a tweener for later use or lazy
        /// instantiation of a tweener in a property.
        /// </summary>
        /// <param name="duration">The duration of tweening.</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        public Tweener(float duration, TweeningFunction tweeningFunction)
        {
            _duration = duration;
            _tweeningFunction = tweeningFunction;
            _running = false;
        }

        /// <summary>
        /// Create a stopped tweener with no information on where to move from and to.
        /// Useful in conjunction with the Reset(from, to) call to ready a tweener for later use or lazy
        /// instantiation of a tweener in a property.
        /// </summary>
        /// <param name="duration">The duration of tweening.</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        public Tweener(TimeSpan duration, TweeningFunction tweeningFunction)
            : this((float)duration.TotalSeconds, tweeningFunction)
        {
        }

        /// <summary>
        /// Create a Tweener with info on where to move from and to, but set the duration using the movement
        /// speed instead of a set timespan.
        /// Note that the speed is used to calculate how fast the tweener should move if it moved in a linear
        /// fashion. This can be upset by the tweening function that can cause the actual movement speed to vary
        /// considerably. So the speed can be looked at as an average speed during the lifetime of the tweener.
        /// </summary>
        /// <param name="from">The starting position</param>
        /// <param name="to">The position reached at the end</param>
        /// <param name="duration">The average movement speed of the tweener</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        public Tweener(float from, float to, TweeningFunction tweeningFunction, float speed)
        {
            _from = from;
            _position = from;
            _change = to - from;
            _tweeningFunction = tweeningFunction;
            _duration = change / speed;           
        }
        #endregion

        #region Properties
        private float _position;
        /// <summary>
        /// This is the current position of the tweener. It cannot be manipulted directly.
        /// Use the Reset method to alter the behaviour of the tweener.
        /// </summary>
        public float Position
        {
            get
            {
                return _position;
            }
            protected set
            {
                if (_position != value)
                {
                    _position = value;
                    if (PositionChanged != null)
                    {
                        PositionChanged(_position);
                    }
                }
            }
        }

        private float _from;
        /// <summary>
        /// This is the positon where the tweener started.
        /// </summary>
        protected float from
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        private float _change;
        /// <summary>
        /// This is the change to the tweener over its lifetime.
        /// </summary>
        protected float change
        {
            get
            {
                return _change;
            }
            set
            {
                _change = value;
            }
        }

        private float _duration;
        /// <summary>
        /// This is the duration of the tweener in seconds.
        /// </summary>
        protected float duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }

        private float _elapsed = 0.0f;
        /// <summary>
        /// This is the total time that has elapsed since the tweener last started.
        /// </summary>
        protected float elapsed
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

        private bool _running = true;
        /// <summary>
        /// Is the tweener currently running. If the tweener is not running, calling Update will not move the
        /// tweener.
        /// </summary>
        public bool Running
        {
            get { return _running; }
            protected set { _running = value; }
        }

        private TweeningFunction _tweeningFunction;
        /// <summary>
        /// This is the function that determines the actual movement of the tweener.
        /// </summary>
        protected TweeningFunction tweeningFunction
        {
            get
            {
                return _tweeningFunction;
            }
        }

        public delegate void PositionChangedHandler(float newPosition);
        /// <summary>
        /// Event that is called whenever the position of the tweener has changed
        /// </summary>
        public event PositionChangedHandler PositionChanged;

        public delegate void EndHandler();
        /// <summary>
        /// Event that is called when the tweener reaches the end. At this point in time the tweener is guaranteed to
        /// to be at the ending position no matter how many times it was stopped and started.
        /// </summary>
        public event EndHandler Ended;
        #endregion

        #region Methods
        /// <summary>
        /// Update the position of the tweener using the current game time.
        /// If the position is paused or has finished, no update to the position or the elapsed time will happen.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            if (!Running || (elapsed == duration))
            {
                return;
            }
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= duration)
            {
                elapsed = duration;
                Position = from + change;
                if (Ended != null)
                {
                    Ended();
                }
            }
            else
            {
                Position = tweeningFunction(elapsed, from, change, duration);
            }
        }

        /// <summary>
        /// Start the tweener if it is paused. If it is already running, nothing happens.
        /// </summary>
        public void Start()
        {
            Running = true;
        }

        /// <summary>
        /// Stop the tweener if it is running. If it is already stopped, nothing happens.
        /// </summary>
        public void Stop()
        {
            Running = false;
        }

        /// <summary>
        /// Reset the tweenr to start again from the beginning.
        /// </summary>
        public void Reset()
        {
            elapsed = 0.0f;
            from = Position;
        }

        /// <summary>
        /// Reset the tweener to move to a new position from the current position.
        /// Great for extending movement from the current position when something happens.
        /// </summary>
        /// <param name="to">The new position to move to</param>
        public void Reset(float to)
        {
            change = to - Position;
            Reset();
        }

        /// <summary>
        /// Reset the tweener with a new set of from and to positons.
        /// </summary>
        /// <param name="to">The new position to move to</param>
        /// <param name="duration">The new duration of the tweener</param>
        public void Reset(float to, TimeSpan duration)
        {            
            Reset(to);
            this.duration = (float)duration.TotalSeconds;
        }

        /// <summary>
        /// Reset the tweener with a new set of from and to positons.
        /// </summary>
        /// <param name="to">The new position to move to</param>
        /// <param name="duration">The new average speed of tweener movement</param>
        public void Reset(float to, float speed)
        {
            Reset(to);
            this.duration = change / speed;
        }

        /// <summary>
        /// Reverses movement of the tweener from the current position back to where it came.
        /// This can reverse the tweener before it is done, but be aware that reversing the tweener again
        /// later will not return it to its original destination, but to the point where it was reversed
        /// for the first time.
        /// </summary>
        public void Reverse()
        {
            elapsed = 0.0f;
            change = -change + (from + change - Position);
            from = Position;
        }

        /// <summary>
        /// Gives a textual representation of the tweener.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}. Tween {2} -> {3} in {4}s. Elapsed {5:##0.##}s",
                tweeningFunction.Method.DeclaringType.Name,
                tweeningFunction.Method.Name,
                from, 
                from + change, 
                duration, 
                elapsed);
        }
        #endregion
    }
}
