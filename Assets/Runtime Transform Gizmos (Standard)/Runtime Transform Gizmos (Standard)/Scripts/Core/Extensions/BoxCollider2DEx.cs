using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxCollider2DEx (Public Static Class)
    // Desc: Contains useful extensions and utility functions for the 'BoxCollider2D'
    //       class.
    //-----------------------------------------------------------------------------
    public static class BoxCollider2DEx
    {
        #region Public Extensions
        //-----------------------------------------------------------------------------
        // Name: GetWorldSize (Public Extension)
        // Desc: Returns the collider's world size.
        // Rtrn: The collider's world size.
        //-----------------------------------------------------------------------------
        public static Vector2 GetWorldSize(this BoxCollider2D c)
        {
            return Vector2Ex.Abs(Vector2.Scale(c.size, c.transform.lossyScale));
        }

        //-----------------------------------------------------------------------------
        // Name: SetWorldSize (Public Extension)
        // Desc: Sets the collider's world size. This function also takes care of
        //       clipping the collider's size so that it doesn't take on invalid values.
        // Parm: worldSize - The collider's world size.
        //-----------------------------------------------------------------------------
        public static void SetWorldSize(this BoxCollider2D c, Vector2 worldSize)
        {
            Vector2 invScale = c.transform.lossyScale.Inverse();
            c.size = Vector2.Scale(worldSize, invScale);
            c.EnsureValidData();
        }

        //-----------------------------------------------------------------------------
        // Name: GetWorldCenter (Public Extension)
        // Desc: Returns the collider's world center.
        // Rtrn: The collider's world center.
        //-----------------------------------------------------------------------------
        public static Vector3 GetWorldCenter(this BoxCollider2D c)
        {
            Vector3 center = c.offset;
            return c.transform.TransformPoint(new Vector3(center.x, center.y, 0.0f));
        }

        //-----------------------------------------------------------------------------
        // Name: SetWorldCenter (Public Extension)
        // Desc: Sets the collider's world center.
        // Parm: worldCenter - World center.
        //-----------------------------------------------------------------------------
        public static void SetWorldCenter(this BoxCollider2D c, Vector3 worldCenter)
        {
            c.offset = c.transform.InverseTransformPoint(worldCenter);
        }

        //-----------------------------------------------------------------------------
        // Name: EnsureValidData (Public Extension)
        // Desc: Ensures the collider's data is valid (i.e. no invalid size etc).
        //-----------------------------------------------------------------------------
        public static void EnsureValidData(this BoxCollider2D c)
        {
            c.size = Vector2Ex.Abs(c.size);
        }
        #endregion
    }
    #endregion
}
