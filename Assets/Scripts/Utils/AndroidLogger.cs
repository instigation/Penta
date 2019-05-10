using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidLogger {
    Text logText;
    public AndroidLogger() {
        logText = MonoBehaviourUtils.renderText().GetComponent<Text>();
    }
    public void log(string content) {
        setText(getText() + '\n' + content);
    }
    private void setText(string content) {
        logText.text = content;
    }
    private string getText() {
        return logText.text;
    }
}
