using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNATweener;
using System.Reflection;

namespace Tweening
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        Texture2D sprite;
        Vector2 spritePosition;
        float spriteScale;

        Texture2D pointer;
        Vector2 pointerPosition;

        Tweener tweenerX;
        Tweener tweenerY;

        float duration = 0.5f;

        Queue<Type> transitions;
        Type currentTransition;
        enum Easing { EaseIn, EaseOut, EaseInOut };
        Easing easing;

#if !XBOX
        MouseState oldMouseState;
#endif
        KeyboardState oldKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            transitions = new Queue<Type>(new Type[] {
                  typeof(Linear),
                  typeof(Quadratic),
                  typeof(Cubic),
                  typeof(Quartic),
                  typeof(Quintic),
                  typeof(Sinusoidal),
                  typeof(Exponential),
                  typeof(Circular),
                  typeof(Elastic),
                  typeof(Back),
                  typeof(Bounce)
                });
            SwitchTransition();
            easing = Easing.EaseIn;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sprite = Content.Load<Texture2D>("woot");
            pointer = Content.Load<Texture2D>("cursorarrow");

            font = Content.Load<SpriteFont>("Arial");

            spritePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            pointerPosition = spritePosition;
            spriteScale = 0.25f;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                || Keyboard.GetState().IsKeyDown(Keys.Escape))                
                this.Exit();

            bool newTweener = false;
#if !XBOX
            CheckMouseControls(ref newTweener);
#endif
            CheckKeyboardControls();

            if (newTweener)
            {
                tweenerX = new Tweener(spritePosition.X, pointerPosition.X, TimeSpan.FromSeconds(duration), GetTweeningFunction());
                tweenerY = new Tweener(spritePosition.Y, pointerPosition.Y, TimeSpan.FromSeconds(duration), GetTweeningFunction());
            }

            if (tweenerX != null)
            {
                tweenerX.Update(gameTime);
                spritePosition.X = tweenerX.Position;
            }
            if (tweenerY != null)
            {
                tweenerY.Update(gameTime);
                spritePosition.Y = tweenerY.Position;
            }

            base.Update(gameTime);
        }

        private void CheckKeyboardControls()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (oldKeyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyUp(Keys.Up))
            {
                SwitchTransition();
            }
            if (oldKeyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyUp(Keys.Down))
            {
                SwitchEasing();
            }
            if (oldKeyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.Left))
            {
                AdjustDuration(-1);
            }
            if (oldKeyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Right))
            {
                AdjustDuration(1);
            }
            oldKeyboardState = keyboardState;
        }

        private void CheckMouseControls(ref bool newTweener)
        {
            MouseState mouseState = Mouse.GetState();
            pointerPosition.X = mouseState.X;
            pointerPosition.Y = mouseState.Y;
            KeyboardState keyboardState = Keyboard.GetState();
            if ((oldMouseState.LeftButton == ButtonState.Pressed) && (mouseState.LeftButton == ButtonState.Released))
            {
                if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
                {
                    ResetTweener(false);
                }
                else if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                {
                    ResetTweener(true);
                }
                else
                {
                    newTweener = true;
                }
            }
            if ((oldMouseState.RightButton == ButtonState.Pressed) && (mouseState.RightButton == ButtonState.Released))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    ReverseTweener();
                }
                else
                {
                    ToggleTweenerRunning();
                }
            }
            oldMouseState = mouseState;
        }

        private void ResetTweener(bool resetToPosition)
        {
            if (resetToPosition)
            {
                tweenerX.Reset(pointerPosition.X);
                tweenerY.Reset(pointerPosition.Y);
            }
            else
            {
                tweenerX.Reset();
                tweenerY.Reset();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            Vector2 hudPosition = new Vector2(10);
            Vector2 lineSpacing = new Vector2(0, font.LineSpacing);
            foreach (string line in GetHudLines())
            {
                spriteBatch.DrawString(font, line, hudPosition, Color.LawnGreen);
                if (!String.IsNullOrEmpty(line))
                {
                    hudPosition += lineSpacing;
                }
            }

            spriteBatch.Draw(pointer, pointerPosition, null, Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(sprite, spritePosition, null, Color.White, 0, new Vector2(sprite.Width, sprite.Height) / 2, spriteScale, SpriteEffects.None, 1);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private IEnumerable<string> GetHudLines()
        {
            return new string[] {
                String.Format("Left Click: Apply new transition {0} {1} for {2} secs", currentTransition.Name, easing, duration),
                "Ctrl + Left Click: Reset transition",
                "Shift + Left Click: Reset transition with new position",
                (tweenerX == null) ? "" : "Right Click: " + ((tweenerX.Running) ? "Stop" : "Start"),
                "Up: Switch transition",
                "Down: Switch easing",
                "Left/Right: Change speed",
                "Ctrl + Left Click: Reverse (does not change transition type)",
                (tweenerX == null) ? "" : "Running: " + tweenerX,
                (tweenerY == null) ? "" : "Running: " + tweenerY
            };
        }

        #region Action functions
        private TweeningFunction GetTweeningFunction()
        {
            return (TweeningFunction)Delegate.CreateDelegate(typeof(TweeningFunction), currentTransition, easing.ToString());
        }

        private void SwitchTransition()
        {
            if (currentTransition != null)
            {
                transitions.Enqueue(currentTransition);
            }
            currentTransition = transitions.Dequeue();
        }

        private void SwitchEasing()
        {
            if (easing == Easing.EaseInOut)
            {
                easing = Easing.EaseIn;
            }
            else
            {
                easing++;
            }
        }

        private void AdjustDuration(int amount)
        {
            if (duration < 1.0f)
            {
                duration += amount * 0.1f;
                return;
            }
            if (duration < 5.0f)
            {
                duration += amount * 0.5f;
                return;
            }
            duration += amount;
        }

        private void ReverseTweener()
        {
            if (tweenerX != null)
            {
                tweenerX.Reverse();
                tweenerY.Reverse();
            }
        }

        private void ToggleTweenerRunning()
        {
            if (tweenerX != null)
            {
                if (tweenerX.Running)
                {
                    tweenerX.Stop();
                    tweenerY.Stop();
                }
                else
                {
                    tweenerX.Start();
                    tweenerY.Start();
                }
            }
        }
        #endregion
    }
}
