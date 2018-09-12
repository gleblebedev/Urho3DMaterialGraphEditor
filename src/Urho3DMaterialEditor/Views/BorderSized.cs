using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toe.Scripting.WPF.Views;

namespace Urho3DMaterialEditor.Views {
    class BorderSized : Border {

        public BorderSized() {
            MouseDown += _MouseDown;
            MouseMove += _MouseMove;
            MouseUp += _MouseUp;
        }

        private void _MouseUp(object sender, MouseButtonEventArgs e) {
            drag = false;
        }

        private void _MouseMove(object sender, MouseEventArgs e) {
            if(e.LeftButton!= MouseButtonState.Released) { drag = false;return; }
            if (drag) {
                Point point = Mouse.GetPosition(this);
                double offset_x = point.X - this.LastPoint.X;
                double offset_y = point.Y - this.LastPoint.Y;

                double new_x = Canvas.GetLeft(this);
                double new_y = Canvas.GetTop(this);
                double new_width = this.Width;
                double new_height = this.Height;
                e.Handled = false;
            }
        }

        private void _MouseDown(object sender, MouseButtonEventArgs e) {
            drag = true;
        }
        private bool drag;







        private enum HitType {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        private bool DragInProgress = false;
        private Point LastPoint;
        private HitType MouseHitType = HitType.None;

        static BorderSized curBord = null;

