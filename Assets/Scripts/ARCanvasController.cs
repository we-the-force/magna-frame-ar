using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vuforia;

public class ARCanvasController : MonoBehaviour, ITrackableEventHandler
{
    //public Text vuforiaSTATUS;
    public TrackableBehaviour mTrackableBehaviour;

    public Transform ThePointer;

    public bool PCTest;
    public GameObject AR_CAMERA;
    public GameObject PC_CAMERA;

    public GameObject SavingReportPanel;

    public PanelID myPanelID;

    public ReasonPanel REASONPANEL;

    public EvidenceCapture ImageCaptureComponent;

    public GameObject ImageEvidencePanel;
    
    public List<GameObject> ModelTargetPrefab;
    public ModelController currentModelTarget;

    public GameObject UIObject;
    public GameObject PopupObject;
    public Text ErrorMsg;

    public GameObject AlertToggle;
    public GameObject Textos;

    public Material Good;
    public Material Bad;
    public Material Normal;
    public Material Actual;

    //public List<string> EvidenceImageCollection;
    //public List<Texture2D> EvidenceByteCollection;


    private int TotalWeldsToReview;
    public Transform WeldsTransform;

    private int CurrentWeldUnderReview;

    public GetWeldCenters WPos;

    public DefaultTrackableEventHandler modelTargetEventTracker;

    private void Start()
    {
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }


#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        PCTest = true;
#elif UNITY_ANDROID
        PCTest = false;
#endif

        AR_CAMERA.SetActive(!PCTest);
        PC_CAMERA.SetActive(PCTest);

        if (PCTest)
        {
            ImageCaptureComponent = PC_CAMERA.GetComponent<EvidenceCapture>();
        }
        else
        {
            ImageCaptureComponent = AR_CAMERA.GetComponent<EvidenceCapture>();
        }

        UIObject.SetActive(false);
        PopupObject.SetActive(false);

        if (SystemController.instance.CurrentReportDataToSend.chassis == null)
        {
            SceneManager.LoadScene("ChassisReader");
            return;
            //--- Regresar a escanear el chassis, no debio haber llegado aqui;
        }

        myPanelID.userName.text = SystemController.instance.CurrentOperatorData.name;

        SystemController.instance.EvidenceImageCollection = new List<string>();
        SystemController.instance.EvidenceByteCollection = new List<Texture2D>();

