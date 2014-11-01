using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Nodes {
    public class TextInputNode : ProjectNode {

        #region + Properties .

        #region . Language .
        /// <summary>
        /// Gets the language of this file.
        /// </summary>
        /// <value>The language of this file.</value>
        [Description("Specifies the language which is being processed."), TypeConverter(typeof(LanguageConverter))]
        public string Language { get; set; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Text { get; set; }
        #endregion

        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (string.IsNullOrEmpty(Language))
                list.Add(new ProjectProblem(this, "The language is not specified."));

            if (string.IsNullOrWhiteSpace(Text))
                list.Add(new ProjectProblem(this, "The text is not specified."));

            return list.Count > 0 ? list.ToArray() : null;
        }
        #endregion

        #region . Prepare .
        /// <summary>
        /// Prepares the task for execution.
        /// </summary>
        /// <returns>A array of pre-loaded outputs.</returns>
        protected override object[] Prepare() {
            if (string.IsNullOrEmpty(Language))
                throw new InvalidOperationException("The language is not specified.");

            if (string.IsNullOrWhiteSpace(Text))
                throw new InvalidOperationException("The text is not specified.");

            return new object[] { new Document(Language, Text) };
        }
        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the node from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="xmlNode">The node.</param>
        protected override void Deserialize(XmlNode xmlNode) {
            if (xmlNode.Attributes == null)
                throw new InvalidFormatException("The node has no attributes.");

            var attFile = xmlNode.Attributes["Lang"];
            if (attFile != null)
                Language = attFile.Value;

            if (xmlNode.FirstChild != null && xmlNode.FirstChild.NodeType == XmlNodeType.CDATA)
                Text = xmlNode.FirstChild.Value;

        }        
        #endregion

        #region . SerializeProjectNode .
        protected override void SerializeProjectNode(XmlWriter writer, bool content) {
            writer.WriteAttributeString("Lang", Language);
            writer.WriteCData(Text);
        }
        #endregion

    }
}
