using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleModels : MonoBehaviour
{
    public GameObject Chassis;
    public GameObject Welding;
    public GameObject Alert;
    public GameObject Ui;
    public GameObject Textos;



    // Start is called before the first frame update
    public void ToggleChassis(bool toggleTarget) {
        Chassis.SetActive(toggleTarget);
    }
    public void ToggleWelding(bool toggleTarget)
    {
        Welding.SetActive(toggleTarget);
    }

    public void ToggleAlert(bool toggleTarget)
    {
        Alert.SetActive(toggleTarget);
    }

    public void ToggleGone(bool toggleTarget)
    {
        Ui.SetActive(toggleTarget);
       
        Textos.SetActive(toggleTarget);
    }



}
