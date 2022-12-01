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
		Canvas.SetLeft(CanvasFunctions, 0);
		Canvas.SetTop(CanvasFunctions, 0);
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
		if (dialog.FileName == "") return;
		if (Database.Functions.Count > 0)
			Database.Functions.Clear();
		Database.Functions.AddRange(FileParser.ParseFile(dialog.FileName));
		/**
		 * TODO сделать точку входа
		 */
		//var mainPanel = new LayoutDocumentPane {Children = {new LayoutDocument{Content = new Canvas().Children.Add()} }};
		//MainPanel = mainPanel;
		
		DataContext = new MainWindowVM();
		InfoStackPanel.Children.Clear();
	}

	void CanvasItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (_selectedElement != null) _selectedElement.Effect = null;
		_selectedElement = e.OriginalSource as FrameworkElement;
		if (_selectedElement is not Rectangle) return;
		_selectedElement!.Effect = new DropShadowEffect { Direction = 0, ShadowDepth = 0, Opacity = 10 };

		/**
		 * TODO сделать таблицу данных о функции
		 */
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
		double scaleMultiplier = (double)1 / 5000 * FuncScaleTransform.ScaleX; //= (double)1 / 50000;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if (FuncScaleTransform.ScaleX is > minScale and < maxScale)
		{
			FuncScaleTransform.ScaleX += direction * absDelta;
			for (int i = 0; i < CanvasFunctionNames.Items.Count; i++)
			{
				var name = (CanvasFunctionNames.Items[i] as Canvas)!.Children[0] as FrameworkElement;
				var func = (CanvasFunctions.Items[i] as Canvas)!.Children[0] as FrameworkElement;
				name!.MaxWidth = func!.ActualWidth * FuncScaleTransform.ScaleX;
				Canvas.SetLeft(name, Canvas.GetLeft(func) * FuncScaleTransform.ScaleX);
			}
		}

		if (FuncScaleTransform.ScaleX <= minScale) FuncScaleTransform.ScaleX += absDelta;
		if (FuncScaleTransform.ScaleX >= maxScale) FuncScaleTransform.ScaleX -= absDelta;

		//FuncScaleTransform.CenterX = e.GetPosition(CanvasItems).X; // CanvasItems
	}

	void CanvasFunctionsPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		_targetElement = CanvasFunctions; // CanvasItems
		if (_targetElement != null)
			_targetPoint = e.GetPosition(_targetElement);
	}

	void CanvasFunctionsPanel_OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || _targetElement == null) return;
		var pCanvas = e.GetPosition(CanvasFunctionsPanel); // CanvasFunctionPanel

		double xOffset = pCanvas.X - _targetPoint.X * FuncScaleTransform.ScaleX; // * FuncScaleTransform.ScaleX

		//if (yOffset <= Canvas.GetTop(CanvasFunctionsPanel)) // Top Border  
		//	yOffset = Canvas.GetTop(CanvasFunctionsPanel);					 
		//if (xOffset <= Canvas.GetLeft(CanvasFunctionsPanel)) // Left border
		//	xOffset = Canvas.GetLeft(CanvasFunctionsPanel);

		Canvas.SetLeft(CanvasFunctions, xOffset);
		Canvas.SetLeft(CanvasFunctionNames, xOffset);
	}

	void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		_targetElement = null;
	}
}
