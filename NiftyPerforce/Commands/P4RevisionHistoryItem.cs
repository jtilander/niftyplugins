using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class P4DiffItem : ItemCommandBase
		{
            public override void OnExecute(SelectedItem item, string fileName, OutputWindowPane pane)
            {
                P4Operations.DiffFile(pane, fileName);
            }
		}
	}
}
