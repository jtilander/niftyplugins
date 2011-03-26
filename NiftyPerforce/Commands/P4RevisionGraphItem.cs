// Copyright (C) 2006-2010 Jim Tilander. See COPYING for and README for more details.
using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
    namespace NiftyPerforce
    {
        class P4RevisionGraphItem : ItemCommandBase
        {
            public P4RevisionGraphItem(Plugin plugin)
                : base("P4RevisionGraphItem", plugin, "Shows the revision graph for an item", true, true)
            {
            }

            override public int IconIndex { get { return 9; } }

            public override bool RegisterGUI(Command vsCommand, CommandBar vsCommandbar, bool toolBarOnly)
            {
                if (toolBarOnly)
                {
                    _RegisterGUIBar(vsCommand, vsCommandbar);
                }
                else
                {
                    _RegisterGuiContext(vsCommand, "Project");
                    _RegisterGuiContext(vsCommand, "Item");
                    _RegisterGuiContext(vsCommand, "Easy MDI Document Window");
                }
                return true;
            }

            public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
            {
                P4Operations.RevisionGraph(pane, fileName);
            }
        }
    }
}
