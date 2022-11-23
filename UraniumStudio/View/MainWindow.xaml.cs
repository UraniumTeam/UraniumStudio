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
		//ScaleTransform.ScaleX = 0.001;
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

		if (Database.Functions.Count > 0)
			Database.Functions.Clear();
		Database.Functions.AddRange(FileParser.ParseFile(dialog.FileName));
		DataContext = new MainWindowVM();
		InfoStackPanel.Children.Clear();
	}

	void CanvasItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (_selectedElement != null) _selectedElement.Effect = null;
		//if (_selectedElement is not Rectangle) return;

		_selectedElement = e.OriginalSource as FrameworkElement;
		if (_selectedElement is not Rectangle) return;
		_selectedElement!.Effect = new DropShadowEffect { Direction = 0, ShadowDepth = 0, Opacity = 10 };

		InfoStackPanel.Children.Clear();
		Stats.Show();
		var element = e.OriginalSource as FrameworkElement;
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function name: " + element?.Name, FontSize = 14 });
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function length: " + element?.Width });
	}

	void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e)
	{
		const double minScale = 0;
		const double maxScale = 10;
		const double scaleMultiplier = (double)1 / 3500;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if ((ScaleTransform.ScaleX > minScale || ScaleTransform.ScaleY > minScale) &&
		    (ScaleTransform.ScaleX < maxScale || ScaleTransform.ScaleY < maxScale))
		{
			ScaleTransform.ScaleX += direction * absDelta;
			//ScaleTransform.ScaleY += direction * absDelta;
		}

		if (ScaleTransform.ScaleX <= minScale || ScaleTransform.ScaleY <= minScale)
		{
			ScaleTransform.ScaleX += absDelta;
			//ScaleTransform.ScaleY += absDelta;
		}

		if (ScaleTransform.ScaleX >= maxScale || ScaleTransform.ScaleY >= maxScale)
		{
			ScaleTransform.ScaleX -= absDelta;
			//ScaleTransform.ScaleY -= absDelta;
		}
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
		double yOffset = pCanvas.Y - _targetPoint.Y;

		if (yOffset <= Canvas.GetTop(CanvasFunctionsPanel)) // Top Border
			yOffset = Canvas.GetTop(CanvasFunctionsPanel);
		//if (xOffset <= Canvas.GetLeft(CanvasFunctionsPanel)) // Left border
		//	xOffset = Canvas.GetLeft(CanvasFunctionsPanel);

		Canvas.SetLeft(_targetElement, xOffset);
		Canvas.SetTop(_targetElement, yOffset);
	}

	void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		_targetElement = null;
	}

}
