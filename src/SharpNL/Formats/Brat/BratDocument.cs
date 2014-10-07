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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace SharpNL.Formats.Brat {
    /// <summary>
    /// Represents a brat rapid annotation tool document.
    /// </summary>
    public class BratDocument {

        private readonly ReadOnlyDictionary<string, BratAnnotation> annotationMap;

        #region . Constructor .
        public BratDocument(AnnotationConfiguration config, string id, string text, IEnumerable<BratAnnotation> annotations) {

            Config = config;
            Id = id;
            Text = text;

            var list = new Dictionary<string, BratAnnotation>();
            foreach (var annotation in annotations) {
                list.Add(annotation.Id, annotation);
            }
            annotationMap = new ReadOnlyDictionary<string, BratAnnotation>(list);
        }
        #endregion
        
        #region + Properties .

        #region . Annotations .
        /// <summary>
        /// Gets the annotations.
        /// </summary>
        /// <value>The annotations.</value>
        public ReadOnlyDictionary<string, BratAnnotation> Annotations {
            get { return annotationMap; }
        }
        #endregion

        #region . Config .
        /// <summary>
        /// Gets the annotation configuration.
        /// </summary>
        /// <value>The annotation configuration.</value>
        public AnnotationConfiguration Config { get; private set; }
        #endregion

        #region . Id .
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; private set; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets the document text.
        /// </summary>
        /// <value>The document text.</value>
        public string Text { get; private set; }
        #endregion

        #endregion

        #region . GetAnnotation .
        /// <summary>
        /// Gets the annotation with the specified id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The <see cref="BratAnnotation"/> object or null if the key does no exist in this document.</returns>
        public BratAnnotation GetAnnotation(string id) {
            if (annotationMap.ContainsKey(id)) {
                return annotationMap[id];
            }
            return null;
        }
        #endregion

        #region . Parse .
        /// <summary>
        /// Parses the specified brat document using two streams for the document and annotations.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="documentStream">The document stream.</param>
        /// <param name="annotationStream">The annotation stream.</param>
        /// <returns>The parsed <see cref="BratDocument"/> object.</returns>
        public static BratDocument Parse(AnnotationConfiguration config, string id, Stream documentStream, Stream annotationStream) {
            string document;
            using (var reader = new StreamReader(documentStream, Encoding.UTF8)) {
                document = reader.ReadToEnd();
            }
            
            var annotations = new List<BratAnnotation>();
            using (var reader = new BratAnnotationStream(config, id, annotationStream)) {
                BratAnnotation ann;
                while ((ann = reader.Read()) != null) {
                    annotations.Add(ann);
                }
            }

            return new BratDocument(config, id, document, annotations);
        }
        #endregion

    }
}