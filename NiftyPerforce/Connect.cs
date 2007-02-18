// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Main stub that interfaces towards Visual Studio.
		public class Connect : IDTExtensibility2, IDTCommandTarget
		{
			public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) { }
			public void OnAddInsUpdate(ref Array custom) { }
			public void OnStartupComplete(ref Array custom) { }

			private Plugin m_plugin = null;
			private AutoAddDelete m_addDelete = null;
			private AutoCheckout m_autoCheckout = null;

			public Connect()
			{
			}

 			public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst_, ref Array custom)
			{
				if( null != m_plugin)
					return;
					
				m_plugin = new Plugin((DTE2)application, (AddIn)addInInst_, "NiftyPerforce", "Aurora.NiftyPerforce.Connect");

                m_plugin.RegisterCommand("NiftyPerforceEdit", new ToolbarCommand<P4EditItem>());
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
                m_plugin.RegisterCommand("NiftyPerforceEditSolution", new P4EditSolution());

                // add the toolbar and menu commands
                CommandBar commandBar = m_plugin.AddCommandBar("NiftyPerforce", MsoBarPosition.msoBarTop);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceEdit", "P4 Edit Current File", "Opens the current document for edit", 1, 1);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceEditAllModified", "P4 Edit All Modified", "Opens all the unsaved, readonly documents for edit", 5, 2);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceDiff", "P4 Diff Current File", "Diffs the current document against the depot", 3, 3);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceRevisionHistory", "P4 Revision History Current File", "Shows the revision history of the current document", 6, 4);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceRevert", "P4 Revert Current File", "Reverts the current document", 4, 5);
                m_plugin.AddToolbarCommand(commandBar, "NiftyPerforceConfiguration", "P4 Configuration", "Opens the configuration dialog", 2, 6);

                m_plugin.AddMenuCommand("Solution", "NiftyPerforceEditSolution", "P4 Edit Solution", "Opens the solution for edit", 1, 6);

                m_plugin.AddMenuCommand("Item", "NiftyPerforceEditItem", "P4 Edit", "Opens the document for edit", 1, 4);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceRenameItem", "P4 Rename", "Renames the item", 1, 5);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceDiffItem", "P4 Diff", "Diffs the selected item with the depot", 3, 6);
                m_plugin.AddMenuCommand("Item", "NiftyPerforceRevisionHistoryItem", "P4 Revision History", "Shows the revision history of the selected item", 6, 7); 
                m_plugin.AddMenuCommand("Item", "NiftyPerforceRevertItem", "P4 Revert", "Reverts the item", 4, 8);

                m_plugin.AddMenuCommand("Project", "NiftyPerforceEditItem", "P4 Edit", "Opens the project for edit", 1, 5);
                m_plugin.AddMenuCommand("Cross Project Multi Project", "NiftyPerforceEditItem", "P4 Edit", "Opens the project for edit", 1, 5);
                m_plugin.AddMenuCommand("Cross Project Multi Item", "NiftyPerforceEditItem", "P4 Edit", "Opens the document for edit", 1, 5);
				m_plugin.AddMenuCommand("Cross Project Multi Item", "NiftyPerforceRevertItem", "P4 Revert", "Reverts the item", 4, 8);

				m_addDelete = new AutoAddDelete( (DTE2)application, m_plugin.OutputPane );
				m_autoCheckout = new AutoCheckout( (DTE2)application, m_plugin.OutputPane );
			}

			public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
			{
                if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone &&
                    m_plugin.CanHandleCommand(commandName))
                {
                    if (m_plugin.IsCommandEnabled(commandName))
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    else
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported;
                }
			}

			public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
			{
				handled = false;
				if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
					return;

				handled = m_plugin.OnCommand(commandName);
			}

			public void OnBeginShutdown(ref Array custom)
			{
				//TODO: Make this thing unregister all the callbacks we've just made... gahhh... C# and destructors... 
			}
		}
	}
}

