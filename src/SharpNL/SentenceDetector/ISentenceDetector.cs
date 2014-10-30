using SharpNL.Utility;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// The interface for sentence detectors, which find the sentence boundaries in a text.
    /// </summary>
    public interface ISentenceDetector {

        /// <summary>
        /// Detects the sentences in the specified string.
        /// </summary>
        /// <param name="text">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:string[]"/> with the individual sentences as the array elements.</returns>
        string[] SentDetect(string text);

        /// <summary>
        /// Detects the position of the sentences in the specified string.
        /// </summary>
        /// <param name="text">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into <paramref name="text"/>) for each detected sentence as the individuals array elements.</returns>
        Span[] SentPosDetect(string text);

    }
}
