using UnityEngine;
using System.Collections;
using InControl;

public class Character : MonoBehaviour
{

	public float movementSpeed = 10.0f;
	GameObject crosshair;
	Vector3 crosshairPrevious;
	Vector3[] corners = {
			new Vector3 (-2.6f, 1.3f),
			new Vector3 (2.6f, 1.3f),
			new Vector3 (-2.6f, -1.3f),
			new Vector3 (2.6f, -1.3f)
	};
	public GameObject shotPrefab;
	public GameObject bulletPrefab;
	public GameObject casingPrefab;
	Vector3 rstick = Vector3.right;
	public int playerId = 0;
	public bool hasWeapon;
	public bool hasFood;
	public bool isAlive;
	public AudioClip pickupClip;
	public AudioClip hitClip;
	private InputDevice controller;


	public float crosshairDistance = 5.0f;

	private InputControl action;
	private InputControl shoot; 
	
	public Vector2 movement;
	public Vector2 aim;
	Vector3 aimDirection;

	public int gunType = 0;
	
	private float timer = 0.0f;
	public float shotgunDelay = 0.25f;
	public float machineGunDelay = 0.05f;
	public float rocketLauncherDelay = 0.75f;

	public float health = 10.0f;

	public AbstractGoTween spritetween;

	// Use this for initialization
	void Start ()
	{
		isAlive = true;

		crosshair = transform.FindChild ("Crosshair").gameObject;
		crosshair.renderer.enabled = false;

		hasWeapon = false;
		hasFood = false;

	}

