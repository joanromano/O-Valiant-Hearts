#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of DanielVaughan's WindowsPhone base library

	DanielVaughan's WindowsPhone base library is free software: you can redistribute it and/or modify
	it under the terms of the GNU Lesser General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	DanielVaughan's WindowsPhone base library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with DanielVaughan's WindowsPhone base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-10-21 15:34:42Z</CreationDate>
</File>
*/
#endregion

using System;

using DanielVaughan.ComponentModel;

namespace DanielVaughan.WindowsPhone7Unleashed
{
	public class DelegateCommand<T> : IEventCommand
	{
		string eventName = "Click";

		public string EventName
		{
			get
			{
				return eventName;
			}
			set
			{
				eventName = value;
			}
		}

		readonly Action<T> executeAction;
		readonly Func<T, bool> canExecuteFunc;
		bool previousCanExecute;

		public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecuteFunc)
		{
			this.executeAction = ArgumentValidator.AssertNotNull(executeAction, "executeAction");
			this.canExecuteFunc = canExecuteFunc;
		}

		public DelegateCommand(Action<T> executeAction)
		{
			this.executeAction = ArgumentValidator.AssertNotNull(executeAction, "executeAction");
		}

		#region ICommand Members

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			object coercedParameter = CoerceParameterToType(parameter);
			return CanExecute((T)coercedParameter);
		}

		public void Execute(object parameter)
		{
			object coercedParameter = CoerceParameterToType(parameter);
			Execute((T)coercedParameter);
		}

		object CoerceParameterToType(object parameter)
		{
			object coercedParameter = parameter;
			Type typeOfT = typeof(T);
			if (parameter != null && !typeOfT.IsAssignableFrom(parameter.GetType()))
			{
				coercedParameter = ImplicitTypeConverter.ConvertToType(parameter, typeOfT);
			}
			return coercedParameter;
		}

		#endregion

		public bool CanExecute(T parameter)
		{
			if (canExecuteFunc == null)
			{
				return true;
			}

			bool temp = canExecuteFunc(parameter);

			if (previousCanExecute != temp)
			{
				previousCanExecute = temp;
				OnCanExecuteChanged(EventArgs.Empty);
			}

			return previousCanExecute;
		}

		public void Execute(T parameter)
		{
			executeAction(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged(EventArgs.Empty);
		}

		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			var tempEvent = CanExecuteChanged;
			if (tempEvent != null)
			{
				tempEvent(this, e);
			}
		}
	}

	public class DelegateCommand : DelegateCommand<object>
	{
		public DelegateCommand(
			Action<object> executeAction, Func<object, bool> canExecute)
			: base(executeAction, canExecute)
		{
			/* Intentionally left blank. */
		}

		public DelegateCommand(Action<object> executeAction)
			: base(executeAction)
		{
			/* Intentionally left blank. */
		}
	}
}
