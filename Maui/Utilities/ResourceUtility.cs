namespace Galaxon.Maui.Utilities;

public static class ResourceUtility
{
    public static T? LookupResource<T>(string resourceName, ContentPage? page = null)
    {
        // 1. Look in the page.
        if (page != null && page.Resources.TryGetValue(resourceName, out var resource))
        {
            return (T)resource;
        }

        // 2. Look in the app resources. This is where I usually put custom styles.
        var app = Application.Current;
        if (app == null)
        {
            return default(T);
        }
        var appResources = app.Resources;
        if (appResources.TryGetValue(resourceName, out resource))
        {
            return (T)resource;
        }

        // 3. Look in the merged dictionaries (Styles.xml and Colors.xml).
        var resourceDicts = appResources.MergedDictionaries;
        if (resourceDicts is { Count: > 0 })
        {
            foreach (var resourceDict in resourceDicts)
            {
                if (resourceDict.TryGetValue(resourceName, out resource))
                {
                    return (T)resource;
                }
            }
        }

        return default(T);
    }

    public static T? LookupResource<T>(string resourceName, ResourceDictionary resources)
    {
        return resources.TryGetValue(resourceName, out object? resource)
            ? (T)resource
            : default(T?);
    }

    public static Color? LookupColor(string resourceName, ContentPage? page = null)
    {
        return LookupResource<Color>(resourceName, page);
    }

    public static Color? LookupColor(string resourceName, ResourceDictionary resources)
    {
        return LookupResource<Color>(resourceName, resources);
    }

    public static Style? LookupStyle(string resourceName, ContentPage? page = null)
    {
        return LookupResource<Style>(resourceName, page);
    }

    public static Style? LookupStyle(string resourceName, ResourceDictionary resources)
    {
        return LookupResource<Style>(resourceName, resources);
    }
}
