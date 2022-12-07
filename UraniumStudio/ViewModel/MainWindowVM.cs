using System.Windows.Controls;
using UraniumStudio.Data;

namespace UraniumStudio.ViewModel;

public class MainWindowVM
{
	public Canvas[] FunctionsToRender { get; }

	public Canvas[] FunctionNamesToRender { get; }

	public Canvas[] TimelineMarks { get; }
	
	public MainWindowVM()
	{
		FunctionsToRender = Renderer.GetCanvasesArray(Database.Functions).Item1;
		FunctionNamesToRender = Renderer.GetCanvasesArray(Database.Functions).Item2;
		TimelineMarks = Database.Marks.ToArray();
	}
}
