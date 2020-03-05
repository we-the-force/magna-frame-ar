using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelID : MonoBehaviour
{
    public GameObject ProfilePic;
    public GameObject LogoutPanel;

    public Text userName;

    public ARCanvasController myCanvas;
         
    public void ProfilePicClick()
    {
        ProfilePic.SetActive(false);
        LogoutPanel.SetActive(true);

        myCanvas.UIObject.SetActive(false);
    }

    public void ReturnButtonClick()
    {
        ProfilePic.SetActive(true);
        LogoutPanel.SetActive(false);
        myCanvas.UIObject.SetActive(true);
    }
}
