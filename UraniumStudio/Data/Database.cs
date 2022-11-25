using System.Collections.Generic;
using System.Windows.Media;
using UraniumStudio.Model;

namespace UraniumStudio.Data;

public struct Database
{
	//public static List<Function> Functions { get; set; } = new();
	public static List<Function> Functions = new()
	{
		new Function("F1000", 0, 1, 1000 , Colors.Aqua),
		new Function("F200", 0, 2, 200, Colors.Red),
		new Function("F450", 0, 3, 450, Colors.Green)
	};
}
