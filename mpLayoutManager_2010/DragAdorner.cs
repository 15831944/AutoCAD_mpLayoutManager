using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mpLayoutManager
{
    public class DragAdorner : Adorner
    {
        private readonly Rectangle child = null;

        private double offsetLeft = 0;

        private double offsetTop = 0;

        public double OffsetLeft
        {
            get => offsetLeft;
            set
            {
                offsetLeft = value;
                UpdateLocation();
            }
        }

        public double OffsetTop
        {
            get => offsetTop;
            set
            {
                offsetTop = value;
                UpdateLocation();
            }
        }

        protected override int VisualChildrenCount => 1;

        public DragAdorner(UIElement adornedElement, Size size, Brush brush) : base(adornedElement)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = brush,
                Width = size.Width,
                Height = size.Height,
                IsHitTestVisible = false
            };
            child = rectangle;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup generalTransformGroup = new GeneralTransformGroup();
            generalTransformGroup.Children.Add(base.GetDesiredTransform(transform));
            generalTransformGroup.Children.Add(new TranslateTransform(offsetLeft, offsetTop));
            return generalTransformGroup;
        }

        protected override Visual GetVisualChild(int index)
        {
            return child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }

        public void SetOffsets(double left, double top)
        {
            offsetLeft = left;
            offsetTop = top;
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            AdornerLayer parent = Parent as AdornerLayer;
            parent?.Update(AdornedElement);
        }
    }
}