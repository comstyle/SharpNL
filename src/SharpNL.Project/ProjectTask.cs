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
using System.Xml;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Project {
    /// <summary>
    /// Provide the basic implementation for a node inside a <see cref="Project"/>.
    /// </summary>
    public abstract class ProjectTask : Node, IComparable<ProjectTask> {

        private static readonly Dictionary<string, Type> taskTypes;

        static ProjectTask() {
            taskTypes = new Dictionary<string, Type>();

            var list = typeof(ProjectTask).GetSubclasses(true);
            foreach (var type in list) 
                AddTaskType(type);
           
        }

        private static void AddTaskType(Type type) {
            if (type.IsAbstract) {
                var list = type.GetSubclasses(true);
                foreach (var child in list)
                    AddTaskType(child);
            } else {
                taskTypes.Add(type.Namespace == "SharpNL.Project.Tasks" ? type.Name : type.FullName, type);
            }
        }

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTask"/>.
        /// </summary>
        protected ProjectTask(float weight) {
            Status = NodeStatus.Chilling;
            Weight = weight;
        }

        #endregion

        #region + Properties .

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="ProjectTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ProjectTask"/>.</value>
        [Browsable(false)]
        public abstract Type[] Input { get; }
        #endregion

        #region . Weight .
        /// <summary>
        /// Property used to control the influence of a task during the execution. The lower values will be executed first.
        /// </summary>
        /// <value>Returns a floating point value indicating the relative weight a task.</value>
        [Browsable(false)]
        public float Weight { get; private set; }
        #endregion

        #endregion

        #region . DeserializeTask .
        /// <summary>
        /// Deserializes the task from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The xml node.</param>
        protected virtual void DeserializeTask(XmlNode node) {
            // nothing here
        }
        #endregion

        #region . DeserializeTask .
        internal static ProjectTask Deserialize(XmlNode taskNode, ProjectNode parent) {
            if (taskNode.Attributes == null)
                throw new InvalidFormatException("The project task has no attributes.");

            var name = taskNode.Attributes["Type"];
            if (name == null)
                throw new InvalidFormatException("The task does not specifies its type.");

            if (!taskTypes.ContainsKey(name.Value))
                throw new InvalidOperationException("The task type " + name.Value + " is not recognized as a project task.");

            var task = (ProjectTask)Activator.CreateInstance(taskTypes[name.Value]);

            task.Parent = parent;
            task.Project = parent.Project;

            task.DeserializeTask(taskNode);
 
            return task;
        }
        #endregion

        protected abstract void SerializeTask(XmlWriter writer);

        protected sealed override void SerializeNode(XmlWriter writer, bool content) {
            SerializeTask(writer);
        }

        #region . Execute .

        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected abstract object[] Execute();

        #endregion

        #region . Run .
        /// <summary>
        /// Execute this task.
        /// </summary>
        internal void Run() {
            try {
                Project.OnTaskStarted(this);
                var outputs = Execute();
                if (outputs == null || outputs.Length <= 0) 
                    return;

                foreach (var output in outputs)
                    Parent.UpdateOutput(output);

            } catch (Exception ex) {
                throw new TaskException(this, "An error occurred during the task execution.", ex);
            } finally {
                Project.OnTaskFinished(this);
            }           
        }
        #endregion

        #region . CompareTo .
        /// <summary>
        /// Compares the current <see cref="ProjectTask"/> with another <see cref="ProjectTask"/> of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: Value Meaning Less than zero This <see cref="ProjectTask"/> is less than the <paramref name="other"/> parameter.Zero This <see cref="ProjectTask"/> is equal to <paramref name="other"/>. Greater than zero This <see cref="ProjectTask"/> is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An <see cref="ProjectTask"/> to compare with this <see cref="ProjectTask"/>.</param>
        public int CompareTo(ProjectTask other) {
            return other == null ? 1 : Weight.CompareTo(other.Weight);
        }
        #endregion

    }
}