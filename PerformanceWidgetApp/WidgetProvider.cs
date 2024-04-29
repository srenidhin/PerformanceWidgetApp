using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Windows.Widgets.Providers;


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
        const string PMWidgetTemplate = @"{
                ""type"": ""AdaptiveCard"",
                ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                ""version"": ""1.6"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""CPU Usage : ${CPUP} @ ${CPUT}"",
                        ""wrap"": true
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""GPU Usage : ${GPUP} @ ${GPUT}"",
                        ""wrap"": true
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Ram : ${RamP}"",
                        ""wrap"": true
                    }
                ]
            }";

        static ManualResetEvent emptyWidgetListEvent = new ManualResetEvent(false);

        public static Dictionary<string, WidgetInfo> activeWidgets = new Dictionary<string, WidgetInfo>();

        public WidgetProvider() {
            
            var runningWidgets = WidgetManager.GetDefault().GetWidgetInfos();

            foreach (var widgetInfo in runningWidgets) {
                var widgetContext = widgetInfo.WidgetContext;
                var widgetId = widgetContext.Id;
                var widgetName = widgetContext.DefinitionId;
                var customState = widgetInfo.CustomState;

                if (!activeWidgets.ContainsKey(widgetId)) { 
                    WidgetInfo widget = new WidgetInfo() { widgetId = widgetId, widgetName = widgetName};
                    try
                    {
                        int c = Convert.ToInt32(customState);
                        widget.customState = c;
                    }
                    catch (Exception ex) { 
                        Console.WriteLine("Failed to recover custom state : " + ex.ToString());
                    }
                    activeWidgets[widgetId] = widget;
                }

            }
        }

        public void CreateWidget(WidgetContext widgetContext)
        {
            var widgetId = widgetContext.Id;
            var widgetName = widgetContext.DefinitionId;
            WidgetInfo widgetInfo = new WidgetInfo() { widgetId = widgetId, widgetName = widgetName };
            activeWidgets.Add(widgetId, widgetInfo);
            UpdateWidget(widgetInfo);
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

        }

        public void OnWidgetContextChanged(WidgetContextChangedArgs widgetContextChangedArgs)
        {

        }

        public void Activate(WidgetContext widgetContext)
        {
            var widgetId = widgetContext.Id;
            if (activeWidgets.ContainsKey(widgetId))
            {
                var widgetInfo = activeWidgets[widgetId];
                widgetInfo.isActive = true;
                UpdateWidget(widgetInfo);
            }
        }

        public void Deactivate(string widgetId)
        {
            if (activeWidgets.ContainsKey(widgetId))
            {
                activeWidgets[widgetId].isActive = false;
            }
        }

        public void UpdateWidget(WidgetInfo widgetInfo)
        {
            WidgetUpdateRequestOptions widgetUpdateRequestOptions = new WidgetUpdateRequestOptions(widgetInfo.widgetId);

            string templateJSON = null;
            string dataJSON = null;

            if(widgetInfo.widgetName == "PerformanceMonitor")
            {
                templateJSON = PMWidgetTemplate;
                dataJSON = "{}";
            }

            widgetUpdateRequestOptions.Template = templateJSON;
            widgetUpdateRequestOptions.Data = dataJSON;
            widgetUpdateRequestOptions.CustomState = widgetInfo.customState.ToString();
            WidgetManager.GetDefault().UpdateWidget(widgetUpdateRequestOptions);
        }

        public static ManualResetEvent GetEmptyWidgetListEvent()
        {
            return emptyWidgetListEvent;
        }

    }
}
