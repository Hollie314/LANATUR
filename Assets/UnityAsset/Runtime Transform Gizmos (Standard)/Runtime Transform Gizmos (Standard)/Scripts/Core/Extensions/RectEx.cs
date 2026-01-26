using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTGStandard
{
    #region Public Enumerations
    //-----------------------------------------------------------------------------
    // Name: ERectPivot (Public Enum)
    // Desc: Defines different rectangle pivot points.
    //-----------------------------------------------------------------------------
    public enum ERectPivot
    {
        Center = 0,     // Rectangle center
        MiddleLeft,     // Left edge middle
        MiddleRight,    // Right edge middle
        TopCenter,      // Top edge center
        BottomCenter,   // Bottom edge center
        TopLeft,        // Top left corner
        TopRight,       // Top right corner
        BottomLeft,     // Bottom left corner
        BottomRight     // Bottom right corner
    }
    #endregion

    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RectEx (Public Static Class)
    // Desc: Implements useful extensions and utility functions for the 'Rect' struct.
    // Note: Functions preceded by the 'GUI' prefix assume a coordinate system in
    //       which the Y axis grows downwards. These should be used with the 'GUI' API.
    //-----------------------------------------------------------------------------
    public static class RectEx
    {
        #region Public Extensions
        //-----------------------------------------------------------------------------
        // Name: HasPositiveSize() (Public Extension)
        // Desc: Checks if both the rectangle width and height are >= 0.
        // Rtrn: True if the rect width and height are >= 0 and false otherwise.
        //-----------------------------------------------------------------------------
        public static bool HasPositiveSize(this Rect rect)
        {
            return rect.width >= 0.0f && rect.height >= 0.0f;
        }

        //-----------------------------------------------------------------------------
        // Name: ScreenToGUIRect() (Public Extension)
        // Desc: Given a rectangle in screen space, the function converts the rectangle
        //       to GUI space where the Y axis grows downwards.
        // Rtrn: The GUI rectangle.
        //-----------------------------------------------------------------------------
        public static Rect ScreenToGUIRect(this Rect rect)
        {
            // Note: Don't subtract 1 from 'Screen.height'. 'rect.yMax' is exclusive so it
            //       already contains that extra one pixel we would need to subtract.
            return new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);
        }

        //-----------------------------------------------------------------------------
        // Name: ContainsX() (Public Extension)
        // Desc: Checks if the rectangle contains the specified point along the X axis.
        // Rtrn: True if the rectangle contains 'pt' along the X axis.
        //-----------------------------------------------------------------------------
        public static bool ContainsX(this Rect rect, Vector2 pt)
        {
            return pt.x >= rect.xMin && pt.x < rect.xMax;
        }

        //-----------------------------------------------------------------------------
        // Name: ContainsY() (Public Extension)
        // Desc: Checks if the rectangle contains the specified point along the Y axis.
        // Rtrn: True if the rectangle contains 'pt' along the Y axis.
        //-----------------------------------------------------------------------------
        public static bool ContainsY(this Rect rect, Vector2 pt)
        {
            return pt.y >= rect.yMin && pt.y < rect.yMax;
        }

        //-----------------------------------------------------------------------------
        // Name: Inflate() (Public Extension)
        // Desc: Inflates the rectangle by the specified amount both horizontally and
        //       vertically.
        // Parm: amount - Amount to inflate. Negative amounts will deflate the rectangle.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Inflate(this Rect rect, Vector2 amount)
        {
            // Inflate in all directions
            float halfX = amount.x / 2.0f;
            rect.xMin -= halfX;
            rect.xMax += halfX;

            float halfY = amount.y / 2.0f;
            rect.yMin -= halfY;
            rect.yMax += halfY;

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: Inflate() (Public Extension)
        // Desc: Inflates the rectangle by the specified amount both horizontally and
        //       vertically.
        // Parm: amount - Amount to inflate. Negative amounts will deflate the rectangle.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Inflate(this Rect rect, float amount)
        {
            // Inflate in all directions
            float half = amount / 2.0f;
            rect.xMin -= half;
            rect.xMax += half;
            rect.yMin -= half;
            rect.yMax += half;

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: InflateX() (Public Extension)
        // Desc: Inflates the rectangle by the specified amount along the X axis.
        // Parm: amount - Amount to inflate. Negative amounts will deflate the rectangle.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect InflateX(this Rect rect, float amount)
        {
            // Inflate along X
            float half = amount / 2.0f;
            rect.xMin -= half;
            rect.xMax += half;

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: InflateY() (Public Extension)
        // Desc: Inflates the rectangle by the specified amount along the Y axis.
        // Parm: amount - Amount to inflate. Negative amounts will deflate the rectangle.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect InflateY(this Rect rect, float amount)
        {
            // Inflate along Y
            float half = amount / 2.0f;
            rect.yMin -= half;
            rect.yMax += half;

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: PositiveSize() (Public Extension)
        // Desc: Clones the rectangle and ensures that the clone has a positive width and 
        //       height.
        // Rtrn: The cloned rectangle but with its min/max coords reordered so that it
        //       has a positive width and height.
        //-----------------------------------------------------------------------------
        public static Rect PositiveSize(this Rect rect) 
        {
            // Use rect coords and size by default and adjust if necessary
            float xStart    = rect.xMin;
            float yStart    = rect.yMin;
            float width     = rect.width;
            float height    = rect.height;

            // Adjust if necessary
            if (width < 0.0f)
            {
                width   = -width;
                xStart  = rect.xMax;
            }
            if (height < 0.0f)
            {
                height = -height;
                yStart = rect.yMax;
            }

            // Return a new rectangle
            return new Rect(xStart, yStart, width, height);
        }

        //-----------------------------------------------------------------------------
        // Name: LRB() (Public Extension)
        // Desc: Sets the rectangle's 'xMin', 'xMax' and 'yMax' (left, right, bottom)
        //       coords to the corresponding values in 'other'.
        // Parm: other - The LRB coords are borrowed from this rectangle.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect LRB(this Rect rect, Rect other)
        {
            // Borrow coords
            rect.xMin = other.xMin;
            rect.xMax = other.xMax;
            rect.yMax = other.yMax;

            // Return rect
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: RoundPosition() (Public Extension)
        // Desc: Rounds the position to the nearest integer.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect RoundPosition(this Rect rect)
        {
            rect.position = rect.position.Round();
            return rect;
        }
        
        //-----------------------------------------------------------------------------
        // Name: Position() (Public Extension)
        // Desc: Changes the position.
        // Parm: pos - Rectangle position.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Position(this Rect rect, Vector2 pos)
        {
            rect.position = pos;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: XPos() (Public Extension)
        // Desc: Changes the X position.
        // Parm: x - X position.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect XPos(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: YPos() (Public Extension)
        // Desc: Changes the Y position.
        // Parm: y - Y position.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect YPos(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: XMin() (Public Extension)
        // Desc: Changes the 'xMin' coordinate.
        // Parm: xMin - 'xMin' coordinate.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect XMin(this Rect rect, float xMin)
        {
            rect.xMin = xMin;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: XMax() (Public Extension)
        // Desc: Changes the 'xMax' coordinate.
        // Parm: xMax - 'xMax' coordinate.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect XMax(this Rect rect, float xMax)
        {
            rect.xMax = xMax;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: YMin() (Public Extension)
        // Desc: Changes the 'yMin' coordinate.
        // Parm: yMin - 'yMin' coordinate.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect YMin(this Rect rect, float yMin)
        {
            rect.yMin = yMin;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: YMax() (Public Extension)
        // Desc: Changes the 'yMax' coordinate.
        // Parm: yMax - 'yMax' coordinate.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect YMax(this Rect rect, float yMax)
        {
            rect.yMax = yMax;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: Offset() (Public Extension)
        // Desc: Offsets the rectangle position.
        // Parm: offset - Offset vector.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Offset(this Rect rect, Vector2 offset)
        {
            rect.position += offset;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: OffsetX() (Public Extension)
        // Desc: Offsets the rectangle position along the X axis.
        // Parm: offset - X offset.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect OffsetX(this Rect rect, float offset)
        {
            rect.x += offset;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: OffsetY() (Public Extension)
        // Desc: Offsets the rectangle position along the Y axis.
        // Parm: offset - Y offset.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect OffsetY(this Rect rect, float offset)
        {
            rect.y += offset;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: Width() (Public Extension)
        // Desc: Changes the width.
        // Parm: width - Width.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Width(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: Height() (Public Extension)
        // Desc: Changes the height.
        // Parm: height - Height.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect Height(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: GUIPivotToPoint() (Public Extension)
        // Desc: Converts the specified rectangle pivot to a 2D point belonging to 'this'
        //       rect.
        // Parm: pivot - Rectangle pivot to convert to a 2D point.
        // Rtrn: 2D pivot point belonging to 'this' rect.
        //-----------------------------------------------------------------------------
        public static Vector2 GUIPivotToPoint(this Rect rect, ERectPivot pivot)
        {
            // Check pivot
            switch (pivot)
            {
                case ERectPivot.Center:         return rect.center;
                case ERectPivot.MiddleLeft:     return new Vector2(rect.xMin, rect.center.y);
                case ERectPivot.MiddleRight:    return new Vector2(rect.xMax, rect.center.y);
                case ERectPivot.TopLeft:        return rect.position;
                case ERectPivot.TopCenter:      return new Vector2(rect.center.x, rect.yMin);
                case ERectPivot.TopRight:       return new Vector2(rect.xMax, rect.yMin);
                case ERectPivot.BottomLeft:     return new Vector2(rect.xMin, rect.yMax);
                case ERectPivot.BottomCenter:   return new Vector2(rect.center.x, rect.yMax);
                case ERectPivot.BottomRight:    return new Vector2(rect.xMax, rect.yMax);
                default:                        return Vector2.zero;
            }
        }
        
        //-----------------------------------------------------------------------------
        // Name: GUISize() (Public Extension)
        // Desc: Changes the size from the specified pivot.
        // Parm: size  - Width and height.
        //       pivot - Size pivot.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect GUISize(this Rect rect, float size, ERectPivot pivot = ERectPivot.TopLeft)
        {
            // Check pivot.
            // Note: Avoid generic solution. Potentially slower and has to handle 0 size.
            switch (pivot)
            {
                case ERectPivot.Center: 
                    
                    Vector2 c = rect.center;
                    rect.x = c.x - size / 2.0f; 
                    rect.y = c.y - size / 2.0f; 
                    break;

                case ERectPivot.MiddleLeft:

                    rect.y = rect.center.y - size / 2.0f; 
                    break;

                case ERectPivot.MiddleRight:

                    rect.x = rect.xMax - size;
                    rect.y = rect.center.y - size / 2.0f; 
                    break;

                case ERectPivot.TopLeft:

                    break;

                case ERectPivot.TopCenter:

                    rect.x = rect.center.x - size / 2.0f; 
                    break;

                case ERectPivot.TopRight:

                    rect.x = rect.xMax - size;
                    break;

                case ERectPivot.BottomLeft:

                    rect.y = rect.yMax - size;
                    break;

                case ERectPivot.BottomCenter:

                    rect.x = rect.center.x - size / 2.0f; 
                    rect.y = rect.yMax - size;
                    break;

                case ERectPivot.BottomRight:

                    rect.x = rect.xMax - size;
                    rect.y = rect.yMax - size;
                    break;
            }

            // Set rect size
            rect.width  = size;
            rect.height = size;

            // Return rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: GUISize() (Public Extension)
        // Desc: Changes the size from the specified pivot.
        // Parm: width  - Width.
        //       height - Height.
        //       pivot  - Size pivot.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect GUISize(this Rect rect, float width, float height, ERectPivot pivot = ERectPivot.TopLeft)
        {
            // Check pivot
            // Note: Avoid generic solution. Potentially slower and has to handle 0 size.
            switch (pivot)
            {
                case ERectPivot.Center: 
                    
                    Vector2 c = rect.center;
                    rect.x = c.x - width / 2.0f; 
                    rect.y = c.y - height / 2.0f; 
                    break;

                case ERectPivot.MiddleLeft:

                    rect.y = rect.center.y - height / 2.0f; 
                    break;

                case ERectPivot.MiddleRight:

                    rect.x = rect.xMax - width;
                    rect.y = rect.center.y - height / 2.0f; 
                    break;

                case ERectPivot.TopLeft:

                    break;

                case ERectPivot.TopCenter:

                    rect.x = rect.center.x - width / 2.0f; 
                    break;

                case ERectPivot.TopRight:

                    rect.x = rect.xMax - width;
                    break;

                case ERectPivot.BottomLeft:

                    rect.y = rect.yMax - height;
                    break;

                case ERectPivot.BottomCenter:

                    rect.x = rect.center.x - width / 2.0f; 
                    rect.y = rect.yMax - height;
                    break;

                case ERectPivot.BottomRight:

                    rect.x = rect.xMax - width;
                    rect.y = rect.yMax - height;
                    break;
            }

            // Set rect size
            rect.width  = width;
            rect.height = height;

            // Return rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: GUISize() (Public Extension)
        // Desc: Changes the size from the specified pivot.
        // Parm: size  - Size.
        //       pivot - Size pivot.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect GUISize(this Rect rect, Vector2 size, ERectPivot pivot = ERectPivot.TopLeft)
        {
            return rect.GUISize(size.x, size.y, pivot);
        }

        //-----------------------------------------------------------------------------
        // Name: RelativeTo (Public Extension)
        // Desc: Changes the position so that it is expressed relative to the position
        //       of 'origin' rectangle.
        // Parm: origin - The rectangle that represents the coordinate system origin.
        // Rtrn: The new rectangle with the same size but its position expressed relative
        //       to the position of 'origin' rectangle.
        //-----------------------------------------------------------------------------
        public static Rect RelativeTo(this Rect rect, Rect origin)
        {
            rect.x = rect.x - origin.x;
            rect.y = rect.y - origin.y;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: CenterOnScreen() (Public Extension)
        // Desc: Centers the rectangle on the screen.
        // Rtrn: The updated rectangle.
        //-----------------------------------------------------------------------------
        public static Rect CenterOnScreen(this Rect rect)
        {
            // Calculate the screen's middle pixel
            float midX = (Screen.width - 1.0f) / 2.0f;
            float midY = (Screen.height - 1.0f) / 2.0f;

            // Calculate the window position
            rect.position = new Vector2(midX - rect.width / 2.0f, midY - rect.height / 2.0f);

            // Return updated rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: TopLeft() (Public Static Function)
	    // Desc: Places the rectangle in the top left corner of 'other' and sets it
        //       width and height to the specified values.
        // Parm: other  - Alignment rectangle.
        //       width  - Width.
        //       height - Height.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect TopLeft(this Rect rect, Rect other, float width = 0.0f, float height = 0.0f)
        {
            // Set rect size
            rect.width  = width;
            rect.height = height;

            // Align
            rect.x += (other.xMin - rect.xMin);
            rect.y += (other.yMin - rect.yMin);

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: TopRight() (Public Static Function)
	    // Desc: Places the rectangle in the top right corner of 'other' and sets it
        //       width and height to the specified values.
        // Parm: other  - Alignment rectangle.
        //       width  - Width.
        //       height - Height.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect TopRight(this Rect rect, Rect other, float width = 0.0f, float height = 0.0f)
        {
            // Set rect size
            rect.width  = width;
            rect.height = height;

            // Align
            rect.x += (other.xMax - rect.xMax);
            rect.y += (other.yMin - rect.yMin);

            // Return new rectangle
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: LeftOf() (Public Static Function)
	    // Desc: Places the rectangle to the left of 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The rectangle sitting at the left of 'other'.
	    //-----------------------------------------------------------------------------
        public static Rect LeftOf(this Rect rect, Rect other)
        {
            rect.x += (other.xMin - rect.xMax);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: RightOf() (Public Static Function)
	    // Desc: Places the rectangle to the right of 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The rectangle sitting at the right of 'other'.
	    //-----------------------------------------------------------------------------
        public static Rect RightOf(this Rect rect, Rect other)
        {
            rect.x += (other.xMax - rect.xMin);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: GUIBelow() (Public Static Function)
	    // Desc: Places the rectangle below 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The rectangle sitting below 'other'.
	    //-----------------------------------------------------------------------------
        public static Rect GUIBelow(this Rect rect, Rect other)
        {
            rect.y += (other.yMax - rect.yMin);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: GUIAbove() (Public Static Function)
	    // Desc: Places the rectangle above 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The rectangle sitting above 'other'.
	    //-----------------------------------------------------------------------------
        public static Rect GUIAbove(this Rect rect, Rect other)
        {
            rect.y += (other.yMin - rect.yMax);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: GUIAlignBottom() (Public Static Function)
	    // Desc: Aligns the rectangle's bottom edge to the bottom edge of 'other' rectangle.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect GUIAlignBottom(this Rect rect, Rect other)
        {
            rect.y += (other.yMax - rect.yMax);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: GUIAlignTop() (Public Static Function)
	    // Desc: Aligns the rectangle's top edge to the top edge of 'other' rectangle.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect GUIAlignTop(this Rect rect, Rect other)
        {
            rect.y += (other.yMin - rect.yMin);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: AlignRight() (Public Static Function)
	    // Desc: Aligns the rectangle's right edge to the right edge of 'other' rectangle.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect AlignRight(this Rect rect, Rect other)
        {
            rect.x += (other.xMax - rect.xMax);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: AlignLeft() (Public Static Function)
	    // Desc: Aligns the rectangle's left edge to the left edge of 'other' rectangle.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect AlignLeft(this Rect rect, Rect other)
        {
            rect.x += (other.xMin - rect.xMin);
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: AlignCenter() (Public Static Function)
	    // Desc: Centers the rectangle inside 'other'. 
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect AlignCenter(this Rect rect, Rect other)
        {
            // Store other rect center
            Vector2 c = other.center;

            // Set rect center without changing its size
            rect.x = c.x - rect.width / 2.0f;
            rect.y = c.y - rect.height / 2.0f;

            // Return updated rect
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: AlignCenterX() (Public Static Function)
	    // Desc: Centers the rectangle along X inside 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect AlignCenterX(this Rect rect, Rect other)
        {
            rect.x = other.center.x - rect.width / 2.0f;
            return rect;
        }

        //-----------------------------------------------------------------------------
	    // Name: AlignCenterY() (Public Static Function)
	    // Desc: Centers the rectangle along Y inside 'other'.
        // Parm: other - Alignment rectangle.
        // Rtrn: The updated rectangle.
	    //-----------------------------------------------------------------------------
        public static Rect AlignCenterY(this Rect rect, Rect other)
        {
            rect.y = other.center.y - rect.height / 2.0f;
            return rect;
        }

        //-----------------------------------------------------------------------------
        // Name: TestRectInside() (Public Extension)
        // Desc: Determines if the specified rectangle is fully contained within 'this'
        //       rectangle.
        // Parm: other - The other rectangle.
        // Rtrn: True if 'other' lies completely inside of 'this' rectangle and false
        //       otherwise.
        //-----------------------------------------------------------------------------
        public static bool TestRectInside(this Rect rect, Rect other)
        {
            // Sort rects
            rect  = rect.PositiveSize();
            other = other.PositiveSize();

            // All corner points must reside inside 'this' rect.
            if (other.xMin < rect.xMin || other.xMin > rect.xMax) return false;
            if (other.xMax < rect.xMin || other.xMax > rect.xMax) return false;

            if (other.yMin < rect.yMin || other.yMin > rect.yMax) return false;
            if (other.yMax < rect.yMin || other.yMax > rect.yMax) return false;

            // The other rect is completely inside
            return true;
        }

        //-----------------------------------------------------------------------------
	    // Name: Restrict() (Public Static Function)
	    // Desc: Keeps the rectangle within the bounds of rectangle 'bounds'.
	    // Parm: bounds - Defines the area where the rectangle is allowed to exist.
        //       r      - Returns the new rectangle. If no adjustments have to be made,
        //                this is the same as 'this' rectangle.
	    // Rtrn: True if changes had to be made to keep the rectangle it within bounds
        //       and false otherwise. 
	    //-----------------------------------------------------------------------------
        public static bool Restrict(this Rect rect, Rect bounds, out Rect r)
        {
            // Keep track of whether we made any changes 
            bool changed = false;

            // Init output rectangle
            r = rect;

            // Push coordinates inside
			if (r.xMin < bounds.xMin)
			{
				float dx = bounds.xMin - r.xMin;
                r.x += dx;
				changed = true;
			}
			if (r.yMin < bounds.yMin)
			{
				float dy = bounds.yMin - r.yMin;
				r.y += dy;
				changed = true;
			}
			if (r.xMax > bounds.xMax)
			{
                float dx = bounds.xMax - r.xMax;
				r.x += dx;
				changed = true;
			}
			if (r.yMax > bounds.yMax)
			{
                float dy = bounds.yMax - r.yMax;
				r.y += dy;
				changed = true;
			}

            // Return result
            return changed;
        }

        //-----------------------------------------------------------------------------
        // Name: Clip() (Public Extension)
        // Desc: Clips the rectangle to the specified clip rect and stores the result
        //       in 'clippedRect'.
        // Parm: clipRect    - The clip rectangle. Defines the clipping area.
        //       clippedRect - Returns the clipped rectangle.
        // Rtrn: True if the rectangle was either completely inside the clipping area
        //       and no clipping was needed or if it was clipped but part of it still
        //       remains inside the clipping area. False if the rectangle was either
        //       completely outside the clipping area or if it was clipped and its width
        //       and height have become 0.
        //-----------------------------------------------------------------------------
        public static bool Clip(this Rect rect, Rect clipRect, out Rect clippedRect)
        {
            // Default to original rect by default
            clippedRect = rect;

            // Force the rectangles to have positive size. It's easier this way.
            clippedRect = rect.PositiveSize();
            clipRect    = clipRect.PositiveSize();

            // Is the rectangle completely outside the clip rect?
            if (clippedRect.xMax <= clipRect.xMin) return false;
            if (clippedRect.xMin >= clipRect.xMax) return false;
            if (clippedRect.yMax <= clipRect.yMin) return false;
            if (clippedRect.yMin >= clipRect.yMax) return false;

            // Clip rectangle
            if (clippedRect.xMin < clipRect.xMin) clippedRect.xMin = clipRect.xMin;
            if (clippedRect.xMax > clipRect.xMax) clippedRect.xMax = clipRect.xMax;
            if (clippedRect.yMin < clipRect.yMin) clippedRect.yMin = clipRect.yMin;
            if (clippedRect.yMax > clipRect.yMax) clippedRect.yMax = clipRect.yMax;

            // Clipping can cause the rectangle to become degenerate
            if (clippedRect.width < 1e-5f || clippedRect.height < 1e-5f)
                return false;

            // Success!
            return true;
        }
        #endregion

        #region Public Static Functions
        //-----------------------------------------------------------------------------
        // Name: FromPoints() (Public Static Function)
        // Desc: Creates and returns a rectangle that encloses all the specified points.
        // Parm: points - Indexable collection of points which must be enclosed by the
        //                calculated rectangle.
        // Rtrn: A rectangle which encloses all points inside 'points'.
        //-----------------------------------------------------------------------------
        public static Rect FromPoints(IList<Vector2> points)
        {
            // Keep track of min/max coords
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

            // Loop through each point
            int pointCount = points.Count();
            for (int i = 0; i < pointCount; ++i)
            {
                // Update min/max coords
                if (points[i].x < xMin) xMin = points[i].x;
                if (points[i].x > xMax) xMax = points[i].x;
                if (points[i].y < yMin) yMin = points[i].y;
                if (points[i].y > yMax) yMax = points[i].y;
            }

            // Return the rectangle
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        //-----------------------------------------------------------------------------
        // Name: FromPoints() (Public Static Function)
        // Desc: Creates and returns a rectangle that encloses all the specified points.
        // Parm: points - Indexable collection of points which must be enclosed by the
        //                calculated rectangle. Only points with a Z coord >= 0 are taken
        //                into account.
        // Rtrn: A rectangle which encloses all points inside 'points'. If all points
        //       are behind the camera, the rectangle has a negative width and height.
        //-----------------------------------------------------------------------------
        public static Rect FromPoints(IList<Vector3> points)
        {
            // Keep track of min/max coords
            bool found = false;
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

            // Loop through each point
            int pointCount = points.Count();
            for (int i = 0; i < pointCount; ++i)
            {
                // Update min/max coords
                if (points[i].z >= 0.0f)
                {
                    if (points[i].x < xMin) xMin = points[i].x;
                    if (points[i].x > xMax) xMax = points[i].x;
                    if (points[i].y < yMin) yMin = points[i].y;
                    if (points[i].y > yMax) yMax = points[i].y;
                    found = true;
                }
            }

            // Return the rectangle
            return found ? new Rect(xMin, yMin, xMax - xMin, yMax - yMin) : new Rect(0.0f, 0.0f, -1.0f, -1.0f);
        }
        #endregion
    }
    #endregion
}
