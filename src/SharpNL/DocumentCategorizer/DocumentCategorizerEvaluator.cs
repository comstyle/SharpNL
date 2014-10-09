using System;
using SharpNL.Utility.Evaluation;

namespace SharpNL.DocumentCategorizer {
    public class DocumentCategorizerEvaluator : Evaluator<DocumentSample, string> {
        private readonly IDocumentCategorizer documentCategorizer;

        public DocumentCategorizerEvaluator(IDocumentCategorizer documentCategorizer,
            params IEvaluationMonitor<DocumentSample>[] listeners) : base(listeners) {
            
            if (documentCategorizer == null)
                throw new ArgumentNullException("documentCategorizer");

            this.documentCategorizer = documentCategorizer;
        }

        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override DocumentSample ProcessSample(DocumentSample reference) {
            var document = reference.Text;

            var probs = documentCategorizer.Categorize(document, reference.ExtraInformation);
            var cat = documentCategorizer.GetBestCategory(probs);

            FMeasure.UpdateScores(new []{ reference.Category }, new []{ cat } );
            
            return new DocumentSample(cat, reference.Text);
        }
    }
}
