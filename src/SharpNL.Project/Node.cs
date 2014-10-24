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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;

namespace SharpNL.Project {
    /// <summary>
    /// Represents a project node.
    /// </summary>
    public abstract class Node {

        #region + Properties .

        #region . Exception .

        /// <summary>
        /// Gets the last exception occur in this <see cref="Node"/>.
        /// </summary>
        /// <value>The exception.</value>
        public ProjectException Exception { get; protected set; }

        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="Node"/>.
        /// </summary>
        /// <value>The output types of this <see cref="Node"/>.</value>
        [Browsable(false)]
        public abstract Type[] Output { get; }
        #endregion

        #region . Parent .
        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public ProjectNode Parent { get; internal set; }
        #endregion

        #region . Project .
        /// <summary>
        /// Gets the project associated to this node.
        /// </summary>
        /// <value>The project associated to this node.</value>
        public Project Project { get; protected set; }
        #endregion

        #region . Status .
        /// <summary>
        /// Gets the node status.
        /// </summary>
        /// <value>The node status.</value>
        public NodeStatus Status { get; protected set; }
        #endregion

        #region . Tag .
        /// <summary>
        /// Gets or sets the object that contains data about the node.
        /// </summary>
        /// <value>An <see cref="object"/> that contains data about the node. The default is <c>null</c>.</value>
        public object Tag { get; set; }
        #endregion

        #endregion

        #region . Serialize .
        internal void Serialize(string name, XmlWriter writer, bool content) {
            var type = GetType();
            var intern = type.Namespace != null && type.Namespace.StartsWith("SharpNL.Project");

            writer.WriteStartElement(name);
            writer.WriteAttributeString("Type", intern ? type.Name : type.FullName);

            SerializeNode(writer, content);

            writer.WriteEndElement();
        }
        #endregion

        #region . SerializeNode .
        /// <summary>
        /// Serializes the content of the node.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> object to used to serialize the node.</param>
        /// <param name="content">Determines whether the content should be saved.</param>
        protected virtual void SerializeNode(XmlWriter writer, bool content) {

        }
        #endregion

        #region . SetProject .

        internal void SetProject(Project project) {
            Project = project;
        }

        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public virtual ProjectProblem[] GetProblems() {
            return null;
        }
        #endregion

        #region . LogException .
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        protected void LogException(Exception ex) {
            if (Project.Monitor != null)
                Project.Monitor.OnException(ex);
        }
        #endregion

        #region . LogMessage .
        /// <summary>
        /// Log a info message.
        /// </summary>
        /// <param name="message">The info message.</param>
        protected void LogMessage(string message) {
            if (Project.Monitor != null)
                Project.Monitor.OnMessage(message);
        }
        #endregion

        #region . LogWarning .
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message">The warning message.</param>
        protected void LogWarning(string message) {
            if (Project.Monitor != null)
                Project.Monitor.OnWarning(message);
        }
        #endregion

    }
}