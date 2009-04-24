// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace Aurora
{
	public class EventBase
	{
		private string mName;
		private Plugin mPlugin;

		public EventBase(string name, Plugin plugin)
		{
			mName = name;
			mPlugin = plugin;
		}
	}
	
	// Holds a dictionary between local command names and the instance that holds 
	// the logic to execute and update the command itself.
	public class EventRegistry
	{
		private Dictionary<string, EventBase> mEvents;
		private Plugin mPlugin;

		public EventRegistry(Plugin plugin)
		{
			mEvents = new Dictionary<string, EventBase>();
			mPlugin = plugin;
		}
	}
}
