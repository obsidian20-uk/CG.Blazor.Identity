namespace CG;

/// <summary>
/// This class contains extension methods related to the <see cref="object"/>
/// type.
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// This method performs a deep copy from the source object
    /// to the destination object.
    /// </summary>
    /// <param name="source">The object to read from.</param>
    /// <param name="dest">The object to write to.</param>
    public static void CopyTo(this object source, object dest)
    {
        // Get the list of properties that can be copied.
        var sourceProps = source.GetType().GetProperties()
            .Where(x => x.CanWrite && x.CanRead);

        // Loop through the properties.
        foreach (var pi in sourceProps)
        {
            // Get the property value (if any).
            var sourcePropValue = pi.GetValue(source, null);
            if (null != sourcePropValue)
            {
                // Deal with value types and strings.
                if (pi.PropertyType == typeof(string) || 
                    pi.PropertyType == typeof(decimal) ||
                    pi.PropertyType == typeof(int) ||
                    pi.PropertyType == typeof(double) ||
                    pi.PropertyType == typeof(float) ||
                    pi.PropertyType == typeof(DateTime) ||
                    pi.PropertyType == typeof(TimeSpan) ||
                    pi.PropertyType.IsEnum
                    )
                {
                    // Set the destination value.
                    pi.SetValue(dest, sourcePropValue, null);
                }

                // Deal with object types.
                else
                {
                    // Get the property value (if any).
                    var destPropValue = pi.GetValue(dest, null);
                    if (null != destPropValue)
                    {
                        // Deep copy the object.
                        sourcePropValue.CopyTo(destPropValue);
                    }
                    else
                    {
                        // Set the destination to null.
                        pi.SetValue(dest, null, null);
                    }
                }
            }
            else
            {
                // Set the destination to null.
                pi.SetValue(dest, null, null);
            }
        }
    }
}
