using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscordRPC;
using Microsoft.Win32;
using SodikmLauncher.Proxy;

namespace SodikmLauncher;

public partial class MainWindow : Window, IComponentConnector, IStyleConnector
{
	private FileSystemWatcher? MapWatcher;

	private FileSystemWatcher? ClientWatcher;

	private DiscordRpcClient? DiscordRPCClient;

	private const string F = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

	public MainWindow()
	{
		InitializeComponent();
		Global.Customisation = new Customisation(this);
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Thread thread = new Thread((ParameterizedThreadStart)delegate
		{
			Server.Start();
		});
		thread.IsBackground = true;
		thread.Start();
		Directory.CreateDirectory(LauncherResources.ClientsPath);
		Directory.CreateDirectory(LauncherResources.MapsPath);
		Directory.CreateDirectory("./data/assetpacks/");
		SplashText.Content = FastSettings.Get<string>("SplashText");
		if (FastSettings.Get<bool>("NewClientAndMapListLogic"))
		{
			InitClientList();
			InitMapList();
			MapWatcher = new FileSystemWatcher();
			foreach (string validMapExtension in Utils.ValidMapExtensions)
			{
				MapWatcher.Filters.Add("*" + validMapExtension);
			}
			MapWatcher.Path = LauncherResources.MapsPath;
			MapWatcher.Created += delegate(object s, FileSystemEventArgs e)
			{
				UpdateMapList(e.FullPath, delete: false);
			};
			MapWatcher.Deleted += delegate(object s, FileSystemEventArgs e)
			{
				UpdateMapList(e.FullPath, delete: true);
			};
			MapWatcher.Renamed += delegate(object s, RenamedEventArgs e)
			{
				UpdateMapList(e.OldFullPath, delete: true);
				UpdateMapList(e.FullPath, delete: false);
			};
			MapWatcher.EnableRaisingEvents = true;
			ClientWatcher = new FileSystemWatcher();
			ClientWatcher.Path = LauncherResources.ClientsPath;
			ClientWatcher.IncludeSubdirectories = false;
			ClientWatcher.Filter = "*.*";
			ClientWatcher.Created += delegate(object s, FileSystemEventArgs e)
			{
				UpdateClientList(e.FullPath, delete: false);
			};
			ClientWatcher.Deleted += delegate(object s, FileSystemEventArgs e)
			{
				UpdateClientList(e.FullPath, delete: true);
			};
			ClientWatcher.Renamed += delegate(object s, RenamedEventArgs e)
			{
				UpdateClientList(e.OldFullPath, delete: true);
				UpdateClientList(e.FullPath, delete: false);
			};
			ClientWatcher.EnableRaisingEvents = true;
		}
		else
		{
			CreateClientList();
			CreateMapList();
			MapWatcher = Utils.CreateFileSystemWatcher(LauncherResources.MapsPath, delegate
			{
				CreateMapList();
			});
		}
		ColourSelection.ItemsSource = BrickColour.RGB;
		if ((DateTime.Today.Day == 1 && DateTime.Today.Month == 4) || FastSettings.Get<bool>("DebugAprilFools"))
		{
			base.Title += " FREE";
			SplashText.Content = "Free Edition (TRIAL EXPIRED)";
			SplashText.Foreground = Brushes.Red;
			SplashText.FontWeight = FontWeights.Bold;
		}
		else
		{
			base.Title += " PREMIUM";
		}
		base.Title += $" [{Assembly.GetExecutingAssembly().GetName().Version}]";
		UsernameBox.Text = Global.Customisation.Settings.Username;
		IDBox.Text = Global.Customisation.Settings.Id.ToString();
		WipeGamePersistenceCheckBox.IsChecked = Global.Customisation.Settings.WipePersistence;
		ProxyRequestsOnlyFromClient.IsChecked = Global.Customisation.Settings.ProxyOnlyRobloxUserAgent;
		Global.Customisation.Load();
		if (!FastSettings.Get<bool>("EnableAssetPacksV1"))
		{
			OpenAssetPackManager.Visibility = Visibility.Hidden;
		}
		DiscordRPCClient = new DiscordRpcClient("1064220338700963931");
		DiscordRPCClient.Initialize();
		DiscordRPCClient.SetPresence(new RichPresence
		{
			Details = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
			Assets = new Assets
			{
				LargeImageKey = "fabricicon3",
				LargeImageText = "Sodikm"
			}
		});
	}

	private void InitClientList()
	{
		string[] directories = Directory.GetDirectories(LauncherResources.ClientsPath);
		foreach (string obj in directories)
		{
			string newItem = obj.Substring(obj.LastIndexOf("\\") + 1);
			if (File.Exists(obj + "/" + LauncherResources.ConfigName))
			{
				ClientList.Items.Add(newItem);
			}
		}
		ClientList.SelectedIndex = 0;
	}

	private void UpdateClientList(string client, bool delete)
	{
		string client2 = client;
		Application.Current.Dispatcher.Invoke(delegate
		{
			string text = client2.Substring(client2.LastIndexOf("\\") + 1);
			if (delete)
			{
				ClientList.Items.Remove(text);
			}
			else if (File.Exists(client2 + "/" + LauncherResources.ConfigName))
			{
				ClientList.Items.Add(text);
			}
		});
	}

	private void InitMapList()
	{
		string[] files = Directory.GetFiles(LauncherResources.MapsPath);
		foreach (string map in files)
		{
			string newItem = map.Substring(map.LastIndexOf("\\") + 1);
			if (Utils.ValidMapExtensions.Any((string x) => map.EndsWith(x)))
			{
				MapList.Items.Add(newItem);
			}
		}
		MapList.SelectedIndex = 0;
	}

