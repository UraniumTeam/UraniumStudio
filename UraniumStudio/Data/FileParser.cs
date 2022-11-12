using System;
using System.Collections.Generic;
using System.Windows.Media;
using UraniumStudio.Model;
using static UraniumStudio.Utilities.Utilities;

namespace UraniumStudio.Data;

public class FileParser
{
	public static readonly List<Function> Functions = new()
	{
		new Function(0, 1, 1000, "F1000", Colors.Aqua),
		new Function(50, 2, 200, "F200", Colors.Red),
		new Function(10, 3, 500, "F500", Colors.Chartreuse),
		new Function(30, 4, 222, "F222", Colors.Blue),
		new Function(40, 5, 100, "F100", Colors.Gray),
		new Function(400, 7, 200, "F200", Colors.Green),
		new Function(300, 6, 400, "F400", Colors.Indigo)
	};

	public static void GenerateRandomFunctions(int count)
	{
		var rnd = new Random();
		for (int i = 0; i < count; i++)
		{
			int length = rnd.Next(10, 10000);
			Functions.Add(
				new Function(
					rnd.Next(10000), rnd.Next(1, 100), length, "F" + length, GetRandomColor()));
		}
	}
}
