// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
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
			private Plugin mPlugin;
			private DTE2 m_application;
			private List<SearchEntry> m_solutionFiles  = new List<SearchEntry>();
			private List<SearchEntry> mHits = new List<SearchEntry>();

			public int Count
			{
				get { return m_solutionFiles.Count; }
			}

			public SolutionFiles(Plugin plugin)
			{
				m_application = plugin.App;
				mPlugin = plugin;
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


				// Now, let's add the files that were explicitly referenced from the config dialog.
				Options options = (Options)mPlugin.Options;
				string[] directories = options.ExtraSearchDirs.Split(';');

				foreach(string dir in directories)
				{
                    if (dir.Length == 0)
                        continue;
                    string expanded = Environment.ExpandEnvironmentVariables(dir);
                    Log.Info("Scanning files from {0}", expanded);
					int count = AddFilesFromDir(expanded);
					Log.Info("Added {0} files", count);
				}
			}

			private int AddFilesFromDir(string dirname)
			{
				int count = 0;

				foreach(string file in Directory.GetFiles(dirname))
				{
					SearchEntry entry = new SearchEntry();
					entry.fullPath = file;
					entry.key = file.ToLower();
					entry.filename = Path.GetFileName(file);
					m_solutionFiles.Add(entry);

					count++;
				}
				
				foreach(string dir in Directory.GetDirectories(dirname))
				{
					count += AddFilesFromDir(dir);
				}
				
				return count;
			}

			private void AddProjectItems(ProjectItems projectItems)
			{
				// HACK: Horrible! But how will we know what not to include in the list?
				// CPP:   {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// H:     {6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}
				// Folder:{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}
				//
				// "{5F422961-1EE4-47EB-A53B-777B4164F8DF}" <-- it's a folder ?
				if(null == projectItems)
					return;
				foreach(ProjectItem item in projectItems)
				{
					if("{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}" == item.Kind)
					{
						// Indices starts at 1 ... http://msdn.microsoft.com/en-us/library/envdte.projectitem.filenames.aspx
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
					
					if(item.SubProject != null)
					{
						AddProjectItems(item.SubProject.ProjectItems);
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
