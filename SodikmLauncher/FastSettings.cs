using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SodikmLauncher;

internal class FastSettings
{
	private static Dictionary<string, object> Variables;

	public static T Get<T>(string name)
	{
		return (T)Variables[name];
	}

	private static void Set<T>(string name, T val)
	{
		if (Variables.ContainsKey(name) && Variables[name].GetType() == val?.GetType())
		{
			Variables[name] = val;
		}
	}

	private static void Register<T>(string name, T def)
	{
		Variables.Add(name, def);
	}

	private static void LoadSettings()
	{
		string path = "./data/flags.json";
		if (!File.Exists(path))
		{
			return;
		}
		Dictionary<string, JsonElement> dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path));
		if (dictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<string, JsonElement> item in dictionary)
		{
			string key = item.Key;
			JsonElement value = item.Value;
			if (key.StartsWith("FFlag"))
			{
				string text = key;
				Set(text.Substring(5, text.Length - 5), value.GetBoolean());
			}
			else if (key.StartsWith("FInt"))
			{
				string text = key;
				Set(text.Substring(4, text.Length - 4), value.GetInt32());
			}
			else if (key.StartsWith("FString"))
			{
				string text = key;
				Set(text.Substring(7, text.Length - 7), value.GetString());
			}
		}
	}

	static FastSettings()
	{
		Variables = new Dictionary<string, object>();
		Register("DebugAprilFools", def: false);
		Register("EnableAssetPacksV1", def: true);
		Register("NewClientAndMapListLogic", def: true);
		Register("SplashText", "The Future™");
		LoadSettings();
	}
}
