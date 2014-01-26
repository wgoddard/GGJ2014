using UnityEngine;
using System.Collections;
using InControl;

public class LevelController : MonoBehaviour {

    public float roundTime;
    public tk2dTextMesh roundTimeText;

    public GameObject secondNarrative;
    public GameObject thirdNarrative;

    public SmoothMoves.BoneAnimationData zombie;

    public GameObject spawnPoint;
    public GameObject ghost;

    public bool zombieKilled;

    public int round;

	// Use this for initialization
	void Start () {
		InputManager.Setup ();
        round = 0;
        zombieKilled = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (GameObject.FindGameObjectWithTag("Narrative") != null)
        {
            return;
        }

		InputManager.Update ();

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

        if (zombieKilled)
            fail = true;

        if (fail)
        {
            //load failure screen
            Application.LoadLevel(1);
        }
        else
        {
            //load success screen
            ++round;
            switch (round)
            {
                case 1:
                    Instantiate(secondNarrative);
                    GameObject p2 = GameObject.Find("Player2");
                    p2.GetComponentInChildren<SmoothMoves.BoneAnimation>().animationData = zombie;
                    break;
                case 2:
                    Instantiate(thirdNarrative);
                    for (int i = 0; i < 5; ++i )
                    {
                        GameObject.Instantiate(ghost, spawnPoint.transform.position, Quaternion.identity);
                    }
                        break;
                case 3:
                    Application.LoadLevel(2);
                    break;
            }
        }
    }
}
