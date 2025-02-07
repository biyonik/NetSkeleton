namespace Infrastructure.Email.Templates;

/// <summary>
/// Email template'lerini render etmek için interface
/// </summary>
public interface ITemplateRenderer
{
    /// <summary>
    /// Template'i verilen model ile render eder
    /// </summary>
    Task<string> RenderAsync(string templateName, object model);
}