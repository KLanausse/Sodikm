using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace SodikmLauncher;

internal class AssetPackHandler
{
	public static List<AssetPack> AssetPacks;

	public static List<string> GetFilesFromAssetPacks(long id)
	{
		List<string> list = new List<string>();
		foreach (AssetPack assetPack in AssetPacks)
		{
			if (assetPack.Disabled)
			{
				continue;
			}
			string[] clients = assetPack.Clients;
			if (!clients.Contains("!" + ClientSettings.Instance.Year) && (clients.Contains("*") || clients.Contains(ClientSettings.Instance.Year)))
			{
				string path = "./data/assetpacks/" + assetPack.Name + "/";
				if (!Directory.Exists(path))
				{
					AssetPacks.Remove(assetPack);
					continue;
				}
				list.AddRange(Directory.GetFiles(path, $"{id}.*"));
			}
		}
		return list;
	}

	public static void ParseAssetPacks()
	{
		AssetPacks.Clear();
		string[] directories = Directory.GetDirectories("./data/assetpacks/");
		foreach (string text in directories)
		{
			string assetPackName = Path.GetFileName(text);
			if (File.Exists(text + "/SodikmAssetPack.ini") && IniParser<AssetPack>.TryParseFromFile(text + "/SodikmAssetPack.ini", out var ini))
			{
				if (assetPackName.EndsWith(".disabled"))
				{
					string text2 = assetPackName;
					assetPackName = text2.Substring(0, text2.Length - 9);
					ini.Disabled = true;
				}
				if (AssetPacks.Any((AssetPack x) => x.Name == assetPackName))
				{
					throw new Exception("Duplicate AssetPack name");
				}
				ini.Name = assetPackName;
				AssetPacks.Add(ini);
			}
		}
	}

	public static void ToggleAssetPack(AssetPack assetPack)
	{
		try
		{
			if (assetPack.Disabled)
			{
				Directory.Move("./data/assetpacks/" + assetPack.Name + ".disabled/", "./data/assetpacks/" + assetPack.Name + "/");
				assetPack.Disabled = false;
			}
			else
			{
				Directory.Move("./data/assetpacks/" + assetPack.Name + "/", "./data/assetpacks/" + assetPack.Name + ".disabled/");
				assetPack.Disabled = true;
			}
		}
		catch (Exception value)
		{
			MessageBox.Show($"Failed to toggle {assetPack.Name}: {value}", "Sodikm Awesome Exception Catcher");
		}
	}

	static AssetPackHandler()
	{
		AssetPacks = new List<AssetPack>();
		Directory.CreateDirectory("./data/assetpacks/");
		ParseAssetPacks();
	}
}
