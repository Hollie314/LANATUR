using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CircleCollider2DEx (Public Static Class)
    // Desc: Contains useful extensions and utility functions for the 'CircleCollider2D'
    //       class.
    //-----------------------------------------------------------------------------
    public static class CircleCollider2DEx
    {
        #region Public Extensions
        //-----------------------------------------------------------------------------
        // Name: GetWorldRadius (Public Extension)
        // Desc: Returns the collider's world radius.
        // Rtrn: The collider's world radius.
        //-----------------------------------------------------------------------------
        public static float GetWorldRadius(this CircleCollider2D c)
        {
            Vector2 scale = c.transform.lossyScale;
            return c.radius * scale.MaxAbsComp();
        }

        //-----------------------------------------------------------------------------
        // Name: SetWorldRadius (Public Extension)
        // Desc: Sets the collider's world radius. This function also takes care of
        //       clipping the collider's radius so that it doesn't take on invalid values.
        // Parm: worldRadius - World radius.
        //-----------------------------------------------------------------------------
        public static void SetWorldRadius(this CircleCollider2D c, float worldRadius)
        {
            Vector2 scale = c.transform.lossyScale;
            c.radius = worldRadius / scale.MaxAbsComp();
            c.EnsureValidData();
        }

        //-----------------------------------------------------------------------------
        // Name: GetWorldCenter (Public Extension)
        // Desc: Returns the collider's world center.
        // Rtrn: The collider's world center.
        //-----------------------------------------------------------------------------
        public static Vector3 GetWorldCenter(this CircleCollider2D c)
        {
            return c.transform.TransformPoint(c.offset);
        }

        //-----------------------------------------------------------------------------
        // Name: SetWorldCenter (Public Extension)
        // Desc: Sets the collider's world center.
        // Parm: worldCenter - World center.
        //-----------------------------------------------------------------------------
        public static void SetWorldCenter(this CircleCollider2D c, Vector3 worldCenter)
        {
            c.offset = c.transform.InverseTransformPoint(worldCenter);
        }

        //-----------------------------------------------------------------------------
        // Name: EnsureValidData (Public Extension)
        // Desc: Ensures the collider's data is valid (i.e. no invalid radius etc).
        //-----------------------------------------------------------------------------
        public static void EnsureValidData(this CircleCollider2D c)
        {
            c.radius = Mathf.Max(c.radius, 0.0f);
        }
        #endregion
    }
    #endregion
}
