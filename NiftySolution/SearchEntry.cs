// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Aurora
{
	namespace NiftySolution
	{
		public class SearchEntry
		{
			public string fullPath = "";
			public string project = "";
			public string description = "";
			public string key = "";
			public string filename = "";

			public SearchEntry()
			{
			}
			
			public class CompareOnRelevance : IComparer<SearchEntry>
			{
				private string mQuery;

				public CompareOnRelevance(string query)
				{
					mQuery = query.ToLower();
				}

				public int Compare(SearchEntry lhs, SearchEntry rhs)
				{
					int relevance = (lhs.filename.StartsWith(mQuery) ? 0 : 1) - (rhs.filename.StartsWith(mQuery) ? 0 : 1);
					if(relevance != 0)
						return relevance;

					return string.Compare(lhs.fullPath, rhs.fullPath);
				}
			}
		}
	}
}
