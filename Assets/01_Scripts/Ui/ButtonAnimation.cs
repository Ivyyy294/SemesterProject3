using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
	[SerializeField] Vector2 offset;
	[Range (0, 2)]
	[SerializeField] float easeInSpeed = 1f;
	[Range (0, 2)]
	[SerializeField] float easeOutSpeed = 1f;
	[SerializeField] RectTransform targetRectTransform;
	Vector2 basePos;

	float timer = 0f;
	bool mouseOver = false;

	public void OnPointerEnter()
	{
		mouseOver = true;
	}

	public void OnPointerExit()
	{
		mouseOver = false;
	}

	private void OnEnable()
	{
		if (targetRectTransform == null)
		{
			targetRectTransform = GetComponent <RectTransform>();
			basePos = targetRectTransform.anchoredPosition;
		}
		else
		{
			timer = 0f;
			mouseOver = false;
			targetRectTransform.anchoredPosition = basePos;
		}
	}

	private void Update()
	{
		if (mouseOver)
			timer += Time.deltaTime * easeInSpeed;
		else
			timer -= Time.deltaTime * easeOutSpeed;

		timer = Mathf.Clamp (timer, 0f, 1f);

		Vector2 newPos = basePos;
		newPos += offset * EaseOutQuint(timer);
		targetRectTransform.anchoredPosition = newPos;
	}

	float EaseOutCirc(float x)
	{
		return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
	}

	float EaseOutQuint (float x)
	{
		return 1 - Mathf.Pow(1 - x, 5);
	}
}
