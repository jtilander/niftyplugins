using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
        class P4RevisionHistoryItem : ItemCommandBase
		{
            public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
            {
                P4Operations.RevisionHistoryFile(pane, fileName);
            }
		}
	}
}
