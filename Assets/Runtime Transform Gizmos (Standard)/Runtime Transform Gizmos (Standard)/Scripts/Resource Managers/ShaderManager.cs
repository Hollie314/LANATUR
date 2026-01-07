using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: ShaderManager (Public Singleton Class)
    // Desc: Manages a collection of shaders used throughout the plugin and implements
    //       relevant utility functions.
    //-----------------------------------------------------------------------------
    public class ShaderManager : Singleton<ShaderManager>
    {
        #region Private Fields
        Shader mRTGizmoHandle;  // Gizmo handles
        Shader mRTGridShader;   // Grid
        Shader mRTCameraBG;     // Camera background
        Shader mRTUnlit;        // Unlit
        Shader mRT2D;           // 2D rendering
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: rtGizmoHandle (Public Property)
        // Desc: Returns the shader which renders gizmo handles.
        //-----------------------------------------------------------------------------
        public Shader rtGizmoHandle
        {
            get
            {
                if (mRTGizmoHandle == null) mRTGizmoHandle = Shader.Find("Hidden/RTG/RTGizmoHandle");
                return mRTGizmoHandle;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rtGrid (Public Property)
        // Desc: Returns the shader which renders the 'RTGrid'.
        //-----------------------------------------------------------------------------
        public Shader rtGrid
        {
            get
            {
                if (mRTGridShader == null) mRTGridShader = Shader.Find("Hidden/RTG/RTGrid");
                return mRTGridShader;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rtCameraBG (Public Property)
        // Desc: Returns the shader which renders the 'RTCamera' background.
        //-----------------------------------------------------------------------------
        public Shader rtCameraBG
        {
            get
            {
                if (mRTCameraBG == null) mRTCameraBG = Shader.Find("Hidden/RTG/RTCameraBG");
                return mRTCameraBG;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rtUnlit (Public Property)
        // Desc: Returns the shader that renders unlit geometry.
        //-----------------------------------------------------------------------------
        public Shader rtUnlit
        {
            get
            {
                if (mRTUnlit == null) mRTUnlit = Shader.Find("Hidden/RTG/RTUnlit");
                return mRTUnlit;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rt2D (Public Property)
        // Desc: Returns the shader that renders 2D geometry.
        //-----------------------------------------------------------------------------
        public Shader rt2D
        {
            get
            {
                if (mRT2D == null) mRT2D = Shader.Find("Hidden/RTG/RT2D");
                return mRT2D;
            }
        }
        #endregion
    }
    #endregion
}
