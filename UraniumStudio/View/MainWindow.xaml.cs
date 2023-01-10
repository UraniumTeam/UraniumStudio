using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using UraniumStudio.Data;
using UraniumStudio.ViewModel;
using Point = System.Windows.Point;

namespace UraniumStudio.View;

public partial class MainWindow
{
	private readonly double maxThreadsWidth;
	private Point targetPoint;
	private FrameworkElement? targetElement;
	private UIElement? selectedElement;
	private readonly ScaleTransform globalScaleTransform;

	public MainWindow()
	{
		InitializeComponent();
		var viewModel = new MainWindowViewModel();
		globalScaleTransform = viewModel.GlobalScaleTransform;

		int index = 0;
		double maxThreadHeight = 0;
		var threads = Database.ThreadPaths.Count;
		for (var i = 0; i < threads; i++)
		{
			var canvas = new Canvas { VerticalAlignment = VerticalAlignment.Top };
			var canvasFunc = new Canvas
			{
				RenderTransform = globalScaleTransform
			};
			BindingOperations.SetBinding(
				canvasFunc, ScaleTransform.ScaleXProperty, new Binding("Value") { Source = globalScaleTransform });
			var canvasFuncNames = new Canvas();
			var funcs = Renderer.GetCanvasesArray(Database.Functions[i]).Item1;
			var funcNames = Renderer.GetCanvasesArray(Database.Functions[i]).Item2;
			var funcHeight = Renderer.GetMaxHeightOfThread(funcs);
			maxThreadHeight += funcHeight;

			for (var counter = 0; counter < funcs.Length; counter++)
			{
				canvasFunc.Children.Add(funcs[counter]);
				canvasFuncNames.Children.Add(funcNames[counter]);
			}

			canvas.Children.Add(canvasFunc);
			canvas.Children.Add(canvasFuncNames);

			var threadRowDefinition = new RowDefinition
			{
				Height = new GridLength(funcHeight),
				MinHeight = funcHeight
			};
			var splitterRowDefinition = new RowDefinition { Height = GridLength.Auto, MinHeight = 8 };
			var gridSplitter = new GridSplitter
			{
				Height = 3, Background = Brushes.White, Width = this.Width,
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
			gridSplitter.SetValue(Grid.RowProperty, index);
			index++;
			Grid.SetRowSpan(gridSplitter, 1);

			ThreadsFunctions.Children.Add(canvas);
			ThreadsFunctions.Children.Add(gridSplitter);
		}

		ThreadsFunctions.Children.RemoveAt(ThreadsFunctions.Children.Count - 1);
		maxThreadsWidth = Renderer.GetMaxThreadsWidth(Database.Functions);
	}

	private void File_OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e) =>
		FileButton.ContextMenu!.IsOpen = true;

	private void FileOpen_OnPreviewLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		var dialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "Файлы UPT |*.UPT"
		};
		dialog.ShowDialog();
	}

	private void CanvasItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (selectedElement != null) selectedElement.Effect = null;
		selectedElement = e.OriginalSource as FrameworkElement;
		if (selectedElement is not Rectangle) return;
		selectedElement!.Effect = new DropShadowEffect { Direction = 0, ShadowDepth = 0, Opacity = 10 };

		InfoStackPanel.Children.Clear();
		Stats.Show();
		var element = e.OriginalSource as FrameworkElement;
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function name: " + element?.Name, FontSize = 14 });
		InfoStackPanel.Children.Add(
			new TextBlock { Text = "Function begins at: " + Canvas.GetLeft(element!), FontSize = 14 });
		InfoStackPanel.Children.Add(new TextBlock { Text = "Function length: " + element?.Width, FontSize = 14 });
	}

	private void MainWindow_OnMouseWheel(object sender, MouseWheelEventArgs e)
	{
		const double minScale = 0;
		const double maxScale = 100;
		double scaleMultiplier = (double)1 / 5000 * globalScaleTransform.ScaleX;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if (globalScaleTransform.ScaleX is > minScale and < maxScale)
		{
			globalScaleTransform.ScaleX += direction * absDelta;
		}

		Ruler.MaxValue = maxThreadsWidth;
		Ruler.Width = Ruler.MaxValue * globalScaleTransform.ScaleX;
		for (var i = 0; i < ThreadsFunctions.Children.Count; i++)
		{
			if (ThreadsFunctions.Children[i] is not Canvas) continue;
			var currentThreadCanvas = ThreadsFunctions.Children[i] as Canvas;
			for (var j = 0; j < (currentThreadCanvas!.Children[1] as Canvas)!.Children.Count; j++)
			{
				var name
					= ((currentThreadCanvas.Children[1] as Canvas)!.Children[j] as Canvas)!.Children[0] as
					FrameworkElement;
				var func
					= ((currentThreadCanvas.Children[0] as Canvas)!.Children[j] as Canvas)!.Children[0] as
					FrameworkElement;

				name!.MaxWidth = func!.ActualWidth * globalScaleTransform.ScaleX;
				Canvas.SetLeft(name, Canvas.GetLeft(func) * globalScaleTransform.ScaleX);
			}
		}

		if (globalScaleTransform.ScaleX <= minScale) globalScaleTransform.ScaleX += absDelta;
		if (globalScaleTransform.ScaleX >= maxScale) globalScaleTransform.ScaleX -= absDelta;

		//CentringTransform.CenterX = e.GetPosition(ThreadsFunctions).X;
	}

	private void CanvasFunctionsPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		targetElement = ThreadsFunctions;
		if (targetElement != null)
			targetPoint = e.GetPosition(targetElement);
	}

	private void CanvasFunctionsPanel_OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || targetElement == null) return;
		var pCanvas = e.GetPosition(CanvasFunctionsPanel);

		double xOffset = pCanvas.X - targetPoint.X;
		Canvas.SetLeft(ThreadsFunctions, xOffset);
		Canvas.SetLeft(Ruler, xOffset);
	}

	private void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		targetElement = null;
	}
}