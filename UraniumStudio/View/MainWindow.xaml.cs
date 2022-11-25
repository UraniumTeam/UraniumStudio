using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using UraniumStudio.Data;
using UraniumStudio.ViewModel;
using Point = System.Windows.Point;

namespace UraniumStudio.View;

public partial class MainWindow
{
	Point _targetPoint;
	FrameworkElement? _targetElement;
	UIElement? _selectedElement;

	public MainWindow()
	{
		InitializeComponent();
		Canvas.SetLeft(CanvasItems, 0);
		Canvas.SetTop(CanvasItems, 0);
		//ScaleTransform.ScaleX = 0.0000001;
	}

	void File_OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e) =>
		FileButton.ContextMenu!.IsOpen = true;

	void FileOpen_OnPreviewLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		var dialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "Файлы UPT |*.UPT"
		};

		dialog.ShowDialog();
		if (dialog.FileName == "")
			return;
		if (Database.Functions.Count > 0)
			Database.Functions.Clear();
		Database.Functions.AddRange(FileParser.ParseFile(dialog.FileName));
		DataContext = new MainWindowVM();
		InfoStackPanel.Children.Clear();
	}

	void CanvasItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (_selectedElement != null) _selectedElement.Effect = null;
		_selectedElement = e.OriginalSource as FrameworkElement;
		if (_selectedElement is not Rectangle) return;
		_selectedElement!.Effect = new DropShadowEffect { Direction = 0, ShadowDepth = 0, Opacity = 10 };

		InfoStackPanel.Children.Clear();
		Stats.Show();
		var element = e.OriginalSource as FrameworkElement;
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function name: " + element?.Name, FontSize = 14 });
		InfoStackPanel.Children.Add(
			new TextBlock { Text = "Function begins at: " + Canvas.GetLeft(element!), FontSize = 14 });
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function length: " + element?.Width, FontSize = 14 });
	}

	void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e)
	{
		const double minScale = 0;
		const double maxScale = 100;
		double scaleMultiplier = (double)1 / 5000 * ScaleTransform.ScaleX; //= (double)1 / 50000;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if (ScaleTransform.ScaleX is > minScale and < maxScale)
			ScaleTransform.ScaleX += direction * absDelta;
		if (ScaleTransform.ScaleX <= minScale) ScaleTransform.ScaleX += absDelta;
		if (ScaleTransform.ScaleX >= maxScale) ScaleTransform.ScaleX -= absDelta;

		ScaleTransform.CenterX = e.GetPosition(CanvasItems).X;
	}

	void CanvasFunctionsPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		_targetElement = CanvasItems;
		if (_targetElement != null)
			_targetPoint = e.GetPosition(_targetElement);
	}

	void CanvasFunctionsPanel_OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || _targetElement == null) return;
		var pCanvas = e.GetPosition(CanvasFunctionsPanel);

		double xOffset = pCanvas.X - _targetPoint.X;

		//if (yOffset <= Canvas.GetTop(CanvasFunctionsPanel)) // Top Border  
		//	yOffset = Canvas.GetTop(CanvasFunctionsPanel);					 
		//if (xOffset <= Canvas.GetLeft(CanvasFunctionsPanel)) // Left border
		//	xOffset = Canvas.GetLeft(CanvasFunctionsPanel);

		Canvas.SetLeft(_targetElement, xOffset);
	}

	void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		_targetElement = null;
	}
}
