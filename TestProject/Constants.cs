namespace TestProject;

/// <summary>
/// The static class contains global constants for the unit tests.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// The constant for the conflict header when searching in html.
    /// </summary>
    public const string ConflictHtmlSearchTag = "<h3>Sorry, the submitted data was detected to be out of date; please go back to the list page and try editing again.</h3>";

    /// <summary>
    /// The constant for the doctype tag in html.
    /// </summary>
    public const string DocTypeHtmlTag = "<!DOCTYPE html>";

    /// <summary>
    /// The constant for the error header when searching in html.
    /// </summary>
    public const string ErrorHtmlSearchTag = "<h3>Sorry, an unexpected error occurred.</h3>";

    /// <summary>
    /// The constant for the not found header when searching in html.
    /// </summary>
    public const string NotFoundHtmlSearchTag = "<h3>Sorry, the page or resource was not found.</h3>";
}
