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
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpNL.Project.Design {
    internal class EncodingConverter : TypeConverter {
        private static readonly List<string> encodings;
        static EncodingConverter() {
            encodings = Encoding.GetEncodings().Select(info => info.Name).ToList();
            encodings.Sort();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {

            return sourceType == typeof (string) || typeof (Encoding).IsAssignableFrom(sourceType);
            
            //return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            var s = value as string;
            if (s != null) {
                return Encoding.GetEncoding(s);
            }

            return null;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            if (destinationType == typeof (string) || destinationType == typeof(Encoding))
                return true;

            return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof (string) && value is string)
                return value;

            if (destinationType == typeof(string) && value is Encoding)
                return ((Encoding) value).HeaderName;

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return true;
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return false;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(encodings);
        }
    }
}