// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Spectralyzer.Shared.UI;

public static class ViewModelLocator
{
    public static readonly DependencyProperty DataContextProperty =
        DependencyProperty.RegisterAttached("DataContext", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(null, OnDataContextChanged));

    [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
    public static Type GetDataContext(DependencyObject element)
    {
        return (Type)element.GetValue(DataContextProperty);
    }

    public static void SetDataContext(DependencyObject element, Type value)
    {
        element.SetValue(DataContextProperty, value);
    }

    private static void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is not FrameworkElement frameworkElement) return;
        var oldValueType = (Type)e.OldValue;
        var newValueType = (Type)e.NewValue;
        if (oldValueType != newValueType)
        {
            frameworkElement.DataContext = ContainerLocator.Current!.GetRequiredService(newValueType);
        }
    }
}