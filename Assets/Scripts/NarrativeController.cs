using UnityEngine;
using System.Collections;
using InControl;

public class NarrativeController : MonoBehaviour {

    public string[] messages;
    public int nextLevel;

    int messageIndex;

    public enum TextState { AnimIn, Waiting, AnimOut };

    public TextState myState;

    float animTime;


    public tk2dTextMesh text;

	// Use this for initialization
	void Start () {

        InputManager.Setup();

        messageIndex = 0;
        ChangeText(messageIndex);
        animTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        InputManager.Update();

        switch (myState)
        {
            case TextState.AnimIn:
                text.color = Color.Lerp(Color.clear, Color.white, animTime);
                text.Commit();
                animTime += Time.deltaTime;
                if (animTime > 1.0f)
                {
                    myState = TextState.Waiting;
                }
            break;
            case TextState.Waiting:
                if (InputManager.ActiveDevice.Action1.IsPressed)
                //if (Input.GetButtonDown("A_1"))
                {
                    myState = TextState.AnimOut;
                    animTime = 0.0f;
                }
            break;
            case TextState.AnimOut:
                text.color = Color.Lerp(Color.white, Color.clear, animTime);
                text.Commit();
                animTime += Time.deltaTime;
                if (animTime > 1.0f)
                {
                    ++messageIndex;
                    if (messageIndex > messages.Length)
                    {
                        Application.LoadLevel(nextLevel);
                    }
                    else
                    {
                        ChangeText(messageIndex);
                        myState = TextState.AnimIn;
                        animTime = 0.0f;
                    }
                }
            break;

        }



	
	}

    void ChangeText(int index)
    {
        text.text = messages[index];
        text.Commit();
    }
}
