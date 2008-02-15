using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATweener;
using Microsoft.Xna.Framework.Input;

namespace Tweening
{
    class LoopDemo : DrawableGameComponent, IUsageText
    {
        public LoopDemo(Game game)
            : base(game)
        {
        }

        #region Fields
        protected SpriteBatch spriteBatch;

        protected Texture2D sprite;

        protected Tweener frontToBackTweener;
        protected Tweener frontToBackTimesTweener;
        protected Tweener backAndForthTweener;
        protected Tweener backAndForthTimesTweener;

        GamePadState oldPadState;
        KeyboardState oldKeyboardState;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            CreateTweeners();
        }

        private void CreateTweeners()
        {
            float start = GraphicsDevice.Viewport.Width * 0.1f;
            float end = GraphicsDevice.Viewport.Width * 0.9f;
            frontToBackTweener = new Tweener(start, end, 3, Quadratic.EaseInOut);
            frontToBackTweener.Loop.FrontToBack();
            frontToBackTimesTweener = new Tweener(start, end, 3, Quadratic.EaseInOut);
            Loop.FrontToBack(frontToBackTimesTweener, 5);
            backAndForthTweener = new Tweener(start, end, 3, Sinusoidal.EaseInOut);
            Loop.BackAndForth(backAndForthTweener);
            backAndForthTimesTweener = new Tweener(start, end, 3, Sinusoidal.EaseInOut);
            backAndForthTimesTweener.Loop.BackAndForth(5);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sprite = Game.Content.Load<Texture2D>("woot");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            frontToBackTweener.Update(gameTime);
            frontToBackTimesTweener.Update(gameTime);
            backAndForthTweener.Update(gameTime);
            backAndForthTimesTweener.Update(gameTime);

            CheckPadControls();
            CheckKeyboardControls();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            Vector2 origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            spriteBatch.Draw(sprite, new Vector2(frontToBackTweener.Position, GraphicsDevice.Viewport.Height * 0.2f), null, Color.White, 0, origin, 0.2f, SpriteEffects.None, 1);
            spriteBatch.Draw(sprite, new Vector2(frontToBackTimesTweener.Position, GraphicsDevice.Viewport.Height * 0.4f), null, Color.White, 0, origin, 0.2f, SpriteEffects.None, 1);
            spriteBatch.Draw(sprite, new Vector2(backAndForthTweener.Position, GraphicsDevice.Viewport.Height * 0.6f), null, Color.White, 0, origin, 0.2f, SpriteEffects.None, 1);
            spriteBatch.Draw(sprite, new Vector2(backAndForthTimesTweener.Position, GraphicsDevice.Viewport.Height * 0.8f), null, Color.White, 0, origin, 0.2f, SpriteEffects.None, 1);
            spriteBatch.End();
        }

        #region Input Methods
        private void CheckKeyboardControls()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (oldKeyboardState.IsKeyDown(Keys.Space) && keyboardState.IsKeyUp(Keys.Space))
            {
                CreateTweeners();
            }
            oldKeyboardState = keyboardState;
        }

        private void CheckPadControls()
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (oldPadState.IsButtonDown(Buttons.A) && padState.IsButtonUp(Buttons.A))
            {
                CreateTweeners();
            }
            oldPadState = padState;
        }
        #endregion

        #region Draw helpers
        public IEnumerable<string> GetUsageText()
        {
            yield return "First and second moves front to back.";
            yield return "Third and fourth moves back and forth.";
            yield return "First and second moves continuously.";
            yield return "Third and fourth moves five times.";
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                yield return "A: Restart tweeners";
            }
            else
            {
                yield return "Space: Restart tweeners";
            }
        }
    	#endregion
    }
}
