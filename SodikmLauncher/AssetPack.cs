using System.Collections.Generic;
using System.Windows.Media;

namespace SodikmLauncher;

internal class AssetPack
{
	[IniVariable("Author")]
	public string Author { get; set; } = "Unknown";


	public string Name { get; set; }

	[IniVariable("Version")]
	public string Version { get; set; } = "1.0";


	[IniVariable("Clients")]
	public string ClientsRaw { get; set; } = "*";


	public string[] Clients => ClientsRaw.Split(';');

	public string ClientsText => string.Join(", ", Extensions.Clone(Clients).Replace("*", "All"));

	public bool Disabled { get; set; }

	public Brush DisplayColour
	{
		get
		{
			if (!Disabled)
			{
				return Brushes.White;
			}
			return Brushes.Red;
		}
	}
}
