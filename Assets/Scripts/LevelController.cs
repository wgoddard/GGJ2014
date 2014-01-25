using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

    public float roundTime;
    public tk2dTextMesh roundTimeText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        roundTime -= Time.deltaTime;
        roundTimeText.text = roundTime.ToString("#.##");

        roundTimeText.Commit();


        if (roundTime < 0.0f)
        {
            EndGame();
        }


	}

    void EndGame()
    {
        bool fail = false;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            Character c = go.GetComponent<Character>();
            if ( !c.isAlive || !c.hasWeapon || !c.hasFood)
            {
                fail = true;
                break;
            }
        }

        if (fail)
        {
            //load failure screen
            Application.LoadLevel(2);
        }
        else
        {
            //load success screen
            Application.LoadLevel(3);
        }
    }
}
