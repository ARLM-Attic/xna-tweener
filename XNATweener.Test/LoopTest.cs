using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace XNATweener.Test
{
    [TestFixture]
    public class LoopTest
    {
        [Test]
        public void LoopingFrontToBackWillWork()
        {
            float from = 0;
            float to = 100;
            Tweener tweener = new Tweener(from, to, 4, Linear.EaseNone);
            Loop.FrontToBack(tweener);
            for (int i = 0; i < 5; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.Greater(tweener.Position, from, "We have moved from the start, again");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.AreEqual(from, tweener.Position, "We have gone back to the start");
            }
        }

        [Test]
        public void LoopingFrontToANumberOfTimesBackWillWork()
        {
            float from = 0;
            float to = 100;
            int times = 3;
            Tweener tweener = new Tweener(from, to, 4, Linear.EaseNone);
            Loop.FrontToBack(tweener, times);
            for (int i = 0; i < times; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.Greater(tweener.Position, from, "We have moved from the start, again");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
            }
            bool positionChanged = false;
            tweener.PositionChanged += delegate { positionChanged = true; };
            tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
            Assert.IsFalse(positionChanged, "The tweener is no longer active as it has looped as many times as it should");
        }

        [Test]
        public void LoopingBackAndForthWillWork()
        {
            float from = 0;
            float to = 100;
            Tweener tweener = new Tweener(from, to, 4, Linear.EaseNone);
            Loop.BackAndForth(tweener);
            for (int i = 0; i < 5; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.Greater(tweener.Position, from, "We have moved from the start, again");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.AreEqual(to, tweener.Position, "We are at the end");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.Less(tweener.Position, to, "We now on the way back");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.AreEqual(from, tweener.Position, "We now at the start");
            }
        }

        [Test]
        public void LoopoingBackAndForthANumberOfTimesWillWork()
        {
            float from = 0;
            float to = 100;
            int times = 4;
            Tweener tweener = new Tweener(from, to, 4, Linear.EaseNone);
            Loop.BackAndForth(tweener, times);
            for (int i = 0; i < times; i++)
            {
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
                Assert.Greater(tweener.Position, from, "We have moved from the start, again");
                tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
            }
            bool positionChanged = false;
            tweener.PositionChanged += delegate { positionChanged = true; };
            tweener.Update(new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
            Assert.IsFalse(positionChanged, "The tweener is no longer active as it has looped as many times as it should");
        }	
    }	
}
