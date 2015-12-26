using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TabControlStyle
{
   /// <summary>
   /// A scrollable TabPanel control.
   /// </summary>
   public class ScrollableTabPanel : Panel, IScrollInfo, INotifyPropertyChanged
   {
      #region --- Members ---

      //For a description of the members below, refer to the respective property's description.
      private ScrollViewer _svOwningScrollViewer;
      private bool _fCanScroll_H = true;
      private Size _szControlExtent = new Size(0, 0);
      private Size _szViewport = new Size(0, 0);
      private Vector _vOffset;

      //The following GradientStopCollections are being used for assigning an OpacityMask
      //to child-controls that are only partially visible.
      private static GradientStopCollection _gscOpacityMaskStops_TransparentOnLeftAndRight = new GradientStopCollection{
            new GradientStop(Colors.Transparent,0.0),
            new GradientStop(Colors.Black, 0.2),
            new GradientStop(Colors.Black, 0.8),
            new GradientStop(Colors.Transparent,1.0)
         };
      private static GradientStopCollection _gscOpacityMaskStops_TransparentOnLeft = new GradientStopCollection{
            new GradientStop(Colors.Transparent,0),
            new GradientStop(Colors.Black, 0.5)
         };
      private static GradientStopCollection _gscOpacityMaskStops_TransparentOnRight = new GradientStopCollection{
            new GradientStop(Colors.Black, 0.5),
            new GradientStop(Colors.Transparent, 1)
         };

      /// <summary>
      /// This will apply the present scroll-position resp. -offset.
      /// </summary>
      private TranslateTransform _ttScrollTransform = new TranslateTransform();

      #endregion

      #region --- C'tor ---

      public ScrollableTabPanel()
      {
         this.RenderTransform = _ttScrollTransform;
         this.SizeChanged += new SizeChangedEventHandler(ScrollableTabPanel_SizeChanged);
      }

      #endregion

      #region --- Helpers ---

      /// <summary>
      /// Calculates the HorizontalOffset for a given child-control, based on a desired value.
      /// </summary>
      /// <param name="dblViewport_Left">The left offset of the Viewport.</param>
      /// <param name="dblViewport_Right">The right offset of the Viewport.</param>
      /// <param name="dblChild_Left">The left offset of the control in question.</param>
      /// <param name="dblChild_Right">The right offset of the control in question.</param>
      /// <returns></returns>
      private static double CalculateNewScrollOffset(
            double dblViewport_Left,
            double dblViewport_Right,
            double dblChild_Left,
            double dblChild_Right
         )
      {
         //Retrieve basic information about the position of the Viewport within the Extent of the control.
         bool fIsFurtherToLeft = (dblChild_Left < dblViewport_Left) && (dblChild_Right < dblViewport_Right);
         bool fIsFurtherToRight = (dblChild_Right > dblViewport_Right) && (dblChild_Left > dblViewport_Left);
         bool fIsWiderThanViewport = (dblChild_Right - dblChild_Left) > (dblViewport_Right - dblViewport_Left);

         if (!fIsFurtherToRight && !fIsFurtherToLeft)
            //Don't change anything - the Viewport is completely visible (inside the Extent's bounds)
            return dblViewport_Left;

         if (fIsFurtherToLeft && !fIsWiderThanViewport)
            //The child is to be placed with its left edge equal to the left edge of the Viewport's present offset.
            return dblChild_Left;

         //The child is to be placed with its right edge equal to the right edge of the Viewport's present offset.
         return (dblChild_Right - (dblViewport_Right - dblViewport_Left));
      }

      /// <summary>
      /// Compares the present sizes (Extent/Viewport) against the local values
      /// and updates them, if required.
      /// </summary>
      private void UpdateMembers(Size szExtent, Size szViewportSize)
      {
         if (szExtent != this.Extent)
         {
            //The Extent of the control has changed.
            this.Extent = szExtent;
            if (this.ScrollOwner != null) this.ScrollOwner.InvalidateScrollInfo();
         }

         if (szViewportSize != this.Viewport)
         {
            //The Viewport of the panel has changed.
            this.Viewport = szViewportSize;
            if (this.ScrollOwner != null)
               this.ScrollOwner.InvalidateScrollInfo();
         }

         //Prevent from getting off to the right
         if (this.HorizontalOffset + this.Viewport.Width + this.RightOverflowMargin > this.ExtentWidth)
            SetHorizontalOffset(HorizontalOffset + this.Viewport.Width + this.RightOverflowMargin);

         //Notify UI-subscribers
         NotifyPropertyChanged("CanScroll");
         NotifyPropertyChanged("CanScrollLeft");
         NotifyPropertyChanged("CanScrollRight");
      }

      /// <summary>
      /// Returns the left position of the requested child (in Viewport-coordinates).
      /// </summary>
      /// <param name="uieChild">The child to retrieve the position for.</param>
      private double getLeftEdge(UIElement uieChild)
      {
         double dblWidth = 0;
         double dblWidth_Total = 0;

         //Loop through all child controls, summing up their required width
         foreach (UIElement uie in this.InternalChildren)
         {
            //The width of the current child control
            dblWidth = uie.DesiredSize.Width;

            if (uieChild != null && uieChild == uie)
               //The current child control is the one in question, so disregard its width
               //and return the total width required for all controls further to the left,
               //equaling the left edge of the requested child control.
               return dblWidth_Total;

            //Sum up the overall width while the child control in question hasn't been hit.
            dblWidth_Total += dblWidth;
         }

         //This shouldn't really be hit as the requested control should've been found beforehand.
         return dblWidth_Total;
      }

      /// <summary>
      /// Determines whether the passed child control is only partially visible
      /// (i.e. whether part of it is outside of the Viewport).
      /// </summary>
      /// <param name="uieChild">The child control to be tested.</param>
      /// <returns>
      /// True if part of the control is further to the left or right of the
      /// Viewport, False otherwise.
      /// </returns>
      public bool IsPartlyVisible(UIElement uieChild)
      {
         Rect rctIntersect = GetIntersectionRectangle(uieChild);
         return (!(rctIntersect == Rect.Empty));
      }

      /// <summary>
      /// Determines the visible part of the passed child control, 
      /// measured between 0 (completely invisible) and 1 (completely visible),
      /// that is overflowing into the right invisible portion of the panel.
      /// </summary>
      /// <param name="uieChild">The child control to be tested.</param>
      /// <returns>
      /// <para>A number between 0 (the control is completely invisible resp. outside of
      /// the Viewport) and 1 (the control is completely visible).</para>
      /// <para>All values between 0 and 1 indicate the part that is visible
      /// (i.e. 0.4 would mean that 40% of the control is visible, the remaining
      /// 60% will overflow into the right invisible portion of the panel.</para>
      /// </returns>
      public double PartlyVisiblePortion_OverflowToRight(UIElement uieChild)
      {
         Rect rctIntersect = GetIntersectionRectangle(uieChild);
         double dblVisiblePortion = 1;
         if (
               !(rctIntersect == Rect.Empty)
               &&
               this.CanScrollRight
               &&
               rctIntersect.Width < uieChild.DesiredSize.Width
               &&
               rctIntersect.X > 0
            )
            dblVisiblePortion = rctIntersect.Width / uieChild.DesiredSize.Width;

         return dblVisiblePortion;
      }

      /// <summary>
      /// Determines the visible part of the passed child control, 
      /// measured between 0 (completely invisible) and 1 (completely visible),
      /// that is overflowing into the left invisible portion of the panel.
      /// </summary>
      /// <param name="uieChild">The child control to be tested.</param>
      /// <returns>
      /// <para>A number between 0 (the control is completely invisible resp. outside of
      /// the Viewport) and 1 (the control is completely visible).</para>
      /// <para>All values between 0 and 1 indicate the part that is visible
      /// (i.e. 0.4 would mean that 40% of the control is visible, the remaining
      /// 60% will overflow into the left invisible portion of the panel.</para>
      /// </returns>
      public double PartlyVisiblePortion_OverflowToLeft(UIElement uieChild)
      {
         Rect rctIntersect = GetIntersectionRectangle(uieChild);
         double dblVisiblePortion = 1;
         if (
               !(rctIntersect == Rect.Empty)
               &&
               this.CanScrollLeft
               &&
               rctIntersect.Width < uieChild.DesiredSize.Width
               &&
               rctIntersect.X == 0
            )
            dblVisiblePortion = rctIntersect.Width / uieChild.DesiredSize.Width;

         return dblVisiblePortion;
      }

      /// <summary>
      /// Returns the currently rendered rectangle that makes up the Viewport.
      /// </summary>
      private Rect GetScrollViewerRectangle()
      {
         return new Rect(new Point(0, 0), this.ScrollOwner.RenderSize);
      }

      /// <summary>
      /// Returns the rectangle that defines the outer bounds of a child control.
      /// </summary>
      /// <param name="uieChild">The child/control for which to return the bounding rectangle.</param>
      private Rect GetChildRectangle(UIElement uieChild)
      {
         //Retrieve the position of the requested child inside the ScrollViewer control
         GeneralTransform childTransform = uieChild.TransformToAncestor(this.ScrollOwner);
         return childTransform.TransformBounds(new Rect(new Point(0, 0), uieChild.RenderSize));
      }

      /// <summary>
      /// Returns a Rectangle that contains the intersection between the ScrollViewer's
      /// and the passed child control's boundaries, that is, the portion of the child control
      /// which is currently visibile within the ScrollViewer's Viewport.
      /// </summary>
      /// <param name="uieChild">The child for which to retrieve Rectangle.</param>
      /// <returns></returns>
      private Rect GetIntersectionRectangle(UIElement uieChild)
      {
         //Retrieve the ScrollViewer's rectangle
         Rect rctScrollViewerRectangle = GetScrollViewerRectangle();
         Rect rctChildRect = GetChildRectangle(uieChild);

         //Return the area/rectangle in which the requested child and the ScrollViewer control's Viewport intersect.
         return Rect.Intersect(rctScrollViewerRectangle, rctChildRect);
      }

      /// <summary>
      /// Will remove the OpacityMask for all child controls.
      /// </summary>
      private void RemoveOpacityMasks()
      {
         foreach (UIElement uieChild in Children)
         {
            RemoveOpacityMask(uieChild);
         }
      }

      /// <summary>
      /// Will remove the OpacityMask for all child controls.
      /// </summary>
      private void RemoveOpacityMask(UIElement uieChild)
      {
         uieChild.OpacityMask = null;
      }

      /// <summary>
      /// Will check all child controls and set their OpacityMasks.
      /// </summary>
      private void UpdateOpacityMasks()
      {
         foreach (UIElement uieChild in Children)
         {
            UpdateOpacityMask(uieChild);
         }
      }

      /// <summary>
      /// Takes the given child control and checks as to whether the control is completely
      /// visible (in the Viewport). If not (i.e. if it's only partially visible), an OpacityMask
      /// will be applied so that it fades out into nothingness.
      /// </summary>
      private void UpdateOpacityMask(UIElement uieChild)
      {
         if (uieChild == null) return;

         //Retrieve the ScrollViewer's rectangle
         Rect rctScrollViewerRectangle = GetScrollViewerRectangle();
         if (rctScrollViewerRectangle == Rect.Empty) return;

         //Retrieve the child control's rectangle
         Rect rctChildRect = GetChildRectangle(uieChild);

         if (rctScrollViewerRectangle.Contains(rctChildRect))
            //This child is completely visible, so dump the OpacityMask.
            uieChild.OpacityMask = null;
         else
         {
            double dblPartlyVisiblePortion_OverflowToLeft = PartlyVisiblePortion_OverflowToLeft(uieChild);
            double dblPartlyVisiblePortion_OverflowToRight = PartlyVisiblePortion_OverflowToRight(uieChild);

            if (dblPartlyVisiblePortion_OverflowToLeft < 1 && dblPartlyVisiblePortion_OverflowToRight < 1)
               uieChild.OpacityMask = new LinearGradientBrush(
                     _gscOpacityMaskStops_TransparentOnLeftAndRight,
                     new Point(0, 0),
                     new Point(1, 0));
            else if (dblPartlyVisiblePortion_OverflowToLeft < 1)
               //A part of the child (to the left) remains invisible, so fade out to the left.
               uieChild.OpacityMask = new LinearGradientBrush(
                     _gscOpacityMaskStops_TransparentOnLeft,
                     new Point(1 - dblPartlyVisiblePortion_OverflowToLeft, 0),
                     new Point(1, 0)
                  );
            else if (dblPartlyVisiblePortion_OverflowToRight < 1)
               //A part of the child (to the right) remains invisible, so fade out to the right.
               uieChild.OpacityMask = new LinearGradientBrush(
                     _gscOpacityMaskStops_TransparentOnRight,
                     new Point(0, 0),
                     new Point(dblPartlyVisiblePortion_OverflowToRight, 0)
                  );
            else
               //This child is completely visible, so dump the OpacityMask.
               //Actually, this part should never be reached as, in this case, the very first
               //checkup should've resulted in the child-rect being completely contained in
               //the SV's rect; Well, I'll leave this here anyhow (just to be save).
               uieChild.OpacityMask = null;
         }
      }
      #endregion

      #region --- Overrides ---

      /// <summary>
      /// This is the 1st pass of the layout process. Here, the Extent's size is being determined.
      /// </summary>
      /// <param name="availableSize">The Viewport's rectangle, as obtained after the 1st pass (MeasureOverride).</param>
      /// <returns>The Viewport's final size.</returns>
      protected override Size MeasureOverride(Size availableSize)
      {
         //The default size will not reflect any width (i.e., no children) and always the default height.
         Size resultSize = new Size(0, availableSize.Height);

         //Loop through all child controls ...
         foreach (UIElement uieChild in this.InternalChildren)
         {
            //... retrieve the desired size of the control ...
            uieChild.Measure(availableSize);
            //... and pass this on to the size we need for the Extent
            resultSize.Width += uieChild.DesiredSize.Width;
         }

         UpdateMembers(resultSize, availableSize);

         double dblNewWidth = double.IsPositiveInfinity(availableSize.Width) ?
             resultSize.Width : availableSize.Width;

         resultSize.Width = dblNewWidth;
         return resultSize;
      }

      /// <summary>
      /// This is the 2nd pass of the layout process, where child controls are
      /// being arranged within the panel.
      /// </summary>
      /// <param name="finalSize">The Viewport's rectangle, as obtained after the 1st pass (MeasureOverride).</param>
      /// <returns>The Viewport's final size.</returns>
      protected override Size ArrangeOverride(Size finalSize)
      {
         if (this.InternalChildren == null || this.InternalChildren.Count < 1)
            return finalSize;

         double dblWidth = 0;
         double dblWidth_Total = 0;
         foreach (UIElement uieChild in this.InternalChildren)
         {
            dblWidth = uieChild.DesiredSize.Width;
            uieChild.Arrange(new Rect(dblWidth_Total, 0, dblWidth, uieChild.DesiredSize.Height));
            dblWidth_Total += dblWidth;
         }

         return finalSize;
      }

      protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
      {
         base.OnVisualChildrenChanged(visualAdded, visualRemoved);
         UpdateOpacityMasks();
      }

      protected override void OnChildDesiredSizeChanged(UIElement child)
      {
         base.OnChildDesiredSizeChanged(child);
         UpdateOpacityMasks();
      }

      #endregion

      #region --- IScrollInfo Members ---

      /// <summary>
      /// Sets or retrieves whether the control is allowed to scroll horizontally.
      /// </summary>
      public bool CanHorizontallyScroll
      {
         get { return _fCanScroll_H; }
         set { _fCanScroll_H = value; }
      }

      /// <summary>
      /// Sets or retrieves whether the control is allowed to scroll vertically.
      /// </summary>
      /// <remarks>
      /// This is DISABLED for the control! Due to the internal plumbing of the ScrollViewer
      /// control, this property needs to be accessible without an exception being thrown;
      /// however, setting this property will do plain nothing.
      /// </remarks>
      public bool CanVerticallyScroll
      {
         //We'll never be able to vertically scroll.
         get { return false; }
         set { }
      }

      /// <summary>
      /// Retrieves the height of the control; since no vertical scrolling has been
      /// implemented, this will return the same value at all times.
      /// </summary>
      public double ExtentHeight
      {
         get { return this.Extent.Height; }
      }

      /// <summary>
      /// Retrieves the overall width of the content hosted in the panel (i.e., the width
      /// measured between [far left of the scrollable portion] and [far right of the scrollable portion].
      /// </summary>
      public double ExtentWidth
      {
         get { return this.Extent.Width; }
      }

      /// <summary>
      /// Retrieves the current horizontal scroll offset.
      /// </summary>
      /// <remarks>The setter is private to the class.</remarks>
      public double HorizontalOffset
      {
         get { return _vOffset.X; }
         private set { _vOffset.X = value; }
      }

      /// <summary>
      /// Increments the vertical offset.
      /// </summary>
      /// <remarks>This is unsupported.</remarks>
      public void LineDown()
      {
         throw new InvalidOperationException();
      }

      /// <summary>
      /// Decrements the horizontal offset by the amount specified in the <see cref="LineScrollPixelCount"/> property.
      /// </summary>
      public void LineLeft()
      {
         SetHorizontalOffset(this.HorizontalOffset - this.LineScrollPixelCount);
      }

      /// <summary>
      /// Increments the horizontal offset by the amount specified in the <see cref="LineScrollPixelCount"/> property.
      /// </summary>
      public void LineRight()
      {
         SetHorizontalOffset(this.HorizontalOffset + this.LineScrollPixelCount);
      }

      /// <summary>
      /// Decrements the vertical offset.
      /// </summary>
      /// <remarks>This is unsupported.</remarks>
      public void LineUp()
      {
         throw new InvalidOperationException();
      }

      /// <summary>
      /// Scrolls a child of the panel (Visual) into view.
      /// </summary>
      public System.Windows.Rect MakeVisible(System.Windows.Media.Visual visual, System.Windows.Rect rectangle)
      {
         if (rectangle.IsEmpty || visual == null
           || visual == this || !base.IsAncestorOf(visual))
         { return Rect.Empty; }

         double dblOffsetX = 0;
         UIElement uieControlToMakeVisible = null;
         for (int i = 0; i < this.InternalChildren.Count; i++)
         {
            if ((Visual)this.InternalChildren[i] == visual)
            {
               uieControlToMakeVisible = this.InternalChildren[i];
               dblOffsetX = getLeftEdge(this.InternalChildren[i]);
               break;
            }
         }

         //Set the offset only if the desired element is not already completely visible.
         if (uieControlToMakeVisible != null)
         {
            if (uieControlToMakeVisible == this.InternalChildren[0])
               //If the first child has been selected, go to the very beginning of the scrollable area
               dblOffsetX = 0;
            else if (uieControlToMakeVisible == this.InternalChildren[this.InternalChildren.Count - 1])
               //If the last child has been selected, go to the very end of the scrollable area
               dblOffsetX = this.ExtentWidth - this.Viewport.Width;
            else
               dblOffsetX = CalculateNewScrollOffset(
                        this.HorizontalOffset,
                        this.HorizontalOffset + this.Viewport.Width,
                        dblOffsetX,
                        dblOffsetX + uieControlToMakeVisible.DesiredSize.Width
                  );

            SetHorizontalOffset(dblOffsetX);
            rectangle = new Rect(this.HorizontalOffset, 0, uieControlToMakeVisible.DesiredSize.Width, this.Viewport.Height);
         }

         return rectangle;
      }

      public void MouseWheelDown()
      {
         //We won't be responding to the mouse-wheel.
      }

      public void MouseWheelLeft()
      {
         //We won't be responding to the mouse-wheel.
      }

      public void MouseWheelRight()
      {
         //We won't be responding to the mouse-wheel.
      }

      public void MouseWheelUp()
      {
         //We won't be responding to the mouse-wheel.
      }

      public void PageDown()
      {
         //We won't be responding to vertical paging.
      }

      public void PageLeft()
      {
         //We won't be responding to horizontal paging.
      }

      public void PageRight()
      {
         //We won't be responding to horizontal paging.
      }

      public void PageUp()
      {
         //We won't be responding to vertical paging.
      }

      /// <summary>
      /// Sets or retrieves the ScrollViewer control that hosts the panel.
      /// </summary>
      public ScrollViewer ScrollOwner
      {
         get { return _svOwningScrollViewer; }
         set
         {
            _svOwningScrollViewer = value;
            if (_svOwningScrollViewer != null)
               this.ScrollOwner.Loaded += new RoutedEventHandler(ScrollOwner_Loaded);
            else
               this.ScrollOwner.Loaded -= new RoutedEventHandler(ScrollOwner_Loaded);
         }
      }

      public void SetHorizontalOffset(double offset)
      {
         //Remove all OpacityMasks while scrolling.
         RemoveOpacityMasks();

         //Assure that the horizontal offset always contains a valid value
         this.HorizontalOffset = Math.Max(0, Math.Min(this.ExtentWidth - this.Viewport.Width, Math.Max(0, offset)));

         if (this.ScrollOwner != null) this.ScrollOwner.InvalidateScrollInfo();

         //If you don't want the animation, you would replace all the code further below (up to but not including)
         //the call to InvalidateMeasure() with the following line:
         //_ttScrollTransform.X = (-this.HorizontalOffset);

         //Animate the new offset
         DoubleAnimation daScrollAnimation =
            new DoubleAnimation(
                  _ttScrollTransform.X,
                  (-this.HorizontalOffset),
                  new Duration(this.AnimationTimeSpan),
                  FillBehavior.HoldEnd
               );

         //Note that, depending on distance between the original and the target scroll-position and
         //the duration of the animation, the  acceleration and deceleration effects might be more
         //or less unnoticeable at runtime.
         daScrollAnimation.AccelerationRatio = 0.5;
         daScrollAnimation.DecelerationRatio = 0.5;

         //The childrens' OpacityMask can only be set reliably after the scroll-animation
         //has finished its work, so attach to the animation's Completed event where the
         //masks will be re-created.
         daScrollAnimation.Completed += new EventHandler(daScrollAnimation_Completed);

         _ttScrollTransform.BeginAnimation(
               TranslateTransform.XProperty,
               daScrollAnimation,
               HandoffBehavior.Compose);

         InvalidateMeasure();
      }

      public void SetVerticalOffset(double offset)
      {
         throw new InvalidOperationException();
      }

      public double VerticalOffset
      {
         get { return 0; }
      }

      public double ViewportHeight
      {
         get { return this.Viewport.Height; }
      }

      public double ViewportWidth
      {
         get { return this.Viewport.Width; }
      }

      #endregion

      #region --- Additional Properties ---

      /// <summary>
      /// Retrieves the overall resp. internal/inner size of the control/panel.
      /// </summary>
      /// <remarks>The setter is private to the class.</remarks>
      public Size Extent
      {
         get { return _szControlExtent; }
         private set { _szControlExtent = value; }
      }

      /// <summary>
      /// Retrieves the outer resp. visible size of the control/panel.
      /// </summary>
      /// <remarks>The setter is private to the class.</remarks>
      public Size Viewport
      {
         get { return _szViewport; }
         private set { _szViewport = value; }
      }


      /// <summary>
      /// Retrieves whether the panel's scroll-position is on the far left (i.e. cannot scroll further to the left).
      /// </summary>
      public bool IsOnFarLeft { get { return this.HorizontalOffset == 0; } }

      /// <summary>
      /// Retrieves whether the panel's scroll-position is on the far right (i.e. cannot scroll further to the right).
      /// </summary>
      public bool IsOnFarRight { get { return (this.HorizontalOffset + this.Viewport.Width) == this.ExtentWidth; } }

      /// <summary>
      /// Retrieves whether the panel's viewport is larger than the control's extent, meaning there is hidden content 
      /// that the user would have to scroll for in order to see it.
      /// </summary>
      public bool CanScroll { get { return this.ExtentWidth > this.Viewport.Width; } }

      /// <summary>
      /// Retrieves whether the panel's scroll-position is NOT on the far left (i.e. can scroll to the left).
      /// </summary>
      public bool CanScrollLeft { get { return this.CanScroll && !this.IsOnFarLeft; } }

      /// <summary>
      /// Retrieves whether the panel's scroll-position is NOT on the far right (i.e. can scroll to the right).
      /// </summary>
      public bool CanScrollRight { get { return this.CanScroll && !this.IsOnFarRight; } }

      #endregion

      #region --- Additional Dependency Properties ---

      public static readonly DependencyProperty RightOverflowMarginProperty =
         DependencyProperty.Register("RightOverflowMargin", typeof(int), typeof(ScrollableTabPanel),
         new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
      /// <summary>
      /// Sets or retrieves the Margin that will be applied to the rightmost item in the panel;
      /// This allows for the item applying a negative margin, i.e. when selected.
      /// If set to a value other than zero (being the default), the control will add the value
      /// specified here to the item's right extent.
      /// </summary>
      public int RightOverflowMargin
      {
         get { return (int)GetValue(RightOverflowMarginProperty); }
         set { SetValue(RightOverflowMarginProperty, value); }
      }

      public static readonly DependencyProperty AnimationTimeSpanProperty =
         DependencyProperty.Register("AnimationTimeSpanProperty", typeof(TimeSpan), typeof(ScrollableTabPanel),
         new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 0, 100), FrameworkPropertyMetadataOptions.AffectsRender));
      /// <summary>
      /// Sets or retrieves the the duration (default: 100ms) for the panel's transition-animation that is
      /// started when an item is selected (scroll from the previously selected item to the
      /// presently selected one).
      /// </summary>
      public TimeSpan AnimationTimeSpan
      {
         get { return (TimeSpan)GetValue(AnimationTimeSpanProperty); }
         set { SetValue(AnimationTimeSpanProperty, value); }
      }

      //The amount of pixels to scroll by for the LineLeft() and LineRight() methods.
      public static readonly DependencyProperty LineScrollPixelCountProperty =
         DependencyProperty.Register("LineScrollPixelCount", typeof(int), typeof(ScrollableTabPanel),
         new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender));
      /// <summary>
      /// Sets or retrieves the count of pixels to scroll by when the LineLeft or LineRight methods
      /// are called (default: 15px).
      /// </summary>
      public int LineScrollPixelCount
      {
         get { return (int)GetValue(LineScrollPixelCountProperty); }
         set { SetValue(LineScrollPixelCountProperty, value); }
      }

      #endregion

      #region --- INotifyPropertyChanged ---

      /// <summary>
      /// Raised when a property on this object has a new value.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Called from within this class whenever subscribers (i.e. bindings) are to be notified of a property-change
      /// </summary>
      /// <param name="strPropertyName">The name of the property that has changed.</param>
      private void NotifyPropertyChanged(String strPropertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
         }
      }

      #endregion

      #region --- Event Handlers ---

      /// <summary>
      /// Fired when the ScrollViewer is initially loaded/displayed. 
      /// Required in order to initially setup the childrens' OpacityMasks.
      /// </summary>
      void ScrollOwner_Loaded(object sender, RoutedEventArgs e)
      {
         UpdateOpacityMasks();
      }

      /// <summary>
      /// Fired when the scroll-animation has finished its work, that is, at the
      /// point in time when the ScrollViewerer has reached its final scroll-position
      /// resp. offset, which is when the childrens' OpacityMasks can be updated.
      /// </summary>
      void daScrollAnimation_Completed(object sender, EventArgs e)
      {
         UpdateOpacityMasks();

         //This is required in order to update the TabItems' FocusVisual
         foreach(UIElement uieChild in this.InternalChildren)
            uieChild.InvalidateArrange();
      }

      void ScrollableTabPanel_SizeChanged(object sender, SizeChangedEventArgs e)
      {
         UpdateOpacityMasks();
      }


      #endregion

   }
}
