using System;
using System.ComponentModel;

namespace UraniumStudio.ViewModel;

public abstract class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public virtual bool IsNew { get; protected set; }

	protected virtual void RaisePropertyChanged(string propertyName)
	{
		if (PropertyChanged is not null)
		{
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

public abstract class BaseViewModel<T> : BaseViewModel
{
	public BaseViewModel(T model)
	{
		Model = model;
	}

	public override bool IsNew
	{
		get => Model == null;

		protected set => throw new NotImplementedException();
	}

	public T Model { get; protected set; }

	public virtual T ToModel()
	{
		throw new NotImplementedException();
	}

	public virtual void Reload(T model)
	{
		Model = model;
	}
}
