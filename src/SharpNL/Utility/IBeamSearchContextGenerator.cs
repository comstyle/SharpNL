// /*
//  * Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *  Unless required by applicable law or agreed to in writing, software
//  *  distributed under the License is distributed on an "AS IS" BASIS,
//  *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *  See the License for the specific language governing permissions and
//  *  limitations under the License.
//  *
//  *  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  *  - May you do good and not evil.                                         -
//  *  - May you find forgiveness for yourself and forgive others.             -
//  *  - May you share freely, never taking more than you give.                -
//  *  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  * 
//  */

namespace SharpNL.Utility {
    /// <summary>Interface for context generators used with a sequence beam search.</summary>
    public interface IBeamSearchContextGenerator<in T> {
        /// <summary>Gets the context for the specified position in the specified sequence (list).</summary>
        /// <param name="index">The index of the sequence.</param>
        /// <param name="sequence">The sequence of items over which the beam search is performed.</param>
        /// <param name="priorDecisions">The sequence of decisions made prior to the context for which this decision is being made.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for the specified position in the specified sequence.</returns>
        string[] GetContext(int index, T[] sequence, string[] priorDecisions, object[] additionalContext);
    }
}