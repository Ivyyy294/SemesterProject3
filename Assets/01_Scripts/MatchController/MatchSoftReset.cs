using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSoftReset : MonoBehaviour
{
	private MatchPauseController pauseController;
	private MatchObjectSpawn objectSpawnController;
	float timer = 0f;

	[SerializeField] float pauseTime;

	public float PauseTimeRemaining => Mathf.Max (0f, pauseTime - timer);

	// Start is called before the first frame update
	void Start()
    {
		timer = pauseTime;
        pauseController = GetComponent<MatchPauseController>();
		objectSpawnController = GetComponent<MatchObjectSpawn>();
    }

	public void Invoke()
	{
		objectSpawnController.RespawnObjects();
		pauseController.PauseMatch (true);
		timer = 0f;
	}
    // Update is called once per frame
    void Update()
    {
        if (timer < pauseTime)
			timer += Time.unscaledDeltaTime;
		else if (pauseController.IsMatchPaused)
			pauseController.PauseMatch (false);
    }
}
