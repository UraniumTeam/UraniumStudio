using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using UraniumStudio.Data;
using UraniumStudio.Model;

namespace UraniumStudio.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
	public IEnumerable<int> MajorStep { get; }
	public Func<double, double> ValueStepTransform { get; }

	public ScaleTransform GlobalScaleTransform { get; }

	private string unit = null!;

	public string Unit
	{
		get => unit;
		set
		{
			if (unit == value) return;
			unit = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Unit)));
		}
	}

	public new event PropertyChangedEventHandler? PropertyChanged;

	public MainWindowViewModel()
	{
		MajorStep = new[] { 1, 2, 5 };
		ValueStepTransform = UpdateStepValues;
		GlobalScaleTransform = new ScaleTransform();

		for (var i = 0; i < Database.ThreadPaths.Count; i++)
		{
			Database.Functions.Add(new List<Function>());
			Database.Functions[i].AddRange(FileParser.ParseFile(Database.ThreadPaths[i]));
		}
	}

	private double UpdateStepValues(double stepValue)
	{
		if (stepValue < 1)
		{
			Unit = "* 1000";
			return stepValue * 1000;
		}

		Unit = "* 1";
		return stepValue;
	}
}