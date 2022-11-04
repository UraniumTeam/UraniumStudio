using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UraniumStudio.View
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		void File_OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e) =>
			FileButton.ContextMenu!.IsOpen = true;

		void FileOpen_OnPreviewLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			new Microsoft.Win32.OpenFileDialog().ShowDialog();
		}

		void CanvasItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var elementName = (e.OriginalSource as FrameworkElement)?.Name;
			MessageBox.Show(elementName);
		}
	}
}