        StartCoroutine("InitializeModelTarget");
    }

    IEnumerator InitializeModelTarget()
    {
        yield return new WaitForFixedUpdate();

        if (!PCTest)
        {
            //--- Aqui debera suceder la busqueda del model target correspondiente al chassis escaneado
            //-- y se debera cargar ese objeto 
            //GameObject obj = Instantiate(ModelTargetPrefab[0]);
            //currentModelTarget = obj.GetComponent<ModelController>();

            TotalWeldsToReview = currentModelTarget.Welds.transform.childCount;
            WeldsTransform = currentModelTarget.Welds.transform;
            CurrentWeldUnderReview = 0;

            foreach (Transform weld in WeldsTransform)
            {
                weld.gameObject.SetActive(true);
                weld.GetComponent<Renderer>().material = Normal;
            }
            WeldsTransform.GetChild(CurrentWeldUnderReview).GetComponent<Renderer>().material = Actual;
            ThePointer.position = WPos.WeldPositionCollection[CurrentWeldUnderReview]; // WeldsTransform.GetChild(CurrentWeldUnderReview).transform.position;
        }
        else
        {
            TotalWeldsToReview = 3;
        }

        while (true)
        {
            if (ModelDetected)
            {
                UIObject.SetActive(true);
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        long _time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        SystemController.instance.CurrentReportDataToSend.start_time = _time.ToString();
    }

    public bool ModelDetected = false;

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        switch (newStatus)
        {
            case TrackableBehaviour.Status.DETECTED:
                //vuforiaSTATUS.text = "DETECTED";
                ModelDetected = true;
                break;
            case TrackableBehaviour.Status.EXTENDED_TRACKED:
                ModelDetected = true;
                //vuforiaSTATUS.text = "EXTENDED_TRACKED";
                break;
            case TrackableBehaviour.Status.LIMITED:
                //vuforiaSTATUS.text = "LIMITED";
                break;
            case TrackableBehaviour.Status.NO_POSE:
                //vuforiaSTATUS.text = "NO POSE";
                break;
            case TrackableBehaviour.Status.TRACKED:
                ModelDetected = true;
                //vuforiaSTATUS.text = "TRACKED";
                break;
        }

        if (newStatus == TrackableBehaviour.Status.DETECTED)
        {
            UIObject.SetActive(true);
        }
    }

    //---- PROCESO DE VALIDACION DE LOS PUNTOS DE SOLDADURA DE LA PIEZA
    private WeldRegistryPOST _weldData;
    public void SetWeldStatus(int status)
    {      
        _weldData = new WeldRegistryPOST();

        //-- status 0 >> bad 1 >> good
        if (!PCTest)
        {
            Transform selectedWeld = WeldsTransform.GetChild(CurrentWeldUnderReview);
            if (status == 1)
            {
                selectedWeld.GetComponent<Renderer>().material = Good;
                _weldData.status = "good";
                _weldData.defect = "good";
                AddWeldInfoToDB();
            }
            else
            {
                selectedWeld.GetComponent<Renderer>().material = Bad;
                _weldData.status = "bad";

                UIObject.SetActive(false);
                myPanelID.gameObject.SetActive(false);
                REASONPANEL.ShowPanel();
            }
        }
        else
        {
            if (status == 1)
            {
                _weldData.status = "good";
                _weldData.defect = "good";
                AddWeldInfoToDB();
            }
            else
            {
                _weldData.status = "bad";
                UIObject.SetActive(false);
                myPanelID.gameObject.SetActive(false);
                REASONPANEL.ShowPanel();
            }
            //SystemController.instance.CurrentReportDataToSend.report_welds.Add(_weldData);
            //CurrentWeldUnderReview++;
        }
    }

    public void WeldReasonSet(string _reason)
    {
        _weldData.defect = _reason;
        REASONPANEL.gameObject.SetActive(false);
        myPanelID.gameObject.SetActive(true);
        UIObject.SetActive(true);
        AddWeldInfoToDB();
    }

    private void AddWeldInfoToDB()
    {
        SystemController.instance.CurrentReportDataToSend.report_welds.Add(_weldData);

        CurrentWeldUnderReview++;

        if (!PCTest)
        {
            if (CurrentWeldUnderReview < TotalWeldsToReview)
            {
                WeldsTransform.GetChild(CurrentWeldUnderReview).GetComponent<Renderer>().material = Actual;
                ThePointer.position = WPos.WeldPositionCollection[CurrentWeldUnderReview]; // WeldsTransform.GetChild(CurrentWeldUnderReview).transform.position;
            }
        }

        //--- Toma foto de evidencia
        UIObject.SetActive(false);
        ImageEvidencePanel.SetActive(true);

        //evidenceTaken = false;
        Debug.LogError("Enviando a capturar imagn");


        ImageCaptureComponent.changeStatus(ImageEvidenceResponse);
        StartCoroutine("BackFromImageThread");
    }
          
    private bool evidenceTaken = false;

    public void ImageEvidenceResponse(bool imageTaken, string fileName, Texture2D _texture)
    {
        evidenceTaken = true;
        Debug.LogError("Archivo guardado en : " + fileName);
        SystemController.instance.EvidenceImageCollection.Add(fileName);
        SystemController.instance.EvidenceByteCollection.Add(_texture);
    }

    IEnumerator BackFromImageThread()
    {
        yield return new WaitForSeconds(0.3f);

        while(!evidenceTaken)
        {
            yield return new WaitForFixedUpdate();
        }

        evidenceTaken = false;

        UIObject.SetActive(true);
        ImageEvidencePanel.SetActive(false);

        if (CurrentWeldUnderReview == TotalWeldsToReview)
        {
            //--- Llego al final de los welds, muestra la ventana para enviar reporte o reiniciar
            UIObject.SetActive(false);
            PopupObject.SetActive(true);
        }
    }


    public void Undo()
    {
        if (PCTest)
        {
            if (CurrentWeldUnderReview > 0)
            {
                CurrentWeldUnderReview--;
                SystemController.instance.CurrentReportDataToSend.report_welds.RemoveAt(SystemController.instance.CurrentReportDataToSend.report_welds.Count - 1);

                int tot = SystemController.instance.EvidenceImageCollection.Count - 1;
                string imageToDelete = SystemController.instance.EvidenceImageCollection[tot];
                //---- ELIMINAR LA IMAGEN CORRESPONDIENTE DEL DISPOSITIVO
                System.IO.File.Delete(imageToDelete);
                SystemController.instance.EvidenceImageCollection.RemoveAt(tot);

                tot = SystemController.instance.EvidenceByteCollection.Count - 1;
                SystemController.instance.EvidenceByteCollection.RemoveAt(tot);
            }
            return;
        }

        if (CurrentWeldUnderReview > 0)
        {
            WeldsTransform.GetChild(CurrentWeldUnderReview).GetComponent<Renderer>().material = Normal;

            CurrentWeldUnderReview--;

            WeldsTransform.GetChild(CurrentWeldUnderReview).GetComponent<Renderer>().material = Actual;
            ThePointer.position = WPos.WeldPositionCollection[CurrentWeldUnderReview]; // WeldsTransform.GetChild(CurrentWeldUnderReview).transform.position;

            SystemController.instance.CurrentReportDataToSend.report_welds.RemoveAt(SystemController.instance.CurrentReportDataToSend.report_welds.Count - 1);

            int tot = SystemController.instance.EvidenceImageCollection.Count - 1;
            string imageToDelete = SystemController.instance.EvidenceImageCollection[tot];
            //---- ELIMINAR LA IMAGEN CORRESPONDIENTE DEL DISPOSITIVO
            System.IO.File.Delete(imageToDelete);
            SystemController.instance.EvidenceImageCollection.RemoveAt(tot);

            tot = SystemController.instance.EvidenceByteCollection.Count - 1;
            SystemController.instance.EvidenceByteCollection.RemoveAt(tot);
        }
    }

    public void Resetinterface()
    {
        if (PCTest)
            return;

        SystemController.instance.CurrentReportDataToSend.report_welds = new List<WeldRegistryPOST>();

        SystemController.instance.EvidenceImageCollection = new List<string>();
        SystemController.instance.EvidenceByteCollection = new List<Texture2D>();

        foreach (Transform weld in WeldsTransform)
        {
            weld.GetComponent<Renderer>().material = Normal;
        }
        CurrentWeldUnderReview = 0;
        WeldsTransform.GetChild(CurrentWeldUnderReview).GetComponent<Renderer>().material = Actual;
        ThePointer.position = WPos.WeldPositionCollection[CurrentWeldUnderReview]; // WeldsTransform.GetChild(CurrentWeldUnderReview).transform.position;

        UIObject.SetActive(true);
        PopupObject.SetActive(false);
    }

    /// <summary>
    /// Send report to the database
    /// </summary>
    public void Send()
    {
        ErrorMsg.gameObject.SetActive(false);
        PopupObject.SetActive(false);

        SavingReportPanel.SetActive(true);

        long _time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        SystemController.instance.CurrentReportDataToSend.finish_time = _time.ToString();

        //--- Sube todas las imagenes de Evidencia tomadas
        StartCoroutine(DataBaseController.instance.SendEvidenceImage(EvidenceUploadResponse));

        /*
        if (SystemController.instance.ReportReplace)
        {
            StartCoroutine(DataBaseController.instance.PutReportData(DatabaseMessage));
        }
        else
        {
            StartCoroutine(DataBaseController.instance.PutReportData(DatabaseMessage));
        }
        */
    }


    private string chassisSerial;
    public void DatabaseMessage(bool status)
    {
        if (status)
        {
            SceneManager.LoadScene("ChassisReader");
        }            
        else
        {
            ErrorMsg.gameObject.SetActive(true);
        }
    }

    /*
    public void GetSavedReport(ReportDataRegistry _reportData)
    {
        if (_reportData != null)
        {
            StartCoroutine(DataBaseController.instance.SendEvidenceImage(CurrentWeldUnderReview, EvidenceUploadResponse));
        }
    }
    */


    //--- Respuesta de cuando se suben las imagenes al servidor
    public void EvidenceUploadResponse(bool success)
    {
        if (success)
        {
            //--- Es hora de enviar el reporte a la base de datos
            StartCoroutine(DataBaseController.instance.PutReportData(DatabaseMessage));
        }
        else
        {
            ErrorMsg.gameObject.SetActive(true);
        }
    }

    //-------------------------------------------------------------------------------------------------

    private bool _showingChassis = true;
    public void ButtonChassis()
    {
        _showingChassis = !_showingChassis;
        currentModelTarget.Chassis.SetActive(_showingChassis);
    }

    private bool _showingWelds = true;
    public void ButtonWelds()
    {
        _showingWelds = !_showingWelds;
        currentModelTarget.BaseWeldingGroup.SetActive(_showingWelds);
    }

    public GameObject AlertObject;
    private bool _showAlert = true;
    public void ButtonAlert()
    {
        _showAlert = !_showAlert;
        AlertObject.SetActive(_showAlert);
    }


    public void ToggleChassis(bool toggleTarget)
    {       
        currentModelTarget.Chassis.SetActive(toggleTarget);
    }
    public void ToggleWelding(bool toggleTarget)
    {
        currentModelTarget.BaseWeldingGroup.SetActive(toggleTarget);
    }

    public void ToggleAlert(bool toggleTarget)
    {
        AlertToggle.SetActive(toggleTarget);
    }

    public void ToggleGone(bool toggleTarget)
    {
        UIObject.SetActive(toggleTarget);
        Textos.SetActive(toggleTarget);
    }
}
