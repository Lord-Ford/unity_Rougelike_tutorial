using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator anim;
	private int food;
	private Vector2 touchOragin = -Vector2.one;

	// Use this for initialization
	protected override void Start ()
	{
		anim = GetComponent<Animator> ();

		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;
		int vertical = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;
#endif

		if(Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];

			if(myTouch.phase == TouchPhase.Began)
			{
				touchOragin = myTouch.position;
			}else if(myTouch.phase == TouchPhase.Ended && touchOragin.x >= 0)
			{
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOragin.x;
				float y = touchEnd.y - touchOragin.y;
				touchOragin.x = -1;
				if(Mathf.Abs (x) > Mathf.Abs (y))
				{
					horizontal = x > 0 ? 1 : -1;
				}else
					vertical = y > 0 ? 1 : -1;
			}
		}

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall> (horizontal, vertical);

	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit))
		{
			SoundManager.instance.RandomSfx (moveSound1,moveSound2);
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food")
		{
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		}else if (other.tag == "Soda")
		{
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove <T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		anim.SetTrigger ("playerChop");
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseFood (int loss)
	{
		anim.SetTrigger ("playerHurt");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver ();
	}

	private void CheckIfGameOver()
	{
		if (food <= 0)
		{
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.misucSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}
}
