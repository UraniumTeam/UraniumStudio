using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UraniumStudio.Model;

namespace UraniumStudio.ViewModel;

public static class Renderer
{
	public static Canvas[] GetGrids(List<Function> functions)
	{
		var rectangles = ConvertFunctionsToRectangles(functions);
		var canvases = new Canvas[functions.Count];
		for (int i = 0; i < canvases.Length; i++)
		{
			var canvas = new Canvas();
			var funcName = new TextBlock { Text = functions[i].Name };
			
			Canvas.SetLeft(rectangles[i], functions[i].StartPosX);
			Canvas.SetLeft(funcName, functions[i].StartPosX);
			Canvas.SetTop(rectangles[i], functions[i].RowPosY * Function.Height);
			Canvas.SetTop(funcName, functions[i].RowPosY * Function.Height);
			canvas.Children.Add(rectangles[i]);
			canvas.Children.Add(funcName);
			canvas.Name = rectangles[i].Name;
			canvases[i] = canvas;
		}

		return canvases;
	}

	private static Rectangle[] ConvertFunctionsToRectangles(IReadOnlyList<Function> functions)
	{
		var rectangles = new Rectangle[functions.Count];
		for (int i = 0; i < rectangles.Length; i++)
		{
			rectangles[i] = new Rectangle()
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
