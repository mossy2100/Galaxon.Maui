namespace Galaxon.Maui.Utilities;

public static class MauiUtility
{
    /// <summary>
    /// Remove all the children from a StackLayout, VerticalStackLayout, or HorizontalStackLayout.
    /// </summary>
    public static void ClearLayout(Layout stack)
    {
        // Remove children.
        while (stack.Children.Count > 0)
        {
            stack.Children.RemoveAt(stack.Children.Count - 1);
        }
    }

    /// <summary>
    /// Remove all the children from a Grid.
    /// If removeCols is true, remove all the column definitions as well.
    /// If removeRows is true, remove all the row definitions as well.
    /// </summary>
    public static void ClearGrid(Grid grid, bool removeCols = false, bool removeRows = false)
    {
        // Remove children.
        ClearLayout(grid);

        // Remove column definitions.
        if (removeCols)
        {
            // Remove column definitions.
            while (grid.ColumnDefinitions.Count > 0)
            {
                grid.ColumnDefinitions.RemoveAt(grid.ColumnDefinitions.Count - 1);
            }
        }

        // Remove row definitions.
        if (removeRows)
        {
            // Remove row definitions.
            while (grid.RowDefinitions.Count > 0)
            {
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
            }
        }
    }

    /// <summary>
    /// Get the device width in device-independent units.
    /// </summary>
    /// <returns></returns>
    public static double GetDeviceWidth()
    {
        return DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
    }

    /// <summary>
    /// Get the device height in device-independent units.
    /// </summary>
    /// <returns></returns>
    public static double GetDeviceHeight()
    {
        return DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    }

    /// <summary>
    /// Get the device orientation.
    /// </summary>
    /// <returns></returns>
    public static DisplayOrientation GetOrientation()
    {
        return DeviceDisplay.Current.MainDisplayInfo.Orientation;
    }

    /// <summary>
    /// Get the device platform (e.g. iOS, Android).
    /// </summary>
    /// <returns></returns>
    public static DevicePlatform GetPlatform()
    {
        return DeviceInfo.Current.Platform;
    }
}
