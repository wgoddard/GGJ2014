using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {

    GameObject target;

	// Use this for initialization
	void Start () {

        GameObject [] gos = GameObject.FindGameObjectsWithTag("Player");

        target = gos[Random.Range(0, gos.Length)];

	}
	
	// Update is called once per frame
	void Update () {

        if (GameObject.FindGameObjectWithTag("Narrative") != null)
        {
            return;
        }

        transform.Translate((target.transform.position - transform.position) * Time.deltaTime * 1.0f);
	
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Bullet"))
        {
            LevelController lc = Component.FindObjectOfType<LevelController>();
            if (lc == null)
            {
                Debug.Log("BAD");
            }
            else
            {
                lc.zombieKilled = true;
            }
        }

    }

}
