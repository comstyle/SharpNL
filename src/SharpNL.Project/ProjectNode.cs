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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using SharpNL.Utility;

namespace SharpNL.Project {
    public abstract class ProjectNode : Node {
        /// <summary>
        /// The synchronize lock for execution
        /// </summary>
        private readonly object syncLock = new object();
        private readonly List<ProjectNode> child;
        private readonly List<ProjectTask> tasks;
        private readonly List<object> outputs = new List<object>();

        private static readonly Dictionary<string, Type> nodeTypes;

        static ProjectNode() {
            nodeTypes = new Dictionary<string, Type>();

            var list = typeof (ProjectNode).GetSubclasses(true);
            foreach (var type in list) {
                nodeTypes.Add(type.Namespace == "SharpNL.Project.Nodes" ? type.Name : type.FullName, type);
            }
        }

        protected ProjectNode() {
            child = new List<ProjectNode>();
            tasks = new List<ProjectTask>();
        }

        #region + Properties .

        #region . Children .
        /// <summary>
        /// Gets a list of the children nodes contained in this <see cref="ProjectNode"/>.
        /// </summary>
        /// <value>A list of the children nodes contained in this <see cref="ProjectNode"/></value>
        public ReadOnlyCollection<ProjectNode> Children {
            get { return child.AsReadOnly(); }
        }
        #endregion

        #region . HasChildren .

        /// <summary>
        /// Gets a value indicating whether the node contains one or more child nodes.
        /// </summary>
        /// <value><c>true</c> if the node contains one or more child nodes, <c>false</c>.</value>
        public bool HasChildren {
            get { return Children.Count > 0; }
        }

        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="ProjectNode"/>.
        /// </summary>
        /// <value>The output types of this <see cref="ProjectNode"/>.</value>
        [Browsable(false)]
        public override Type[] Output {
            get {
                var list = new List<Type>();
                foreach (var task in Tasks) {
                    foreach (var type in task.Output) {
                        if (!list.Contains(type))
                            list.Add(type);
                    }
                }
                return list.ToArray();
            }
        }

        #endregion

        #region . Outputs .
        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>The outputs.</value>
        [Description("The outputs of this node."), Browsable(false)]       
        public ReadOnlyCollection<object> Outputs {
            get { return outputs.AsReadOnly(); }
        }
        #endregion

        #region . Tasks .

        /// <summary>
        /// Gets an list of <see cref="ProjectTask"/> objects, each of which represent a task that is defined in this <see cref="ProjectNode"/>.
        /// </summary>
        /// <value>An list of <see cref="ProjectTask"/> objects in this node.</value>
        public ReadOnlyCollection<ProjectTask> Tasks {
            get { return tasks.AsReadOnly(); }
        }

        #endregion

        #endregion

        #region + Add .
        /// <summary>
        /// Adds the specified child node.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        /// <exception cref="System.ArgumentNullException">childNode</exception>
        public void Add(ProjectNode childNode) {
            if (childNode == null)
                throw new ArgumentNullException("childNode");

            childNode.Parent = this;
            childNode.Project = Project;

            Project.Changed();

            child.Add(childNode);
        }

        /// <summary>
        /// Adds the specified task.
        /// </summary>
        /// <param name="projectTask">The task.</param>
        /// <exception cref="System.ArgumentNullException">task</exception>
        public void Add(ProjectTask projectTask) {
            if (projectTask == null)
                throw new ArgumentNullException("projectTask");

            projectTask.Parent = this;
            projectTask.SetProject(Project);

            Project.Changed();

            tasks.Add(projectTask);
            tasks.Sort();
        }

        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the node from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The node.</param>
        protected virtual void Deserialize(XmlNode node) {
            // nothing here
        }
        #endregion

