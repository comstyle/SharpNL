// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

using System;
using NUnit.Framework;
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Tests.Utility.Evaluation {
    public class FMeasureTest {
        private const double DELTA = 1.0E-9d;

        private readonly object[] gold = {
            new Span(8, 9),
            new Span(9, 10),
            new Span(10, 12),
            new Span(13, 14),
            new Span(14, 15),
            new Span(15, 16)
        };

        private readonly object[] goldToMerge = {
            new Span(8, 9),
            new Span(9, 10),
            new Span(11, 11),
            new Span(13, 14),
            new Span(14, 15),
            new Span(15, 16),
            new Span(18, 19)
        };

        private readonly object[] predicted = {
            new Span(14, 15),
            new Span(15, 16),
            new Span(100, 120),
            new Span(210, 220),
            new Span(220, 230)
        };

        private readonly object[] predictedCompletelyDistinct = {
            new Span(100, 120),
            new Span(210, 220),
            new Span(211, 220),
            new Span(212, 220),
            new Span(220, 230)
        };

        private readonly object[] predictedToMerge = {
            new Span(8, 9),
            new Span(14, 15),
            new Span(15, 16),
            new Span(100, 120),
            new Span(210, 220),
            new Span(220, 230)
        };

        [Test]
        public void TestCountTruePositives() {
            Assert.AreEqual(0, FMeasure.CountTruePositives(new object[] {}, new object[] {}));
            Assert.AreEqual(gold.Length, FMeasure.CountTruePositives(gold, gold));
            Assert.AreEqual(0, FMeasure.CountTruePositives(gold, predictedCompletelyDistinct));
            Assert.AreEqual(2, FMeasure.CountTruePositives(gold, predicted));
        }

        [Test]
        public void TestPrecision() {
            Assert.AreEqual(1.0d, FMeasure.Precision(gold, gold), DELTA);
            Assert.AreEqual(0, FMeasure.Precision(gold, predictedCompletelyDistinct), DELTA);
            Assert.AreEqual(Double.NaN, FMeasure.Precision(gold, new object[] {}), DELTA);
            Assert.AreEqual(0, FMeasure.Precision(new object[] {}, gold), DELTA);
            Assert.AreEqual(2d/predicted.Length, FMeasure.Precision(gold, predicted), DELTA);
        }

        [Test]
        public void TestRecall() {
            Assert.AreEqual(1.0d, FMeasure.Recall(gold, gold), DELTA);
            Assert.AreEqual(0, FMeasure.Recall(gold, predictedCompletelyDistinct), DELTA);
            Assert.AreEqual(0, FMeasure.Recall(gold, new object[] {}), DELTA);
            Assert.AreEqual(Double.NaN, FMeasure.Recall(new object[] {}, gold), DELTA);
            Assert.AreEqual(2d/gold.Length, FMeasure.Recall(gold, predicted), DELTA);
        }

        [Test]
        public void TestEmpty() {
            var fm = new FMeasure();
            Assert.AreEqual(-1, fm.Value, DELTA);
            Assert.AreEqual(0, fm.RecallScore, DELTA);
            Assert.AreEqual(0, fm.PrecisionScore, DELTA);
        }

        [Test]
        public void TestPerfect() {
            var fm = new FMeasure();
            fm.UpdateScores(gold, gold);
            Assert.AreEqual(1, fm.Value, DELTA);
            Assert.AreEqual(1, fm.RecallScore, DELTA);
            Assert.AreEqual(1, fm.PrecisionScore, DELTA);
        }

        [Test]
        public void TestMerge() {
            var fm = new FMeasure();
            fm.UpdateScores(gold, predicted);
            fm.UpdateScores(goldToMerge, predictedToMerge);

            var fmMerge = new FMeasure();
            fmMerge.UpdateScores(gold, predicted);
            var toMerge = new FMeasure();
            toMerge.UpdateScores(goldToMerge, predictedToMerge);
            fmMerge.MergeInto(toMerge);

            double selected1 = predicted.Length;
            double target1 = gold.Length;
            double tp1 = FMeasure.CountTruePositives(gold, predicted);

            double selected2 = predictedToMerge.Length;
            double target2 = goldToMerge.Length;
            double tp2 = FMeasure.CountTruePositives(goldToMerge, predictedToMerge);


            Assert.AreEqual((tp1 + tp2)/(target1 + target2), fm.RecallScore, DELTA);
            Assert.AreEqual((tp1 + tp2)/(selected1 + selected2), fm.PrecisionScore, DELTA);

            Assert.AreEqual(fm.RecallScore, fmMerge.RecallScore, DELTA);
            Assert.AreEqual(fm.PrecisionScore, fmMerge.PrecisionScore, DELTA);
        }
    }
}