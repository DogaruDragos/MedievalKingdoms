using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenPanelTutorial : MonoBehaviour
{
    public GameObject Panel;
    public bool isOn = true;

    public void OpenPanel(string textT)
    {
        if(Panel != null && isOn)
        {
            TextMeshProUGUI textt=Panel.transform.Find("TutorialText").GetComponent<TextMeshProUGUI>();
            textt.text = textT;
            Panel.SetActive(true);
        }
    }
    public void ClosePanel()
    {
        Panel.SetActive(false);
    }

    public void JustOpenPanel()
    {
        if (Panel != null && isOn)
        {
            Panel.SetActive(true);
        }
    }

    public void ToggleTutorial()
    {
        isOn = !isOn;
    }

    public void CloseOptionsPanel()
    {
        StaticInformation.isHighSettings = GameObject.Find("ToggleLG").GetComponent<Toggle>().isOn;
        StaticInformation.isMediumSettings = GameObject.Find("ToggleMG").GetComponent<Toggle>().isOn;
        StaticInformation.isLowSettings = GameObject.Find("ToggleSG").GetComponent<Toggle>().isOn;

        StaticInformation.isHugeMap = GameObject.Find("ToggleLMap").GetComponent<Toggle>().isOn;
        StaticInformation.isMediumMap = GameObject.Find("ToggleMMap").GetComponent<Toggle>().isOn;
        StaticInformation.isSmallMap = GameObject.Find("ToggleSMap").GetComponent<Toggle>().isOn;

        StaticInformation.NumberOfPlayers = (int)GameObject.Find("SliderNr").GetComponent<Slider>().value;

        if (Panel != null && isOn)
        {
            Panel.SetActive(false);
        }
    }

    public void TogglePanel()
    {
        if(Panel != null)
        {
            if (Panel.activeSelf)
            {
                Panel.SetActive(false);
            }
            else
            {
                Panel.SetActive(true);
            }
        }
    }
}
