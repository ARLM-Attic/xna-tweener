using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace XNATweener.Test
{
    [TestFixture]
    public class TweenerTest
    {
        protected Tweener tweener;
        protected float from = 0;
        protected float to = 100;
        protected float duration = 10;
        protected GameTime timeTick = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        private void RunToEnd()
        {
            bool ended = false;
            tweener.Ended += delegate { ended = true; };
            while (!ended)
            {
                tweener.Update(timeTick);
            }
        }

        [SetUp]
        public void SetUp()
        {
            tweener = new Tweener(from, to, duration, Linear.EaseNone);
        }

        [Test]
        public void TestConstruction()
        {
            Assert.IsNotNull(tweener, "Test object is constructed correctly");
        }

        [Test]
        public void UpdatingWillMovePosition()
        {
            tweener.Update(timeTick);
            Assert.Greater(tweener.Position, from, "The position has moved");
        }

        [Test]
        public void TheTweenerIsPlaying()
        {
            Assert.IsTrue(tweener.Playing, "The tweener is playing");
        }

        [Test]
        public void PauseWillTogglePlaying()
        {
            tweener.Pause();
            Assert.IsFalse(tweener.Playing, "The tweener is not playing");
        }

        [Test]
        public void PlayWillTogglePlaying()
        {
            tweener.Pause();
            tweener.Play();
            Assert.IsTrue(tweener.Playing, "The tweener is not playing");
        }	

        [Test]
        public void PausingTheTweenerWillNotUpdate()
        {
            tweener.Pause();
            tweener.Update(timeTick);
            Assert.AreEqual(from, tweener.Position, "The position has not moved");
        }

        [Test]
        public void ResettingTheTweenerWillMovePositionBack()
        {
            tweener.Update(timeTick);
            tweener.Reset();
            Assert.AreEqual(from, tweener.Position, "The tweener has moved back to the original position.");
        }

        [Test]
        public void ResettingWithANewTargetWillNotMoveTweener()
        {
            tweener.Update(timeTick);
            float oldPosition = tweener.Position;
            tweener.Reset(200);
            Assert.AreEqual(oldPosition, tweener.Position, "The tweener has not moved.");
        }

        [Test]
        public void ResettingWithANewTargetAndUpdatingWillMoveCorrectly()
        {
            tweener.Update(timeTick);
            float oldPosition = tweener.Position;
            float newTarget = 200;
            tweener.Reset(newTarget);
            RunToEnd();
            Assert.AreEqual(newTarget, tweener.Position, "The tweener has moved to the new target.");
        }

        [Test]
        public void ResettingWithNewFromAndToWillMoveCorrectly()
        {
            float newFrom = -10;
            float newTo = -50;
            tweener.Reset(newFrom, newTo, duration);
            Assert.AreEqual(newFrom, tweener.Position, "The position has been set to the new from");
            RunToEnd();
            Assert.AreEqual(newTo, tweener.Position, "The tweener has moved to the new to position");
        }

        [Test]
        public void ReverseWillSendTheTweenerBack()
        {
            RunToEnd();
            tweener.Reverse();
            RunToEnd();
            Assert.AreEqual(from, tweener.Position, "The tweener has moved back to the beginning.");
        }
	
	
    }
	
}
