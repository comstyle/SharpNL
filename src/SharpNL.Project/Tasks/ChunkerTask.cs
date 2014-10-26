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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using SharpNL.Chunker;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Tasks {
    /// <summary>
    /// Represents the chunker analyzer.
    /// </summary>
    public class ChunkerTask : ProjectTask {

        public ChunkerTask() : base(6f) {
            
        }

        #region + Project .

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="ChunkerTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ChunkerTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get { return new[] { typeof(Document) }; }
        }
        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="ChunkerTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="ChunkerTask"/>.</value>
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
        [TypeConverter(typeof (NodeModelConverter<ChunkerModel>))]
        public string Model {
            get {
                return model;               
            }
            set {
                model = value;
                Project.IsDirty = true;
            }
            
        }
        #endregion

        #endregion

        #region . Execute .

        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {
            IDocument doc = null;

            if (Parent != null)
                doc = Parent.GetOutput<IDocument>();
   
            if (doc == null)
                throw new NotSupportedException("Unable to retrieve the document from the parent node.");


            var chunkerModel = Project.Manager.GetModel<ChunkerModel>(Model);
            var chunker = new ChunkerME(chunkerModel);

            int count = 0;
            foreach (var sentence in doc.Sentences) {
                var tokens = sentence.Tokens;
                var tokStr = TextUtils.TokensToString(tokens);
                var tags = new string[tokens.Count];
                for (var i = 0; i < tokens.Count; i++) {
                    tags[i] = tokens[i].POSTag;
                }

                string[] chunkTags;
                lock (chunker) {
                    chunkTags = chunker.Chunk(tokStr, tags);
                }

                for (var i = 0; i < chunkTags.Length; i++) {
                    tokens[i].ChunkTag = chunkTags[i];
                }

                var chunkSpans = ChunkSample.PhrasesAsSpanList(tokStr, tags, chunkTags);
                var chunks = new List<IChunk>(chunkSpans.Length);

                foreach (var span in chunkSpans) {
                    var chunk = Project.Factory.CreateChunk(sentence, span);
                    if (chunk == null)
                        continue;
                    
                    chunks.Add(chunk);
                }

                count += chunks.Count;

                sentence.Chunks = new ReadOnlyCollection<IChunk>(chunks);
            }

            LogMessage(string.Format(" {0} sentences analyzed - {1} chunks found.", doc.Text.Length, count));

            return new object[] {doc};

        }


        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (Project.Factory == null)
                list.Add(new ProjectProblem("The factory is not specified in the project."));

            if (string.IsNullOrEmpty(Model))
               list.Add(new ProjectProblem(this, "The chunker model is not specified."));

            return list.Count > 0 ? list.ToArray() : null;
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
            }
        }
        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("Model", Model);
        }
        #endregion


    }
}