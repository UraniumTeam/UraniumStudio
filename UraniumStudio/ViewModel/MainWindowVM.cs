using System.Windows.Controls;
using UraniumStudio.Data;

namespace UraniumStudio.ViewModel;

public class MainWindowVM
{
	public Canvas[] FunctionsToRender { get; }

	public MainWindowVM()
	{
		FunctionsToRender = Renderer.GetCanvasesArray(Database.Functions);
	}
}
