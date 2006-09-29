using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	// Wrapper class around registering other classes to handle the actual commands.
	// Interfaces with visual studio and handles the dispatch.
	class Plugin
	{
		public delegate void OnCommandFunction(DTE2 app, OutputWindowPane pane);
		private OutputWindowPane m_outputPane;
		
		private DTE2 m_application;
		private Dictionary<string, OnCommandFunction> m_commands;
		
		public Plugin( DTE2 application, string panelName )
		{
			m_commands = new Dictionary<string, OnCommandFunction>();
			m_application = application;
			m_outputPane = AquireOutputPane(application, panelName);
		}

		public void RegisterCommand(AddIn addIn, string commandName, string toolbar, string itemName, string description, OnCommandFunction callback)
		{
			m_commands.Add(commandName, callback);
			RegisterWithVisual(addIn, commandName, toolbar, itemName, description);
		}

		public bool CanHandleCommand( string name )
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach( string key in m_commands.Keys )
			{
				if( name.EndsWith("." + key) )
					return true;
			}
			
			return false;
		}
		
		public bool OnCommand( string name )
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach (string key in m_commands.Keys)
			{
				if (name.EndsWith("." + key))
				{
					m_commands[key](m_application, m_outputPane);
					return true;
				}
			}
			
			return false;
		}

		private void RegisterWithVisual(AddIn addIn, string commandName, string toolbar, string itemName, string description)
		{
			object[] contextGuids = new object[] { };
			Commands2 commands = (Commands2)m_application.Commands;
			try
			{
				int commandStatus = (int)vsCommandStatus.vsCommandStatusSupported +
									(int)vsCommandStatus.vsCommandStatusEnabled;

				int commandStyle = (int)vsCommandStyle.vsCommandStyleText;
				vsCommandControlType controlType = vsCommandControlType.vsCommandControlTypeButton;

				Command command = commands.AddNamedCommand2(addIn,
												commandName,
												itemName,
												description,
												true,
												59,
												ref contextGuids,
												commandStatus,
												commandStyle,
												controlType);


				if ("" != toolbar)
				{
					command.AddControl(((CommandBars)m_application.CommandBars)[toolbar], 1);
				}
			}
			catch (System.ArgumentException)
			{
			}
		}
		
		private static OutputWindowPane AquireOutputPane( DTE2 app, string name )
		{
			if( "" == name )
				return null;
				
			OutputWindow outputWindow = (OutputWindow)app.Windows.Item(Constants.vsWindowKindOutput).Object;
			OutputWindowPanes panes = outputWindow.OutputWindowPanes;
			
			foreach (OutputWindowPane pane in panes)
			{
				if (name != pane.Name)
					continue;

				return pane;
			}

			return panes.Add(name);
		}
	}
}
