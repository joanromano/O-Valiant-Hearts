#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2011-01-24 20:41:43Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DanielVaughan.WindowsPhone7Unleashed
{
	public class ScrollViewerMonitor
	{
		public static DependencyProperty AtEndCommandProperty
			= DependencyProperty.RegisterAttached(
				"AtEndCommand", typeof(ICommand),
				typeof(ScrollViewerMonitor),
				new PropertyMetadata(OnAtEndCommandChanged));

		public static ICommand GetAtEndCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(AtEndCommandProperty);
		}

		public static void SetAtEndCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(AtEndCommandProperty, value);
		}


		public static void OnAtEndCommandChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement element = (FrameworkElement)d;
			if (element != null)
			{
				element.Loaded -= element_Loaded;
				element.Loaded += element_Loaded;
			}
		}

		static void element_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement element = (FrameworkElement)sender;
			element.Loaded -= element_Loaded;
			ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(element);
			if (scrollViewer == null)
			{
				throw new InvalidOperationException("ScrollViewer not found.");
			}

			var listener = new DependencyPropertyListener();
			listener.Changed
				+= delegate
				{
                    int atBottomFirst = System.Convert.ToInt16(scrollViewer.VerticalOffset).CompareTo(System.Convert.ToInt16(scrollViewer.ScrollableHeight));
                    int atBottomSec = (System.Convert.ToInt16(scrollViewer.VerticalOffset)+1).CompareTo(System.Convert.ToInt16(scrollViewer.ScrollableHeight));
                  //  System.Diagnostics.Debug.WriteLine("Bottom test? " + System.Convert.ToInt16(scrollViewer.VerticalOffset) + " " + System.Convert.ToInt16(scrollViewer.ScrollableHeight));
                    if (atBottomFirst >= 0 || atBottomSec >= 0)
                    {
                       // System.Diagnostics.Debug.WriteLine("Already at bottom: " + scrollViewer.VerticalOffset + " " + scrollViewer.ScrollableHeight);
						var atEnd = GetAtEndCommand(element);
						if (atEnd != null)
						{
							atEnd.Execute(null);
						}
					}
				};
			Binding binding = new Binding("VerticalOffset") { Source = scrollViewer };
			listener.Attach(scrollViewer, binding);
		}

		static T FindChildOfType<T>(DependencyObject root) where T : class
		{
			var queue = new Queue<DependencyObject>();
			queue.Enqueue(root);

			while (queue.Count > 0)
			{
				DependencyObject current = queue.Dequeue();
				for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
				{
					var child = VisualTreeHelper.GetChild(current, i);
					var typedChild = child as T;
					if (typedChild != null)
					{
						return typedChild;
					}
					queue.Enqueue(child);
				}
			}
			return null;
		}
	}
}
