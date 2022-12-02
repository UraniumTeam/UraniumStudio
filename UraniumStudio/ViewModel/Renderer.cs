using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UraniumStudio.Model;

namespace UraniumStudio.ViewModel;

public static class Renderer
{
	public static Tuple<Canvas[], Canvas[]> GetCanvasesArray(List<Function>? functions)
	{
		var rectangles = ConvertFunctionsToRectangles(functions);
		var functionCanvases = new Canvas[functions!.Count];
		var functionNameCanvases = new Canvas[functions.Count];

		for (int i = 0; i < functions.Count; i++)
		{
			var functionCanvas = new Canvas();
			var functionNameCanvas = new Canvas();

			var funcName = new TextBlock
			{
				Text = functions[i].Name, MaxWidth = rectangles[i].Width
			};

			Canvas.SetLeft(rectangles[i], functions[i].StartPosX);
			Canvas.SetLeft(funcName, functions[i].StartPosX);
			Canvas.SetTop(rectangles[i], functions[i].RowPosY * Function.Height);
			Canvas.SetTop(funcName, functions[i].RowPosY * Function.Height);

			functionCanvas.Children.Add(rectangles[i]);
			functionNameCanvas.Children.Add(funcName);
			functionCanvas.Name = rectangles[i].Name;

			functionCanvases[i] = functionCanvas;
			functionNameCanvases[i] = functionNameCanvas;
		}

		return new Tuple<Canvas[], Canvas[]>(functionCanvases, functionNameCanvases);
	}

	static Rectangle[] ConvertFunctionsToRectangles(IReadOnlyList<Function>? functions)
	{
		var rectangles = new Rectangle[functions!.Count];
		for (int i = 0; i < rectangles.Length; i++)
		{
			rectangles[i] = new Rectangle
			{
				Focusable = true,
				Fill = new SolidColorBrush(functions[i].Color),
				Width = functions[i].Length,
				Height = Function.Height,
				Name = functions[i].Name
			};
		}

		return rectangles;
	}
}
