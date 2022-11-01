using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using UraniumStudio.Model;

namespace UraniumStudio.ViewModel;

public class Controller
{
	public static List<Function> Functions = new();

	public static void DrawFunction()
	{
		Functions.Add(new Function(0, 0, 50, "name1", GetRandomColor()));
	}

	public static Color GetRandomColor()
	{
		var rnd = new Random();
		return Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
	}
	
	
}
