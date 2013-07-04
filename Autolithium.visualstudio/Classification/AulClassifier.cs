// Copyright (c) Microsoft Corporation
// All rights reserved

namespace AulLanguage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType("Aul")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class AulClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("Aul")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition AulContentType = null;

        [Export]
        [FileExtension(".aul")]
        [ContentType("Aul")]
        internal static FileExtensionToContentTypeDefinition AulFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<AulTokenTag> aulTagAggregator = 
                                            aggregatorFactory.CreateTagAggregator<AulTokenTag>(buffer);

            return new AulClassifier(buffer, aulTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class AulClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<AulTokenTag> _aggregator;
        IDictionary<AulTokenTypes, IClassificationType> _aulTypes;

        internal AulClassifier(ITextBuffer buffer, 
                               ITagAggregator<AulTokenTag> aulTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = aulTagAggregator;
            _aulTypes = new Dictionary<AulTokenTypes, IClassificationType>();
            _aulTypes[AulTokenTypes.OokExclaimation] = typeService.GetClassificationType("Aul");
            _aulTypes[AulTokenTypes.OokPeriod] = typeService.GetClassificationType("ook.");
            _aulTypes[AulTokenTypes.OokQuestion] = typeService.GetClassificationType("ook?");
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return 
                    new TagSpan<ClassificationTag>(tagSpans[0], 
                                                   new ClassificationTag(_aulTypes[tagSpan.Tag.type]));
            }
        }
    }
}
