using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace XtraChef.Content.Command.Model
{
    public class EmailReport
    {
        /// <summary>
        /// Generate html content.
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="razorViewEngine"></param>
        /// <param name="tempDataProvider"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task<string> GetMailBody(string viewName, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, List<ChartDataList> chartData = null)
        {
            try
            {
                //Local variable
                var actionContext = GetActionContext(serviceProvider);
                var view = FindView(actionContext, viewName, razorViewEngine);

                if (chartData == null)
                {
                    using (var output = new StringWriter())
                    {
                        var viewContext = new ViewContext(
                            actionContext,
                            view,
                            new ViewDataDictionary<EmailReport>(
                                metadataProvider: new EmptyModelMetadataProvider(),
                                modelState: new ModelStateDictionary())
                            {
                                Model = this
                            },

                            new TempDataDictionary(
                                actionContext.HttpContext,
                                tempDataProvider),
                            output,
                            new HtmlHelperOptions());
                        await view.RenderAsync(viewContext);
                        return output.ToString();
                    }
                }
                else
                {
                    using (var output = new StringWriter())
                    {
                        var viewContext = new ViewContext(
                            actionContext,
                            view,
                            new ViewDataDictionary<List<ChartDataList>>(
                                metadataProvider: new EmptyModelMetadataProvider(),
                                modelState: new ModelStateDictionary())
                            {
                                Model = chartData
                            },

                            new TempDataDictionary(
                                actionContext.HttpContext,
                                tempDataProvider),
                            output,
                            new HtmlHelperOptions());
                        await view.RenderAsync(viewContext);
                        return output.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ActionContext GetActionContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        /// <summary>
        /// Find the view name
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="viewName"></param>
        /// <param name="razorViewEngine"></param>
        /// <returns></returns>
        private IView FindView(ActionContext actionContext, string viewName, IRazorViewEngine razorViewEngine)
        {
            try
            {
                var getViewResult = razorViewEngine.GetView(null, $"~/Views/Content/{viewName}.cshtml", false);

                if (getViewResult.Success)
                {
                    return getViewResult.View;
                }
                else
                    throw new Exception($"View: {viewName} is not found");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