        #region . DeserializeNode .
        internal static ProjectNode DeserializeNode(XmlNode xmlNode, ProjectNode parent, Project project) {
            if (xmlNode.Attributes == null)
                throw new InvalidFormatException("The project node has no attributes.");

            var name = xmlNode.Attributes["Type"];
            if (name == null)
                throw new InvalidFormatException("The node does not specifies its type.");
            
            if (!nodeTypes.ContainsKey(name.Value))
                throw new InvalidOperationException("The node type " + name.Value + " is not recognized as a project node.");
            
            var node = (ProjectNode)Activator.CreateInstance(nodeTypes[name.Value]);

            node.Parent = parent;
            node.Project = project;

            node.Deserialize(xmlNode);

            if (!xmlNode.HasChildNodes) 
                return node;

            var tasks = xmlNode.SelectNodes("Tasks/Task");
            if (tasks != null) {
                foreach (XmlNode task in tasks) {
                    node.tasks.Add(ProjectTask.Deserialize(task, node));
                }
                node.tasks.Sort();
            }

            var nodes = xmlNode.SelectNodes("Nodes/Node");
            if (nodes != null) {
                foreach (XmlNode child in nodes) {
                    node.child.Add(DeserializeNode(child, node, project));
                }
            }

            return node;
        }
        #endregion

        #region . Execute .


        /// <summary>
        /// Executes the specified node.
        /// </summary>
        /// <param name="force">if set to <c>true</c> the parent nodes will be executed.</param>
        /// <exception cref="System.InvalidOperationException">
        /// This node is already waiting for a parent node be executed.
        /// or
        /// This node is already being executed.
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Execute(bool force) {
            if (Status == NodeStatus.Waiting)
                throw new InvalidOperationException("This node is already waiting for a parent node be executed.");

            if (Status == NodeStatus.Running)
                throw new InvalidOperationException("This node is already being executed.");

            Exception = null;

            if (Parent != null && (Parent.Status != NodeStatus.Success || force)) {
                try {
                    Parent.Execute(force);
                } catch {
                    Status = NodeStatus.Fail;
                    Exception = new ProjectException("An error occurred in a parent node.");
                    return;
                }
            }

            lock (syncLock) {
                Status = NodeStatus.Running;
                outputs.Clear();

                var outs = Prepare();
                if (outs != null) {
                    outputs.AddRange(outs);
                }

                try {
                    foreach (var task in Tasks) {
                        task.Run();
                    }
                    Status = NodeStatus.Success;
                } catch (Exception ex) {
                    Exception = new ProjectException(this, "An error occurred during the execution of this node.", ex);
                    Status = NodeStatus.Fail;
                    throw Exception;
                }
            }
        }

        #endregion

        #region . GetOutput .
        /// <summary>
        /// Gets the output of this node.
        /// </summary>
        /// <typeparam name="T">The desired output type</typeparam>
        /// <returns>The output type of this node. If the requested type is not supported, throw an <see cref="InvalidOperationException"/>.</returns>
        public T GetOutput<T>() {
            return (T)outputs.FirstOrDefault(o => o.GetType() == typeof(T));
        }
        #endregion

        #region . Prepare .
        /// <summary>
        /// Prepares the task for execution.
        /// </summary>
        /// <returns>A array of pre-loaded outputs.</returns>
        protected abstract object[] Prepare();
        #endregion


        protected abstract void SerializeProjectNode(XmlWriter writer, bool content);

        /// <summary>
        /// Serializes the content of the node.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> object to used to serialize the node.</param>
        /// <param name="content">Determines whether the content should be saved.</param>
        protected sealed override void SerializeNode(XmlWriter writer, bool content) {

            SerializeProjectNode(writer, content);

            if (Tasks.Count > 0) {
                writer.WriteStartElement("Tasks");
                foreach (var task in Tasks) {
                    task.Serialize("Task", writer, content);
                }
                writer.WriteEndElement();
            }

            if (Children.Count > 0) {
                writer.WriteStartElement("Nodes");
                foreach (var node in Children) {                   
                    node.Serialize("Node", writer, content);
                }
                writer.WriteEndElement();  
            }



        }

    }
}
