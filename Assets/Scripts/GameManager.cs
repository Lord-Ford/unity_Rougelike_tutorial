using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public BordManager bordScript;

	private int level = 3;
	
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
		bordScript = GetComponent<BordManager> ();
		InitGame ();
	}

	void InitGame ()
	{
		bordScript.SetupScene (level);
	}
}
