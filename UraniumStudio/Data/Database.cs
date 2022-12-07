using System.Collections.Generic;
using System.Windows.Controls;
using UraniumStudio.Model;

namespace UraniumStudio.Data;

public struct Database
{
	public static readonly List<Function> Functions = new();

	public static List<Canvas> Marks = new();
}
