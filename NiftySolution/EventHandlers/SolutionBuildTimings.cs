// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;

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
			
			public SolutionBuildTimings(Plugin plugin)
			{
				m_pane = Plugin.FindOutputPane(plugin.App, "Build");
				if (null == m_pane)
					return;

				m_buildEvents = ((EnvDTE80.Events2)plugin.App.Events).BuildEvents;
				m_buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(OnBuildBegin);
				m_buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(OnBuildDone);

				m_timer = new Stopwatch();
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
