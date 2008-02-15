using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XNATweener;
using Microsoft.Xna.Framework.Graphics;

namespace Tweening
{
    class ColorDemo : DrawableGameComponent, IUsageText
    {
        public ColorDemo(Game game)
            : base(game)
        {
        }

        #region Fields
        ColorTweener tweener;
        ColorTweener tweener1;
        #endregion

        #region IUsageText Members
        public IEnumerable<string> GetUsageText()
        {
            yield return String.Format("Position: ({0}, {1}, {2})", tweener.Position.R, tweener.Position.G, tweener.Position.B);
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            tweener = new ColorTweener(Color.Red, Color.Blue, 3.0f, Circular.EaseInOut);
            tweener1 = new ColorTweener(Color.Blue, Color.Red, 3.0f, Circular.EaseInOut);
            tweener.Ended += SwapTweeners;
            tweener1.Ended += SwapTweeners;
        }

        private void SwapTweeners()
        {
            ColorTweener temp = tweener;
            tweener = tweener1;
            tweener1 = temp;
            tweener.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            tweener.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.GraphicsDevice.Clear(tweener.Position);
        }
        #endregion
    }
}
