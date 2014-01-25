using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public float movementSpeed = 10.0f;

    GameObject crosshair;

    Vector3[] corners = { new Vector3(-2.6f, 1.3f), new Vector3(2.6f, 1.3f), new Vector3(-2.6f, -1.3f), new Vector3(2.6f, -1.3f) };
    public string movementX = "L_XAxis_";
    public string movementY = "L_YAxis_";
    public string aimXAxis = "R_XAxis_";
    public string aimYAxis = "R_YAxis_";
    public string shoot = "RB_";
    public GameObject shotPrefab;
    public GameObject bulletPrefab;

    Vector3 rstick = Vector3.right;

    public int playerId = 0;

    public bool hasWeapon;
    public bool hasFood;
    public bool isAlive;

    public AudioClip pickupClip;
    public AudioClip hitClip;

	// Use this for initialization
	void Start () {

        string playerString = playerId.ToString();

        movementX += playerString;
        movementY += playerString;
        aimXAxis += playerString;
        aimYAxis += playerString;
        shoot += playerString;
  

        crosshair = transform.FindChild("Crosshair").gameObject;
        crosshair.renderer.enabled = false;

        hasWeapon = false;
        hasFood = false;
        isAlive = true;
	
	}
	
	// Update is called once per frame
	void Update () {

        //debug
        if (Input.GetButtonDown("A_1"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (isAlive)
        {

            transform.Translate(new Vector3(Input.GetAxis(movementX), -Input.GetAxis(movementY)) * Time.deltaTime * movementSpeed);

            Vector3 newRstick = new Vector3(Input.GetAxis(aimXAxis), -Input.GetAxis(aimYAxis));
            if (newRstick.magnitude > 0.0f)
            {
                rstick = newRstick;
            }
            Vector3 aimDirection = rstick.normalized;
            //aimDirection *= rstick.magnitude;

            crosshair.transform.position = transform.position + aimDirection * 1.0f;

            if (hasWeapon && Input.GetButtonDown(shoot))
            {
                audio.Play();
                GameObject blast = GameObject.Instantiate(shotPrefab, transform.position + aimDirection * 0.1f, Quaternion.identity) as GameObject;
                blast.transform.parent = gameObject.transform;
                GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
                Bullet b = bullet.GetComponent<Bullet>();
                b.speed = 20.0f;
                b.direction = aimDirection;
            }
        }
	
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Gun"))
        {
            Debug.Log(playerId + "is player id");
            Vector3 destination = corners[playerId- 1];
            Vector3[] path = { new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)), destination };
            iTween.MoveTo(c.gameObject, iTween.Hash("path", path, "time", 1.0f, "easetype", iTween.EaseType.easeInQuad, "oncomplete", "PunchHit", "oncompleteparams", c.gameObject, "oncompletetarget", gameObject));
            c.enabled = false;
            //SoundManager.PlaySFX(pickupClip);
            audio.PlayOneShot(pickupClip);
            hasWeapon = true;
            crosshair.renderer.enabled = true;
        } 
        else if (c.gameObject.CompareTag("Bullet"))
        {
            audio.PlayOneShot(hitClip);
            isAlive = false;
            transform.Rotate(0, 0, 90.0f);
        } 
        else if (c.gameObject.CompareTag("Food"))
        {
            Vector3 destination = corners[playerId - 1];
            Vector3[] path = { new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)), destination + new Vector3(0.4f, 0.0f, 0.0f)};
            iTween.MoveTo(c.gameObject, iTween.Hash("path", path, "time", 1.0f, "easetype", iTween.EaseType.easeInQuad, "oncomplete", "PunchHit", "oncompleteparams", c.gameObject, "oncompletetarget", gameObject));
            c.enabled = false;
            audio.PlayOneShot(pickupClip);
            hasFood = true;
        }
    
    }

    void PunchHit(GameObject _t)
    {
        iTween.ShakePosition(_t, new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
    }
}
