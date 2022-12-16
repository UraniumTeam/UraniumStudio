using System.ComponentModel;
using System.Windows.Input;
using UraniumStudio.Data;

namespace UraniumStudio.View;

public partial class StartWindow
{
	public StartWindow()
	{
		InitializeComponent();
	}

	void OpenExisting_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		var dialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "Uranium Profiler Session file|*.ups"
		};
		dialog.ShowDialog();
		if (dialog.FileName == "") return;

		Database.ThreadPaths = JsonParser.ParseUpsJson(dialog.FileName);
		var mainWindow = new MainWindow();
		mainWindow.Closing += MainWindowOnClosing;
		mainWindow.Show();
		Hide();
	}

	void MainWindowOnClosing(object? sender, CancelEventArgs e) => Close();
}
