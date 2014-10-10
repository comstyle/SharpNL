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
using System.IO;
using System.Text;
using SharpNL.Java;

namespace SharpNL.Utility.FeatureGen {
    [TypeClass("opennlp.tools.util.featuregen.W2VClassesDictionary")]
    public class W2VClassesDictionary {

        private readonly Dictionary<string, string> tokenToClusterMap;

        public W2VClassesDictionary(Stream inputStream) {
            tokenToClusterMap = new Dictionary<string, string>();

            var reader = new StreamReader(inputStream, Encoding.UTF8);

            string line;
            while ((line = reader.ReadLine()) != null) {
                var parts = line.Split(' ');

                if (parts.Length == 2) {
                    tokenToClusterMap.Add(parts[0], parts[1]);
                }
            }
        }


        public string LookupToken(string value) {
            return tokenToClusterMap[value];
        }



        
    }
}