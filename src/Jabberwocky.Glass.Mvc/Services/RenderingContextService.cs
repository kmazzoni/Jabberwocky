using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Models;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Services
{
    public class RenderingContextService : IRenderingContextService
    {
        private const string RenderingParameterPropertyName = "Parameters";
        private const string RenderingItemIdPropertyName = "ItemId";

        private readonly IGlassHtml _glassHtml;
        private readonly ISitecoreContext _context;

        private static readonly ConcurrentDictionary<Type, MethodInfo> GenericRenderingMethodCache = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> GenericModelMethodCache = new ConcurrentDictionary<Type, MethodInfo>();

        public RenderingContextService(IGlassHtml glassHtml, ISitecoreContext context)
        {
            if (glassHtml == null) throw new ArgumentNullException(nameof(glassHtml));
            if (context == null) throw new ArgumentNullException(nameof(context));
            _glassHtml = glassHtml;
            _context = context;
        }

        public Rendering GetCurrentRendering()
        {
            var context = RenderingContext.CurrentOrNull;

            return context?.Rendering;
        }

        public T GetCurrentRenderingDatasource<T>(DatasourceNestingOptions options = DatasourceNestingOptions.Default) where T : class
        {
            var rendering = GetCurrentRendering();
            if (rendering == null)
            {
                return null;
            }

            Guid dataSourceGuid;
            if (!string.IsNullOrEmpty(rendering.DataSource))
            {
                // Depending on if the datasource is a GUID vs Path, use the correct overload
                return Guid.TryParse(rendering.DataSource, out dataSourceGuid)
                    ? _context.GetItem<T>(dataSourceGuid, inferType: true)
                    : _context.GetItem<T>(rendering.DataSource, inferType: true);
            }

            // Try to get from the Rendering StaticItem (without getting the ContextItem)
            var propertyItemId = rendering[RenderingItemIdPropertyName];
            T propertyItem = null;
            if (!string.IsNullOrEmpty(propertyItemId))
            {
                propertyItem = Guid.TryParse(propertyItemId, out dataSourceGuid)
                    ? _context.GetItem<T>(dataSourceGuid, inferType: true)
                    : _context.GetItem<T>(propertyItemId, inferType: true);
            }

            // Vary the fall-back logic (Always and Never recreate the behavior of Default, but with/without their respective fallback logic)
            switch (options)
            {
                case DatasourceNestingOptions.Default:
                    var staticItem = rendering.Item;
                    if (staticItem != null)
                    {
                        return _context.GetItem<T>(staticItem.ID.Guid, inferType: true);
                    }
                    break;
                case DatasourceNestingOptions.Always:
                    if (propertyItem != null)
                    {
                        return propertyItem;
                    }

                    var nestedItem = RenderingContext.CurrentOrNull?.ContextItem;
                    if (nestedItem != null)
                    {
                        return _context.GetItem<T>(nestedItem.ID.Guid, inferType: true);
                    }
                    break;
                case DatasourceNestingOptions.Never:
                    if (propertyItem != null)
                    {
                        return propertyItem;
                    }
                    break;
            }

            // Finally just fall back to the context item
            return _context.GetCurrentItem<T>(inferType: true);
        }

        public object GetCurrentRenderingDatasource(Type modelType, DatasourceNestingOptions options = DatasourceNestingOptions.Default)
        {
            var genericThunk = GenericModelMethodCache.GetOrAdd(modelType, type =>
            {
                return typeof(RenderingContextService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(method => method.Name == nameof(this.GetCurrentRenderingDatasource) && method.IsGenericMethodDefinition)
                    .MakeGenericMethod(modelType);
            });

            return genericThunk.Invoke(this, new object[] { options });
        }

        public T GetCurrentRenderingParameters<T>() where T : class
        {
            var parameterString = RenderingContext.CurrentOrNull?.Rendering?[RenderingParameterPropertyName];

            return string.IsNullOrEmpty(parameterString) 
                ? null 
                : _glassHtml.GetRenderingParameters<T>(parameterString);
        }

        public object GetCurrentRenderingParameters(Type renderingParamType)
        {
            var genericThunk = GenericRenderingMethodCache.GetOrAdd(renderingParamType, type =>
            {
                return typeof (RenderingContextService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(method => method.Name == nameof(RenderingContextService.GetCurrentRenderingParameters) && method.IsGenericMethodDefinition)
                    .MakeGenericMethod(renderingParamType);
            });

            return genericThunk.Invoke(this, null);
        }
    }
}