	// Update is called once per frame
	void Update ()
	{
		if (InputManager.Devices.Count >= playerId && isAlive) {
			controller = InputManager.Devices [playerId - 1];
			action = controller.GetControl (InputControlType.Action1);
			shoot = controller.GetControl (InputControlType.RightBumper);

			movement = controller.LeftStickVector;
			aim = controller.RightStickVector;

			//debug
			if (action.IsPressed) {
				Application.LoadLevel (Application.loadedLevel);
			}

			if(!shoot.IsPressed ||!hasWeapon)
				transform.Translate (new Vector3 (movement.x, movement.y) * Time.deltaTime * movementSpeed);


			//float angle = Mathf.Atan2(aim.y, aim.x) * (180 / Mathf.PI); 
			if(aim.magnitude > 0) {
				aimDirection = aim.normalized;
			}

			crosshair.transform.position = transform.position + aimDirection * 1.0f;

			if (hasWeapon && shoot.IsPressed && timer <= 0.0f) {
				audio.Play ();
				GameObject blast = GameObject.Instantiate (shotPrefab, transform.position + aimDirection * 0.1f, Quaternion.identity) as GameObject;
				blast.transform.parent = gameObject.transform;
				Go.to( blast.transform, 0.1f, new GoTweenConfig().shake( new Vector3(0.15f, 0.15f, 0 ), GoShakeType.Position | GoShakeType.Scale));

				GameObject bullet = null;
				//Machine Gun
				if(gunType == 1)
				{
					bullet = GameObject.Instantiate (bulletPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
					Bullet b = bullet.GetComponent<Bullet> ();
					b.speed = 20.0f + Random.Range(-5, 5);
					b.direction = new Vector3(aimDirection.x + Random.Range(-0.1f, 0.1f), aimDirection.y + Random.Range(-0.1f, 0.1f));
					timer = machineGunDelay;
					b.lifespan = 1.0f;
					b.damage = 2.0f;
				}
				else if(gunType == 2)
				{
					//Shot Gun
					for(int i = -2; i < 4; i++)
					{
						bullet = GameObject.Instantiate (bulletPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
						Bullet b = bullet.GetComponent<Bullet> ();
						b.speed = 15.0f + Random.Range(-5, 5);
						b.direction = new Vector3(aimDirection.x + Random.Range(-0.1f, 0.1f) * (i * 3), aimDirection.y + Random.Range(-0.1f, 0.1f) * (i * 3));
						b.lifespan = 0.5f;
						b.damage = 4.0f;
					}
					timer = shotgunDelay;
				}
				else if(gunType == 3)
				{
					//Rocket Launcher
					bullet = GameObject.Instantiate (bulletPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
					Bullet b = bullet.GetComponent<Bullet> ();
					b.speed = 5.0f + Random.Range(-1, 1);
					b.direction = new Vector3(aimDirection.x + Random.Range(-0.1f, 0.1f), aimDirection.y + Random.Range(-0.1f, 0.1f));
					b.lifespan = 1.5f;
					b.damage = 10.0f;
					timer = rocketLauncherDelay;
				}
				else if(gunType == 4)
				{
					//Flamethrower
					for(int i = 0; i < 10; i++)
					{
						bullet = GameObject.Instantiate (bulletPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
						Bullet b = bullet.GetComponent<Bullet> ();
						b.speed = 1f + Random.Range(-0.5f, 0.5f);
						b.direction = new Vector3(aimDirection.x + Random.Range(-0.5f, 0.5f), aimDirection.y + Random.Range(-0.5f, 0.5f));
						b.lifespan = 0.75f;
						b.damage = 0.5f;
					}
				}
				Go.to( bullet.transform, 0.25f, new GoTweenConfig().shake( new Vector3(0.25f, 0.25f, 0 ), GoShakeType.Scale));

				//Juicing
				Go.to( tk2dCamera.Instance.transform, 0.1f, new GoTweenConfig().shake( new Vector3( 0.25f, 0.25f, 0 ), GoShakeType.Position) );
				Go.to(gameObject.transform, 0.1f, new GoTweenConfig().position(transform.position - aimDirection/10.0f));


				//GameObject casing = GameObject.Instantiate(casingPrefab, transform.position + aimDirection * 0.5f, Quaternion.identity) as GameObject;
			}
		}

		if(timer > 0.0f)
			timer -= Time.deltaTime;
	}

	void OnTriggerEnter2D (Collider2D c)
	{
		if (c.gameObject.CompareTag ("Gun")) {
			UIpickup(c.gameObject);
			c.enabled = false;
			gunType = c.GetComponent<GunBox>().gunType;
			//SoundManager.PlaySFX(pickupClip);
			audio.PlayOneShot (pickupClip);
			hasWeapon = true;
			crosshair.renderer.enabled = true;
		} else if (c.gameObject.CompareTag ("Bullet")) {
			audio.PlayOneShot (hitClip);
			health -= c.gameObject.GetComponent<Bullet>().damage;
			if(health <= 0.0f) {
				isAlive = false;
			}
			PunchHit(gameObject);
			//Go.to(gameObject.transform, 0.1f, new GoTweenConfig().position(c.GetComponent<Bullet>().direction));
		} else if (c.gameObject.CompareTag ("Food")) {
			UIpickup(c.gameObject);
			c.enabled = false;
			audio.PlayOneShot (pickupClip);
			hasFood = true;
		}

	}

	void UIpickup(GameObject pickup)
	{
		Vector3 destination = corners [playerId - 1];
		Vector3[] path = {
			-(destination - transform.position) * 0.25f,
			destination + new Vector3 (0.4f, 0.0f, 0.0f)
		};
		Go.to(pickup.transform, 0.25f, new GoTweenConfig().setEaseType(GoEaseType.QuadIn).position(destination));
		Go.to(pickup.transform, 0.25f, new GoTweenConfig().scale(new Vector3(0.5f, 0.5f, 1)));
	}
	
	void PunchHit (GameObject _t)
	{
		if(spritetween != null) {
			spritetween.complete();
			spritetween.destroy();
		}
		spritetween = Go.to( _t.transform, 0.1f, new GoTweenConfig().shake( new Vector3( 0.2f, 0.2f, 0.0f), GoShakeType.Position));
	}
}
