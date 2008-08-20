// Copyright (C) 2006-2008 Jim Tilander. See COPYING for and README for more details.
using System;
using Microsoft.Win32;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class RegistrySettingsProvider
		{
			public RegistrySettingsProvider()
			{
			}

			public static void SetPropertyValue(string name, string value)
			{
				GetRegKey().SetValue(name, value);
			}

			public static string GetPropertyValue(string name, string defaultValue)
			{
				return (string)GetRegKey().GetValue(name, defaultValue);
			}

			private static RegistryKey GetRegKey()
			{
				RegistryKey regKey;
				regKey = Registry.CurrentUser;
				regKey = regKey.CreateSubKey(GetSubKeyPath());
				return regKey;
			}

			private static string GetSubKeyPath()
			{
				return "Software\\Aurora\\NiftyPerforce\\1.0.0";
			}
		}
	}
}
