using System.Collections.Generic;
using UraniumStudio.Model;

namespace UraniumStudio.Data;

public struct Database
{
	public static readonly List<List<Function>> Functions = new();

	public static List<string> ThreadPaths = new();
}
