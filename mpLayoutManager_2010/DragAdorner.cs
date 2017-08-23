using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF.JoshSmith.Adorners
{
    public class DragAdorner : Adorner
    {
        private Rectangle child = null;

        private double offsetLeft = 0;

        private double offsetTop = 0;

        public double OffsetLeft
        {
            get
            {
                return this.offsetLeft;
            }
            set
            {
                this.offsetLeft = value;
                this.UpdateLocation();
            }
        }

        public double OffsetTop
        {
            get
            {
                return this.offsetTop;
            }
            set
            {
                this.offsetTop = value;
                this.UpdateLocation();
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public DragAdorner(UIElement adornedElement, Size size, Brush brush) : base(adornedElement)
        {
            Rectangle rectangle = new Rectangle()
            {
                Fill = brush,
                Width = size.Width,
                Height = size.Height,
                IsHitTestVisible = false
            };
            this.child = rectangle;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup generalTransformGroup = new GeneralTransformGroup();
            generalTransformGroup.Children.Add(base.GetDesiredTransform(transform));
            generalTransformGroup.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
            return generalTransformGroup;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        public void SetOffsets(double left, double top)
        {
            this.offsetLeft = left;
            this.offsetTop = top;
            this.UpdateLocation();
        }

        private void UpdateLocation()
        {
            AdornerLayer parent = base.Parent as AdornerLayer;
            if (parent != null)
            {
                parent.Update(base.AdornedElement);
            }
        }
    }
}