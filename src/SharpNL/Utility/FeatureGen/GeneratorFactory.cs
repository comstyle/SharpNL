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
using System.IO;
using System.Xml;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Creates a set of feature generators based on a provided XML descriptor.
    /// </summary>
    /// <example>
    /// Example of an XML descriptor:
    /// <p>
    ///  &lt;generators&gt;
    ///    &lt;charngram min = "2" max = "5"/&gt;
    ///    &lt;definition/&gt;
    ///    &lt;cache&gt;
    ///      &lt;window prevLength = "3" nextLength = "3"&gt;
    ///        &lt;generators&gt;
    ///          &lt;prevmap/&gt;
    ///          &lt;sentence/&gt;
    ///          &lt;tokenclass/&gt;
    ///          &lt;tokenpattern/&gt;
    ///          &lt;/generators&gt;
    ///      &lt;/window&gt;
    ///    &lt;/cache&gt;
    ///  &lt;/generators&gt;
    /// </p>
    /// </example>
    /// <remarks>
    /// Each XML element is mapped to a <see cref="XmlFeatureGeneratorFactory"/> which
    /// is responsible to process the element and create the specified
    /// <see cref="IAdaptiveFeatureGenerator"/>. Elements can contain other elements 
    /// in this case it is the responsibility of the mapped factory to process the 
    /// child elements correctly. In some factories this leads to recursive calls 
    /// the <see cref="Create"/> method.
    /// 
    /// In the example above the generators element is mapped to the
    /// <see cref="Factories.AggregatedFeatureGeneratorFactory"/> <see cref="GeneratorFactory"/> which then
    /// creates all the aggregated <see cref="IAdaptiveFeatureGenerator"/> to
    /// accomplish this it evaluates the mapping with the same mechanism
    /// and gives the child element to the corresponding factories. All
    /// created generators are added to a new instance of the
    /// <see cref="AggregatedFeatureGenerator"/>  which is then returned.
    /// </remarks>
    public class GeneratorFactory {
        private static readonly Dictionary<string, XmlFeatureGeneratorFactory> factories;

        static GeneratorFactory() {
            factories = new Dictionary<string, XmlFeatureGeneratorFactory>();

            // auto load the factories ;)
            foreach (var type in Library.GetKnownTypes(typeof(XmlFeatureGeneratorFactory))) {
                var factory = (XmlFeatureGeneratorFactory) Activator.CreateInstance(type);
                factories.Add(factory.Name, factory);
            }
        }

        #region . Create .
        /// <summary>
        /// Creates an <see cref="IAdaptiveFeatureGenerator"/> from an provided XML descriptor.
        /// 
        /// Usually this XML descriptor contains a set of nested feature generators
        /// which are then used to generate the features by one of the opennlp components.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/> from which the descriptor is read, the stream remains open and must be closed by the caller.</param>
        /// <param name="provider">The resource provider which is used to resolve resources referenced by a key in the descriptor.</param>
        /// <returns>Created feature generators.</returns>
        public static IAdaptiveFeatureGenerator Create(Stream inputStream, FeatureGeneratorResourceProvider provider) {

            var doc = new XmlDocument();
            try {
                doc.Load(inputStream);
            } catch (Exception ex) {
                throw new InvalidDataException("Unable to load the XML file.", ex);
            }

            return CreateGenerator(doc.DocumentElement, provider);
        }
        #endregion

        #region . CreateGenerator .

        /// <summary>
        /// Creates a <see cref="IAdaptiveFeatureGenerator"/> for the provided element.
        /// To accomplish this it looks up the corresponding factory by the
        /// element tag name. The factory is then responsible for the creation
        /// of the generator from the element.
        /// </summary>
        /// <param name="generatorElement">The generator element.</param>
        /// <param name="provider">The resource provider which is used to resolve resources referenced by a key in the descriptor.</param>
        /// <returns>IAdaptiveFeatureGenerator.</returns>
        /// <exception cref="InvalidFormatException">Unexpected element:  + generatorElement.Name</exception>
        internal static IAdaptiveFeatureGenerator CreateGenerator(XmlElement generatorElement, FeatureGeneratorResourceProvider provider) {
            if (factories.ContainsKey(generatorElement.Name)) {
                var factory = factories[generatorElement.Name];

                return factory.Create(generatorElement, provider);
            }

            throw new InvalidFormatException("Unexpected element: " + generatorElement.Name);
        }

        #endregion

    }
}