        private HitType SetHitType(Point point) {
           //var p = TranslatePoint(new Point(0, 0), (this));
           // double currentLeft = p.X;
           // double currentTop = p.Y;
            // return HitType.LR;

            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            double right = left + this.ActualWidth;
            double bottom = top + this.ActualHeight;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP) {
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP) {
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        private void SetMouseCursor() {
            Cursor desired_cursor = Cursors.Arrow;
            switch (this.MouseHitType) {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            if (this.Cursor != desired_cursor) this.Cursor = desired_cursor;
        }

        static public void canvas1_MouseDown(object sender, MouseButtonEventArgs e) {
            //if (e.LeftButton != MouseButtonState.Pressed) return;
            var canvas1 = sender as UIElement;
            if (canvas1 == null) return;

            var movObj = e.OriginalSource as BorderSized;
            if (movObj == null) return;


            movObj.MouseHitType = movObj.SetHitType(Mouse.GetPosition(canvas1));
            movObj.SetMouseCursor();
            if (movObj.MouseHitType == HitType.None) return;

            movObj.LastPoint = Mouse.GetPosition(canvas1);
            movObj.DragInProgress = true;
            curBord = movObj;
        }

        public static void canvas1_MouseMove(object sender, MouseEventArgs e) {

            var canvas1 = sender as UIElement;
            if (canvas1 == null) return;

            // if (curBord != movObj  ) { movObj.DragInProgress = false; return; }e.LeftButton != MouseButtonState.Pressed ||
            if ( curBord == null) {
                var movObj2 = e.OriginalSource as BorderSized;
                if (movObj2 == null) return;

                movObj2.MouseHitType = movObj2.SetHitType(Mouse.GetPosition(canvas1));
                movObj2.SetMouseCursor();
                movObj2.DragInProgress = false;
                curBord = null;
                return;
            }
            var movObj = curBord;


            Point point = Mouse.GetPosition(canvas1);
            double offset_x = point.X - movObj.LastPoint.X;
            double offset_y = point.Y - movObj.LastPoint.Y;

            double new_x = Canvas.GetLeft(movObj);
            double new_y = Canvas.GetTop(movObj);
            double new_width = movObj.Width;
            double new_height = movObj.Height;

            switch (movObj.MouseHitType) {
                case HitType.Body:
                    new_x += offset_x;
                    new_y += offset_y;
                    break;
                case HitType.UL:
                    new_x += offset_x;
                    new_y += offset_y;
                    new_width -= offset_x;
                    new_height -= offset_y;
                    break;
                case HitType.UR:
                    new_y += offset_y;
                    new_width += offset_x;
                    new_height -= offset_y;
                    break;
                case HitType.LR:
                    new_width += offset_x;
                    new_height += offset_y;
                    break;
                case HitType.LL:
                    new_x += offset_x;
                    new_width -= offset_x;
                    new_height += offset_y;
                    break;
                case HitType.L:
                    new_x += offset_x;
                    new_width -= offset_x;
                    break;
                case HitType.R:
                    new_width += offset_x;
                    break;
                case HitType.B:
                    new_height += offset_y;
                    break;
                case HitType.T:
                    new_y += offset_y;
                    new_height -= offset_y;
                    break;

            }
            if ((new_width > 0) && (new_height > 0)) {
                Canvas.SetLeft(movObj, new_x);
                Canvas.SetTop(movObj, new_y);
                movObj.Width = new_width;
                movObj.Height = new_height;

                movObj.LastPoint = point;
            }
        }
    

        public static void canvas1_MouseUp(object sender, MouseButtonEventArgs e) {
            var movObj = e.Source as BorderSized;
            if (movObj == null) return;

            curBord =null;
            movObj.DragInProgress = false;
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using Toe.Scripting.WPF.Views;

//namespace Urho3DMaterialEditor.Views {
//    class BorderSized : Border {

//        public BorderSized() {
//            //MouseDown += canvas1_MouseDown;
//            //MouseMove +=canvas1_MouseMove;
//            //MouseUp += canvas1_MouseUp;
//        }

//        private enum HitType {
//            None, Body, UL, UR, LR, LL, L, R, T, B
//        };

//        private bool DragInProgress = false;
//        private Point LastPoint;
//        private HitType MouseHitType = HitType.None;

//        static BorderSized curBord = null;

//        private HitType SetHitType(Point point) {
//            //var p = TranslatePoint(new Point(0, 0), (this));
//            // double currentLeft = p.X;
//            // double currentTop = p.Y;
//            // return HitType.LR;

//            double left = Canvas.GetLeft(this);
//            double top = Canvas.GetTop(this);
//            double right = left + this.ActualWidth;
//            double bottom = top + this.ActualHeight;
//            if (point.X < left) return HitType.None;
//            if (point.X > right) return HitType.None;
//            if (point.Y < top) return HitType.None;
//            if (point.Y > bottom) return HitType.None;

//            const double GAP = 10;
//            if (point.X - left < GAP) {
//                if (point.Y - top < GAP) return HitType.UL;
//                if (bottom - point.Y < GAP) return HitType.LL;
//                return HitType.L;
//            }
//            if (right - point.X < GAP) {
//                if (point.Y - top < GAP) return HitType.UR;
//                if (bottom - point.Y < GAP) return HitType.LR;
//                return HitType.R;
//            }
//            if (point.Y - top < GAP) return HitType.T;
//            if (bottom - point.Y < GAP) return HitType.B;
//            return HitType.Body;
//        }

//        private void SetMouseCursor() {
//            Cursor desired_cursor = Cursors.Arrow;
//            switch (this.MouseHitType) {
//                case HitType.None:
//                    desired_cursor = Cursors.Arrow;
//                    break;
//                case HitType.Body:
//                    desired_cursor = Cursors.ScrollAll;
//                    break;
//                case HitType.UL:
//                case HitType.LR:
//                    desired_cursor = Cursors.SizeNWSE;
//                    break;
//                case HitType.LL:
//                case HitType.UR:
//                    desired_cursor = Cursors.SizeNESW;
//                    break;
//                case HitType.T:
//                case HitType.B:
//                    desired_cursor = Cursors.SizeNS;
//                    break;
//                case HitType.L:
//                case HitType.R:
//                    desired_cursor = Cursors.SizeWE;
//                    break;
//            }

//            if (this.Cursor != desired_cursor) this.Cursor = desired_cursor;
//        }

//        static public void canvas1_MouseDown(object sender, MouseButtonEventArgs e) {
//            //if (e.LeftButton != MouseButtonState.Pressed) return;
//            var canvas1 = sender as UIElement;
//            if (canvas1 == null) return;

//            var movObj = e.OriginalSource as BorderSized;
//            if (movObj == null) return;


//            movObj.MouseHitType = movObj.SetHitType(Mouse.GetPosition(canvas1));
//            movObj.SetMouseCursor();
//            if (movObj.MouseHitType == HitType.None) return;

//            movObj.LastPoint = Mouse.GetPosition(canvas1);
//            movObj.DragInProgress = true;
//            curBord = movObj;
//        }

//        public static void canvas1_MouseMove(object sender, MouseEventArgs e) {

//            var canvas1 = sender as UIElement;
//            if (canvas1 == null) return;

//            // if (curBord != movObj  ) { movObj.DragInProgress = false; return; }e.LeftButton != MouseButtonState.Pressed ||
//            if (curBord == null) {
//                var movObj2 = e.OriginalSource as BorderSized;
//                if (movObj2 == null) return;

//                movObj2.MouseHitType = movObj2.SetHitType(Mouse.GetPosition(canvas1));
//                movObj2.SetMouseCursor();
//                movObj2.DragInProgress = false;
//                curBord = null;
//                return;
//            }
//            var movObj = curBord;


//            Point point = Mouse.GetPosition(canvas1);
//            double offset_x = point.X - movObj.LastPoint.X;
//            double offset_y = point.Y - movObj.LastPoint.Y;

//            double new_x = Canvas.GetLeft(movObj);
//            double new_y = Canvas.GetTop(movObj);
//            double new_width = movObj.Width;
//            double new_height = movObj.Height;

//            switch (movObj.MouseHitType) {
//                case HitType.Body:
//                    new_x += offset_x;
//                    new_y += offset_y;
//                    break;
//                case HitType.UL:
//                    new_x += offset_x;
//                    new_y += offset_y;
//                    new_width -= offset_x;
//                    new_height -= offset_y;
//                    break;
//                case HitType.UR:
//                    new_y += offset_y;
//                    new_width += offset_x;
//                    new_height -= offset_y;
//                    break;
//                case HitType.LR:
//                    new_width += offset_x;
//                    new_height += offset_y;
//                    break;
//                case HitType.LL:
//                    new_x += offset_x;
//                    new_width -= offset_x;
//                    new_height += offset_y;
//                    break;
//                case HitType.L:
//                    new_x += offset_x;
//                    new_width -= offset_x;
//                    break;
//                case HitType.R:
//                    new_width += offset_x;
//                    break;
//                case HitType.B:
//                    new_height += offset_y;
//                    break;
//                case HitType.T:
//                    new_y += offset_y;
//                    new_height -= offset_y;
//                    break;

//            }
//            if ((new_width > 0) && (new_height > 0)) {
//                Canvas.SetLeft(movObj, new_x);
//                Canvas.SetTop(movObj, new_y);
//                movObj.Width = new_width;
//                movObj.Height = new_height;

//                movObj.LastPoint = point;
//            }
//        }


//        public static void canvas1_MouseUp(object sender, MouseButtonEventArgs e) {
//            var movObj = e.Source as BorderSized;
//            if (movObj == null) return;

//            curBord = null;
//            movObj.DragInProgress = false;
//        }
//    }
//}