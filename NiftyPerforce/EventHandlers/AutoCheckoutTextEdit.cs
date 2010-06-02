// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class AutoCheckoutTextEdit : PreCommandFeature
		{
			private EnvDTE80.TextDocumentKeyPressEvents mTextDocEvents;
			private EnvDTE.TextEditorEvents mTextEditorEvents;

			public AutoCheckoutTextEdit(Plugin plugin)
				: base(plugin, "AutoCheckoutTextEdit", "Automatically checks out the text file on edits")
			{
				if(!Singleton<Config>.Instance.autoCheckoutOnEdit)
					return;

				Log.Info("Adding handlers for automatically checking out text files as you edit them");
				mTextDocEvents = ((EnvDTE80.Events2)plugin.App.Events).get_TextDocumentKeyPressEvents(null);
				mTextDocEvents.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(OnBeforeKeyPress);

				mTextEditorEvents = ((EnvDTE80.Events2)plugin.App.Events).get_TextEditorEvents(null);
				mTextEditorEvents.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(OnLineChanged);

				RegisterHandler("Edit.Delete", OnCheckoutCurrentDocument);
				RegisterHandler("Edit.DeleteBackwards", OnCheckoutCurrentDocument);
				RegisterHandler("Edit.Paste", OnCheckoutCurrentDocument);
			}

			private void OnBeforeKeyPress(string Keypress, EnvDTE.TextSelection Selection, bool InStatementCompletion, ref bool CancelKeypress)
			{
				if(mPlugin.App.ActiveDocument.ReadOnly)
					P4Operations.EditFile(mPlugin.OutputPane, mPlugin.App.ActiveDocument.FullName, false); // no auto p4 edit on writeable files here since it will be too costly.
			}

			// [jt] This handler checks for things like paste operations. In theory we should be able to remove the handler above, but
			// I can't get this one to fire reliably... Wonder how much these handlers will slow down the IDE?
			private void OnLineChanged(TextPoint StartPoint, TextPoint EndPoint, int Hint)
			{
				if((Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedMultiLine) == 0 &&
					(Hint & (int)vsTextChanged.vsTextChangedNewline) == 0 &&
					(Hint != 0))
					return;
				if(mPlugin.App.ActiveDocument.ReadOnly && !mPlugin.App.ActiveDocument.Saved)
					P4Operations.EditFile(mPlugin.OutputPane, mPlugin.App.ActiveDocument.FullName, false); // no auto p4 edit on writeable files here since it will be too costly.
			}

			private void OnCheckoutCurrentDocument(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
			{
				if(mPlugin.App.ActiveDocument.ReadOnly && !mPlugin.App.ActiveDocument.Saved)
					P4Operations.EditFile(mPlugin.OutputPane, mPlugin.App.ActiveDocument.FullName, false); // no auto p4 edit on writeable files here since it will be too costly.
			}

		}
	}
}
