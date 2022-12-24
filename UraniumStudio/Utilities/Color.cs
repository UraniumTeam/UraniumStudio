using System;

namespace UraniumStudio.Utilities;

public static class Color
{
	public static System.Windows.Media.Color GetRandomColor()
	{
		var rnd = new Random();
		return System.Windows.Media.Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
	}
}
