using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReasonPanel : MonoBehaviour
{
    public ARCanvasController myCanvas;

    public List<GameObject> BulletCollection;

    public List<string> DefectReasonCollection;

    public Text ReasonText;

    public InputField OtherReasonText;

    public string selectedReason;
    
    private int currentDefect = 0;

    public GameObject errorTxt;

    public void ShowPanel()
    {
        errorTxt.SetActive(false);
        OtherReasonText.interactable = false;
        OtherReasonText.text = "";

        currentDefect = -1;

        showBullet();

        //ReasonText.text = "Select defect reason";

        gameObject.SetActive(true);
    }

    private void showBullet()
    {
        errorTxt.SetActive(false);

        for (int i = 0; i < BulletCollection.Count; i++)
        {
            BulletCollection[i].SetActive(false);
        }

        if (currentDefect >= 0)
        {
            BulletCollection[currentDefect].SetActive(true);
        }

        if (currentDefect > 2)
            OtherReasonText.interactable = true;
        else
            OtherReasonText.interactable = false;
    }

    public void SelectReason(int _reason)
    {
        currentDefect = _reason;

        showBullet();

        selectedReason = DefectReasonCollection[_reason];
               

       // ReasonText.text = "Selected reason: " + DefectReasonCollection[_reason];
    }

    public void AcceptReason()
    {
        //-- en caso de que la razon sea == 3; es Otro, asignar el texto
        if (currentDefect == 3)
        {
            selectedReason = OtherReasonText.text;

            if (string.IsNullOrEmpty(selectedReason))
            {
                errorTxt.SetActive(true);
                return;
            }
        }

        //myCanvas.WeldReasonSet(DefectReasonCollection[currentDefect]);
        myCanvas.WeldReasonSet(selectedReason);
    }
}
