﻿using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UraniumStudio.ViewModel;

public static class TimelineCreator
{
	public static Canvas[] GetTimelineMarks(double width, double scaleX)
	{
		if (width is double.NaN) throw new ArgumentOutOfRangeException();
		uint countMarks = (uint)width / 10 + 2;
		var marks = new Canvas[countMarks];
		const int step = 10;

		for (int i = 0; i < countMarks; i++)
			marks[i] =
				new Canvas
				{
					Children =
					{
						new Line
						{
							X1 = step * i, X2 = step * i, Y1 = 10, Y2 = 20, Stroke = Brushes.Black,
							StrokeThickness = 0.5
						}

					}
				};

		return marks;
	}
}
