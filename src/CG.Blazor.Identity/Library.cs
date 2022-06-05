
namespace CG.Blazor.Identity;

/// <summary>
/// This class utility contains library related state.
/// </summary>
public static class Library
{
    /// <summary>
    /// This property contains the library's assembly, which is useful
    /// for plugging into Blazor's routing logic.
    /// </summary>
    public static Assembly Assembly => typeof(Library).Assembly;
}
