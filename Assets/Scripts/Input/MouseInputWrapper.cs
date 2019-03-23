using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface GeneralInput {
    void update();
    bool noTouching();
    bool touchBegan();
    bool touchMoved();
    bool touchEnded();
    // precondition: valid only during BEGAN~MOVED~ENDED
    float timeCollapsedAfterLastTouch();
    Vector2 anchoredPosition();
    Vector2 deltaAnchoredPosition();
}

public class MouseInputWrapper : GeneralInput {
    private enum state { BEGAN, MOVED, ENDED, IDLE };
    private state current = state.IDLE;
    private Vector3 mouseAnchoredPosition;
    private GameObject canvas;
    private Camera camera;
    private InputValidator validator;
    private int lastTouchedFrame;
    private float deltaTouchTime;

    public MouseInputWrapper(GameObject canvas, Camera camera, InputValidator validator) {
        this.canvas = canvas;
        this.camera = camera;
        this.validator = validator;
        this.lastTouchedFrame = -1000;
    }
    public void update() {
        stateTransition();
    }
    private void stateTransition() {
        if (current == state.IDLE) {
            mouseAnchoredPosition = anchoredPosition();
            if (Input.GetMouseButtonDown(0)) {
                current = state.BEGAN;
                int currentTouchedFrame = Time.frameCount;
                deltaTouchTime = (currentTouchedFrame - lastTouchedFrame)*Time.deltaTime;
                lastTouchedFrame = currentTouchedFrame;
            }
        }
        else if (current == state.BEGAN) {
            current = state.MOVED;
        }
        else if (current == state.MOVED) {
            if (Input.GetMouseButtonUp(0))
                current = state.ENDED;
        }
        else if (current == state.ENDED) {
            current = state.IDLE;
        }
    }
    public bool noTouching() {
        return (current == state.IDLE);
    }
    public bool touchBegan() {
        return current == state.BEGAN;
    }
    public bool touchMoved() {
        return current == state.MOVED;
    }
    public bool touchEnded() {
        return current == state.ENDED;
    }
    public Vector2 anchoredPosition() {
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePos.x *= canvasWidth;
        //mousePos.y *= canvasHeight;
        Vector2 mousePos;
        // TODO: Is canvas anchor and children anchors should be the same or something?
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, camera, out mousePos);
        return mousePos;
    }
    public Vector2 deltaAnchoredPosition() {
        Vector2 ret = anchoredPosition() - (Vector2)mouseAnchoredPosition;
        mouseAnchoredPosition = anchoredPosition();
        return ret;
    }
    public float timeCollapsedAfterLastTouch() {
        return deltaTouchTime;
    }
}