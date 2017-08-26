using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputValidator {
    bool isValid(Vector3 mousePosition);
}

public class RButtonValidator : InputValidator {
    public bool isValid(Vector3 mousePosition) {
        return Input.GetKey(KeyCode.R);
    }
}

public class EmptyValidator : InputValidator {
    public bool isValid(Vector3 mousePosition) {
        return true;
    }
}
