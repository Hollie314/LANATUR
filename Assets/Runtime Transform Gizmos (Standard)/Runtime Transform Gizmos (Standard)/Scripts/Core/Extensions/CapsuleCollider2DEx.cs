using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //----------------------------------------------------------------------------- 
    // Name: CapsuleCollider2DEx (Public Static Class)
    // Desc: Contains useful extensions and utility functions for the 'CapsuleCollider2D'
    //       class.
    //----------------------------------------------------------------------------- 
    public static class CapsuleCollider2DEx
    {
        #region Public Extensions
        //----------------------------------------------------------------------------- 
        // Name: GetWorldRadius (Public Extension)
        // Desc: Returns the collider's world radius.
        // Rtrn: The collider's world radius.
        //----------------------------------------------------------------------------- 
        public static float GetWorldRadius(this CapsuleCollider2D c)
        {
            int radiusAxis = c.GetRadiusAxisIndex();
            Vector2 scale = c.transform.lossyScale;

            // Radius is half the size along the radius axis scaled by the transform
            return 0.5f * c.size[radiusAxis] * MathEx.FastAbs(scale[radiusAxis]);
        }

        //----------------------------------------------------------------------------- 
        // Name: SetWorldRadius (Public Extension)
        // Desc: Sets the collider's world radius.
        // Parm: worldRadius - World radius.
        //----------------------------------------------------------------------------- 
        public static void SetWorldRadius(this CapsuleCollider2D c, float worldRadius)
        {
            int radiusAxis = c.GetRadiusAxisIndex();
            Vector2 size = c.size;

            // Adjust size along radius axis based on inverse scale
            size[radiusAxis] = (worldRadius * 2.0f) / MathEx.FastAbs(c.transform.lossyScale[radiusAxis]);
            c.size = size;

            c.EnsureValidData();
        }

        //----------------------------------------------------------------------------- 
        // Name: GetWorldHeight (Public Extension)
        // Desc: Returns the collider's world height.
        // Rtrn: The collider's world height.
        //----------------------------------------------------------------------------- 
        public static float GetWorldHeight(this CapsuleCollider2D c)
        {
            int heightAxis = c.GetHeightAxisIndex();

            // Height is size scaled by height axis
            return c.size[heightAxis] * MathEx.FastAbs(c.transform.lossyScale[heightAxis]);
        }

        //----------------------------------------------------------------------------- 
        // Name: GetPartialWorldHeight (Public Extension)
        // Desc: Returns the collider's partial world height. This is the world height
        //       minus 2 times the world radius.
        // Rtrn: The collider's partial world height.
        //----------------------------------------------------------------------------- 
        public static float GetPartialWorldHeight(this CapsuleCollider2D c)
        {
            // Subtract 2 * radius from height
            return Mathf.Max(c.GetWorldHeight() - 2.0f * c.GetWorldRadius(), 0.0f);
        }

        //----------------------------------------------------------------------------- 
        // Name: SetWorldHeight (Public Extension)
        // Desc: Sets the collider's world height.
        // Parm: worldHeight - World height.
        //----------------------------------------------------------------------------- 
        public static void SetWorldHeight(this CapsuleCollider2D c, float worldHeight)
        {
            int heightAxis = c.GetHeightAxisIndex();
            Vector2 size = c.size;

            // Adjust size along height axis based on inverse scale
            size[heightAxis] = worldHeight / MathEx.FastAbs(c.transform.lossyScale[heightAxis]);
            c.size = size;

            c.EnsureValidData();
        }

        //----------------------------------------------------------------------------- 
        // Name: SetPartialWorldHeight (Public Extension)
        // Desc: Sets the collider's partial world height. This is the height of the
        //       collider minus 2 times the radius.
        // Parm: partialWorldHeight - Partial world height.
        //----------------------------------------------------------------------------- 
        public static void SetPartialWorldHeight(this CapsuleCollider2D c, float partialWorldHeight)
        {
            // Add 2 * radius to get full height
            float worldHeight = partialWorldHeight + 2.0f * c.GetWorldRadius();
            c.SetWorldHeight(worldHeight);
        }

        //----------------------------------------------------------------------------- 
        // Name: GetWorldCenter (Public Extension)
        // Desc: Returns the collider's world center.
        // Rtrn: The collider's world center.
        //----------------------------------------------------------------------------- 
        public static Vector3 GetWorldCenter(this CapsuleCollider2D c)
        {
            // Transform local offset into world space
            return c.transform.TransformPoint(c.offset);
        }

        //----------------------------------------------------------------------------- 
        // Name: SetWorldCenter (Public Extension)
        // Desc: Sets the collider's world center.
        // Parm: worldCenter - World center.
        //----------------------------------------------------------------------------- 
        public static void SetWorldCenter(this CapsuleCollider2D c, Vector3 worldCenter)
        {
            // Transform world position into local offset
            c.offset = c.transform.InverseTransformPoint(worldCenter);
        }

        //----------------------------------------------------------------------------- 
        // Name: GetWorldHeightAxis (Public Extension)
        // Desc: Returns the collider's height axis in world space.
        // Rtrn: The collider's height axis in world space.
        //----------------------------------------------------------------------------- 
        public static Vector3 GetWorldHeightAxis(this CapsuleCollider2D c)
        {
            switch (c.direction)
            {
                case CapsuleDirection2D.Horizontal: return c.transform.TransformDirection(Vector3.right);
                case CapsuleDirection2D.Vertical:   return c.transform.TransformDirection(Vector3.up);
                default: return Vector3.zero;
            }
        }

        //----------------------------------------------------------------------------- 
        // Name: GetHeightAxisIndex (Public Extension)
        // Desc: Returns the collider's height axis index.
        // Rtrn: The collider's height axis index.
        //----------------------------------------------------------------------------- 
        public static int GetHeightAxisIndex(this CapsuleCollider2D c)
        {
            switch (c.direction)
            {
                case CapsuleDirection2D.Horizontal: return 0;
                case CapsuleDirection2D.Vertical:   return 1;
                default: return 0;
            }
        }

        //----------------------------------------------------------------------------- 
        // Name: GetRadiusAxisIndex (Public Extension)
        // Desc: Returns the collider's radius axis index.
        // Rtrn: The collider's radius axis index.
        //----------------------------------------------------------------------------- 
        public static int GetRadiusAxisIndex(this CapsuleCollider2D c)
        {
            switch (c.direction)
            {
                case CapsuleDirection2D.Horizontal: return 1;
                case CapsuleDirection2D.Vertical:   return 0;
                default: return 0;
            }
        }

        //----------------------------------------------------------------------------- 
        // Name: EnsureValidData (Public Extension)
        // Desc: Ensures the collider's data is valid (e.g. radius, height etc are valid).
        //----------------------------------------------------------------------------- 
        public static void EnsureValidData(this CapsuleCollider2D c)
        {
            float radius = c.GetWorldRadius();
            float height = c.GetWorldHeight();

            // Clip height. Must be no less than 2 * radius
            if (height < 2.0f * radius) c.SetWorldHeight(2.0f * radius);
        }
        #endregion
    }
    #endregion
}