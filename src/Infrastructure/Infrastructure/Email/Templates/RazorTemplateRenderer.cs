using Infrastructure.Email.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Email.Templates;

/// <summary>
/// Razor view engine kullanan template renderer implementasyonu
/// </summary>
public class RazorTemplateRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider)
    : ITemplateRenderer
{
    /// <summary>
    /// Template'i render eder
    /// </summary>
    public async Task<string> RenderAsync(string templateName, object model)
    {
        // View path'i oluştur
        var viewPath = $"/Views/EmailTemplates/{templateName}.cshtml";

        // View'ı bul
        var viewResult = viewEngine.GetView(null, viewPath, false);

        if (!viewResult.Success)
        {
            throw new TemplateNotFoundException($"Template not found: {templateName}");
        }

        // View context oluştur
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        await using var writer = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            writer,
            new HtmlHelperOptions());

        // View'ı render et
        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}