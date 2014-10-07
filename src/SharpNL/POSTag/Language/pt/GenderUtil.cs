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

namespace SharpNL.POSTag.Language.pt {
    public static class GenderUtil {
        public static string RemoveGender(string pos) {
            if (pos == null)
                return null;

            if (pos.StartsWith("art"))
                return "art";

            if (pos == "nm" || pos == "nf" || pos == "nn")
                return "n";

            return pos;
        }

        public static string[] RemoveGender(string[] pos) {
            if (pos == null)
                return null;

            for (int i = 0; i < pos.Length; i++) {
                pos[i] = RemoveGender(pos[i]);
            }

            return pos;
        }
    }
}