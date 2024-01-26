using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

using TMPro;


public class OxygenUi : MonoBehaviour
{
    public int MinimumOxygenRange;
    public int MaximumOxygenRange;
    public int MaximumBarWidth;
    public int MaximumBarHeight;
    public int BarX;
    public int BarY;

    [SerializeField] TextMeshProUGUI oxygenLabel;
    PlayerOxygen playerOxygen;


    void Start()
    {
        playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
    }

	#if UNITY_EDITOR
    void OnGUI()
    {
        int CurrentBarWidth = 0;

        /*if(int(playerOxygen) < MinimumOxygenRange)
        {
            CurrentBarWidth = MaximumBarWidth;
        }*/


        int a1 = BarX;
        int a2 = BarY;
        int b1 = BarX + MaximumBarWidth - CurrentBarWidth;
        int b2 = BarY + MaximumBarWidth;
        EditorGUI.DrawRect(new Rect(a1, a2, b1, b2), Color.green);
    }
	#endif
}

/*
int min_oxygen_range;
int max_oxygen_range;
int max_bar_width;
int max_bar_height;
int bar_x;
int bar_y;

a1 = bar_x (top right)
a2 = bar_y (top right)
b1 = bar_x + max_bar_width - bar_current_width;
a2 = bar_y + max_bar_height;
EditorGUI.DrawRect(new Rect(a1, a2, b1, b2), Color.black);

if(player_oxygen_amount < int_min_oxygen_range) {draw(all);}

if(player_oxygen_amount > int_min_oxygen_range && player_oxygen_amount < int_max_oxygen_range)
{
    calculate difference percentage;
    draw(percentage);
}

if(player_oxygen_amount > int_max_oxygen_range) {draw(none);}



*/


/*public class OxygenUi : EditorWindow
{
    //public float BarMaxWidth;
    //public Vector2 BarOrigin;
    //This is the value of the Slider
    float m_Value = 50;

    [MenuItem("OxygenBar")]
    static void Init()
    {
        var window = (OxygenUi)GetWindow(typeof(OxygenUi));
        window.position = new Rect(0, 0, 350, 300);
    }

    void OnGUI()
    {
        //This is the Label for the Slider
        GUI.Label(new Rect(0, 0, 100, 30), "Rectangle Width");
        //This is the Slider that changes the size of the Rectangle drawn
        m_Value = GUI.HorizontalSlider(new Rect(100, 0, 100, 30), m_Value, 1.0f, 250.0f);

        //The rectangle is drawn in the Editor (when MyScript is attached) with the width depending on the value of the Slider
        EditorGUI.DrawRect(new Rect(50, 50, m_Value, 70), Color.green);
    }
}*/