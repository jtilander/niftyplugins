using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.IO;

namespace Aurora
{
	namespace NiftySolution
	{
		public class SolutionFiles
		{
			private DTE2 m_application;
			private List<SearchEntry> m_solutionFiles  = new List<SearchEntry>();
			private List<SearchEntry> mHits = new List<SearchEntry>();

			public int Count
			{
				get { return m_solutionFiles.Count; }
			}

			public SolutionFiles(DTE2 application)
			{
				m_application = application;
			}

			public void Refresh()
			{
				m_solutionFiles.Clear();

				foreach(Project project in m_application.Solution.Projects)
				{
					Log.Info("\tScanning project {0}", project.Name);
					AddProjectItems(project.ProjectItems);
				}
				Log.Info("Scanning done ({0} files in {1} projects)", Count, m_application.Solution.Projects.Count);
			}

			private void AddProjectItems(ProjectItems projectItems)
			{
				// HACK: Horrible! But how will we know what not to include in the list?
				// CPP:   {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// H:     {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// Folder:{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}

				if(null!=projectItems)
				{
					foreach(ProjectItem item in projectItems)
					{
						if("{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}" == item.Kind)
						{
							for(short i = 0; i < item.FileCount; i++)
							{
								string name = item.get_FileNames((short)i);

								SearchEntry entry = new SearchEntry();
								entry.fullPath = name;
								entry.key = name.ToLower();
								entry.filename = Path.GetFileName(entry.key);
								m_solutionFiles.Add(entry);
							}
						}

						AddProjectItems(item.ProjectItems);
					}
				}
			}

			public int CandidateCount
			{
				get { return mHits.Count; }
			}

			public SearchEntry Candidate(int i)
			{
				return mHits[i];
			}

			public void UpdateSearchQuery(string query, bool incremental)
			{
				if(query.Length > 0)
				{
					if(!incremental)
					{
						mHits = Filter(m_solutionFiles, query);
					}
					else
					{
						mHits = Filter(mHits, query);
					}
				}
				else
				{
				}

				// TODO: Sort the files based on relevance.
				mHits.Sort(new SearchEntry.CompareOnRelevance(query));
			}
			
			private List<SearchEntry> Filter(List<SearchEntry> candidates, string query)
			{
				query = query.ToLower();
				return candidates.FindAll(delegate(SearchEntry e) { return e.key.Contains(query); });
			}
		}
	}
}
