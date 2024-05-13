using Microsoft.Windows.Widgets.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PerformanceWidgetApp
{
    internal delegate WidgetImplBase WidgetCreateDelegate(string widgetId, string initialState);
    internal abstract class WidgetImplBase
    {
        protected string id;
        protected string name;
        protected string state;
        protected bool isActivated = false;

        protected WidgetImplBase(string widgetId, string widgetName, string initialState)
        {
            state = initialState;
            name = widgetName;
            id = widgetId;
        }

        public string Id { get => id; }
        public string Name { get => name; }
        public string State { get => state; }
        public bool IsActivated { get => isActivated; }

        protected static string ReadPackageFileFromUri(string packageUri)
        {
            var uri = new Uri(packageUri);
            var storageFileTask = StorageFile.GetFileFromApplicationUriAsync(uri).AsTask();
            storageFileTask.Wait();

            var readTextTask = FileIO.ReadTextAsync(storageFileTask.Result).AsTask();
            readTextTask.Wait();

            return readTextTask.Result;
        }

        public virtual void Activate(WidgetContext widgetContext) { }
        public virtual void Deactivate() { }
        public virtual void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs) { }
        public virtual void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs) { }

        public abstract string GetTemplateForWidget();
        public abstract string GetDataForWidget();
    }
}
