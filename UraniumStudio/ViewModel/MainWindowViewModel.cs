using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace UraniumStudio.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
	public IEnumerable<int> MajorStep { get; }
	public Func<double, double> ValueStepTransform { get; }

	public ScaleTransform GlobalScaleTransform { get; }

	string _unit = null!;

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

	public new event PropertyChangedEventHandler? PropertyChanged;

	public MainWindowViewModel()
	{
		MajorStep = new[] { 1, 2, 5 };
		ValueStepTransform = UpdateStepValues;
		GlobalScaleTransform = new ScaleTransform();
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
}