using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SodikmLauncher;

public partial class AssetPackManager : Window, IComponentConnector
{
	public AssetPackManager()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		AssetPackHandler.ParseAssetPacks();
		if (AssetPackHandler.AssetPacks.Count() < 1)
		{
			MessageBox.Show("Please install an asset pack to use the asset pack manager.", "Sodikm");
			Close();
		}
		else
		{
			AssetPackList.ItemsSource = AssetPackHandler.AssetPacks;
			AssetPackList.SelectedIndex = 0;
		}
	}

	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		AssetPackHandler.ParseAssetPacks();
		AssetPackList.Items.Refresh();
	}

	private void AssetPackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (AssetPackList.SelectedItem is AssetPack assetPack)
		{
			Name.Text = assetPack.Name;
			Author.Text = assetPack.Author;
			Version.Text = assetPack.Version;
			Years.Text = assetPack.ClientsText;
			DisabledText.Visibility = ((!assetPack.Disabled) ? Visibility.Hidden : Visibility.Visible);
		}
	}

	private void ToggleButton_Click(object sender, RoutedEventArgs e)
	{
		if (AssetPackList.SelectedItem is AssetPack assetPack)
		{
			AssetPackHandler.ToggleAssetPack(assetPack);
			AssetPackList.Items.Refresh();
			AssetPackList_SelectionChanged(null, null);
		}
	}
}
