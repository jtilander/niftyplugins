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
