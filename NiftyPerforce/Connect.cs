// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.IO;
using System.Reflection;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Main stub that interfaces towards Visual Studio.
		public class Connect : IDTExtensibility2, IDTCommandTarget
		{
			private Plugin m_plugin = null;
			private CommandRegistry m_commandRegistry = null;

			public Connect()
			{
			}

			public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst_, ref Array custom)
			{
				if(null != m_plugin)
					return;

				// Load up the options from file.
				string optionsFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NiftyPerforce.xml");
				Config options = Config.Load(optionsFileName);

				// Every plugin needs a command bar.
				m_plugin = new Plugin((DTE2)application, (AddIn)addInInst_, "NiftyPerforce", "Aurora.NiftyPerforce.Connect", options);
				CommandBar commandBar = m_plugin.AddCommandBar("NiftyPerforce", MsoBarPosition.msoBarTop);
				m_commandRegistry = new CommandRegistry(m_plugin, commandBar);


				// Initialize the logging system.
				if(Log.HandlerCount == 0)
				{
#if DEBUG
					Log.AddHandler(new DebugLogHandler());
#endif

					Log.AddHandler(new VisualStudioLogHandler(m_plugin.OutputPane));
					Log.Prefix = "NiftyPerforce";
				}

				// Now we can take care of registering ourselves and all our commands and hooks.
				Log.Debug("Booting up...");
				Log.IncIndent();

				bool doContextCommands = true;

				bool doBindings = options.EnableBindings;
				m_commandRegistry.RegisterCommand("NiftyConfig", doBindings, new NiftyConfigure(m_plugin), true);

				m_commandRegistry.RegisterCommand("NiftyEditModified", doBindings, new P4EditModified(m_plugin), true);

				m_commandRegistry.RegisterCommand("NiftyEdit", doBindings, new P4EditItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyEditItem", doBindings, new P4EditItem(m_plugin), false);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyEditSolution", doBindings, new P4EditSolution(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyDiff", doBindings, new P4DiffItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyDiffItem", doBindings, new P4DiffItem(m_plugin), false);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyDiffSolution", doBindings, new P4DiffSolution(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyHistory", doBindings, new P4RevisionHistoryItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyHistoryItem", doBindings, new P4RevisionHistoryItem(m_plugin), false);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyHistorySolution", doBindings, new P4RevisionHistorySolution(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyTimeLapse", doBindings, new P4TimeLapseItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyTimeLapseItem", doBindings, new P4TimeLapseItem(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyRevert", doBindings, new P4RevertItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyRevertItem", doBindings, new P4RevertItem(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyShow", doBindings, new P4ShowItem(m_plugin), true);
				if(doContextCommands) m_commandRegistry.RegisterCommand("NiftyShowItem", doBindings, new P4ShowItem(m_plugin), false);

				m_plugin.AddFeature(new AutoAddDelete(m_plugin));
				m_plugin.AddFeature(new AutoCheckoutProject(m_plugin));
				m_plugin.AddFeature(new AutoCheckoutOnBuild(m_plugin));
				m_plugin.AddFeature(new AutoCheckoutTextEdit(m_plugin));
				m_plugin.AddFeature(new AutoCheckoutOnSave(m_plugin));

#if DEBUG
				// Use this to track down event GUIDs.
				//m_plugin.AddFeature(new FindEvents(m_plugin));
#endif

				P4Operations.CheckInstalledFiles();

				P4Operations.InitThreadHelper();

				Log.DecIndent();
				Log.Debug("Initialized...");

#if DEBUG
				Log.Info("NiftyPerforce (Debug)");
#else
                //Log.Info("NiftyPerforce (Release)");
#endif
				// Show where we are and when we were compiled...
				Log.Info("exe :" + Assembly.GetExecutingAssembly().Location);
				Log.Info("exe :" + System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location));
			}

			public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
			{
				if(null == m_plugin || null == m_commandRegistry)
					return;

				if(neededText != vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
					return;

				status = m_commandRegistry.Query(commandName);
			}

			public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
			{
				handled = false;

				if(null == m_plugin || null == m_commandRegistry)
					return;
				if(executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
					return;

				handled = m_commandRegistry.Execute(commandName);
			}

			public void OnBeginShutdown(ref Array custom)
			{
				//TODO: Make this thing unregister all the callbacks we've just made... gahhh... C# and destructors... 
				P4Operations.KillThreadHelper();
			}

			public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
			{
				if(null == m_plugin)
					return;

				Log.Debug("Disconnect called...");
				((Config)m_plugin.Options).Save();
				Log.ClearHandlers();
			}

			public void OnAddInsUpdate(ref Array custom)
			{
			}
			public void OnStartupComplete(ref Array custom)
			{
			}

		}
	}
}

