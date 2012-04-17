using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Aurora
{
	namespace NiftySolution
	{
		// TODO: How do we handle different versions of this XML file on disk?
		[Serializable]
		public class Options
		{
			private bool mDirty = false;
			private bool mEnableBindings = false;
            private bool mSilentDebuggerExceptions = false;
			private bool mIgnoreDebuggerExceptions = false;

			private string mExtraSearchDirectories = "";
			private string mCompileTimeLogFile = "";

			[XmlIgnore]
			public string mFileName = "";

			[XmlIgnore]
			[BrowsableAttribute(false)]
			public bool Dirty
			{
				get 
				{ 
					return mDirty; 
				}
			}

			[Category("General"), Description("Register Key Bindings")]
			public bool EnableBindings
			{
				get
				{
					return mEnableBindings;
				}

				set
				{
					mEnableBindings = value;
					mDirty = true;
				}
			}

            [Category("General"), Description("Exceptions in debugger behaves as in VC6")]
            public bool SilentDebuggerExceptions
            {
                get
                {
                    return mSilentDebuggerExceptions;
                }

                set
                {
                    mSilentDebuggerExceptions = value;
                    mDirty = true;
                }
            }

            [Category("General"), Description("All exceptions in debugger are ignored")]
            public bool IgnoreDebuggerExceptions
            {
                get
                {
                    return mIgnoreDebuggerExceptions;
                }

                set
                {
                    mIgnoreDebuggerExceptions = value;
                    mDirty = true;
                }
            }

			[Category("NiftyOpen"), Description("Also add all the files underneath these ; delimited directories")]
			public string ExtraSearchDirs
			{
				get { return mExtraSearchDirectories; }
				set { mExtraSearchDirectories = value; mDirty = true; }
			}

            [Category("Productivity"), Description("Log the compilation time to a separate log")]
            public string CompileTimeLogFile
            {
                get
                {
                    return mCompileTimeLogFile;
                }

                set
                {
                    mCompileTimeLogFile = value;
                    mDirty = true;
                }
            }

			public static Options Load(string filename)
			{
				Options o;
				if(System.IO.File.Exists(filename))
				{
					o = File.LoadXML<Options>(filename);
				}
				else
				{
					o = new Options();
				}
				o.mFileName = filename;
				return o;
			}

			public void Save()
			{
				if(mDirty)
				{
					File.SaveXML<Options>(mFileName, this);
					mDirty = false;
				}
			}
		}
	}
}
