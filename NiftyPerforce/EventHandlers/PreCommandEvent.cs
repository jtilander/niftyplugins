// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace Aurora
{
	namespace NiftyPerforce
	{
		public class Dispatcher
		{
			CommandBase mCommand;

			public Dispatcher(CommandBase command)
			{
				mCommand = command;
			}

			public void BeforeCommandExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				mCommand.OnCommand();
			}
		};

		public class PreCommandEvent
		{
			private Plugin mPlugin;
			private List<CommandEvents> mCommandHandlers = new List<CommandEvents>();
			private List<Dispatcher> mDispatchers = new List<Dispatcher>();

			public PreCommandEvent(Plugin plugin)
			{
				mPlugin = plugin;
			}

			public bool RegisterHandler(string commandName, CommandBase command)
			{
				CommandEvents events = mPlugin.FindCommandEvents(commandName);
				if(null == events)
					return false;

				Dispatcher dispatcher = new Dispatcher(command);
				events.BeforeExecute += dispatcher.BeforeCommandExecute;

				if(mCommandHandlers.Contains(events))
					return false;

				mCommandHandlers.Add(events);
				mDispatchers.Add(dispatcher);
				return true;
			}
		}
	}
}