	private void UpdateMapList(string map, bool delete)
	{
		string map2 = map;
		Application.Current.Dispatcher.Invoke(delegate
		{
			string text = map2.Substring(map2.LastIndexOf("\\") + 1);
			if (delete)
			{
				MapList.Items.Remove(text);
			}
			else
			{
				MapList.Items.Add(text);
			}
		});
	}

	private void CreateClientList()
	{
		if (FastSettings.Get<bool>("NewClientAndMapListLogic"))
		{
			throw new NotSupportedException("MainWindow.CreateClientList is deprecated and will be removed in a later update");
		}
		ClientList.Items.Clear();
		string[] directories = Directory.GetDirectories(LauncherResources.ClientsPath);
		foreach (string text in directories)
		{
			if (File.Exists(text + "/" + LauncherResources.ConfigName))
			{
				ClientList.Items.Add(text.Substring(text.LastIndexOf("\\") + 1));
			}
		}
		ClientList.SelectedIndex = 0;
	}

	private void CreateMapList()
	{
		if (FastSettings.Get<bool>("NewClientAndMapListLogic"))
		{
			throw new NotSupportedException("MainWindow.CreateMapList is deprecated and will be removed in a later update");
		}
		Application.Current.Dispatcher.Invoke(delegate
		{
			MapList.Items.Clear();
			string[] files = Directory.GetFiles(LauncherResources.MapsPath);
			foreach (string map in files)
			{
				if (Utils.ValidMapExtensions.Any((string x) => map.EndsWith(x)))
				{
					MapList.Items.Add(map.Substring(map.LastIndexOf("\\") + 1));
				}
			}
			MapList.SelectedIndex = 0;
		});
	}

	private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		string text = ClientList.SelectedItem.ToString();
		if (text != null)
		{
			if (!IniParser<ClientSettings>.TryParseFromFile("./data/clients/" + text + "/Sodikm.ini", out var ini))
			{
				MessageBox.Show("Failed to parse \"" + text + "\"'s Sodikm configuration file!", "Sodikm");
				return;
			}
			ClientSettings.Instance.Dispose();
			ClientSettings.Instance = ini;
			ClientSettings.Instance.Year = text;
			ClientSettings.Instance.SetupWatcher();
		}
	}

	private async void JoinButton_Click(object sender, RoutedEventArgs e)
	{
		await Launcher.Play();
	}

	private async void HostButton_Click(object sender, RoutedEventArgs e)
	{
		await Launcher.Host();
	}

	private void StudioButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ClientSettings.Instance.Studio && !ClientSettings.Instance.RobloxApp)
		{
			MessageBox.Show("Selected year has no studio available", "Sodikm");
		}
		else
		{
			Launcher.Launch("", (!ClientSettings.Instance.RobloxApp) ? LaunchType.Studio : LaunchType.Player, useArgs: false);
		}
	}

	private void ColourSelectionItem_MouseDown(object sender, MouseButtonEventArgs e)
	{
		Label label = (Label)sender;
		int key = BrickColour.RGB.ToList().Find((KeyValuePair<int, Brush> x) => x.Value == label.Background).Key;
		Global.Customisation.SelectedColourID = key;
	}

	private void ColourReset_Click(object sender, RoutedEventArgs e)
	{
		Global.Customisation.SetLimbsColourToID(24, 23, 24, 24, 119, 119);
	}

	private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (Global.Customisation != null)
		{
			Global.Customisation.Settings.Username = UsernameBox.Text;
		}
	}

	private void IDBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (Global.Customisation != null)
		{
			if (int.TryParse(IDBox.Text, out var result))
			{
				Global.Customisation.Settings.Id = result;
			}
			else
			{
				IDBox.Text = Global.Customisation.Settings.Id.ToString();
			}
		}
	}

	private void WipeGamePersistenceCheckBox_Click(object sender, RoutedEventArgs e)
	{
		Global.Customisation.Settings.WipePersistence = WipeGamePersistenceCheckBox.IsChecked ?? true;
	}

	private void ImportSOBLButton_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Title = "Load Sodikm Blob",
			Filter = "Sodikm Blob Save|*.sobl"
		};
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		using StreamReader streamReader = new StreamReader((FileStream)openFileDialog.OpenFile());
		using MemoryStream memoryStream = new MemoryStream();
		streamReader.BaseStream.CopyTo(memoryStream);
		MessageBox.Show(GameService.Instance.ImportSOBL(memoryStream.ToArray()) ? "Imported save file" : "Failed to load save file", "Sodikm");
	}

	private void ExportSOBLButton_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Title = "Save Sodikm Blob",
			Filter = "Sodikm Blob Save|*.sobl"
		};
		if (saveFileDialog.ShowDialog() == true)
		{
			using (FileStream fileStream = saveFileDialog.OpenFile() as FileStream)
			{
				fileStream?.Write(GameService.Instance.ExportSOBL());
			}
			MessageBox.Show("Exported save file", "Sodikm");
		}
	}

	private void IPBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		Global.IP = IPBox.Text;
	}

	private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (int.TryParse(PortBox.Text, out var result))
		{
			Global.Port = result;
		}
		else
		{
			PortBox.Text = Global.Port.ToString();
		}
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		Global.Customisation.SaveSettings();
		Server.Stop();
	}

	private void ProxyRequestsFromClient_Click(object sender, RoutedEventArgs e)
	{
		Global.Customisation.Settings.ProxyOnlyRobloxUserAgent = ProxyRequestsOnlyFromClient.IsChecked.GetValueOrDefault();
	}

	private void OpenAssetPackManager_Click(object sender, RoutedEventArgs e)
	{
		new AssetPackManager().ShowDialog();
	}
}
