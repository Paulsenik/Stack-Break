using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour {
    public int minSwipeRadius = 50;

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;
    private bool isDragging = false, isSwipe = false;

    private bool wasTap = false;

    private void Update() {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;
        wasTap = false;

        // Standalone
        if (Input.GetMouseButtonDown(0)) {
            isDragging = true;
            tap = true;
            startTouch = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0) && isDragging) {
            isDragging = false;
            if (!isSwipe) {
                wasTap = true;
                //Debug.Log("tap Mouse!!!");
            }
            isSwipe = false;
            Reset();
        }

        // Mobile Inputs
        if (Input.touches.Length > 0) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                tap = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            } else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                isDragging = false;
                if (!isSwipe) {
                    wasTap = true;
                    //Debug.Log("tap!!!");
                }
                isSwipe = false;
                Reset();
            }
        }

        //Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDragging) {
            if (Input.touches.Length > 0) {
                swipeDelta = Input.touches[0].position - startTouch;
                //Debug.Log("Smartphone swipe " + swipeDelta.magnitude);
            } else if (Input.GetMouseButtonDown(0)) {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
                //Debug.Log("Standalone swipe "+swipeDelta.magnitude);
            }
        }

        // Did we cross the deadzone?
        if (swipeDelta.magnitude > minSwipeRadius) {
            //Which Direction?
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            isSwipe = true;

            if (Mathf.Abs(x) > Mathf.Abs(y)) {
                //left or right
                if (x < 0) {
                    swipeLeft = true;
                } else {
                    swipeRight = true;
                }
            } else {
                //Up or down
                if (y < 0) {
                    swipeDown = true;
                } else {
                    swipeUp = true;
                }
            }

            Reset();
        }
    }

    private void Reset() {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }


    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool Tap { get { return wasTap; } }
}
