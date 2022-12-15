using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace UraniumStudio.ViewModel;

public class MainWindowVM
{
	public IEnumerable<int> MajorStep { get; } = new[] { 1, 2, 5 };
	public Func<double, double> ValueStepTransform { get; } = null!;

	string _unit = null!;

	public event PropertyChangedEventHandler PropertyChanged = null!;

	public MainWindowVM()
	{

	}

	double UpdateStepValues(double stepValue)
	{
		if (stepValue < 1)
		{
			Unit = "* 1000";
			return stepValue * 1000;
		}

		Unit = "* 1";

		return stepValue;
	}

	public string Unit
	{
		get => _unit;
		set
		{
			if (_unit == value) return;

			_unit = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Unit)));
		}
	}
}
