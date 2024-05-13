using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PerformanceWidgetApp
{
    internal class Notes : WidgetImplBase
    {
        public static string DefinitionId { get; } = "Notes_Widget_App";
        public Notes(string widgetId, string initialState) : base(widgetId, DefinitionId, initialState) { }
        private static string WidgetTemplate { get; set; } = "";

        private static string GetDefaultTemplate()
        {
            if (string.IsNullOrEmpty(WidgetTemplate))
            {
                WidgetTemplate = ReadPackageFileFromUri("ms-appx:///Templates/NotesWidgetTemplate.json");
            }

            return WidgetTemplate;
        }
        public override string GetTemplateForWidget()
        {
            return GetDefaultTemplate();
        }
        public override string GetDataForWidget()
        {
            return "{}";
        }

    }
}
