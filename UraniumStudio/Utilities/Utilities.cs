using System;
using System.Windows.Media;

namespace UraniumStudio.Utilities;

public class Utilities
{
	public static Color GetRandomColor()
	{
		var rnd = new Random();
		return Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
	}
}