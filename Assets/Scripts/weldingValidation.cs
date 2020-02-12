using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class weldingValidation : MonoBehaviour
{
    public GameObject cLog;
    public GameObject Welds;
    public GameObject Interface;
    public GameObject PopUp;
    public Material Good;
    public Material Bad;
    public Material Normal;
    public Material Actual;
    public Text ChildLenght;
    public class Global
    {

        public static int CurrentChild = 0;
        
        // public static Weld currentWeld;
       
    }




    // Start is called before the first frame update
    void Start()
    {
        PopUp.SetActive(false);
        ChildLenght.text = Welds.transform.childCount.ToString();
       
        foreach (Transform weld in Welds.transform)
        {
            weld.gameObject.SetActive(true);
            weld.GetComponent<Renderer>().material = Normal;
        }
        Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;



    }
    public void GoodWeld()
    {
        int maxChilds = Welds.transform.childCount - 1;
        //Weld currentWeld = new Weld();
        if (Global.CurrentChild < maxChilds)
        {
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Good;
            
            //currentWeld.id = Global.CurrentChild;
            //currentWeld.status = "Good";
            //dataCtrl.report.chassis.welds.Add(currentWeld);
            //weldsList.Add(new Weld(currentWeld.id, currentWeld.status));
             
            
            Global.CurrentChild = Global.CurrentChild + 1;
            //Debug.Log (currentWeld);
           
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;
            
        }

        else
        {
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Good;
           
            Interface.SetActive(false);
            PopUp.SetActive(true);

        }
        
    }
    public void BadWeld()
    {
        int maxChilds = Welds.transform.childCount - 1;
        //Weld currentWeld = new Weld();
        if (Global.CurrentChild < maxChilds)
        {
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Bad;

            //currentWeld.id = Global.CurrentChild;
            //currentWeld.status = "Bad";
            //dataCtrl.report.chassis.welds.Add(currentWeld);

            Global.CurrentChild = Global.CurrentChild + 1;
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;
        }
        else
        {
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Bad;
            Interface.SetActive(false);
            PopUp.SetActive(true);

        }
    }

    public void Undo()
    {
        
        if (Global.CurrentChild > 0)
        {
            
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Normal;
            
            //Global.CurrentChild = Global.CurrentChild - 1;
            //dataCtrl.report.chassis.welds.RemoveAt(Global.CurrentChild);
            Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;

        }

    }

    public void Resetinterface()
    {
        
        foreach (Transform weld in Welds.transform)
        {
            
            weld.GetComponent<Renderer>().material = Normal;
        }
        Global.CurrentChild = 0;
        Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;
        
        Interface.SetActive(true);
        PopUp.SetActive(false);



    }

    public void Send()
    {
        foreach (Transform weld in Welds.transform)
        {
            
            weld.GetComponent<Renderer>().material = Normal;
        }
        Global.CurrentChild = 0;
        Welds.transform.GetChild(Global.CurrentChild).GetComponent<Renderer>().material = Actual;
        
        Interface.SetActive(false);
        PopUp.SetActive(false);
        //SceneManager.LoadScene("ChassisReader", LoadSceneMode.Additive);
        //string json = JsonUtility.ToJson(dataCtrl.report);
        //cLog.GetComponent<UnityEngine.UI.Text>().text=json;
        //Debug.Log(json);

    }



}



