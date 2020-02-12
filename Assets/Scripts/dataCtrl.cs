using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Weld {
  public int id;
  public string status;

  public Weld(Weld welding)
  {
    id = welding.id;
    status = welding.status;

  }

  
}

public class Chassis {
  public string id;
  public List<Weld> welds;
}

 

public class Operator {
  public string id;
  public string name;
  public string lineaProd;
} 

public class Report {
  public int id;
  public int date;
  public Operator operador;
  public Chassis chassis;
  
  
}



 

public class dataCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    
    public static Report report;
    public GameObject Employee;
    public GameObject Debogo;
    public GameObject ChassisID;
    public GameObject OperatorID;

    public GameObject Chasis;

    
    void Awake()
    {
       
      
    }  
    // Start is called before the first frame update
    void Start()
    {
      DontDestroyOnLoad (this.gameObject);
      List<Weld> weldsList = new List<Weld>();
      
      
      
     
        
        
        
    }
    

    

    // Update is called once per frame
    void Update()
    {
      
        
    }

    
    
   
    
}
