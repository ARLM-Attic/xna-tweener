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
        Texture2D sprite;
        Vector2 position;
        Tweener tweener;
        SpriteFont font;
        Type transitionClass;
        enum Easing { In, Out, InOut };
        Easing easing;

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
            sprite = Content.Load<Texture2D>("explosion");

            font = Content.Load<SpriteFont>("Arial");

            position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            transitionClass = typeof(Bounce);
            easing = Easing.Out;

            tweener = new Tweener(position.X, sprite.Width / 2, TimeSpan.FromSeconds(1.0), GetTweeningFunction());
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

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                tweener.Reset();
            }

            tweener.Update(gameTime);
            position.X = tweener.position;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None);
            spriteBatch.DrawString(font, "SPACE: Reset", new Vector2(10), Color.Green);

            spriteBatch.Draw(sprite, position - (new Vector2(sprite.Width, sprite.Height) / 2), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
