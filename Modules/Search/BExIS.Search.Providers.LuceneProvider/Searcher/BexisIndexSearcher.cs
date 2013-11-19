﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using BExIS.Search.Model;
using BExIS.Search.Providers.LuceneProvider.Helpers;
using BExIS.Search.Providers.LuceneProvider.Indexer;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;

namespace BExIS.Search.Providers.LuceneProvider.Searcher
{
    public static class BexisIndexSearcher
    {


        private static string luceneIndexPath = Path.Combine(FileHelper.IndexFolderPath, "BexisSearchIndex");
        private static string autoCompleteIndexPath = Path.Combine(FileHelper.IndexFolderPath, "AutoCompleteBexisDir");


        private static Lucene.Net.Store.Directory pathIndex = FSDirectory.Open(new DirectoryInfo(luceneIndexPath));
        private static Lucene.Net.Store.Directory autoCompleteIndex = FSDirectory.Open(new DirectoryInfo(autoCompleteIndexPath));
        public static IndexReader _Reader = IndexReader.Open(pathIndex, true);
        public static IndexSearcher searcher = new IndexSearcher(_Reader);

        public static IndexReader autoCompleteIndexReader = IndexReader.Open(autoCompleteIndex, true);
        public static IndexSearcher autoCompleteSearcher = new IndexSearcher(autoCompleteIndexReader);

        public static string[] facetFields { get; set; }
        public static string[] storedFields { get; set; }
        public static string[] categoryFields { get; set; }
        public static string[] propertyFields { get; set; }

        private static Boolean isInit = false;
        static XmlDocument configXML;

        public static string[] getCategoryFields() { init(); return categoryFields; }

        public static IndexReader getIndexReader()
        {
            init();
            return _Reader;
        }

        public static IndexSearcher getIndexSearcher()
        {
            init();
            return searcher;
        }

        private static void init()
        {

            if (!isInit) { BexisIndexSearcherInit(); isInit = true; }

        }


        private static void BexisIndexSearcherInit()
        {

            List<string> facetFieldList = new List<string>();
            List<string> categoryFieldList = new List<string>();
            List<string> propertyFieldList = new List<string>();
            List<string> storedFieldList = new List<string>();


            configXML = new XmlDocument();

            configXML.Load(FileHelper.ConfigFilePath);
            XmlNodeList fieldProperties = configXML.GetElementsByTagName("field");

            foreach (XmlNode fieldProperty in fieldProperties)
            {
                String metadataIndexingType = fieldProperty.Attributes.GetNamedItem("type").Value;
                String metadataIndexingStore = fieldProperty.Attributes.GetNamedItem("store").Value;
                if (metadataIndexingType.ToLower().Equals("category_field")) { categoryFieldList.Add("category_" + fieldProperty.Attributes.GetNamedItem("lucene_name").Value); }
                else if (metadataIndexingType.ToLower().Equals("property_field")) { propertyFieldList.Add("property_" + fieldProperty.Attributes.GetNamedItem("lucene_name").Value); }
                else if (metadataIndexingType.ToLower().Equals("facet_field")) { facetFieldList.Add("facet_" + fieldProperty.Attributes.GetNamedItem("lucene_name").Value); }
                if (metadataIndexingStore.ToLower().Equals("yes")) { storedFieldList.Add(fieldProperty.Attributes.GetNamedItem("lucene_name").Value); }
            }
            storedFields = storedFieldList.ToArray();
            facetFields = facetFieldList.ToArray();
            propertyFields = propertyFieldList.ToArray();
            categoryFields = categoryFieldList.ToArray();
        }


        public static SearchResult search(Query query, List<XmlNode> headerItemXmlNodeList)
        {
            TopDocs docs = searcher.Search(query, 100);
            SearchResult sro = new SearchResult();
            sro.PageSize = 10;
            sro.CurrentPage = 1;
            sro.NumberOfHits = 100;

            List<HeaderItem> Header = new List<HeaderItem>();
            List<HeaderItem> DefaultHeader = new List<HeaderItem>();
            foreach (XmlNode ade in headerItemXmlNodeList)
            {
                HeaderItem hi = new HeaderItem();
                hi = new HeaderItem();
                hi.Name = ade.Attributes.GetNamedItem("lucene_name").Value;
                hi.DisplayName = ade.Attributes.GetNamedItem("display_name").Value;
                Header.Add(hi);

                if (ade.Attributes.GetNamedItem("default_visible_item").Value.ToLower().Equals("yes"))
                {
                    DefaultHeader.Add(hi);
                }
                if (ade.Attributes.GetNamedItem("lucene_name").Value.ToLower().Equals("id"))
                {
                    sro.Id = hi;
                }
            }
            List<Row> RowList = new List<Row>();
            foreach (ScoreDoc sd in docs.ScoreDocs)
            {
                Document doc = searcher.Doc(sd.Doc);
                Row r = new Row();
                List<object> ValueList = new List<object>();
                ValueList = new List<object>();
                foreach (XmlNode ade in headerItemXmlNodeList)
                {
                    String fieldType = ade.Attributes.GetNamedItem("type").Value;
                    String luceneName = ade.Attributes.GetNamedItem("lucene_name").Value;
                    if (fieldType.ToLower().Equals("facet_field"))
                    {
                        luceneName = "facet_" + luceneName;
                    }
                    else if (fieldType.ToLower().Equals("category_field"))
                    {
                        luceneName = "category_" + luceneName;
                    }
                    else if (fieldType.ToLower().Equals("property_field"))
                    {
                        luceneName = "property_" + luceneName;
                    }

                    ValueList.Add(doc.Get(luceneName));
                }
                r.Values = ValueList;
                RowList.Add(r);
            }
            sro.Header = Header;
            sro.DefaultVisibleHeaderItem = DefaultHeader;
            sro.Rows = RowList;
            return sro;
        }


