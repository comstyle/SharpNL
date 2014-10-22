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

namespace SharpNL.Project {
    /// <summary>
    /// Represents a problem in the project. This class cannot be inherited.
    /// </summary>
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public sealed class ProjectProblem {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectProblem"/> with a description of the problem.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <exception cref="System.ArgumentNullException">description</exception>
        public ProjectProblem(string description) {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectProblem"/> with the object that holds the problem and a description of the problem.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="description">The description.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="obj"/>
        /// or
        /// <paramref name="description"/>
        /// </exception>
        public ProjectProblem(object obj, string description) {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            Object = obj;
            Description = description;
        }

        #region . Object .

        /// <summary>
        /// Gets the object that has the problem.
        /// </summary>
        /// <value>The object that has the problem.</value>
        [Description("The object that has the problem."), TypeConverter(typeof(ExpandableObjectConverter))]
        public object Object { get; private set; }

        #endregion

        #region . Description .
        /// <summary>
        /// Gets the description of the problem.
        /// </summary>
        /// <value>The description of the problem.</value>
        [Description("Describes the problem.")]
        public string Description { get; private set; }
        #endregion

    }
}