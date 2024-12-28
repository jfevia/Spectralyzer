// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Spectralyzer.App.Host.Helpers;

public static class VisibilityHelper
{
    public static readonly DependencyProperty IsVisibleProperty =
        DependencyProperty.RegisterAttached("IsVisible", typeof(bool?), typeof(VisibilityHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                                                                                                      FrameworkPropertyMetadataOptions.AffectsRender, OnIsVisibleChanged));

    public static readonly DependencyProperty IsCollapsedProperty =
        DependencyProperty.RegisterAttached("IsCollapsed", typeof(bool?), typeof(VisibilityHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                                                                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                                                                                                        FrameworkPropertyMetadataOptions.AffectsRender, OnIsCollapsedChanged));

    public static readonly DependencyProperty IsHiddenProperty =
        DependencyProperty.RegisterAttached("IsHidden", typeof(bool?), typeof(VisibilityHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                                                                     FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                                                                                                     FrameworkPropertyMetadataOptions.AffectsRender, OnIsHiddenChanged));


    public static bool? GetIsCollapsed(DependencyObject dependencyObject)
    {
        return (bool?)dependencyObject.GetValue(IsCollapsedProperty);
    }

    public static bool? GetIsHidden(DependencyObject dependencyObject)
    {
        return (bool?)dependencyObject.GetValue(IsHiddenProperty);
    }

    public static bool? GetIsVisible(DependencyObject dependencyObject)
    {
        return (bool?)dependencyObject.GetValue(IsVisibleProperty);
    }

    public static void SetIsCollapsed(DependencyObject dependencyObject, bool? value)
    {
        dependencyObject.SetValue(IsCollapsedProperty, value);
    }

    public static void SetIsHidden(DependencyObject dependencyObject, bool? value)
    {
        dependencyObject.SetValue(IsHiddenProperty, value);
    }

    public static void SetIsVisible(DependencyObject dependencyObject, bool? value)
    {
        dependencyObject.SetValue(IsVisibleProperty, value);
    }

    private static void OnIsCollapsedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        SetVisibility(dependencyObject, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Collapsed : Visibility.Visible);
    }

    private static void OnIsHiddenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        SetVisibility(dependencyObject, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Hidden : Visibility.Visible);
    }

    private static void OnIsVisibleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        SetVisibility(dependencyObject, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed);
    }

    private static void SetVisibility(DependencyObject dependencyObject, Visibility value)
    {
        switch (dependencyObject)
        {
            case FrameworkElement frameworkElement:
                frameworkElement.Visibility = value;
                break;
            case DataGridColumn dataGridColumn:
                dataGridColumn.Visibility = value;
                break;
        }
    }
}