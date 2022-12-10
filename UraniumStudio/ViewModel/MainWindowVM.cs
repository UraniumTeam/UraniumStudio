using System.Windows.Controls;

namespace UraniumStudio.ViewModel;

public class MainWindowVM
{
	public Canvas[] TimelineMarks { get; }

	public MainWindowVM()
	{
		TimelineMarks = TimelineCreator.GetTimelineMarks(2000, 1);
	}
}
