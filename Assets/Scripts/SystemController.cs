using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--- Usado para guardar en disco -----------------
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//--------------------------------------------------

public class SystemController : MonoBehaviour
{
    public static SystemController instance;

   // public DataBaseFile DBFILE { get; set; }

    public int CurrentOperator;
    public int CurrentChassisID;

    //public Operator CurrentOperatorData;
    //public ReportData CurrentReportData;

    public bool ReportReplace = false;

    public OperatorRegistry CurrentOperatorData;
    public ReportDataRegistry CurrentReportData;

    public ReportDataRegistryPOST CurrentReportDataToSend;

    public List<Texture2D> EvidenceByteCollection;
    public List<string> EvidenceImageCollection;

    public ChassisRegistry CurrentChassisData;

    //-- SINGLETON PERSISTANCY ----------------
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }
    //-----------------------------------------

    private void Start()
    {
        CurrentChassisID = 0;
        CurrentOperator = 0;

        //LoadDatabase();

       // DataBaseController.instance.dbtest();
    }

    /*
    public DataBaseFile CreateDatabase()
    {
        DataBaseFile save = new DataBaseFile();
        return save;
    }


    public void SaveDatabase()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/magnaReport.save");
        bf.Serialize(file, DBFILE);
        file.Close();
    }
    */

    public void LoadDatabase()
    {
        /*
        if (File.Exists(Application.persistentDataPath + "/magnaReport.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/magnaReport.save", FileMode.Open);
            DBFILE = (DataBaseFile)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            //--- No existe un archivo, crear archivo nuevo
            DBFILE = CreateDatabase();
        }
        */
    }

    public void AddOperator(OperatorRegistry _operator)
    {
        //--- Agregar nuevo operador a la base de datos
    }


    /// <summary>
    /// Searches by name for the operator, and returns the ID if it is found
    /// else, returns -1
    /// </summary>
    /// <param name="_name">String value representing operator Name</param>
    /// <returns></returns>
    public int OperatorExist(string _name)
    {
        int returnID = -1;

        //--- 
        for (int i = 0; i < DataBaseController.instance.OperatorRegistryCollection.operators.Count; i++)
        {
            if (DataBaseController.instance.OperatorRegistryCollection.operators[i].name == _name)
            {
                returnID = DataBaseController.instance.OperatorRegistryCollection.operators[i].id;
            }
        }
        
        return returnID;
    }
       
    /// <summary>
    /// Searches by ID for the operator, and returns operator Data if found, 
    /// else returns null
    /// </summary>
    /// <param name="_id">Int value representing the Operator ID</param>
    /// <returns></returns>
    public OperatorRegistry OperatorExist(int _id)
    {
        OperatorRegistry _operatorData = null;

        for (int i = 0; i < DataBaseController.instance.OperatorRegistryCollection.operators.Count; i++)
        {
            int _opId = -1;

            if (int.TryParse(DataBaseController.instance.OperatorRegistryCollection.operators[i].operator_number, out _opId))
            {
                if (_opId == _id)
                {
                    _operatorData = DataBaseController.instance.OperatorRegistryCollection.operators[i];
                }
            }
        }

        return _operatorData;
    }

    /// <summary>
    /// Searches for QA Report Data using Chassis Serial Number, and returns this data if found,
    /// else returns null
    /// If the parameter _replace is true, the data will be replaced with _replaceWithThisData
    /// If the report does not exist in the DB, and _replaceWithThisData is not null, and _replace is true
    /// the report data will be added to the DB
    /// </summary>
    /// <param name="_chassisId"></param>
    /// <param name="_replace"></param>
    /// <param name="_replaceWithThisData"></param>
    /// <returns></returns>
    public ReportDataRegistry ReportForChassisExist(int _chassisId, bool _replace = false, ReportDataRegistry _replaceWithThisData = null)
    {
        ReportDataRegistry _reportData = null;

        for (int i = 0; i < DataBaseController.instance.ReportDataRegistryCollection.reportData.Count; i++)
        {
            ReportDataRegistry _r = DataBaseController.instance.ReportDataRegistryCollection.reportData[i];

            if (_r.chassis_serial == _chassisId.ToString())
            {
                _reportData = _r;
                if (_replace)
                {
                    _reportData = _replaceWithThisData;
                    if (_replaceWithThisData != null)
                    {
                        //--- AQUI DEBERA HACER LA SUSTITUCION DEL REPORTE EN LA BASE DE DATOS
                    }
                }
                break;
            }
        }

        //--- Agregar nuevo registro ? 
        if (_replace && _reportData == null && _replaceWithThisData != null)
        {
            //--- Agregar nuevo registro en la base de datos (reporte)
        }

        return _reportData;
    }


    /*
    //--- Para cosa relacionada con tiempos y Timestamps
                    long _l = new System.DateTimeOffset(System.DateTime.Now).ToUnixTimeSeconds();

                for (int i = 0; i < _save.TimeCapsuleCollection.Count; i++)
                {
                    float tiempoDiferencia = _l - _save.TimeCapsuleCollection[i].TimeStamp;
                    float tiempoRestante = _save.TimeCapsuleCollection[i].OriginalTimerSeconds - tiempoDiferencia;

                    if (tiempoRestante < 0)
                        tiempoRestante = 0;

                    _save.TimeCapsuleCollection[i].TimerSeconds = tiempoRestante;
                }
    */
}
