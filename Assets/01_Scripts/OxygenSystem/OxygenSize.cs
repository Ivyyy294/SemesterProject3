using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (OxygenBubbleRefill))]
public class OxygenSize : MonoBehaviour
{
	OxygenBubbleRefill oxygenRefill;

	[Range (0.1f, 1f)]
	[SerializeField] float minScale;

    // Start is called before the first frame update
    void Start()
    {
        oxygenRefill = GetComponent<OxygenBubbleRefill>();
    }

    // Update is called once per frame
    void Update()
    {
        float capacity = oxygenRefill.CapacityOxygen;
		float current = oxygenRefill.CurrentOxygen;
		float scale = minScale + ((1f - minScale) * current / capacity);
		transform.localScale = new Vector3 (scale, scale, scale);
    }
}
