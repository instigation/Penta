using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleStageController : MonoBehaviour {
    public PuzzleSetRenderer __renderer;
    public PieceController __controller;
    private GeneralInput input;
    public GameObject __canvas;

	// Use this for initialization
	void Start () {
        renderPuzzle();
        if (Application.platform == RuntimePlatform.Android)
            input = new TouchInputWrapper();
        else {
            var rect = __canvas.GetComponent<RectTransform>().rect;
            input = new MouseInputWrapper(rect.width, rect.height);
        }
	}
	
	// Update is called once per frame
	void Update () {
        input.update();
        if(!input.noTouching()) {
            //only process the first touch
            if (input.touchBegan()) {
                Debug.Log("touch began!");
                __controller.selectOnPosition(input.position());
                __controller.tryToExtractSelected();
            }
            else if (input.touchMoved()) {
                Debug.Log("touch moved!");
                __controller.moveSelectedFor(input.deltaPosition());
            }
            else if (input.touchEnded()) {
                Debug.Log("touch ended!");
                __controller.tryToInsertSelected();
            }
        }
    }

    public void renderPuzzle() {
        PuzzleGenerator generator = new PuzzleGenerator(4, Difficulty.HARD);
        Puzzle p = generator.generatePuzzle();
        RenderedPuzzleSet puzzleSet = __renderer.render(p);
        __controller.setPuzzleSet(puzzleSet);
    }
}

public interface GeneralInput {
    void update();
    bool noTouching();
    bool touchBegan();
    bool touchMoved();
    bool touchEnded();
    Vector3 position();
    Vector3 deltaPosition();
}

public class TouchInputWrapper : GeneralInput {
    public void update() { }
    public bool noTouching() {
        return Input.touchCount == 0;
    }
    public bool touchBegan() {
        return Input.touches[0].phase == TouchPhase.Began;
    }
    public bool touchMoved() {
        return Input.touches[0].phase == TouchPhase.Moved;
    }
    public bool touchEnded() {
        return Input.touches[0].phase == TouchPhase.Ended;
    }
    public Vector3 position() {
        return Input.touches[0].position;
    }
    public Vector3 deltaPosition() {
        return Input.touches[0].deltaPosition;
    }
}

public class MouseInputWrapper : GeneralInput {
    private enum state {BEGAN, MOVED, ENDED, IDLE };
    private state current = state.IDLE;
    private Vector3 mousePosition;
    private float canvasWidth;
    private float canvasHeight;

    public MouseInputWrapper(float canvasWidth, float canvasHeight) {
        this.canvasWidth = canvasWidth;
        this.canvasHeight = canvasHeight;
    }
    public void update() {
        stateTransition();
        if (touchBegan())
            mousePosition = position();
    }
    private void stateTransition() {
        if(current == state.IDLE) {
            if (Input.GetMouseButtonDown(0))
                current = state.BEGAN;
        }
        else if(current == state.BEGAN) {
            current = state.MOVED;
        }
        else if(current == state.MOVED) {
            if (Input.GetMouseButtonUp(0))
                current = state.ENDED;
        }
        else if(current == state.ENDED) {
            current = state.IDLE;
        }
    }
    public bool noTouching() {
        return current == state.IDLE;
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

}