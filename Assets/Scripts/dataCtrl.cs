using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

/*
[System.Serializable]
public class Weld {
    public int id;
    public string status;
    public int FailureID;
}


[System.Serializable]
public class Chassis {
    public int id;
    public List<Weld> welds = new List<Weld>();
}

[System.Serializable]
public class Operator {
    public int id;
    public string name;
    public string lineaProd;
    public bool isActive;
} 

[System.Serializable]
public class ReportData {
    public int id;
    
    public long DateTimeStamp; //-- TimeStamp in UnixTime representing the Date of the report
    public long StartTimeStamp; //-- TimeStamp in UnixTime representing the initial time of the report
    public long EndTimeStamp; //-- TimeStamp in UnixTime representing the ending time of the report 
                              //this and StartTimeStamp are used to get total time of the report process

    public Operator operador;
    public Chassis chassis;
    public bool isActive;
}
*/

[System.Serializable]
public class IDPair
{
    public string id;
}

[System.Serializable]
public class IntIDPair
{
    public int id;
}

[System.Serializable]
public class ReportDataRegistry
{
    public int id;
    public OperatorReportInfo employee;
    public ChassisReportInfo chassis;
    public string start_time;
    public string finish_time;
    public string chassis_serial;
    public string created_at;
    public string updated_at;
    public List<WeldRegistry> report_welds = new List<WeldRegistry>();
}

[System.Serializable]
public class OperatorReportInfo
{
    public int id;
    public string operator_number;
}

[System.Serializable]
public class ChassisReportInfo
{
    public int id;
    public string model_name;
    public string model_number;
}

[System.Serializable]
public class WeldRegistry
{
    public int id;
    public string status;
    //public IntIDPair evidence;
}

[System.Serializable]
public class ReportDataTable
{
    public List<ReportDataRegistry> reportData;
}

[System.Serializable]
public class ReportDataRegistryPOST
{
    public OperatorReportInfoPOST employee;
    public ChassisReportInfoPOST chassis;
    public string start_time;
    public string finish_time;
    public string chassis_serial;
    public List<WeldRegistryPOST> report_welds = new List<WeldRegistryPOST>();
}

[System.Serializable]
public class OperatorReportInfoPOST
{
    public int id;
}

[System.Serializable]
public class ChassisReportInfoPOST
{
    public int id;
}

[System.Serializable]
public class WeldRegistryPOST
{
    public string status;
    public int evidence;
    public string defect;
}

[System.Serializable]
public class WeldRegistryPOSTTable
{
    public List<WeldRegistryPOST> report_welds = new List<WeldRegistryPOST>();
}

[System.Serializable]
public class OperatorRegistry
{
    public int id;
    public string name;
    public string operator_number;
    public string created_at;
    public string updated_at;
    //public OperatorCodeImage code_image;
}

/*
[System.Serializable]
public class OperatorCodeImage
{
    public int id;
    public string name;
    public string hash;
    public string sha256;
    public string ext;
    public string mime;
    public float size;
    public string url;
    public string provider;
    public object provider_metadata;
    public string created_at;
    public string updated_at;
}
*/

[System.Serializable]
public class OperatorTable
{
    public List<OperatorRegistry> operators;// = new List<OperatorRegistry>();
    //public OperatorRegistry[] operators;
}

[System.Serializable]
public class ChassisRegistry
{
    public string id;
    public string model_name;
    public string model_number;
    public List<WeldRegistryOnChassis> welds;
}

[System.Serializable]
public class WeldRegistryOnChassis
{
    public int id;
    public string codename;
}


[System.Serializable]
public class ChassisTable
{
    public List<ChassisRegistry> chassis;
}
    
[System.Serializable]
public class FailureReasonCatalog
{
    public int FailureID;
    public string FailureName;
}

/*
[System.Serializable]
public class DataBaseFile
{
    public List<ReportData> QA_Data = new List<ReportData>();
    public List<Operator> OperatorData = new List<Operator>();
    public List<FailureReasonCatalog> FailureReasonData = new List<FailureReasonCatalog>();
}*/


[System.Serializable]
public class UserInfo
{
    public string user;
    public string password;
}

public class dataCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    
    //public static ReportData report;
    public GameObject Employee;
    public GameObject Debogo;
    public GameObject ChassisID;
    public GameObject OperatorID;

    public GameObject Chasis;

    
    void Awake()
    {
       
      
    }  
    // Start is called before the first frame update
   /*void Start()
    {
      DontDestroyOnLoad (this.gameObject);
      List<Weld> weldsList = new List<Weld>();

    }*/
   
}
