using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchScoreControllerUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI labelPointsTeam1;
	[SerializeField] TextMeshProUGUI labelPointsTeam2;
	[SerializeField] AudioAsset audioScore;

	MatchScoreController matchScoreController;

    // Start is called before the first frame update
    void Start()
    {
        matchScoreController = MatchController.Me.MatchScoreController;
    }

    // Update is called once per frame
    void Update()
    {
		labelPointsTeam1.text = matchScoreController.PointsTeam1.ToString();
		labelPointsTeam2.text = matchScoreController.PointsTeam2.ToString();
    }
}
