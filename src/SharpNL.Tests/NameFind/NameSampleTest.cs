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
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {
    /// <summary>
    /// This is the test class for <see cref="NameSample"/>.
    /// </summary>
    [TestFixture]
    internal class NameSampleTest {

        #region . CreateSimpleNameSample .

        /// <summary>
        /// Create a NameSample from scratch and validate it.
        /// </summary>
        /// <param name="useTypes">if set to <c>true</c> use nametypes.</param>
        /// <returns>NameSample.</returns>
        private static NameSample CreateSimpleNameSample(bool useTypes) {
            var sentence = new[] {
                "U", ".", "S", ".", "President", "Barack", "Obama", "is",
                "considering", "sending", "additional", "American", "forces",
                "to", "Afghanistan", "."
            };

            Span[] names = {
                new Span(0, 4, "Location"),
                new Span(5, 7, "Person"),
                new Span(14, 15, "Location")
            };

            NameSample nameSample;
            if (useTypes) {
                nameSample = new NameSample(sentence, names, false);
            } else {
                var namesWithoutType = new Span[names.Length];
                for (var i = 0; i < names.Length; i++) {
                    namesWithoutType[i] = new Span(names[i].Start,
                        names[i].End);
                }

                nameSample = new NameSample(sentence, namesWithoutType, false);
            }

            return nameSample;
        }

        #endregion

        #region . CreateGoldSample .

        internal static NameSample CreateGoldSample() {
            return CreateSimpleNameSample(true);
        }

        #endregion

        #region . CreatePredSample .

        internal static NameSample CreatePredSample() {
            return CreateSimpleNameSample(false);
        }

        #endregion

        #region . TestNoTypesToString .

        /// <summary>
        /// Checks if could create a NameSample without NameTypes, generate the 
        /// string representation and validate it.
        /// </summary>
        [Test]
        public void TestNoTypesToString() {
            var nameSampleStr = CreateSimpleNameSample(false).ToString();
            Assert.AreEqual("<START> U . S . <END> President <START> Barack Obama <END> is considering " +
                            "sending additional American forces to <START> Afghanistan <END> .", nameSampleStr);
        }

        #endregion

        #region . TestWithTypesToString .

        /// <summary>
        /// Checks if could create a NameSample with NameTypes, generate the string representation and validate it.
        /// </summary>
        [Test]
        public void TestWithTypesToString() {
            var nameSampleStr = CreateSimpleNameSample(false).ToString();
            Assert.AreEqual("<START> U . S . <END> President <START> Barack Obama <END> is considering " +
                            "sending additional American forces to <START> Afghanistan <END> .", nameSampleStr);

            var parsedSample = NameSample.Parse("<START:Location> U . S . <END> " +
                                                "President <START:Person> Barack Obama <END> is considering sending " +
                                                "additional American forces to <START:Location> Afghanistan <END> .",
                false);

            Assert.AreEqual(CreateSimpleNameSample(true), parsedSample);
        }

        #endregion

        #region . TestNameAtEnd .

        /// <summary>
        /// Checks that if the name is the last token in a sentence it is still outputed correctly.
        /// </summary>
        [Test]
        public void TestNameAtEnd() {
            var sentence = new[] {
                "My",
                "name",
                "is",
                "Anna"
            };

            var sample = new NameSample(sentence, new[] {new Span(3, 4)}, false);

            Assert.AreEqual("My name is <START> Anna <END>", sample.ToString());
        }

        #endregion

        #region . TestParseWithAdditionalSpace .

        /// <summary>
        /// Tests if an additional space is correctly treated as one space.
        /// </summary>
        [Test]
        public void TestParseWithAdditionalSpace() {
            const string line = "<START> M . K . <END> <START> Schwitters <END> ?  <START> Heartfield <END> ?";

            var test = NameSample.Parse(line, false);
            Assert.AreEqual(8, test.Sentence.Length);
        }

        #endregion

        #region . TestTypeWithSpecialChars .

        /// <summary>
        /// Checks if it accepts name type with some special characters
        /// </summary>
        [Test]
        public void TestTypeWithSpecialChars() {
            var parsedSample = NameSample.Parse(
                "<START:type-1> U . S . <END> "
                + "President <START:type_2> Barack Obama <END> is considering sending "
                + "additional American forces to <START:type_3-/;.,&%$> Afghanistan <END> .", false);

            Assert.AreEqual(3, parsedSample.Names.Length);
            Assert.AreEqual("type-1", parsedSample.Names[0].Type);
            Assert.AreEqual("type_2", parsedSample.Names[1].Type);
            Assert.AreEqual("type_3-/;.,&%$", parsedSample.Names[2].Type);
        }

        #endregion

        #region . TestMissingType .

        /// <summary>
        /// Test if it fails to parse empty type
        /// </summary>
        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void TestMissingType() {
            NameSample.Parse("<START:> token <END>", false);
        }

        #endregion

        #region . TestTypeWithSpace .

        /// <summary>
        /// Test if it fails to parse type with space
        /// </summary>
        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void TestTypeWithSpace() {
            NameSample.Parse("<START:abc a> token <END>", false);
        }

        #endregion

        #region . TestTypeWithNewLine .

        /// <summary>
        /// Test if it fails to parse type with new line
        /// </summary>
        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void TestTypeWithNewLine() {
            NameSample.Parse("<START:abc\na> token <END>", false);
        }

        #endregion

        #region . TestTypeWithInvalidChar1 .

        /// <summary>
        /// Test if it fails to parse type with new line
        /// </summary>
        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void TestTypeWithInvalidChar1() {
            NameSample.Parse("<START:abc:a> token <END>", false);
        }

        #endregion

        #region . TestTypeWithInvalidChar2 .

        /// <summary>
        /// Test if it fails to parse type with new line
        /// </summary>
        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void TestTypeWithInvalidChar2() {
            NameSample.Parse("<START:abc>a> token <END>", false);
        }

        #endregion

        #region . TestEquals .

        [Test]
        public void TestEquals() {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            // ReSharper disable once EqualExpressionComparison
            Assert.False(CreateGoldSample() == CreateGoldSample());

            // ReSharper disable once EqualExpressionComparison
            Assert.True(CreateGoldSample().Equals(CreateGoldSample()));
            Assert.False(CreateGoldSample().Equals(CreatePredSample()));
            Assert.False(CreatePredSample().Equals(new Object()));
        }

        #endregion

    }
}
