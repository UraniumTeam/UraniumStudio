using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AvalonDock.Layout;
using UraniumStudio.Model;
using UraniumStudio.ViewModel;

namespace UraniumStudio.View
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public int Counter = 0;

		public MainWindow()
		{
			
			InitializeComponent();

		}

		void AddButton_OnClick(object sender, RoutedEventArgs e)
		{
			var rnd = new Random();

			var f = new Function(
				rnd.Next(50, 250), 0, rnd.Next(50, 1500), "name" + rnd.Next(0, 100), Controller.GetRandomColor());
			//foreach (var f in Controller.Functions)
			//{
			var rectangle = new Rectangle
			{
				Fill = new SolidColorBrush(f.Color),
				Width = f.Length,
				Height = Function.Width,

				RadiusX = 2,
				RadiusY = 2
			};
			var textBlock = new TextBlock()
			{
				Text = f.Name,
				Foreground = Brushes.Black
			};
			var grid = new Grid();
			grid.Margin = new Thickness(2);
			grid.Children.Add(rectangle);
			grid.Children.Add(textBlock);
			TreeView.Items.Add(grid);
			//}
		}

		void File_OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e) =>
			FileButton.ContextMenu!.IsOpen = true;

		void FileOpen_OnPreviewLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var fileDialogWindow = new Microsoft.Win32.OpenFileDialog().ShowDialog();
		}
	}
}
