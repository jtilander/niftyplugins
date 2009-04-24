// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	// Holds a dictionary between local command names and the instance that holds 
	// the logic to execute and update the command itself.
	public class CommandRegistry
	{
		private Dictionary<string, CommandBase> mCommands;
		private Plugin mPlugin;
		private CommandBar mCommandBar;

		public CommandRegistry(Plugin plugin, CommandBar commandBar)
		{
			mCommands = new Dictionary<string, CommandBase>();
			mPlugin = plugin;
			mCommandBar = commandBar;
		}

		public void RegisterCommand(string name, bool doBindings, CommandBase commandHandler)
		{
			RegisterCommand(name, doBindings, commandHandler, true);
		}

		public void RegisterCommand(string name, bool doBindings, CommandBase commandHandler, bool onlyToolbar)
		{
			Command command = RegisterCommandPrivate(name, commandHandler, onlyToolbar);

			if(doBindings)
			{
				try
				{
					commandHandler.BindToKeyboard(command);
				}
				catch(ArgumentException e)
				{
					Log.Error("Failed to register keybindings for {0}: {1}", name, e.ToString());
				}
			}

			mCommands.Add(name, commandHandler);
		}
		
		private Command RegisterCommandPrivate(string name, CommandBase commandHandler, bool toolbarOnly)
		{
			Command vscommand = null;

			try
			{
				vscommand = mPlugin.Commands.Item(Absname(name), -1);
				return vscommand;
			}
			catch(System.ArgumentException)
			{
			}

			Log.Info("Registering the command {0} from scratch", name);
			object[] contextGuids = new object[] { };

			int commandStatus = (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled;

			if(0 == commandHandler.IconIndex)
			{
				vscommand = mPlugin.Commands.AddNamedCommand2(mPlugin.AddIn, name, commandHandler.Name, commandHandler.Tooltip, true, -1, ref contextGuids, commandStatus, (int)vsCommandStyle.vsCommandStyleText, vsCommandControlType.vsCommandControlTypeButton);
			}
			else
			{
				int style = (int)vsCommandStyle.vsCommandStylePictAndText;
				if(toolbarOnly)
				{
					style = (int)vsCommandStyle.vsCommandStylePict;
				}
			
				vscommand = mPlugin.Commands.AddNamedCommand2(mPlugin.AddIn, name, commandHandler.Name, commandHandler.Tooltip, false, commandHandler.IconIndex, ref contextGuids, commandStatus, style, vsCommandControlType.vsCommandControlTypeButton);
			}

			// Register the graphics controls for this command as well.
			// First let the command itself have a stab at register whatever it needs.
			// Then by default we always register ourselves in the main toolbar of the application.
			if(!commandHandler.RegisterGUI(vscommand, mCommandBar, toolbarOnly))
			{
			}

			return vscommand;
		}

		public bool Execute(string name_)
		{
			Log.Debug("Trying to execute command \"{0}\"", name_);

			string name = Basename(name_);
			if(mCommands.ContainsKey(name))
			{
				bool dispatched = mCommands[name].OnCommand();
				if(dispatched)
				{
					Log.Debug("{0} was dispatched", name_);
					return true;
				}
			}

			return false;
		}

		public vsCommandStatus Query(string name_)
		{
			Log.Debug("Trying to query command \"{0}\"", name_);

			string name = Basename(name_);
			if(!mCommands.ContainsKey(name))
			{
				Log.Debug("{0} is an unkown command", name_);
				return vsCommandStatus.vsCommandStatusUnsupported;
			}

			CommandBase cmd = mCommands[name];

			if(cmd.IsEnabled())
			{
				Log.Debug("{0} is enabled", name_);
				return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			else
			{
				Log.Debug("{0} is disabled", name_);
				return vsCommandStatus.vsCommandStatusSupported;
			}
		}

		// Converts the incoming name from Visual Studio into the local name.
		private string Basename(string globalname)
		{
			return globalname.Replace(mPlugin.Prefix + ".", "");
		}

		private string Absname(string localname)
		{
			return mPlugin.Prefix + "." + localname;
		}
	}
}
