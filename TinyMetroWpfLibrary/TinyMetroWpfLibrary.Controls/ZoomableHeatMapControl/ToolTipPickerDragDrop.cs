using System.Windows;
using TinyMetroWpfLibrary.Controls.DragDropFramework;

namespace TinyMetroWpfLibrary.Controls.ZoomableHeatMapControl
{

    public class ToolTipPickerDrag<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ZoomableHeatMapControl
        where TObject : UIElement
    {

        public ToolTipPickerDrag(string dataFormatString) :
            base(dataFormatString)
        {
        }

        public override bool IsSupportedContainerAndObject(bool initFlag, object dragSourceContainer, object dragSourceObject, object dragOriginalSourceObject)
        {
            TObject sourceObject = dragSourceObject as TObject;
            if (sourceObject == null)
            {
                sourceObject = Utilities.FindParentControlExcludingMe<TObject>(dragSourceObject as DependencyObject);
            }

            if (initFlag)
            {
                this.Init();
                this.SourceContainer = dragSourceContainer;
                this.SourceObject = sourceObject;
                this.OriginalSourceObject = dragOriginalSourceObject;
            }

            return
                (sourceObject != null);
        }
    }

    public class ToolTipPickerDrop<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ZoomableHeatMapControl
        where TObject : UIElement
    {

        public ToolTipPickerDrop(string[] dataFormats)
            : base(dataFormats)
        {
        }

        protected override void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            ToolTipPickerDrag<TContainer, TObject> dataProvider = this.GetData(e) as ToolTipPickerDrag<TContainer, TObject>;
            if (dataProvider != null)
            {
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                TContainer dropContainer = sender as TContainer;

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        Point dropPosition = e.GetPosition(dropContainer.FloorPlanImage);
                        Point objectOrigin = dataProvider.StartPosition;
                        dropContainer.ToolTipLocation = new Point(dropPosition.X+  (25 - objectOrigin.X) / dropContainer.Scale, dropPosition.Y+ (50 - objectOrigin.Y) / dropContainer.Scale);
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
        }
    }
}
