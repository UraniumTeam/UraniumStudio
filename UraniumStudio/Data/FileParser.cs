using System.Collections.Generic;
using System.Windows.Media;
using UraniumStudio.Model;

namespace UraniumStudio.Data;

public class FileParser
{
	public readonly List<Function> Functions = new List<Function>
	{
		new Function(0, 1, 1000, "F1000", Colors.Aqua),
		new Function(50, 2, 200, "F200", Colors.Red),
		new Function(10, 3, 500, "F500", Colors.Chartreuse),
		new Function(30, 4, 222, "F222", Colors.Blue),
		new Function(40, 5, 100, "F100", Colors.Gray),
		new Function(100, 10, 200, "F200", Colors.Green),
		new Function(300, 6, 400, "F400", Colors.Indigo)
	};
}
