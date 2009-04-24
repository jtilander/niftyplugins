// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
    namespace NiftyPerforce
    {
        // an item command is a command associated with selected items in solution explorer
        abstract class ItemCommandBase : CommandBase
        {
            private bool m_executeForFileItems = true;
            private bool m_executeForProjectItems = true;

			protected ItemCommandBase(string name, Plugin plugin, string tooltip, bool executeForFileItems, bool executeForProjectItems)
				: base(name, plugin, tooltip)
            {
				m_executeForFileItems = executeForFileItems;
				m_executeForProjectItems = executeForProjectItems;
			}

            private const string m_fileItemGUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";

            public override bool OnCommand()
            {
                foreach (SelectedItem sel in Plugin.App.SelectedItems)
                {
                    if (m_executeForFileItems && sel.ProjectItem != null && m_fileItemGUID == sel.ProjectItem.Kind)
                        OnExecute(sel, sel.ProjectItem.get_FileNames(0), Plugin.OutputPane);
                    else if (m_executeForProjectItems && sel.Project != null)
						OnExecute(sel, sel.Project.FullName, Plugin.OutputPane);
                }

				return true;
            }

            public override bool IsEnabled()
            {
                return Plugin.App.SelectedItems.Count > 0;
            }

            public abstract void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane);
        }
    }
}
