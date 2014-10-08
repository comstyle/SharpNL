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

using System.Xml;
using SharpNL.Java;

namespace SharpNL.Utility.FeatureGen.Factories {
    [JavaClass("opennlp.tools.util.featuregen.GeneratorFactory.SentenceFeatureGeneratorFactory")]
    internal class SentenceFeatureGeneratorFactory : XmlFeatureGeneratorFactory {
        public SentenceFeatureGeneratorFactory() : base("sentence") {}

        /// <summary>
        /// Creates an <see cref="IAdaptiveFeatureGenerator"/> from a the describing XML element.
        /// </summary>
        /// <param name="generatorElement">The element which contains the configuration.</param>
        /// <param name="provider">The resource provider which could be used to access referenced resources.</param>
        /// <returns>The configured <see cref="IAdaptiveFeatureGenerator"/> </returns>
        public override IAdaptiveFeatureGenerator Create(XmlElement generatorElement,
            FeatureGeneratorResourceProvider provider) {
            var beginFeatureString = generatorElement.GetAttribute("begin");

            var beginFeature = true;
            if (beginFeatureString.Length != 0)
                beginFeature = bool.Parse(beginFeatureString);

            var endFeatureString = generatorElement.GetAttribute("end");
            var endFeature = true;
            if (endFeatureString.Length != 0)
                endFeature = bool.Parse(endFeatureString);

            return new SentenceFeatureGenerator(beginFeature, endFeature);
        }
    }
}