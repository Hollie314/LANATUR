using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RTInput (Public Singleton Class)
    // Desc: Wraps input specific functionality and hides the underlying Input API
    //       details.
    //-----------------------------------------------------------------------------
    [ExecuteInEditMode] public class RTInput : MonoSingleton<RTInput>
    {
        #region Private Fields
        [SerializeField] ShortcutProfileManager mShortcutProfileManager = new ShortcutProfileManager();    // Manages a collection of shortcut profiles

        bool[]  mUsed_MouseUp       = new bool[typeof(EMouseButton).GetEnumValues().Length];    // Mouse button up used map per mouse button
        bool[]  mDelayedUse_MouseUp = new bool[typeof(EMouseButton).GetEnumValues().Length];    // Delayed mouse button up used map per mouse button
        bool    mUsed_TouchUp       = false;                                                    // Was the touch up event used?

        // The pointing input device
        PointingInputDevice mPointingInputDevice;

        // Buffers used to avoid memory allocations
        List<Shortcut> mClutchShortcutBuffer    = new List<Shortcut>();
        List<Shortcut> mActionShortcutBuffer    = new List<Shortcut>();
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: pointingInputDevice (Public Property)
        // Desc: Returns or sets the pointing input device. If you are using your own
        //       pointing input device, you can set it with this property.
        //-----------------------------------------------------------------------------
        public PointingInputDevice      pointingInputDevice     { get { return mPointingInputDevice; } set { if (value != null) mPointingInputDevice = value; } }

        //-----------------------------------------------------------------------------
        // Name: shortcutProfileManager (Public Property)
        // Desc: Returns the shortcut profile manager.
        //-----------------------------------------------------------------------------
        public ShortcutProfileManager   shortcutProfileManager  { get { return mShortcutProfileManager; } }

        //-----------------------------------------------------------------------------
        // Name: mousePosition (Public Property)
        // Desc: Returns the mouse position.
        //-----------------------------------------------------------------------------
        public Vector3  mousePosition   { get { return Mouse.current.position.ReadValue(); } }

        //-----------------------------------------------------------------------------
        // Name: hasMouse (Public Property)
        // Desc: Returns whether or not a mouse is present.
        //-----------------------------------------------------------------------------
        public bool     hasMouse        { get { return Mouse.current != null; } }

        //-----------------------------------------------------------------------------
        // Name: scrollDelta (Public Property)
        // Desc: Returns the mouse scroll delta.
        //-----------------------------------------------------------------------------
        public Vector2  scrollDelta
        {
            get
            {
                // Note: When using windows, we need to divide by 120
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return Mouse.current.scroll.ReadValue() / new Vector2(120.0f, 120.0f);
                #else
                return Mouse.current.scroll.ReadValue();
                #endif
            }
        }

        //-----------------------------------------------------------------------------
        // Name: anyKeyPressed (Public Property)
        // Desc: Returns true if any key is pressed.
        //-----------------------------------------------------------------------------
        public bool     anyKeyPressed       { get { return Keyboard.current.anyKey.isPressed; } }

        //-----------------------------------------------------------------------------
        // Name: anyKeyWentDown (Public Property)
        // Desc: Returns true if any key was pressed during the current frame.
        //-----------------------------------------------------------------------------
        public bool     anyKeyWentDown      { get { return Keyboard.current.anyKey.wasPressedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: anyKeyWentUp (Public Property)
        // Desc: Returns true if any key was released during the current frame.
        //-----------------------------------------------------------------------------
        public bool     anyKeyWentUp        { get { return Keyboard.current.anyKey.wasReleasedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: altPresed (Public Property)
        // Desc: Returns true if any Alt key is pressed.
        //-----------------------------------------------------------------------------
        public bool     altPresed           { get { return KeyPressed(Key.LeftAlt) || KeyPressed(Key.RightAlt); } }

        //-----------------------------------------------------------------------------
        // Name: ctrlPressed (Public Property)
        // Desc: Returns true if any Control key is pressed.
        //-----------------------------------------------------------------------------
        public bool     ctrlPressed         { get { return KeyPressed(Key.LeftCtrl) || KeyPressed(Key.RightCtrl); } }

        //-----------------------------------------------------------------------------
        // Name: cmdPressed (Public Property)
        // Desc: Returns true if any Command key is pressed.
        //-----------------------------------------------------------------------------
        public bool     cmdPressed         { get { return KeyPressed(Key.LeftCommand) || KeyPressed(Key.RightCommand); } }

        //-----------------------------------------------------------------------------
        // Name: shiftPressed (Public Property)
        // Desc: Returns true if any Shift key is pressed.
        //-----------------------------------------------------------------------------
        public bool     shiftPressed        { get { return KeyPressed(Key.LeftShift) || KeyPressed(Key.RightShift); } }

        //-----------------------------------------------------------------------------
        // Name: leftMBWentDown (Public Property)
        // Desc: Return true if the left mouse button was pressed during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     leftMBWentDown      { get { return Mouse.current.leftButton.wasPressedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: rightMBWentDown (Public Property)
        // Desc: Return true if the right mouse button was pressed during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     rightMBWentDown     { get { return Mouse.current.rightButton.wasPressedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: middleMBWentDown (Public Property)
        // Desc: Return true if the middle mouse button was pressed during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     middleMBWentDown    { get { return Mouse.current.middleButton.wasPressedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: leftMBWentUp (Public Property)
        // Desc: Return true if the left mouse button was released during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     leftMBWentUp        { get { return !IsMouseUpUsed(EMouseButton.Left) && Mouse.current.leftButton.wasReleasedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: rightMBWentUp (Public Property)
        // Desc: Return true if the right mouse button was released during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     rightMBWentUp       { get { return !IsMouseUpUsed(EMouseButton.Right) && Mouse.current.rightButton.wasReleasedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: middleMBWentUp (Public Property)
        // Desc: Return true if the middle mouse button was released during the current
        //       frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     middleMBWentUp      { get { return !IsMouseUpUsed(EMouseButton.Middle) && Mouse.current.middleButton.wasReleasedThisFrame; } }

        //-----------------------------------------------------------------------------
        // Name: leftMBPressed (Public Property)
        // Desc: Return true if the left mouse button is pressed and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     leftMBPressed       { get { return Mouse.current.leftButton.isPressed; } }

        //-----------------------------------------------------------------------------
        // Name: rightMBPressed (Public Property)
        // Desc: Return true if the right mouse button is pressed and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     rightMBPressed      { get { return Mouse.current.rightButton.isPressed; } }

        //-----------------------------------------------------------------------------
        // Name: middleMBPressed (Public Property)
        // Desc: Return true if the middle mouse button is pressed and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     middleMBPressed     { get { return Mouse.current.middleButton.isPressed; } }

        //-----------------------------------------------------------------------------
        // Name: anyMBPressed (Public Property)
        // Desc: Returns true if any mouse button is pressed.
        //-----------------------------------------------------------------------------
        public bool     anyMBPressed        { get { return leftMBPressed || rightMBPressed || middleMBPressed; } }

        //-----------------------------------------------------------------------------
        // Name: anyMBWentDown (Public Property)
        // Desc: Returns true if any mouse button was pressed during the current frame.
        //-----------------------------------------------------------------------------
        public bool     anyMBWentDown       { get { return leftMBWentDown || rightMBWentDown || middleMBWentDown; } }

        //-----------------------------------------------------------------------------
        // Name: anyMBWentUp (Public Property)
        // Desc: Returns true if any mouse button was released during the current frame.
        //-----------------------------------------------------------------------------
        public bool     anyMBWentUp         { get { return leftMBWentUp || rightMBWentUp || middleMBWentUp; } }

        //-----------------------------------------------------------------------------
        // Name: mouseMoved (Public Property)
        // Desc: Return true if the mouse was moved since the last frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     mouseMoved          { get { return Mouse.current.delta.ReadValue().sqrMagnitude != 0.0f; } }

        //-----------------------------------------------------------------------------
        // Name: mouseDeltaX (Public Property)
        // Desc: Return the horizontal mouse delta.
        //-----------------------------------------------------------------------------
        public float    mouseDeltaX         { get { return Mouse.current.delta.x.ReadValue(); } }

        //-----------------------------------------------------------------------------
        // Name: mouseDeltaY (Public Property)
        // Desc: Return the vertical mouse delta.
        //-----------------------------------------------------------------------------
        public float    mouseDeltaY         { get { return Mouse.current.delta.y.ReadValue(); } }
        
        //-----------------------------------------------------------------------------
        // Name: mouseDelta (Public Property)
        // Desc: Return the mouse delta.
        //-----------------------------------------------------------------------------
        public Vector2  mouseDelta          { get { return Mouse.current.delta.value; } }

        //-----------------------------------------------------------------------------
        // Name: touchCount (Public Property)
        // Desc: Return the number of touches.
        //-----------------------------------------------------------------------------
        public int      touchCount          { get { return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Internal_Update() (Public Function)
        // Desc: Called by the system to allow the input module to perform any necessary
        //       updates. One of the main tasks this function has to perform is to trigger
        //       commands attached to the active shortcut profile.
        // Note: If the 'RTInput' MonoBehaviour script is disabled, this function has 
        //       no effect. 
        //-----------------------------------------------------------------------------
        public void Internal_Update()
        {
            int shortcutCount;

            // No-op?
            if (!enabled)
                return;

            // Reset data
            mClutchShortcutBuffer.Clear();
            mActionShortcutBuffer.Clear();

            // Clear used event maps
            mUsed_TouchUp = false;
            int count = mUsed_MouseUp.Length;
            for (int i = 0; i < count; ++i)
                mUsed_MouseUp[i] = false;

            // Delay use events
            count = mDelayedUse_MouseUp.Length;
            for (int i = 0; i < count; ++i)
            {
                if (mDelayedUse_MouseUp[i] && MBWentUp((EMouseButton)i))
                {
                    UseMouseUp((EMouseButton)i);
                    mDelayedUse_MouseUp[i] = false;
                }
            }

            // Loop through each shortcut category and separate the Clutch from the Action commands
            int shortcutCategoryCount = shortcutProfileManager.activeProfile.shortcutCategoryCount;
            for (int c = 0; c < shortcutCategoryCount; ++c)
            {
                // Store category for easy access
                var category = shortcutProfileManager.activeProfile[c];

                // Loop through each shortcut in this category
                shortcutCount = category.shortcutCount;
                for (int s = 0; s < shortcutCount; ++s)
                {
                    // Store shortcut in the corresponding list
                    Shortcut shortcut = category[s];
                    if (shortcut.commandType == ECommandType.Clutch) mClutchShortcutBuffer.Add(shortcut);
                    else mActionShortcutBuffer.Add(shortcut);
                }
            }

            // Handle 'Clutch' shortcuts first. If we find a 'Clutch' command that was just deactivated,
            // we will no longer evaluate the 'Action' commands. For example, holding ALT + left MB to orbit
            // the camera will cause a pick-selection to occur when the left mouse button is released if
            // the mouse cursor happens to be hovering an object at that time.
            bool ignoreActionShortcuts = false;
            shortcutCount = mClutchShortcutBuffer.Count;
            for (int s = 0; s < shortcutCount; ++s)
            {
                // Evaluate and if the shortcut has been deactivated, ignore 'Action' shortcuts
                var result = mClutchShortcutBuffer[s].Evaluate();
                if (result == EShortcutEvalAction.Deactivated)
                {
                    ignoreActionShortcuts = true;

                    // Also use the mouse up event. It was consumed by the Clutch command.
                    var binding = mClutchShortcutBuffer[s].binding;
                    if (binding.leftMB)
                    {
                        if (leftMBWentUp) UseMouseUp(EMouseButton.Left);
                        else mDelayedUse_MouseUp[(int)EMouseButton.Left] = true;
                    }
                    if (binding.middleMB)
                    {
                        if (middleMBWentUp) UseMouseUp(EMouseButton.Middle);
                        else mDelayedUse_MouseUp[(int)EMouseButton.Middle] = true;
                    }
                    if (binding.rightMB)
                    {
                        if (rightMBWentUp) UseMouseUp(EMouseButton.Right);
                        else mDelayedUse_MouseUp[(int)EMouseButton.Right] = true;
                    }
                }
            }

            // Evaluate action commands
            if (!ignoreActionShortcuts)
            {
                shortcutCount = mActionShortcutBuffer.Count;
                for (int s = 0; s < shortcutCount; ++s)
                    mActionShortcutBuffer[s].Evaluate();
            }
        }

        //-----------------------------------------------------------------------------
        // Name: UsePickButtonUp() (Public Function)
        // Desc: Uses the pick button up event. This also affects queries performed
        //       using 'pointingInputDevice'.
        //-----------------------------------------------------------------------------
        public void UsePickButtonUp()
        {
            if (mPointingInputDevice is MouseInputDevice)
                UseMouseUp(EMouseButton.Left);
            else mUsed_TouchUp = false;
        }

        //-----------------------------------------------------------------------------
        // Name: UseMouseUp() (Public Function)
        // Desc: Marks the mouse button up event as used for the current frame.
        //       Queries for 'mouseButton' will return false after this call,
        //       but only if they occur within the same frame.
        // Parm: mouseButton - Use the mouse up event for this button.
        //-----------------------------------------------------------------------------
        public void UseMouseUp(EMouseButton mouseButton)
        {
            mUsed_MouseUp[(int)mouseButton] = true;
        }

        //-----------------------------------------------------------------------------
        // Name: IsMouseUpUsed() (Public Function)
        // Desc: Checks if the mouse up event for the specified button was used
        //       during the current frame.
        // Parm: mouseButton - Query mouse button.
        // Rtrn: True if the mouse up event for 'mouseButton' was used this frame;
        //       false otherwise.
        //-----------------------------------------------------------------------------
        public bool IsMouseUpUsed(EMouseButton mouseButton)
        {
            return mUsed_MouseUp[(int)mouseButton];
        }
        
        //-----------------------------------------------------------------------------
        // Name: SyncShortcutEnabledStates() (Public Function)
        // Desc: Syncs the shortcut enabled states across profiles.
        // Parm: syncProfile - All shortcut profiles are synced with this profile. If
        //                     null, the active profile is used instead.
        //-----------------------------------------------------------------------------
        public void SyncShortcutEnabledStates(ShortcutProfile syncProfile = null)
        {
            // Default to the active profile if no sync profile was specified
            if (syncProfile == null)
                syncProfile = mShortcutProfileManager.activeProfile;

            // Loop through each shortcut in the sync profile
            int categoryCount = syncProfile.shortcutCategoryCount;
            for (int c = 0; c < categoryCount; ++c)
            {
                var category = syncProfile[c];
                int shCount  = category.shortcutCount;
                for (int s = 0; s < shCount; ++s)
                {
                    // Sync enabled states
                    SetShortcutEnabled(category[s].name, category.name, category[s].enabled);
                }
            }
        }

        //-----------------------------------------------------------------------------
        // Name: SetShortcutEnabled() (Public Function)
        // Desc: Sets the enabled state of the shortcut with the specified name.
        // Parm: shortcutName   - Shortcut name.
        //       categoryName   - Name of the category that contains the shortcut.
        //       enabled        - Shortcut enabled state.
        //-----------------------------------------------------------------------------
        public void SetShortcutEnabled(string shortcutName, string categoryName, bool enabled)
        {
            // Loop through each profile and set shortcut enabled state
            int count = mShortcutProfileManager.profileCount;
            for (int i = 0; i < count; ++i)
                mShortcutProfileManager[i].SetShortcutEnabled(shortcutName, categoryName, enabled);
        }

        //-----------------------------------------------------------------------------
        // Name: GetTouch() (Public Function)
        // Desc: Returns the touch with the specified index.
        // Parm: index - Touch index.
        // Rtrn: An instance of 'Touch' which stores touch data.
        //-----------------------------------------------------------------------------
        public UnityEngine.InputSystem.EnhancedTouch.Touch GetTouch(int index)
        {
            if (index == 0 && mUsed_TouchUp)
                return new UnityEngine.InputSystem.EnhancedTouch.Touch();

            if (index >= touchCount) return new UnityEngine.InputSystem.EnhancedTouch.Touch();
            return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[index];
        }

        //-----------------------------------------------------------------------------
        // Name: MBWentDown() (Public Function)
        // Desc: Checks if the specified mouse button was pressed this frame.
        // Parm: mouseButton - The mouse button to check for.
        // Rtrn: True if the specified mouse button was pressed this frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool MBWentDown(EMouseButton mouseButton)
        {
            // Check button state
            if (mouseButton == EMouseButton.Left) return Mouse.current.leftButton.wasPressedThisFrame;
            else if (mouseButton == EMouseButton.Right) return Mouse.current.rightButton.wasPressedThisFrame;
            else if (mouseButton == EMouseButton.Middle) return Mouse.current.middleButton.wasPressedThisFrame;
            return false;
        }

        //-----------------------------------------------------------------------------
        // Name: MBWentUp() (Public Function)
        // Desc: Checks if the specified mouse button was released this frame.
        // Parm: mouseButton - The mouse button to check for.
        // Rtrn: True if the specified mouse button was released this frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool MBWentUp(EMouseButton mouseButton)
        {
            // Was the mouse up event for this button used?
            if (IsMouseUpUsed(EMouseButton.Right))
                return false;

            // Check button state
            if (mouseButton == EMouseButton.Left) return Mouse.current.leftButton.wasReleasedThisFrame;
            else if (mouseButton == EMouseButton.Right) return Mouse.current.rightButton.wasReleasedThisFrame;
            else if (mouseButton == EMouseButton.Middle) return Mouse.current.middleButton.wasReleasedThisFrame;
            return false;
        }

        //-----------------------------------------------------------------------------
        // Name: MBPressed() (Public Function)
        // Desc: Checks if the specified mouse button is pressed.
        // Parm: mouseButton - The mouse button to check for.
        // Rtrn: True if the specified mouse button is pressed and false otherwise.
        //-----------------------------------------------------------------------------
        public bool MBPressed(EMouseButton mouseButton)
        {
            // Check button state
            if (mouseButton == EMouseButton.Left) return Mouse.current.leftButton.isPressed;
            else if (mouseButton == EMouseButton.Right) return Mouse.current.rightButton.isPressed;
            else if (mouseButton == EMouseButton.Middle) return Mouse.current.middleButton.isPressed;
            return false;
        }

        //-----------------------------------------------------------------------------
        // Name: KeyWentDown() (Public Function)
        // Desc: Checks if the specified key was pressed this frame.
        // Parm: keyCode - Query key.
        // Rtrn: True if the key was pressed this frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool KeyWentDown(Key keyCode)
        {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }

        //-----------------------------------------------------------------------------
        // Name: KeyWentUp() (Public Function)
        // Desc: Checks if the specified key was released this frame.
        // Parm: keyCode - Query key.
        // Rtrn: True if the key was released this frame and false otherwise.
        //-----------------------------------------------------------------------------
        public bool KeyWentUp(Key keyCode)
        {
            return Keyboard.current[keyCode].wasReleasedThisFrame;
        }

        //-----------------------------------------------------------------------------
        // Name: KeyPressed() (Public Function)
        // Desc: Checks if the specified key is pressed.
        // Parm: keyCode - Query key.
        // Rtrn: True if the key is pressed and false otherwise.
        //-----------------------------------------------------------------------------
        public bool KeyPressed(Key keyCode)
        {
            return Keyboard.current[keyCode].isPressed;
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: Start() (Private Function)
        // Desc: Called by Unity to allow the object to initialize itself.
        //-----------------------------------------------------------------------------
        void Start()
        {
            // Enable enhanced touch
            EnhancedTouchSupport.Enable();
            
            // Create the pointing input device
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID || UNITY_WP_8_1)
            mPointingInputDevice = new TouchInputDevice();
            #else
            mPointingInputDevice = new MouseInputDevice();
            #endif
        }

        //-----------------------------------------------------------------------------
        // Name: OnEnable() (Private Function)
        // Desc: Called by Unity when the object is enabled.
        //-----------------------------------------------------------------------------
        void OnEnable()
        {
            // Init data
            mShortcutProfileManager.Init();

            // Refresh shortcuts in case new ones were added 
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                int count = mShortcutProfileManager.profileCount;
                for (int i = 0; i < count; ++i)
                {
                    mShortcutProfileManager[i].RefreshShortcuts();
                    mShortcutProfileManager[i].DetectShortcutConflicts();
                }
            }
            #endif
        }
        #endregion
    }
    #endregion
}