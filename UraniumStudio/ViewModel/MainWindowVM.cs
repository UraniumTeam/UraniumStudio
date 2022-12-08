using System.Collections.Generic;
using System.Windows.Controls;
using UraniumStudio.Data;
using UraniumStudio.Model;

namespace UraniumStudio.ViewModel;

public class MainWindowVM
{
	public Canvas[] TimelineMarks { get; }
	public Canvas[]? SourceItemControls { get; }

	public MainWindowVM()
	{
		var threadPaths = Database.ThreadPaths;
		var sourceItemsControl = new List<Canvas>();
		for (int i = 0; i < threadPaths.Count; i++)
		{
			Database.Functions.Add(new List<Function>());
			Database.Functions[i].AddRange(FileParser.ParseFile(threadPaths[i]));
		}

		for (int i = 0; i < threadPaths.Count; i++)
		{
			string thread = threadPaths[i];
			var canvas = new Canvas()
			{

			};
		
			var canvasFunc = new Canvas();
			var canvasFuncNames = new Canvas();

			var funcs = Renderer.GetCanvasesArray(Database.Functions[i]).Item1;
			var funcNames = Renderer.GetCanvasesArray(Database.Functions[i]).Item2;
			foreach (var function in funcs)
				canvasFunc.Children.Add(function);
			foreach (var function in funcNames)
				canvasFuncNames.Children.Add(function);

			sourceItemsControl.Add(canvas);

		}

		SourceItemControls = sourceItemsControl.ToArray();

		TimelineMarks = Database.Marks.ToArray();
	}
}
