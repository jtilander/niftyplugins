// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
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
			private AutoAddDelete m_addDelete = null;
			private AutoCheckout m_autoCheckout = null;

			public Connect()
			{
			}

 			public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst_, ref Array custom)
			{
				if( null != m_plugin)
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
					Log.Prefix = "NiftySolution";
				}

				// Now we can take care of registering ourselves and all our commands and hooks.
				Log.Debug("Booting up...");
				Log.IncIndent();


				bool doBindings = options.EnableBindings;
				m_commandRegistry.RegisterCommand("NiftyConfig", doBindings, new NiftyConfigure(m_plugin), true);

				m_commandRegistry.RegisterCommand("NiftyEditModified", doBindings, new P4EditModified(m_plugin), true);
				
				m_commandRegistry.RegisterCommand("NiftyEdit", doBindings, new P4EditItem(m_plugin), true);
				m_commandRegistry.RegisterCommand("NiftyEditItem", doBindings, new P4EditItem(m_plugin), false);
				m_commandRegistry.RegisterCommand("NiftyEditSolution", doBindings, new P4EditSolution(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyDiff", doBindings, new P4DiffItem(m_plugin), true);
				m_commandRegistry.RegisterCommand("NiftyDiffItem", doBindings, new P4DiffItem(m_plugin), false);
				m_commandRegistry.RegisterCommand("NiftyDiffSolution", doBindings, new P4DiffSolution(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyHistory", doBindings, new P4RevisionHistoryItem(m_plugin), true);
				m_commandRegistry.RegisterCommand("NiftyHistoryItem", doBindings, new P4RevisionHistoryItem(m_plugin), false);
				m_commandRegistry.RegisterCommand("NiftyHistorySolution", doBindings, new P4RevisionHistorySolution(m_plugin), false);


				m_commandRegistry.RegisterCommand("NiftyTimeLapse", doBindings, new P4TimeLapseItem(m_plugin), true);
				m_commandRegistry.RegisterCommand("NiftyTimeLapseItem", doBindings, new P4TimeLapseItem(m_plugin), false);

				m_commandRegistry.RegisterCommand("NiftyRevert", doBindings, new P4RevertItem(m_plugin), true);
				m_commandRegistry.RegisterCommand("NiftyRevertItem", doBindings, new P4RevertItem(m_plugin), false);
				


                /*m_plugin.RegisterCommand("NiftyPerforceEdit", new ToolbarCommand<P4EditItem>());
                m_plugin.RegisterCommand("NiftyPerforceDiff", new ToolbarCommand<P4DiffItem>());
                m_plugin.RegisterCommand("NiftyPerforceRevert", new ToolbarCommand<P4RevertItem>());
                m_plugin.RegisterCommand("NiftyPerforceRevisionHistory", new ToolbarCommand<P4RevisionHistoryItem>());
                m_plugin.RegisterCommand("NiftyPerforceEditAllModified", new P4EditModified());
                m_plugin.RegisterCommand("NiftyPerforceConfiguration", new NiftyConfigure());
                m_plugin.RegisterCommand("NiftyPerforceEditItem", new P4EditItem());
                m_plugin.RegisterCommand("NiftyPerforceRenameItem", new P4RenameItem());
                m_plugin.RegisterCommand("NiftyPerforceDiffItem", new P4DiffItem());
                m_plugin.RegisterCommand("NiftyPerforceRevertItem", new P4RevertItem());
                m_plugin.RegisterCommand("NiftyPerforceRevisionHistoryItem", new P4RevisionHistoryItem());
				m_plugin.RegisterCommand("NiftyPerforceTimeLapseItem", new P4TimeLapseItem());
				m_plugin.RegisterCommand("NiftyPerforceEditSolution", new P4EditSolution());
				m_plugin.RegisterCommand("NiftyPerforceDiffSolution", new P4DiffSolution());
				m_plugin.RegisterCommand("NiftyPerforceRevisionHistorySolution", new P4RevisionHistorySolution());

                // add the toolbar and menu commands
                CommandBar commandBar = m_plugin.AddCommandBar("NiftyPerforce", MsoBarPosition.msoBarTop);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceEdit", "P4 Edit Current File", "Opens the current document for edit", 1, 1);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceEditAllModified", "P4 Edit All Modified", "Opens all the unsaved, readonly documents for edit", 5, 2);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceDiff", "P4 Diff Current File", "Diffs the current document against the depot", 3, 3);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceRevisionHistory", "P4 Revision History Current File", "Shows the revision history of the current document", 6, 4);
				m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceTimeLapseItem", "P4 Time lapse view", "Brings up the time lapse view", 7, 5);
				m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceRevert", "P4 Revert Current File", "Reverts the current document", 4, 6);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceConfiguration", "P4 Configuration", "Opens the configuration dialog", 2, 7);

                m_plugin.AddMenuCommand("Solution", "NiftyPerforceEditSolution", "P4 Edit Solution", "Opens the solution for edit", 1, 6);
				m_plugin.AddMenuCommand("Solution", "NiftyPerforceDiffSolution", "P4 Diff", "Diffs the selected item with the depot", 3, 6);
				m_plugin.AddMenuCommand("Solution", "NiftyPerforceRevisionHistorySolution", "P4 Revision History", "Shows the revision history of the selected item", 6, 7); 

                m_plugin.AddMenuCommand("Item", "NiftyPerforceEditItem", "P4 Edit", "Opens the document for edit", 1, 4);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceRenameItem", "P4 Rename", "Renames the item", 1, 5);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceDiffItem", "P4 Diff", "Diffs the selected item with the depot", 3, 6);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceRevisionHistoryItem", "P4 Revision History", "Shows the revision history of the selected item", 6, 7);
				m_plugin.AddMenuCommand("Item", "NiftyPerforceTimeLapseItem", "P4 Time lapse view", "Brings up the time lapse view", 7, 8);
				m_plugin.AddMenuCommand("Item", "NiftyPerforceRevertItem", "P4 Revert", "Reverts the item", 4, 9);

                m_plugin.AddMenuCommand("Project", "NiftyPerforceEditItem", "P4 Edit", "Opens the project for edit", 1, 5);
				m_plugin.AddMenuCommand("Project", "NiftyPerforceDiffItem", "P4 Diff", "Diffs the selected item with the depot", 3, 6);
				m_plugin.AddMenuCommand("Project", "NiftyPerforceRevisionHistoryItem", "P4 Revision History", "Shows the revision history of the selected item", 6, 7);
				
				m_plugin.AddMenuCommand("Cross Project Multi Project", "NiftyPerforceEditItem", "P4 Edit", "Opens the project for edit", 1, 5);
                m_plugin.AddMenuCommand("Cross Project Multi Item", "NiftyPerforceEditItem", "P4 Edit", "Opens the document for edit", 1, 5);
				m_plugin.AddMenuCommand("Cross Project Multi Item", "NiftyPerforceRevertItem", "P4 Revert", "Reverts the item", 4, 8);

				// Adds mouse shortcuts to the MDI tab.
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceEditItem", "P4 Edit", "Opens the document for edit", 1, 4);
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceRenameItem", "P4 Rename", "Renames the item", 1, 5);
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceDiffItem", "P4 Diff", "Diffs the selected item with the depot", 3, 6);
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceRevisionHistoryItem", "P4 Revision History", "Shows the revision history of the selected item", 6, 7);
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceTimeLapseItem", "P4 Time lapse view", "Brings up the timelapse view", 7, 8);
				m_plugin.AddMenuCommand("Easy MDI Document Window", "NiftyPerforceRevertItem", "P4 Revert", "Reverts the item", 4, 9);
				*/

				m_addDelete = new AutoAddDelete( (DTE2)application, m_plugin.OutputPane, m_plugin );
				m_autoCheckout = new AutoCheckout((DTE2)application, m_plugin.OutputPane, m_plugin);
				
				P4Operations.InitThreadHelper();

				Log.DecIndent();
				Log.Debug("Initialized...");
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

