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

namespace SharpNL.Utility {
    /// <summary>
    /// Represents the OpenNLP Tools library version.
    /// </summary>
    public class Version : IComparable, IComparable<Version>, IEquatable<Version> {
        private const string DEV_VERSION_STRING = "0.0.0-SNAPSHOT";

        private const string SNAPSHOT_MARKER = "-SNAPSHOT";
        public static readonly Version DEV_VERSION = Parse(DEV_VERSION_STRING);


        /// <summary>
        /// Initializes the current instance with the provided versions.
        /// </summary>
        public Version(int major, int minor, int revision, bool snapshot) {
            Major = major;
            Minor = minor;
            Revision = revision;
            Snapshot = snapshot;
        }

        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int Revision { get; private set; }

        public bool Snapshot { get; private set; }

        public static Version Parse(string value) {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var snapshot = value.Contains(SNAPSHOT_MARKER);

            if (snapshot)
                value = value.Replace(SNAPSHOT_MARKER, string.Empty);

            // ignore tags like "-incubating"
            if (value.Contains("-")) {
                value = value.Split('-')[0];
            }

            var tokens = value.Split('.');

            if (tokens.Length < 3) {
                throw new InvalidFormatException("Invalid version format! Expected two dots...");
            }

            var v = new int[3];
            for (var i = 0; i < 3; i++) {
                if (!int.TryParse(tokens[i], out v[i])) {
                    throw new InvalidFormatException("Invalid version number.");
                }
            }

            return new Version(v[0], v[1], v[2], snapshot);
        }

        #region + CompareTo .

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="other"/>. Greater than zero This instance follows <paramref name="other"/> in the sort order. 
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="other"/> is not the same type as this instance. </exception>
        public int CompareTo(object other) {
            if (other == null) {
                return 1;
            }
            return CompareTo(other as Version);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="version"/> parameter.Zero This object is equal to <paramref name="version"/>. Greater than zero This object is greater than <paramref name="version"/>. 
        /// </returns>
        /// <param name="version">An object to compare with this object.</param>
        public int CompareTo(Version version) {
            if (version == null) {
                throw new ArgumentException("other");
            }
            if (Major > version.Major)
                return 1;

            if (Major < version.Major)
                return -1;

            if (Minor > version.Minor)
                return 1;

            if (Minor < version.Minor)
                return -1;

            if (Revision > version.Revision)
                return 1;

            if (Revision < version.Revision)
                return -1;

            if (Snapshot && !version.Snapshot)
                return 1;

            if (!Snapshot && version.Snapshot)
                return -1;

            return 0;
        }

        #endregion

        #region + Equals .

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Version other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Major == other.Major && Minor == other.Minor && Revision == other.Revision &&
                   Snapshot.Equals(other.Snapshot);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Version) obj);
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("{0}.{1}.{2}{3}", Major, Minor, Revision, Snapshot ? SNAPSHOT_MARKER : string.Empty);
        }

        #endregion

        #region + Operators .

        public static bool operator ==(Version one, Version two) {
            if (ReferenceEquals(one, null) && ReferenceEquals(two, null))
                return true;

            if (ReferenceEquals(one, null))
                return false;

            return one.Equals(two);
        }

        public static bool operator !=(Version one, Version two) {
            return !(one == two);
        }

        public static bool operator >(Version one, Version two) {
            return (two < one);
        }

        public static bool operator >=(Version one, Version two) {
            return (two <= one);
        }

        public static bool operator <(Version one, Version two) {
            if (one == null) {
                throw new ArgumentNullException("one");
            }
            return (one.CompareTo(two) < 0);
        }

        public static bool operator <=(Version one, Version two) {
            if (one == null) {
                throw new ArgumentNullException("one");
            }
            return (one.CompareTo(two) <= 0);
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = Major;
                hashCode = (hashCode*397) ^ Minor;
                hashCode = (hashCode*397) ^ Revision;
                hashCode = (hashCode*397) ^ Snapshot.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}