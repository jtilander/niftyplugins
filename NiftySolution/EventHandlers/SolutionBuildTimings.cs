// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System.IO;

namespace Aurora
{
	namespace NiftySolution
	{
		class SolutionBuildTimings
		{
			private OutputWindowPane m_pane;
			private Stopwatch m_timer;
			private BuildEvents m_buildEvents;
            private DateTime m_start;
			private string m_logfilename;
			
			public SolutionBuildTimings(Plugin plugin)
			{
				m_pane = Plugin.FindOutputPane(plugin.App, "Build");
				if (null == m_pane)
					return;

				m_buildEvents = ((EnvDTE80.Events2)plugin.App.Events).BuildEvents;
				m_buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(OnBuildBegin);
				m_buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(OnBuildDone);

				m_timer = new Stopwatch();
				
				Options options = (Options)plugin.Options;
                m_logfilename = options.CompileTimeLogFile;
			}

			void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
			{
				if (Scope != vsBuildScope.vsBuildScopeSolution)
					return;
                
                DateTime now = DateTime.Now;

				m_timer.Stop();
				TimeSpan ts = m_timer.Elapsed;
				string timeMessage = String.Format("Total solution build time: {0:00}:{1:00}:{2:00}.{3:00} (started {4} {5} and ended {6} {7})\n", 
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, m_start.ToShortDateString(), m_start.ToLongTimeString(), now.ToShortDateString(), now.ToLongTimeString());
                m_pane.OutputString(timeMessage);
				
				m_timer.Reset();


                if (m_logfilename != "")
				{
                    using (StreamWriter w = new StreamWriter(m_logfilename, true))
				    {
				        string logMessage = String.Format("{0} {1}|{2:00}:{3:00}:{4:00}", now.ToShortDateString(), now.ToLongTimeString(), ts.Hours, ts.Minutes, ts.Seconds );
				        w.WriteLine(logMessage);
				    }
				}
			}

			void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
			{
				if (Scope != vsBuildScope.vsBuildScopeSolution)
					return;
				m_timer.Start();
                m_start = DateTime.Now;
			}
		}
	}
}
