using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
	[SerializeField] float xOffset;
	[Range (0, 2)]
	[SerializeField] float easeInSpeed = 1f;
	[Range (0, 2)]
	[SerializeField] float easeOutSpeed = 1f;
	Vector2 basePos;
	RectTransform rectTransform;

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
		if (rectTransform == null)
		{
			rectTransform = GetComponent <RectTransform>();
			basePos = rectTransform.anchoredPosition;
		}
		else
		{
			timer = 0f;
			mouseOver = false;
			rectTransform.anchoredPosition = basePos;
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
		newPos.x += xOffset * EaseOutQuint(timer);
		rectTransform.anchoredPosition = newPos;
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
