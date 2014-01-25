using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public Vector3 direction;
    public float speed;
	public float lifespan;
	private float timer;

	// Use this for initialization
	void Start () {
		timer = lifespan;
	}
	
	// Update is called once per frame
	void Update () {
		if(timer <= 0.0f)
			Destroy (this.gameObject);

		timer -= Time.deltaTime;

        transform.Translate(direction * speed * Time.deltaTime);
	
	}
}
