using System;
using System.Collections.Generic;
using System.Text;
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
    /// The BaseTweener class handles moving a Position from start to end in the specified time using a specific function.
    /// Whenever the Tweener is updated, which is done by a call to Update, it will move the Position further along 
    /// the path to completion. On each update an updated event is called so you can respond to the Position change.
    /// When the Tweener has reached the end it will stop and signal that is it finished using the Ended event.
    /// It is possible to stop the Tweener, pausing it until it is started again.
    /// You can also reset the Tweener to repeat the same movement, reset it with new parameters or even reverse the
    /// direction of the tweener.
    /// 
    /// Note that this is an abstract class, refer to the concrete subclasses for tweening the value you want tweened.
    /// </summary>
    public abstract class BaseTweener<T>
    {
        #region Properties
        /// <summary>
        /// Create a Tweener with info on where to move from and to, how long it should take and the function to use.
        /// </summary>
        /// <param name="from">The starting position</param>
        /// <param name="to">The position reached at the end</param>
        /// <param name="duration">How long befor we reach the end?</param>
        /// <param name="tweeningFunction">Which function to use for calculating the current position.</param>
        protected BaseTweener(T from, T to, float duration, TweeningFunction tweeningFunction)
        {
            _from = from;
            _position = from;
            _change = CalculateChange(to, from);
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
        protected BaseTweener(T from, T to, TimeSpan duration, TweeningFunction tweeningFunction)
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
        protected BaseTweener(float duration, TweeningFunction tweeningFunction)
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
        protected BaseTweener(T from, T to, TweeningFunction tweeningFunction, float speed)
        {
            _from = from;
            _position = from;
            _change = CalculateChange(to, from);
            _tweeningFunction = tweeningFunction;
            _duration = CalculateDurationFromSpeed(speed);
        }
        #endregion

        #region Properties
        private T _position;
        /// <summary>
        /// This is the current position of the tweener. It cannot be manipulted directly.
        /// Use the Reset method to alter the behaviour of the tweener.
        /// </summary>
        public T Position
        {
            get
            {
                return _position;
            }
            protected set
            {
                _position = value;
                if (PositionChanged != null)
                {
                    PositionChanged(_position);
                }
            }
        }

        private T _from;
        /// <summary>
        /// This is the positon where the tweener started.
        /// </summary>
        protected T from
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

        private T _change;
        /// <summary>
        /// This is the change to the tweener over its lifetime.
        /// </summary>
        protected T change
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

        public delegate void PositionChangedHandler(T newPosition);
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
                Position = CalculateEndPosition();
                if (Ended != null)
                {
                    Ended();
                }
            }
            else
            {
                UpdatePosition(elapsed, from, change, duration);
            }
        }

        /// <summary>
        /// Do the actual update of the position.
        /// Usually we will use the tweening function here.
        /// </summary>
        /// <param name="timeElapsed">The time that has elapsed since the beginning of the tweener.</param>
        /// <param name="start">Where did the tweener start</param>
        /// <param name="change">How much will the tweener move from start to end</param>
        /// <param name="duration">The total duration of tweening.</param>
        protected abstract void UpdatePosition(float elapsed, T from, T change, float duration);

        /// <summary>
        /// Calculate the change value. Usually this is to - from.
        /// </summary>
        /// <param name="to">Where do we want to end</param>
        /// <param name="from">Where we are now</param>
        /// <returns>Returns the new change value</returns>
        protected abstract T CalculateChange(T to, T from);

        /// <summary>
        /// Calculate the position we want to end up in. This is nessecary as to is not saved.
        /// Usually this is from + change
        /// </summary>
        /// <returns>Returns the end position when the tweener is finished.</returns>
        protected abstract T CalculateEndPosition();

        /// <summary>
        /// Calculate the new change value if we reverse the tweener from the current position.
        /// Usually this is from - Position
        /// </summary>
        /// <returns>Returns the new change value when tweening is reversed</returns>
        protected abstract T CalculateReverseChange();

        /// <summary>
        /// Calculate the duration of the tween in seconds given the average speed of movement.
        /// Usually this is change / speed
        /// </summary>
        /// <param name="speed">The average movement speed</param>
        /// <returns>The duration of the tweener</returns>
        protected abstract float CalculateDurationFromSpeed(float speed);

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
        public void Reset(T to)
        {
            change = CalculateChange(to, Position);
            Reset();
        }

        /// <summary>
        /// Reset the tweener with a new set of from and to positons.
        /// </summary>
        /// <param name="to">The new position to move to</param>
        /// <param name="duration">The new duration of the tweener</param>
        public void Reset(T to, TimeSpan duration)
        {
            Reset(to);
            this.duration = (float)duration.TotalSeconds;
        }

        /// <summary>
        /// Reset the tweener with a new set of from and to positons.
        /// </summary>
        /// <param name="to">The new position to move to</param>
        /// <param name="duration">The new average speed of tweener movement</param>
        public void Reset(T to, float speed)
        {
            Reset(to);
            this.duration = CalculateDurationFromSpeed(speed);
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
            change = CalculateReverseChange();
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
                CalculateEndPosition(),
                duration,
                elapsed);
        }
        #endregion
    }
}
