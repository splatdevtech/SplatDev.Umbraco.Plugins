using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Util;

namespace SplatDev.Umbraco.Examine.Analyzers
{
    public class NoStopWordsAnalyzer : Analyzer
    {
        private readonly CharArraySet stopWords;

        public NoStopWordsAnalyzer()
        {
            stopWords = new CharArraySet(LuceneVersion.LUCENE_48, 0, true);
        }
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var tokenizer = new StandardTokenizer(LuceneVersion.LUCENE_48, reader);
            var tokenStream = new StopFilter(LuceneVersion.LUCENE_48, tokenizer, stopWords);
            return new TokenStreamComponents(tokenizer, tokenStream);
        }
    }
}
