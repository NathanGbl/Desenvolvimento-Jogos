using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SpaceShipGame
{
    public static class SpaceShipInput
    {
        public static bool SubmitPressedThisFrame()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                pressed = Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed = pressed || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif

            return pressed;
        }

        public static float HorizontalRaw()
        {
            float horizontal = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                {
                    horizontal -= 1f;
                }

                if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                {
                    horizontal += 1f;
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            horizontal += Input.GetAxisRaw("Horizontal");
#endif

            return Mathf.Clamp(horizontal, -1f, 1f);
        }

        public static float VerticalRaw()
        {
            float vertical = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
                {
                    vertical -= 1f;
                }

                if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
                {
                    vertical += 1f;
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            vertical += Input.GetAxisRaw("Vertical");
#endif

            return Mathf.Clamp(vertical, -1f, 1f);
        }

        public static bool FireHeld()
        {
            bool held = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                held = Keyboard.current.spaceKey.isPressed;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            held = held || Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1");
#endif

            return held;
        }

        public static bool FirePressedThisFrame()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                pressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed = pressed || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1");
#endif

            return pressed;
        }

        public static bool SlowHeld()
        {
            bool held = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                held = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            held = held || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif

            return held;
        }
    }
}
