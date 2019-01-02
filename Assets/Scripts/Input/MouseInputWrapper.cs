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
    Vector3 position();
    Vector3 deltaPosition();
}

public class MouseInputWrapper : GeneralInput {
    private enum state { BEGAN, MOVED, ENDED, IDLE };
    private state current = state.IDLE;
    private Vector3 mousePosition;
    private float canvasWidth;
    private float canvasHeight;
    private InputValidator validator;
    private int lastTouchedFrame;
    private float deltaTouchTime;

    public MouseInputWrapper(float canvasWidth, float canvasHeight, InputValidator validator) {
        this.canvasWidth = canvasWidth;
        this.canvasHeight = canvasHeight;
        this.validator = validator;
        this.lastTouchedFrame = -1000;
    }
    public void update() {
        stateTransition();
    }
    private void stateTransition() {
        if (current == state.IDLE) {
            mousePosition = position();
            if (Input.GetMouseButtonDown(0) && validator.isValid(mousePosition)) {
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
            if (Input.GetMouseButtonUp(0) && validator.isValid(mousePosition))
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
    public Vector3 position() {
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        mousePos -= new Vector3(0.5f, 0.5f, 0.0f);
        mousePos.x *= canvasWidth;
        mousePos.y *= canvasHeight;
        return mousePos;
    }
    public Vector3 deltaPosition() {
        Vector3 ret = position() - mousePosition;
        mousePosition = position();
        return ret;
    }
    public float timeCollapsedAfterLastTouch() {
        return deltaTouchTime;
    }
}