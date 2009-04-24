// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class FindEvents
		{
			private Plugin mPlugin;
			private CommandEvents mCommandEvents;

			public FindEvents(Plugin plugin)
			{
				mPlugin = plugin;
				mCommandEvents = mPlugin.App.DTE.Events.get_CommandEvents(null, 0);
				mCommandEvents.BeforeExecute += BeforeCommandExecute;
			}

			void BeforeCommandExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				try
				{
					Command command = mPlugin.App.DTE.Commands.Item(Guid, ID);
					if(command == null)
						return;

					string commandName = command.Name.Length > 0 ? command.Name : "<unnamed>";

					Log.Info("Command: {0} (GUID: {1}, ID: {2})", commandName, Guid, ID);
				}
				catch
				{
				}
			}
		}
	}
}
