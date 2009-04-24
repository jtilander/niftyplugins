// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	// Wrapper class around registering other classes to handle the actual commands.
	// Interfaces with visual studio and handles the dispatch.
	public class Plugin
	{
		private DTE2 m_application;
		private AddIn m_addIn;
		private OutputWindowPane m_outputPane;

		private Dictionary<string, CommandBase> m_commands = new Dictionary<string, CommandBase>();
		private string m_connectPath;
		private object mOptions;

		public OutputWindowPane OutputPane { get { return m_outputPane; } }
		public string Prefix { get { return m_connectPath; } }
		public AddIn AddIn { get { return m_addIn; } }
		public DTE2 App	{ get { return m_application; }	}
		public Commands2 Commands {	get { return (Commands2)m_application.Commands; }}
		public object Options { get { return mOptions; } set { mOptions = value; } }

		public Plugin(DTE2 application, AddIn addIn, string panelName, string connectPath, object options)
		{
			// TODO: This can be figured out from traversing the assembly and locating the Connect class...
			m_connectPath = connectPath;

			m_application = application;
			m_addIn = addIn;
			m_outputPane = AquireOutputPane(application, panelName);
			mOptions = options;
		}

		public void RegisterCommand(string commandName, CommandBase command)
		{
			m_commands.Add(commandName, command);
		}

		public bool CanHandleCommand(string commandName)
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach(string key in m_commands.Keys)
			{
				if(commandName.EndsWith("." + key))
					return true;
			}

			return false;
		}

		public bool IsCommandEnabled(string commandName)
		{
			foreach(string key in m_commands.Keys)
			{
				if(commandName.EndsWith("." + key))
					return m_commands[key].IsEnabled();
			}

			return false;
		}

		public bool OnCommand(string name)
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach(string key in m_commands.Keys)
			{
				if(name.EndsWith("." + key))
				{
					m_commands[key].OnCommand();
					return true;
				}
			}

			return false;
		}

		public CommandEvents FindCommandEvents(string commandName)
		{
			CommandEvents events = null;
			try
			{
				Command command = App.DTE.Commands.Item(commandName, -1);
				if(command != null)
					events = App.DTE.Events.get_CommandEvents(command.Guid, command.ID);
			}
			catch
			{
			}
			return events;
		}

		private Command GetCommand(string commandName)
		{
			Commands2 commands = (Commands2)m_application.Commands;

			string fullName = m_connectPath + "." + commandName;

			try
			{
				Command command = commands.Item(fullName, 0);
				return command;
			}
			catch(System.ArgumentException)
			{
				return null;
			}
		}

		private bool IsCommandRegistered(string commandName)
		{
			return GetCommand(commandName) != null;
		}

		public static bool HasCommand(CommandBar commandBar, string caption)
		{
			foreach(CommandBarControl control in commandBar.Controls)
			{
				if(control.Caption == caption)
					return true;
			}
			return false;
		}

		public CommandBar AddCommandBar(string name, MsoBarPosition position)
		{
			CommandBars cmdBars = (Microsoft.VisualStudio.CommandBars.CommandBars)m_application.CommandBars;
			CommandBar bar = null;

			try
			{
				try
				{
					// Create the new CommandBar
					bar = cmdBars.Add(name, position, false, false);
					bar.Visible = true;
				}
				catch(ArgumentException)
				{
					// Try to find an existing CommandBar
					bar = cmdBars[name];
				}
			}
			catch
			{
			}

			return bar;
		}

		private static OutputWindowPane AquireOutputPane(DTE2 app, string name)
		{
			if("" == name)
				return null;

			OutputWindowPane result = FindOutputPane(app, name);
			if(null != result)
				return result;

			OutputWindow outputWindow = (OutputWindow)app.Windows.Item(Constants.vsWindowKindOutput).Object;
			OutputWindowPanes panes = outputWindow.OutputWindowPanes;
			return panes.Add(name);
		}

		public static OutputWindowPane FindOutputPane(DTE2 app, string name)
		{
			if("" == name)
				return null;

			OutputWindow outputWindow = (OutputWindow)app.Windows.Item(Constants.vsWindowKindOutput).Object;
			OutputWindowPanes panes = outputWindow.OutputWindowPanes;

			foreach(OutputWindowPane pane in panes)
			{
				if(name != pane.Name)
					continue;

				return pane;
			}

			return null;
		}

		public static void AddKeyboardBinding(Command command, string binding)
		{
			command.Bindings = binding;
		}
	}
}
