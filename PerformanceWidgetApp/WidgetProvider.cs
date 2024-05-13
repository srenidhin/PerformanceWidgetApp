using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Windows.Widgets.Providers;
using System.Diagnostics;
using System.Text.Json;
using System.Management;

namespace PerformanceWidgetApp
{
    public class WidgetInfo
    {
        public string widgetId { get; set; }
        public string widgetName { get; set; }
        public int customState = 0;
        public bool isActive = false;        
    }

    [ComVisible(true)]
    [ComDefaultInterface(typeof(IWidgetProvider))]
    [Guid("FEDAAF47-7AAE-400E-BBBC-C9EE5D32F050")]
    internal class WidgetProvider : IWidgetProvider
    {
        static ManualResetEvent emptyWidgetListEvent = new ManualResetEvent(false);

        public static Dictionary<string, WidgetImplBase> activeWidgets = new Dictionary<string, WidgetImplBase>();

        private static readonly Dictionary<string, WidgetCreateDelegate> WidgetImpls = new()
        {
            [PerformanceMonitor.DefinitionId] = (widgetId, initialState) => new PerformanceMonitor(widgetId, initialState),
            [Notes.DefinitionId] = (widgetId, initialState) => new Notes(widgetId, initialState)
        };

        public WidgetProvider() {
            
            var runningWidgets = WidgetManager.GetDefault().GetWidgetInfos();

            foreach (var widgetInfo in runningWidgets) {
                var widgetContext = widgetInfo.WidgetContext;
                var widgetId = widgetContext.Id;
                var widgetName = widgetContext.DefinitionId;
                var customState = widgetInfo.CustomState;

                if (!activeWidgets.ContainsKey(widgetId)) {
                    if (WidgetImpls.ContainsKey(widgetName))
                        activeWidgets[widgetId] = WidgetImpls[widgetName](widgetId, customState);
                    else
                        WidgetManager.GetDefault().DeleteWidget(widgetId);
                }

            }
        }

        public void CreateWidget(WidgetContext widgetContext)
        {
            var widgetId = widgetContext.Id;
            var widgetName = widgetContext.DefinitionId;
            var widget = WidgetImpls[widgetName](widgetId, "");
            activeWidgets.Add(widgetId, widget);
        }

        public void DeleteWidget(string widgetId, string customState)
        {
            activeWidgets.Remove(widgetId);

            if (activeWidgets.Count == 0)
            {
                emptyWidgetListEvent.Set();
            }
        }

        public void OnActionInvoked(WidgetActionInvokedArgs widgetActionInvokedArgs)
        {
            activeWidgets[widgetActionInvokedArgs.WidgetContext.Id].OnActionInvoked(widgetActionInvokedArgs);
        }

        public void OnWidgetContextChanged(WidgetContextChangedArgs widgetContextChangedArgs)
        {
           activeWidgets[widgetContextChangedArgs.WidgetContext.Id].OnWidgetContextChanged(widgetContextChangedArgs);
        }

        public void Activate(WidgetContext widgetContext)
        {
            activeWidgets[widgetContext.Id].Activate(widgetContext);
        }

        public void Deactivate(string widgetId)
        {
            activeWidgets[widgetId].Deactivate();
        }

        public static ManualResetEvent GetEmptyWidgetListEvent()
        {
            return emptyWidgetListEvent;
        }

    }
}
