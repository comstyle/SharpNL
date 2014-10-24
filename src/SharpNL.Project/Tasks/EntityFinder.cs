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
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using SharpNL.NameFind;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Tasks {

    /// <summary>
    /// The <see cref="EntityFinderTask"/> class searches for entities in the document sentences 
    /// using a <see cref="ITokenNameFinder"/>.
    /// 
    /// Each result is processed by the specified <see cref="IEntityResolver"/> resolver.
    /// </summary>
    public class EntityFinderTask : ProjectTask {
        //private readonly IEntityResolver entityResolver;

        public EntityFinderTask() : base(4f) { }

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="EntityFinderTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="EntityFinderTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get { return new[] { typeof(Document) }; }
        }
        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="EntityFinderTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="EntityFinderTask"/>.</value>
        [Browsable(false)]
        public override Type[] Output {
            get { return new[] { typeof(Document) }; }
        }
        #endregion

        #region . Model .

        private string model;

        /// <summary>
        /// Gets or sets the model name used in this task.
        /// </summary>
        /// <value>The model name used in this task.</value>
        [Description("The tokenizer model."), TypeConverter(typeof (NodeModelConverter<TokenNameFinderModel>))]
        public string Model {
            get { return model; }
            set {
                model = value;
                Project.IsDirty = true;
            }
        }
        #endregion

        #region . Resolver .
        /// <summary>
        /// Gets or sets the entity resolver.
        /// </summary>
        /// <value>The entity resolver.</value>
        [DefaultValue(null)]
        public IEntityResolver Resolver { get; set; }
        #endregion

        #region . Execute .

        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {

            Document doc = null;

            if (Parent != null)
                doc = Parent.GetOutput<Document>();

            if (doc == null)
                throw new NotSupportedException("Unable to retrieve the document from the parent node.");

            var sentences = doc.Sentences;

            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            var nameFinderModel = Project.Manager.GetModel<TokenNameFinderModel>(Model);
            var nameFinder = new NameFinderME(nameFinderModel);

            foreach (var sentence in sentences) {

                Span[] spans;
                string[] tokens = TextUtils.TokensToString(sentence.Tokens);
                lock (nameFinder) {
                    spans = nameFinder.Find(tokens);
                }
                

                if (spans.Length > 0) {
                    Debug.Print("");
                }

                var entities = new List<IEntity>(spans.Length);
                if (Resolver != null) {
                    for (var i = 0; i < spans.Length; i++) {
                        var e = Resolver.Resolve(doc.Language, sentence, spans[i]);
                        if (e != null)
                            entities.Add(e);
                    }
                    
                } else {
                    for (var i = 0; i < spans.Length; i++) {
                        entities.Add(new Entity(spans[i], tokens));
                    }
                }

                sentence.Entities = entities.AsReadOnly();
            }

            return new object[] {doc};
        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            if (string.IsNullOrEmpty(Model))
                return new[] { new ProjectProblem(this, "The namefinder model is not specified.") };

            return null;
        }
        #endregion

        #region . DeserializeTask .
        /// <summary>
        /// Deserializes the task from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The xml node.</param>
        protected override void DeserializeTask(XmlNode node) {
            if (node.Attributes != null) {
                var attModel = node.Attributes["Model"];
                if (attModel != null)
                    Model = attModel.Value;

                var attRes = node.Attributes["Resolver"];
                if (attRes != null) {
                    var type = Type.GetType(attRes.Value);
                    if (type != null) {
                        Resolver = (IEntityResolver)Activator.CreateInstance(type);
                    }
                }
            }
        }
        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {

            writer.WriteAttributeString("Model", Model);

            if (Resolver != null)
                writer.WriteAttributeString("Resolver", Resolver.GetType().FullName);

        }
        #endregion


    }
}