// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
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
		public virtual int IconIndex { get { return -1; } }

		public CommandBase(string name, Plugin plugin, string tooltip)
		{
			mName = name;
			mPlugin = plugin;
			mTooltip = tooltip;
		}

		public virtual bool RegisterGUI(Command vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
		{
			// The default command is registered in the toolbar.
			if(IconIndex >= 0 && toolBarOnly)
				vsCommand.AddControl(vsCommandbar, vsCommandbar.Controls.Count + 1);

			return true;
		}

		public virtual void BindToKeyboard(Command vsCommand)
		{
		}

		public abstract bool OnCommand();	// returns if the command was dispatched or not.
		public abstract bool IsEnabled();	// is the command active?

		protected void _RegisterGUIBar(Command vsCommand, CommandBar vsCommandbar)
		{
			CommandBarControl control = (CommandBarControl)vsCommand.AddControl(vsCommandbar, vsCommandbar.Controls.Count + 1);
		}

		protected void _RegisterGuiContext(Command vsCommand, string name)
		{
			CommandBar b = ((CommandBars)Plugin.App.CommandBars)[name];
			if(null != b)
			{
				CommandBarControl control = (CommandBarControl)vsCommand.AddControl(b, b.Controls.Count + 1);
			}
		}
    }
}
