// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	public abstract class CommandBase
    {
		private readonly Plugin mPlugin;
		private readonly string mName;
		private readonly string mTooltip;

		public Plugin Plugin	{ get { return mPlugin; } }
		public string Name		{ get { return mName; } }
		public string Tooltip	{ get { return mTooltip; } }
		virtual public int IconIndex { get { return -1; } }

		public CommandBase(string name, Plugin plugin, string tooltip)
		{
			mName = name;
			mPlugin = plugin;
			mTooltip = tooltip;
		}

		virtual public bool RegisterGUI(Command vsCommand, CommandBar vsCommandbar)
		{
			// The default command is registered in the toolbar.
			// pluginCommandbar;

			return false;
		}

		virtual public void BindToKeyboard(Command vsCommand)
		{
		}

		abstract public bool OnCommand();	// returns if the command was dispatched or not.
        abstract public bool IsEnabled();	// is the command active?
    }
}
