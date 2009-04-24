// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
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
			BuildEvents m_buildEvents;
			
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

				m_timer.Stop();
				TimeSpan ts = m_timer.Elapsed;
				string timeMessage = String.Format("Total solution build time: {0:00}:{1:00}:{2:00}.{3:00}\n", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
				m_pane.OutputString(timeMessage);
				
				m_timer.Reset();
			}

			void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
			{
				if (Scope != vsBuildScope.vsBuildScopeSolution)
					return;
				m_timer.Start();
			}
		}
	}
}
