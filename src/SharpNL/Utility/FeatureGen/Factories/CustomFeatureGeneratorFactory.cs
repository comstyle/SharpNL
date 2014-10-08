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
using System.Collections.Generic;
using System.Xml;
using SharpNL.Java;

namespace SharpNL.Utility.FeatureGen.Factories {
    // TODO: We have to support custom resources here. How does it work ?!
    // Attributes get into a Map<String, String> properties

    // How can serialization be supported ?!
    // The model is loaded, and the manifest should contain all serializer classes registered for the
    // resources by name.
    // When training, the descriptor could be consulted first to register the serializers, and afterwards
    // they are stored in the model.
    [JavaClass("opennlp.tools.util.featuregen.GeneratorFactory.CustomFeatureGeneratorFactory")]
    public class CustomFeatureGeneratorFactory : XmlFeatureGeneratorFactory {
        
        public CustomFeatureGeneratorFactory() : base("custom") { }

        /// <summary>
        /// Normalizes the class name from java.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>The normalized </returns>
        private static string NormalizeClassName(string className) {

            //opennlp.tools.util.featuregen.TokenFeatureGenerator

            var token = className.Split('.');

            if (token.Length > 1) {

                if (token[0] == "SharpNL")
                    return className;

                var last = token[token.Length - 2];
                switch (last) {
                    case "featuregen":
                        return "SharpNL.Utility.FeatureGen." + token[token.Length - 1];
                    case "GeneratorFactory":
                        return "SharpNL.Utility.FeatureGen.Factories." + token[token.Length - 1];
                }
            }

            // worst case we return the original value and try to get the class

            return className;

            //throw new InvalidFormatException("Unable to normalize the class name!");
        }


        /// <summary>
        /// Creates an <see cref="IAdaptiveFeatureGenerator"/> from a the describing XML element.
        /// </summary>
        /// <param name="generatorElement">The element which contains the configuration.</param>
        /// <param name="provider">The resource provider which could be used to access referenced resources.</param>
        /// <returns>The configured <see cref="IAdaptiveFeatureGenerator"/> </returns>
        public override IAdaptiveFeatureGenerator Create(XmlElement generatorElement, FeatureGeneratorResourceProvider provider) {

            //opennlp.tools.util.featuregen.TokenFeatureGenerator
            var className = generatorElement.GetAttribute("class");
            var normalize = NormalizeClassName(className);

            var type = Type.GetType(normalize, false);
            if (type != null) {
                var generator = (IAdaptiveFeatureGenerator) Activator.CreateInstance(type);

                var customGenerator = generator as CustomFeatureGenerator;
                if (customGenerator != null) {

                    var properties = new Dictionary<string, string>();

                    foreach (XmlAttribute attribute in generatorElement.Attributes) {
                        if (attribute.Name != "class") {
                            properties.Add(attribute.Name, attribute.Value);
                        }                       
                    }

                    // TODO: why this condition ?!
                    if (provider != null) {
                        customGenerator.Init(properties, provider);
                    }
                }

                return generator;
            }

            throw new InvalidOperationException("Unable to create the class object.");
        }
    }
}