using Microsoft.Windows.Widgets.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceWidgetApp
{
    internal class PerformanceMonitor : WidgetImplBase
    {
        public static string DefinitionId { get; } = "Performance_Widget_App";
        public PerformanceMonitor(string widgetId, string initialState) : base(widgetId, DefinitionId,initialState) { }

        private static string WidgetTemplate { get; set; } = "";

        private static string GetDefaultTemplate()
        {
            if (string.IsNullOrEmpty(WidgetTemplate))
            {
                WidgetTemplate = ReadPackageFileFromUri("ms-appx:///Templates/PerformanceWidgetTemplate.json");
            }

            return WidgetTemplate;
        }

        public override void Activate(WidgetContext widgetContext)
        {
            WidgetUpdateRequestOptions widgetUpdateRequestOptions = new WidgetUpdateRequestOptions(widgetContext.Id);

            widgetUpdateRequestOptions.Template = GetTemplateForWidget();
            widgetUpdateRequestOptions.Data = GetDataForWidget();
            widgetUpdateRequestOptions.CustomState = this.State;

            WidgetManager.GetDefault().UpdateWidget(widgetUpdateRequestOptions);
        }

        public override string GetTemplateForWidget()
        {
            return GetDefaultTemplate();
        }

        public override string GetDataForWidget()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            PMClass pM = new PMClass();

            pM.cpup = Math.Round(cpuCounter.NextValue(), 2);
            System.Threading.Thread.Sleep(1000);
            pM.cpup = Math.Round(cpuCounter.NextValue(), 2);
            pM.ramp = Math.Round(ramCounter.NextValue(), 2);
            pM.gpup = Math.Round(GetGPUUsage(), 2);

            return JsonSerializer.Serialize(pM);
        }

        internal static float GetGPUUsage()
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var counterNames = category.GetInstanceNames();

            var gpuCounters = counterNames
                                .Where(counterName => counterName.EndsWith("engtype_3D"))
                                .SelectMany(counterName => category.GetCounters(counterName))
                                .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                                .ToList();

            gpuCounters.ForEach(x => x.NextValue());

            Thread.Sleep(1000);

            var result = gpuCounters.Sum(x => x.NextValue());

            return result;
        }
    }
}
