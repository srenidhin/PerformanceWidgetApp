using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.Widgets.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private static string filePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NotesWidget.txt";

        private static string GetDefaultTemplate()
        {
            if (string.IsNullOrEmpty(WidgetTemplate))
            {
                WidgetTemplate = ReadPackageFileFromUri("ms-appx:///Templates/NotesWidgetTemplate.json");
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
        public override void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs) 
        {
            string data = JsonDocument.Parse(actionInvokedArgs.Data).RootElement.GetProperty("TextArea").ToString();
            File.WriteAllText(filePath, data);
        }
        public override string GetTemplateForWidget()
        {
            string res = GetDefaultTemplate();
            string currentNotes = "";
            if (File.Exists(filePath))
            {
                currentNotes = File.ReadAllText(filePath);
            }
            res = res.Replace("${data}",currentNotes);
            return res;
        }
        public override string GetDataForWidget()
        {
            return "{}";
        }

    }
}
