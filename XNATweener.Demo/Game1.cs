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

        Type transitionClass;
        enum Easing { In, Out, InOut };
        Easing easing;

        MouseState oldMouseState;

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

            transitionClass = typeof(Bounce);
            easing = Easing.Out;
        }

        private TweeningFunction GetTweeningFunction()
        {
            return (TweeningFunction)Delegate.CreateDelegate(typeof(TweeningFunction), transitionClass, "Ease" + easing);
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
            MouseState mouseState = Mouse.GetState();
            pointerPosition.X = mouseState.X;
            pointerPosition.Y = mouseState.Y;
            if ((oldMouseState.LeftButton == ButtonState.Pressed) && (mouseState.LeftButton == ButtonState.Released))
            {
                newTweener = true;
            }
            oldMouseState = mouseState;
#endif

            if (newTweener)
            {
                tweenerX = new Tweener(spritePosition.X, pointerPosition.X, TimeSpan.FromSeconds(1.0), GetTweeningFunction());
                tweenerY = new Tweener(spritePosition.Y, pointerPosition.Y, TimeSpan.FromSeconds(1.0), GetTweeningFunction());
            }

            if (tweenerX != null)
            {
                tweenerX.Update(gameTime);
                spritePosition.X = tweenerX.position;
            }
            if (tweenerY != null)
            {
                tweenerY.Update(gameTime);
                spritePosition.Y = tweenerY.position;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            spriteBatch.DrawString(font, "Left Click: Apply transition", new Vector2(10), Color.LawnGreen);

            spriteBatch.Draw(pointer, pointerPosition, null, Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(sprite, spritePosition, null, Color.White, 0, new Vector2(sprite.Width, sprite.Height) / 2, spriteScale, SpriteEffects.None, 1);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
