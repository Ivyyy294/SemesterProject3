using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenEmptyBlackOutUi : MonoBehaviour
{
	[SerializeField] AnimationCurve alphaAnimation;

	[Header ("Lara Values")]
	[SerializeField] Image image;

	PlayerOxygen playerOxygen;
	PlayerOxygenEmergencyRefill emergencyRefill;

	float timer;
	float animationLength;

    // Start is called before the first frame update
    void Start()
    {
        playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
		emergencyRefill = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygenEmergencyRefill>();

		animationLength = alphaAnimation.keys[alphaAnimation.keys.Length - 1].time;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerOxygen.OxygenEmpty)
		{
			if (timer <= animationLength)
			{
				float alpha = alphaAnimation.Evaluate (timer);
				ChangeAlpha (alpha);
				timer += Time.deltaTime;
			}
			else
				emergencyRefill.Refill();
		}
		else if (timer > 0f)
			timer = 0f;
    }

	private void ChangeAlpha (float alpha)
	{
		var tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
	}
}