        public static IEnumerable<TextValue> doTextSearch(Query origQuery, String queryFilter, String searchtext)
        {

            String filter = queryFilter;
            BooleanQuery query = new BooleanQuery();
            query.Add(origQuery, Occur.MUST);
            if (!filter.ToLower().StartsWith("ng_"))
            {
                filter = "ng_" + filter;
            }
            if (filter.ToLower().Equals("ng_all"))
            {
                filter = "ng_all";
                queryFilter = "ng_all";
            }

            HashSet<string> uniqueText = new HashSet<string>();
            searchtext = searchtext.ToLower();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, filter, new KeywordAnalyzer());
            parser.DefaultOperator = QueryParser.Operator.AND;
            Query X1 = parser.Parse(searchtext);
            query.Add(X1, Occur.MUST);
            // Query query = parser.Parse("tree data");
            TopDocs tds = searcher.Search(query, 100);
            QueryScorer scorer = new QueryScorer(query, searchtext);
            //SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<span class=\"highlight\">", "</span>");
            //Highlighter highlighter = new Highlighter(formatter, scorer);
            // Highlighter highlighter = new Highlighter(scorer);
            //highlighter.TextFragmenter = new SimpleSpanFragmenter(scorer);

            Analyzer analyzer = new NGramAnalyzer();
            List<TextValue> l = new List<TextValue>();
            foreach (ScoreDoc sd in tds.ScoreDocs)
            {
                Document doc = searcher.Doc(sd.Doc);
                String docId = doc.GetField("doc_id").StringValue;
                TermQuery q1 = new TermQuery(new Term("id", docId.ToLower()));
                TermQuery q0 = new TermQuery(new Term("field", queryFilter.ToLower()));
                QueryParser parser1 = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "value", new KeywordAnalyzer());
                parser1.DefaultOperator = QueryParser.Operator.AND;
                Query q2 = parser1.Parse(searchtext);
                //Query q2 = (new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "value", new NGramAnalyzer())).Parse(searchtext);
                BooleanQuery q3 = new BooleanQuery();
                q3.Add(q1, Occur.MUST);
                q3.Add(q2, Occur.MUST);
                q3.Add(q0, Occur.MUST);
                TopDocs tdAutoComp = autoCompleteSearcher.Search(q3, 100);
                foreach (ScoreDoc sdAutoComp in tdAutoComp.ScoreDocs)
                {
                    Document docAutoComp = autoCompleteSearcher.Doc(sdAutoComp.Doc);
                    //String text = doc.GetFields(filter)[0].StringValue;
                    //TokenStream stream = TokenSources.GetAnyTokenStream(searcher.IndexReader, sd.Doc, filter, doc, analyzer);
                    // String fragment = highlighter.GetBestFragments(stream, text, 3, "....");
                    // String fragment = (highlighter.GetBestTextFragments(stream, text, true, 3 ))[0].ToString();
                    String toAdd = docAutoComp.GetField("value").StringValue;
                    if (!uniqueText.Contains(toAdd))
                    {
                        TextValue abcd = new TextValue();
                        abcd.Name = toAdd;
                        abcd.Value = toAdd;
                        l.Add(abcd);
                        uniqueText.Add(toAdd);
                    }
                }
            }
            return l;
        }

        public static IEnumerable<Facet> facetSearch(Query query, IEnumerable<Facet> facets)
        {
            List<Facet> l = new List<Facet>();
            foreach (Facet f in facets)
            {
                Facet c = new Facet();
                c.Name = f.Name;
                c.Text = f.Text;
                c.Value = f.Value;
                c.DisplayName = f.DisplayName;
                //c.Expanded = true;
                //c.Enabled = true;
                c.Childrens = new List<Facet>();
                List<Facet> lc = new List<Facet>();
                SimpleFacetedSearch sfs = new SimpleFacetedSearch(_Reader, new string[] { "facet_" + f.Name });
                SimpleFacetedSearch.Hits hits = sfs.Search(query);
                foreach (SimpleFacetedSearch.HitsPerFacet hpg in hits.HitsPerFacet)
                {
                    Facet cc = new Facet();
                    cc.Name = hpg.Name.ToString();
                    cc.Text = hpg.Name.ToString();
                    cc.Value = hpg.Name.ToString();
                    cc.Count = (int)hpg.HitCount;
                    lc.Add(cc);
                }
                //SetParent(c);
                if (lc.Count() > 0)
                {
                    int childCount = 0;
                    foreach (Facet c_child in lc)
                    {
                        childCount += c_child.Count;
                        //c.Items.Add(c_child);
                        c.Childrens.Add(c_child);
                    }

                    //c.Childs = true;
                    //c.Text = c.CategoryName + " (" + childCount.ToString() + ")";
                    c.Text = c.Name;
                    c.Count += childCount;
                }
                else c.Count = 0;
                //c.Count = c.Childrens.Count();
                l.Add(c);
            }
            return l;
        }


    }
}