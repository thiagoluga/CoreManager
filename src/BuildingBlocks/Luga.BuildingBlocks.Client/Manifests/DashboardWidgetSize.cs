namespace Luga.BuildingBlocks.Client.Manifests;

/// <summary>
/// Grid footprint of a dashboard widget. The dashboard layout uses these to
/// pack widgets into a responsive grid.
/// </summary>
public enum DashboardWidgetSize
{
    /// <summary>Single column on desktop, single column on mobile.</summary>
    Small = 0,

    /// <summary>Two columns on desktop.</summary>
    Medium = 1,

    /// <summary>Three columns on desktop.</summary>
    Large = 2,

    /// <summary>Spans the full row on every breakpoint.</summary>
    FullWidth = 3,
}
