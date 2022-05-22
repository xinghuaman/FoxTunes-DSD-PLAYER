﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FoxTunes
{
    [TestFixture]
    public class RendererTests : RendererBase
    {
        public RendererTests() : base(false)
        {

        }

        public static object[][] AnimateTestSource = new[]
        {
            new object[] { 0, 100, 0, 100, 20, new int[] { 5, 9, 13, 17, 21, 25, 29, 33, 37, 41, 45, 49, 53, 57, 61, 64, 67, 70, 73, 76, 78, 80, 82, 84, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 } },
            new object[] { 100, 0, 0, 100, 20, new int[] { 95, 91, 87, 83, 79, 75, 71, 67, 63, 59, 55, 51, 47, 43, 39, 36, 33, 30, 27, 24, 22, 20, 18, 16, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 } }
        };

        [TestCaseSource("AnimateTestSource")]
        public void AnimateTest(int value, int target, int min, int max, int smoothing, int[] expected)
        {
            var actual = new List<int>();
            do
            {
                Animate(ref value, target, min, max, smoothing);
                actual.Add(value);
            } while (value != target);
            Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
        }

        protected override void CreateViewBox()
        {
            throw new NotImplementedException();
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}