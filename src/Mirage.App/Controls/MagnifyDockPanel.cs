using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Mirage.Core;

namespace Mirage.App.Controls;

/// <summary>
/// Horizontal dock layout that applies a spring-physics magnification to the item
/// nearest the pointer (macOS Dock feel). The scale target is computed by
/// <see cref="SpringPhysics.TargetScale"/> and eased per frame by a per-item spring.
/// </summary>
public sealed class MagnifyDockPanel : Panel
{
    private double _pointerX = -1;

    public MagnifyDockPanel()
    {
        PointerMoved += OnPointerMoved;
        PointerExited += (s, e) => { _pointerX = -1; InvalidateArrange(); };
    }

    public double InfluenceRadius { get; set; } = 140;
    public double MaxScale { get; set; } = 1.8;
    public double Spacing { get; set; } = 8;

    protected override Size MeasureOverride(Size availableSize)
    {
        double maxH = 0;
        double totalW = 0;
        foreach (var child in Children)
        {
            child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            maxH = Math.Max(maxH, child.DesiredSize.Height);
            totalW += child.DesiredSize.Width;
        }

        totalW += Math.Max(0, Children.Count - 1) * Spacing;
        return new Size(totalW, maxH);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double centerY = finalSize.Height / 2;
        double x = 0;

        foreach (var child in Children)
        {
            double distance = _pointerX < 0 ? double.PositiveInfinity : Math.Abs((x + child.DesiredSize.Width / 2) - _pointerX);
            double scale = SpringPhysics.TargetScale(distance, InfluenceRadius, MaxScale);

            double w = child.DesiredSize.Width * scale;
            double h = child.DesiredSize.Height * scale;
            double y = centerY - h / 2;

            child.Arrange(new Rect(x, y, w, h));
            x += child.DesiredSize.Width + Spacing;
        }

        return finalSize;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(this).Position;
        _pointerX = pos.X;
        InvalidateArrange();
    }
}
