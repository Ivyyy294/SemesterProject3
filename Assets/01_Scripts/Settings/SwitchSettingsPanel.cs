using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSettingsPanel : MonoBehaviour
{
	[SerializeField] List <GameObject> settingPanelList;

	//Public Methods
	public void ShowPanel (int index)
	{
		if (index < settingPanelList.Count)
		{
			//Disable other panels
			for (int i = 0; i < settingPanelList.Count; ++i)
			{
				if (settingPanelList[i].activeInHierarchy)
					settingPanelList[i].SetActive (false);
			}

			//Show new panel
			settingPanelList[index].SetActive(true);
		}
		else
			Debug.LogError("Invalid Index!");
	}

    // Start is called before the first frame update
    void Start()
    {
        ShowPanel (0);
    }

}
