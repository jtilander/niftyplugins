// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
        class ToolbarCommand<ItemCommandT> : CommandBase
            where ItemCommandT : ItemCommandBase, new()
        {
            public override void OnCommand(DTE2 application, OutputWindowPane pane)
            {
                // Check if we've selected stuff in the solution explorer and we currently have this as the active window.
                if ("Tool" == application.ActiveWindow.Kind &&
                    application.ActiveWindow.Caption.StartsWith("Solution Explorer") &&
                    application.SelectedItems.Count > 0)
                {
                    new ItemCommandT().OnCommand(application, pane);
                }
                // let's just see if the text editor is active
                else if ("Document" == application.ActiveWindow.Kind && application.ActiveDocument != null)
                {
                    new ItemCommandT().OnExecute(null, application.ActiveDocument.FullName, pane);
                }
            }

            public override bool IsEnabled(DTE2 application)
            {
                return
                    ("Tool" == application.ActiveWindow.Kind &&
                    application.ActiveWindow.Caption.StartsWith("Solution Explorer") &&
                    application.SelectedItems.Count > 0) ||
                    ("Document" == application.ActiveWindow.Kind &&
                    application.ActiveDocument != null);
            }
        }
	}
}
