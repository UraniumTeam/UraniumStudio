using System.Collections.Generic;
using System.Windows.Controls;
using UraniumStudio.Model;

namespace UraniumStudio.Data;

public struct Database
{
	public static readonly List<List<Function>> Functions = new();

	public static List<Canvas> Marks = new();

	public static List<string> ThreadPaths = new();

	public static ItemsControl[]? SourceItemControls;
}
