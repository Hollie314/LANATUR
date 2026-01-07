using UnityEngine;
using System.Reflection;

namespace RTGStandard
{
    //-----------------------------------------------------------------------------
    // Name: RTGStaticStateReset (Public Static Class)
    // Desc: Ensures that all static runtime state is reset when Unity initializes
    //       subsystems. This is required to support domain reload disabled and script
    //       recompilation during play mode.
    // Note: Based on a community-provided solution discussed on the Unity forums:
    //       https://shorturl.at/uEazq
    //-----------------------------------------------------------------------------
    public static class RTGStaticStateReset
    {
        #region Public Static Functions
        //-----------------------------------------------------------------------------
        // Name: ResetStaticState() (Public Static Function)
        // Desc: Ensures that all static runtime state is reset when Unity initializes
        //       subsystems. This is required to support domain reload disabled and script
        //       recompilation during play mode.
        //-----------------------------------------------------------------------------
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticState()
        {
            // Reset all Singleton instances
            ResetSingleton<MaterialManager>();
            ResetSingleton<ShaderManager>();
            ResetSingleton<TextureManager>();
            ResetSingleton<MeshManager>();
            ResetSingleton<RTGizmosSkinManager>();
            ResetSingleton<RTMeshManager>();

            // Reset static fields in RTGizmos
            var rtGizmosType = typeof(RTGizmos);
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

            // Reset sMtrlPropertyBlock
            var mtrlPropertyBlockField = rtGizmosType.GetField("sMtrlPropertyBlock", bindingFlags);
            if (mtrlPropertyBlockField != null)
                mtrlPropertyBlockField.SetValue(null, null);

            // Reset sMaterial
            var materialField = rtGizmosType.GetField("sMaterial", bindingFlags);
            if (materialField != null)
                materialField.SetValue(null, null);

            // Reset sGRS
            var grsField = rtGizmosType.GetField("sGRS", bindingFlags);
            if (grsField != null)
                grsField.SetValue(null, new GizmoRenderStates());
        }

        //-----------------------------------------------------------------------------
        // Name: ResetSingleton() (Public Static Function)
        // Desc: Resets the state of the singleton with the specified type.
        // Parm: T - Singleton type. Must derive from 'Singleton'.
        //-----------------------------------------------------------------------------
        static void ResetSingleton<T>() where T : Singleton<T>, new()
        {
            var singletonType = typeof(Singleton<T>);
            var instanceField = singletonType.GetField("sInstance", BindingFlags.NonPublic | BindingFlags.Static);
            if (instanceField != null)
                instanceField.SetValue(null, new T());
        }
        #endregion
    }
}