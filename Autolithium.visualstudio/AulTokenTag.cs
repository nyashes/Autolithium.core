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
    [TagType(typeof(AulTokenTag))]
    internal sealed class AulTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new OokTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class AulTokenTag : ITag 
    {
        public AulTokenTypes type { get; private set; }

        public AulTokenTag(AulTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class OokTokenTagger : ITagger<AulTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, AulTokenTypes> _aulTypes;

        internal OokTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _aulTypes = new Dictionary<string, AulTokenTypes>();
            _aulTypes["Aul"] = AulTokenTypes.OokExclaimation;
            _aulTypes["ook."] = AulTokenTypes.OokPeriod;
            _aulTypes["ook?"] = AulTokenTypes.OokQuestion;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<AulTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (string aulToken in tokens)
                {
                    if (_aulTypes.ContainsKey(aulToken))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, aulToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<AulTokenTag>(tokenSpan, 
                                                                  new AulTokenTag(_aulTypes[aulToken]));
                    }

                    //add an extra char location because of the space
                    curLoc += aulToken.Length + 1;
                }
            }
            
        }
    }
}
