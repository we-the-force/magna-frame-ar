using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class report : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showOperatorId(){
        Debug.Log ("Operador ID: "+ dataCtrl.report.operador.id);
    }
}
