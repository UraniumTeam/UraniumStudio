using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using UraniumStudio.Data;
using UraniumStudio.Model;
using UraniumStudio.ViewModel;
using Point = System.Windows.Point;

namespace UraniumStudio.View;

public partial class MainWindow
{
	readonly double _maxThreadsWidth;
	Point _targetPoint;
	FrameworkElement? _targetElement;
	UIElement? _selectedElement;
	readonly ScaleTransform _globalScaleTransform;

	public MainWindow()
	{
		InitializeComponent();
		var viewModel = new MainWindowViewModel();
		_globalScaleTransform = viewModel.GlobalScaleTransform;

		int index = 0;
		double maxThreadHeight = 0;
		var threadPaths = Database.ThreadPaths;
		for (int i = 0; i < threadPaths.Count; i++)
		{
			Database.Functions.Add(new List<Function>());
			Database.Functions[i].AddRange(FileParser.ParseFile(threadPaths[i]));
		}

		for (int i = 0; i < threadPaths.Count; i++)
		{
			string thread = threadPaths[i];
			var canvas = new Canvas { VerticalAlignment = VerticalAlignment.Top };
			var canvasFunc = new Canvas
			{
				RenderTransform = _globalScaleTransform
			};
			BindingOperations.SetBinding(
				canvasFunc, ScaleTransform.ScaleXProperty, new Binding("Value") { Source = _globalScaleTransform });
			var canvasFuncNames = new Canvas();

			var funcs = Renderer.GetCanvasesArray(Database.Functions[i]).Item1;
			var funcNames = Renderer.GetCanvasesArray(Database.Functions[i]).Item2;
			maxThreadHeight += Renderer.GetMaxHeightOfThread(funcs);
			foreach (var function in funcs)
			{
				canvasFunc.Children.Add(function);
				Canvas.SetTop(canvasFunc, maxThreadHeight * i);
			}

			foreach (var function in funcNames)
			{
				canvasFuncNames.Children.Add(function);
				Canvas.SetTop(canvasFuncNames, maxThreadHeight * i);
			}

			canvas.Children.Add(canvasFunc);
			canvas.Children.Add(canvasFuncNames);
			double funcHeight = Renderer.GetMaxHeightOfThread(funcs);
			var threadRowDefinition = new RowDefinition
				{ Height = new GridLength(funcHeight), MinHeight = funcHeight };
			var splitterRowDefinition = new RowDefinition { Height = GridLength.Auto, MinHeight = 8 };
			var gridSplitter = new GridSplitter
			{
				Height = 3, Background = Brushes.White, Width = this.Width, //
				HorizontalAlignment = HorizontalAlignment.Stretch,
				ShowsPreview = false, ResizeDirection = GridResizeDirection.Rows,
				VerticalAlignment = VerticalAlignment.Center
			};
			BindingOperations.SetBinding(
				gridSplitter, WidthProperty, new Binding("Width") { Source = Ruler });
			Canvas.SetTop(gridSplitter, maxThreadHeight * i);
			ThreadsFunctions.RowDefinitions.Add(threadRowDefinition);
			ThreadsFunctions.RowDefinitions.Add(splitterRowDefinition);

			canvas.SetValue(Grid.RowProperty, index);
			index++;
			//if splitters created
			gridSplitter.SetValue(Grid.RowProperty, index);
			index++;
			//
			Grid.SetRowSpan(gridSplitter, 1);

			ThreadsFunctions.Children.Add(canvas);
			ThreadsFunctions.Children.Add(gridSplitter);
		}

		// if splitters created
		ThreadsFunctions.Children.RemoveAt(ThreadsFunctions.Children.Count - 1);
		_maxThreadsWidth = Renderer.GetMaxThreadsWidth(Database.Functions);
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
		double scaleMultiplier = (double)1 / 5000 * _globalScaleTransform.ScaleX;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if (_globalScaleTransform.ScaleX is > minScale and < maxScale)
		{
			_globalScaleTransform.ScaleX += direction * absDelta;
		}

		Ruler.MaxValue = _maxThreadsWidth;
		Ruler.Width = _maxThreadsWidth * _globalScaleTransform.ScaleX;
		for (int i = 0; i < ThreadsFunctions.Children.Count; i++)
		{
			if (ThreadsFunctions.Children[i] is not Canvas) continue;
			var currentThreadCanvas = ThreadsFunctions.Children[i] as Canvas;
			for (int j = 0; j < (currentThreadCanvas!.Children[1] as Canvas)!.Children.Count; j++)
			{
				var name
					= ((currentThreadCanvas.Children[1] as Canvas)!.Children[j] as Canvas)!.Children[0] as
					FrameworkElement;
				var func
					= ((currentThreadCanvas.Children[0] as Canvas)!.Children[j] as Canvas)!.Children[0] as
					FrameworkElement;

				name!.MaxWidth = func!.ActualWidth * _globalScaleTransform.ScaleX;
				Canvas.SetLeft(name, Canvas.GetLeft(func) * _globalScaleTransform.ScaleX);
			}
		}

		if (_globalScaleTransform.ScaleX <= minScale) _globalScaleTransform.ScaleX += absDelta;
		if (_globalScaleTransform.ScaleX >= maxScale) _globalScaleTransform.ScaleX -= absDelta;

		//CentringTransform.CenterX = e.GetPosition(ThreadsFunctions).X;
	}

	void CanvasFunctionsPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		_targetElement = ThreadsFunctions;
		if (_targetElement != null)
			_targetPoint = e.GetPosition(_targetElement);
	}

	void CanvasFunctionsPanel_OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || _targetElement == null) return;
		var pCanvas = e.GetPosition(CanvasFunctionsPanel);

		double xOffset = pCanvas.X - _targetPoint.X;
		Canvas.SetLeft(ThreadsFunctions, xOffset);
		Canvas.SetLeft(Ruler, xOffset);
	}

	void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		_targetElement = null;
	}
}
