using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATweener.Test
{
    [TestFixture]
    public class ColorTweenerTest
    {
        protected ITweener<Color> tweener;

        [SetUp]
        public void SetUp()
        {
            tweener = new ColorTweener(Linear.EaseNone);
        }

        [Test]
        public void TestConstruction()
        {
            Assert.IsNotNull(tweener, "Test object is constructed correctly");
        }

        [Test]
        public void WeCanTweenFromBlackToWhite()
        {
            tweener.Reset(Color.Black, Color.White, TimeSpan.FromSeconds(10));
            tweener.Play();
            for (int i = 0; i < 10; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(i), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(i), TimeSpan.FromSeconds(1)));
                Console.WriteLine(tweener.Position);
            }
            Assert.AreEqual(Color.White, tweener.Position);
        }

        [Test]
        public void WeCanTweenFromWhiteToBlack()
        {
            tweener.Reset(Color.White, Color.Black, TimeSpan.FromSeconds(10));
            tweener.Play();
            for (int i = 0; i < 10; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(i), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(i), TimeSpan.FromSeconds(1)));
                Console.WriteLine(tweener.Position);
            }
            Assert.AreEqual(Color.Black, tweener.Position);
        }

        [Test]
        public void WeCanReverseTweening()
        {
            tweener.Reset(Color.Black, Color.White, TimeSpan.FromSeconds(1));
            tweener.Play();
            tweener.Update(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
            tweener.Reverse();
            tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)));
            Assert.AreEqual(Color.Black, tweener.Position);
        }
	
    }
	
}
