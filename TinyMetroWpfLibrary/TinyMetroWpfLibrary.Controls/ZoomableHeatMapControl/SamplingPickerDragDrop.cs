using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TinyMetroWpfLibrary.Controls
{

    public class SamplingPickerDrag<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ZoomableHeatMapControl
        where TObject : UIElement
    {

        public SamplingPickerDrag(string dataFormatString) :
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
            return (sourceObject != null);      
        }
 
    }


    public class SamplingPickerDrop<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ZoomableHeatMapControl
        where TObject : UIElement
    {

        public SamplingPickerDrop(string[] dataFormats)
            : base(dataFormats)
        {
        }


        protected override void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e)
        {
            SamplingPickerDrag<TContainer, TObject> dataProvider = this.GetData(e) as SamplingPickerDrag<TContainer, TObject>;
            if (dataProvider != null)
            {
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);

                TContainer dropContainer = sender as TContainer;

                if (dropContainer != null)
                {
                    if (bDrop)
                    {
                        Point dropPosition = e.GetPosition(dropContainer.FloorPlanImage);
                        Point objectOrigin = dataProvider.StartPosition;
                        double x = dropPosition.X + 25 / dropContainer.Scale - objectOrigin.X / dropContainer.Scale;
                        double y = dropPosition.Y + 25 / dropContainer.Scale - objectOrigin.Y / dropContainer.Scale;
                        dropContainer.PickerPoint = new Point(x,y);
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
