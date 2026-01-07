using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: TextureManager (Public Singleton Class)
    // Desc: Manages a collection of textures used throughout the plugin and implements
    //       relevant utility functions.
    //-----------------------------------------------------------------------------
    public class TextureManager : Singleton<TextureManager>
    {
        #region Private Fields
        Texture2D[] mGizmoAxisLables = new Texture2D[3];    // Stores a label texture for each of the X, Y and Z gizmo axes
        Texture2D   mOrthoCamMode;                          // Texture that indicates that a camera is in ortho mode
        Texture2D   mPerspectiveCamMode;                    // Texture that indicates that a camera is in perspective mode
        Texture2D   mGizmoLabelBG;                          // Gizmo label BG texture
        Texture2D   mGizmoLabelBGBorder;                    // Gizmo label BG border texture

        Texture2D   mWarning;                               // The warning icon
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: orthoCamMode (Public Property)
        // Desc: Returns the texture that indicates that a camera is in ortho mode.
        //-----------------------------------------------------------------------------
        public Texture2D orthoCamMode       { get { if (mOrthoCamMode == null) mOrthoCamMode = Resources.Load<Texture2D>("Gizmos/Textures/OrthoCameraMode"); return mOrthoCamMode; } }

        //-----------------------------------------------------------------------------
        // Name: perspectiveCamMode (Public Property)
        // Desc: Returns the texture that indicates that a camera is in perspective mode.
        //-----------------------------------------------------------------------------
        public Texture2D perspectiveCamMode { get { if (mPerspectiveCamMode == null) mPerspectiveCamMode = Resources.Load<Texture2D>("Gizmos/Textures/PerspectiveCameraMode"); return mPerspectiveCamMode; } }

        //-----------------------------------------------------------------------------
        // Name: gizmoLabelBG (Public Property)
        // Desc: Returns the texture that is used to draw gizmo label backgrounds.
        //-----------------------------------------------------------------------------
        public Texture2D gizmoLabelBG       { get { if (mGizmoLabelBG == null) mGizmoLabelBG = Resources.Load<Texture2D>("Gizmos/Textures/GizmoLabelBG"); return mGizmoLabelBG; } }

        //-----------------------------------------------------------------------------
        // Name: gizmoLabelBGBorder (Public Property)
        // Desc: Returns the texture that is used to draw gizmo label background borders.
        //-----------------------------------------------------------------------------
        public Texture2D gizmoLabelBGBorder { get { if (mGizmoLabelBGBorder == null) mGizmoLabelBGBorder = Resources.Load<Texture2D>("Gizmos/Textures/GizmoLabelBGBorder"); return mGizmoLabelBGBorder; } }
        
        //-----------------------------------------------------------------------------
        // Name: warning (Public Property)
        // Desc: Returns the warning icon texture.
        //-----------------------------------------------------------------------------
        public Texture2D warning            { get { if (mWarning == null) mWarning = Resources.Load<Texture2D>("UI/Textures/Warning"); return mWarning; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: GetGizmoAxisLabel() (Public Function)
        // Desc: Returns a gizmo axis label texture for the specified gizmo axis.
        // Parm: axis - The gizmo axis index: (0 = X, 1 = Y, 2 = Z).
        // Rtrn: The label texture for the specified gizmo axis.
        //-----------------------------------------------------------------------------
        public Texture2D GetGizmoAxisLabel(int axis)
        {
            // Create the texture if necessary
            if (mGizmoAxisLables[axis] == null)
            {
                // Check what texture we're dealing with
                switch (axis)
                {
                    case 0: mGizmoAxisLables[0] = Resources.Load<Texture2D>("Gizmos/Textures/XAxisLabel"); break;
                    case 1: mGizmoAxisLables[1] = Resources.Load<Texture2D>("Gizmos/Textures/YAxisLabel"); break;
                    case 2: mGizmoAxisLables[2] = Resources.Load<Texture2D>("Gizmos/Textures/ZAxisLabel"); break;
                }
            }

            // Return texture
            return mGizmoAxisLables[axis];
        }
        #endregion
    }
    #endregion
}
