using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hitrush.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Touch Settings")]
        [SerializeField] private float swipeThreshold = 50f;
        [SerializeField] private float swipeTimeLimit = 0.5f;
        
        // Events
        public Action OnMoveLeft;
        public Action OnMoveRight;
        public Action OnJump;
        public Action OnSlide;
        public Action OnPause;
        
        // Touch tracking
        private Vector2 touchStartPos;
        private float touchStartTime;
        private bool isTrackingTouch;
        
        // Keyboard
        private bool leftPressed, rightPressed, jumpPressed, slidePressed, pausePressed;
        
        private void Update()
        {
            HandleKeyboardInput();
            HandleTouchInput();
        }
        
        private void HandleKeyboardInput()
        {
            // Left/Right
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
                    OnMoveLeft?.Invoke();
                    
                if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
                    OnMoveRight?.Invoke();
                    
                // Jump
                if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                    OnJump?.Invoke();
                    
                // Slide
                if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
                    OnSlide?.Invoke();
                    
                // Pause
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    OnPause?.Invoke();
            }
        }
        
        private void HandleTouchInput()
        {
            if (Touchscreen.current == null) return;
            
            var touch = Touchscreen.current.primaryTouch;
            
            if (touch.press.wasPressedThisFrame)
            {
                touchStartPos = touch.position.ReadValue();
                touchStartTime = Time.time;
                isTrackingTouch = true;
            }
            else if (touch.press.wasReleasedThisFrame && isTrackingTouch)
            {
                Vector2 touchEndPos = touch.position.ReadValue();
                float touchDuration = Time.time - touchStartTime;
                
                if (touchDuration <= swipeTimeLimit)
                {
                    Vector2 delta = touchEndPos - touchStartPos;
                    float absX = Mathf.Abs(delta.x);
                    float absY = Mathf.Abs(delta.y);
                    
                    if (absX > swipeThreshold || absY > swipeThreshold)
                    {
                        if (absX > absY)
                        {
                            // Horizontal swipe
                            if (delta.x > 0)
                                OnMoveRight?.Invoke();
                            else
                                OnMoveLeft?.Invoke();
                        }
                        else
                        {
                            // Vertical swipe
                            if (delta.y > 0)
                                OnJump?.Invoke();
                            else
                                OnSlide?.Invoke();
                        }
                    }
                }
                
                isTrackingTouch = false;
            }
        }
    }
}
