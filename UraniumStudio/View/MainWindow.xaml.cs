using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using UraniumStudio.Data;
using UraniumStudio.Model;
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
		if (Database.Marks.Count > 0)
			Database.Marks.Clear();

		//Database.Functions.AddRange(FileParser.ParseFile(dialog.FileName));
		//Database.Marks = TimelineCreator.GetTimelineMarks(
		//		Database.Functions.Last().StartPosX + Database.Functions.Last().Length, GlobalScaleTransform.ScaleX).ToList();

		//TODO сделать точку входа

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

		//TODO сделать таблицу данных о функции

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
		double scaleMultiplier = (double)1 / 5000 * GlobalScaleTransform.ScaleX; //= (double)1 / 50000;

		double absDelta = Math.Abs(e.Delta * scaleMultiplier);
		sbyte direction = e.Delta switch
		{
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};

		if (GlobalScaleTransform.ScaleX is > minScale and < maxScale)
		{
			GlobalScaleTransform.ScaleX += direction * absDelta;
			for (int i = 0; i < CanvasFunctionNames.Items.Count; i++)
			{
				var name = (CanvasFunctionNames.Items[i] as Canvas)!.Children[0] as FrameworkElement;
				var func = (CanvasFunctions.Items[i] as Canvas)!.Children[0] as FrameworkElement;
				name!.MaxWidth = func!.ActualWidth * GlobalScaleTransform.ScaleX;

				Canvas.SetLeft(name, Canvas.GetLeft(func) * GlobalScaleTransform.ScaleX);

				/*if (i < TimelineMarks.Items.Count)
				{
					var mark = (TimelineMarks.Items[i] as Canvas)!.Children[0] as Line;
					Canvas.SetLeft(mark,  10 * FuncScaleTransform.ScaleX);
				}*/
			}

			/* Test changing marks while scaling
				if (FuncScaleTransform.ScaleX > 2)
				{
					Database.Marks = new List<Canvas>();
					TimelineMarks.ItemsSource = Database.Marks;
				}*/
			/*for (int i = 0; i < TimelineMarks.Items.Count; i++)
			{
				var mark = (TimelineMarks.Items[i] as Canvas)!.Children[0] as Line;
				//mark.X1=mark.X2*=FuncScaleTransform
				Canvas.SetLeft(mark, mark.X1 * FuncScaleTransform.ScaleX);
				//mark.RenderSize.Width
				
			}*/
		}

		if (GlobalScaleTransform.ScaleX <= minScale) GlobalScaleTransform.ScaleX += absDelta;
		if (GlobalScaleTransform.ScaleX >= maxScale) GlobalScaleTransform.ScaleX -= absDelta;

		//FuncScaleTransform.CenterX = e.GetPosition(CanvasItems).X; // CanvasItems
	}

	void CanvasFunctionsPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		_targetElement = ThreadsFunctions; // CanvasFunctions
		if (_targetElement != null)
			_targetPoint = e.GetPosition(_targetElement);
	}

	void CanvasFunctionsPanel_OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || _targetElement == null) return;
		var pCanvas = e.GetPosition(TimelineMarks); // canvasFunctionsPanel

		double xOffset = (pCanvas.X - _targetPoint.X) * GlobalScaleTransform.ScaleX;

		//if (yOffset <= Canvas.GetTop(CanvasFunctionsPanel)) // Top Border  
		//	yOffset = Canvas.GetTop(CanvasFunctionsPanel);					 
		//if (xOffset <= Canvas.GetLeft(CanvasFunctionsPanel)) // Left border
		//	xOffset = Canvas.GetLeft(CanvasFunctionsPanel);

		Canvas.SetLeft(ThreadsFunctions, xOffset); // CanvasFunctions
		//Canvas.SetLeft(CanvasFunctions, xOffset);
		//Canvas.SetLeft(CanvasFunctionNames, xOffset);
		//Canvas.SetLeft(TimelineMarks, xOffset);
	}

	void CanvasFunctionsPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		_targetElement = null;
	}
}
