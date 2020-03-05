using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizcorp.Utils.Logger;

public class ChassisReader : MonoBehaviour {

	private IScanner BarcodeScanner;
	public Text ScannerStatusText;
    public Text OperatorIDText;

    public GameObject inspectChassisPanel;

    public GameObject BGNormal;
    public GameObject BGScan;

    public Text ReportDataTxt;

    public GameObject ErrorPanel;
    public Text ErrorText;

    public RawImage _Image;
	public AudioSource Audio;
    
    public InputField ChassisIDField;
    public GameObject StartScanButton;
    public GameObject InputChassisButton;
    public GameObject StopScanbutton;
    public GameObject NextButton;
    public GameObject BackButton;

    public GameObject PanelYesNo;
    private bool hasAnswer = false;
    private bool Answer = false;

    // Disable Screen Rotation on that screen
    void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

    private void Start()
    {
        if (SystemController.instance.CurrentOperatorData == null)
        {
            //-- No debería haber llegado aqui, pero si llega, lo regresa a la pantalla anterior
            //-- pues no existe informacion del operador
            ClickBack();
            return;
        }

        SystemController.instance.ReportReplace = false;

        hasAnswer = false;
        Answer = false;

        OperatorIDText.text = "Operator ID: " + SystemController.instance.CurrentOperatorData.id.ToString() + " Operator Name: " + SystemController.instance.CurrentOperatorData.name;
    }

    void StartScanBarcode() {
        // Create a basic scanner
        BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			_Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			_Image.transform.localScale = BarcodeScanner.Camera.GetScale();
			_Image.texture = BarcodeScanner.Camera.Texture;

			// Keep Image Aspect Ratio
			var rect = _Image.GetComponent<RectTransform>();
			var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

            StartCoroutine("iStartScanner");
		};

		// Track status of the scanner
		BarcodeScanner.StatusChanged += (sender, arg) => {
            ScannerStatusText.text = "Item: " + BarcodeScanner.Status;
		};
	}

	/// <summary>
	/// The Update method from unity need to be propagated to the scanner
	/// </summary>
	void Update()
	{
		if (BarcodeScanner == null)
		{
			return;
		}

		BarcodeScanner.Update();
	}

    #region UI Buttons

    public void ClickStart()
    {
        _Image.gameObject.SetActive(true);
        StartScanBarcode();
    }

    IEnumerator iStartScanner()
    {
        yield return new WaitForSeconds(0.25f);

		if (BarcodeScanner == null)
		{
			Log.Warning("No valid camera - Click Start");
			yield break;
		}
        
        ChassisIDField.interactable = false;
        
        BGNormal.SetActive(false);
        BGScan.SetActive(true);
        StartScanButton.SetActive(false);
        //InputChassisButton.SetActive(false);
        //BackButton.SetActive(true);
        //NextButton.SetActive(true);
        //StopScanbutton.SetActive(true);

        // Start Scanning
        BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();

            ChassisIDField.text = barCodeValue;
                                                  
            ClickStop();
			// Feedback
			Audio.Play();

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
            #endif
            
            //ValidateData(barCodeValue);
		});
	}

    /// <summary>
    /// Aqui es donde se registra la respuesta de la base de datos, cuando valida si existe el serial del Chassis en algun reporte
    /// </summary>
    /// <param name="_reportData"></param>
    public void ChassisValidateResponse(ReportDataRegistry _reportData)
    {
        SystemController.instance.CurrentReportData = _reportData;
               
        if (SystemController.instance.CurrentReportData != null)
        {
            //--- El reporte para este chassis en particular ya existe, preguntar si quiere reemplazarlo (volverlo a hacer)
            //--- o quisiera escanear / capturar otro numero

            hasAnswer = false;

            ReportDataTxt.text = JsonUtility.ToJson(SystemController.instance.CurrentReportData);

            PanelYesNo.SetActive(true);
            StartCoroutine(WaitForDesicion());
        }
        else
        {
            Debug.LogError("La respuesta es nula, creando registro nuevo");

            createNewReportToSend();
            ProceedToQA();
        }
    }

    private void createNewReportToSend(bool fromExistingReport = false)
    {
        ReportDataRegistryPOST _reportDataPost = new ReportDataRegistryPOST();
        OperatorReportInfoPOST OperatorPair = new OperatorReportInfoPOST();
        ChassisReportInfoPOST ChassisPair = new ChassisReportInfoPOST();

        if (fromExistingReport)
        {
            OperatorPair.id = SystemController.instance.CurrentReportData.employee.id;
            ChassisPair.id = SystemController.instance.CurrentReportData.chassis.id;
        }
        else
        {
            OperatorPair.id = SystemController.instance.CurrentOperatorData.id;
            ChassisPair.id = 1;
        }

        _reportDataPost.report_welds = new List<WeldRegistryPOST>();
        _reportDataPost.chassis_serial = SystemController.instance.CurrentChassisID.ToString();
        _reportDataPost.employee = OperatorPair;
        _reportDataPost.chassis = ChassisPair;

        SystemController.instance.CurrentReportDataToSend = _reportDataPost;
    }

    private void ValidateData(string _value)
    {
        int ChassisSerialNumber = 0;
        if (!int.TryParse(_value, out ChassisSerialNumber))
        {
            ShowError(true, "Error en el formato de No de Serie del Chassis");
            return;
        }

        StartCoroutine(DataBaseController.instance.GetReportDataFromChassis(_value, ChassisValidateResponse));
        //SystemController.instance.CurrentReportData = SystemController.instance.ReportForChassisExist(ChassisSerialNumber);
    }

    public void YesNoAnswer(int a)
    {
        Debug.LogError("YES NO ANSWER " + a);

        if (a == 0)
            Answer = false;
        else
            Answer = true;

        hasAnswer = true;
        PanelYesNo.SetActive(false);
    }


    IEnumerator WaitForDesicion()
    {
        while (true)
        {
            if (hasAnswer)
            {
                Debug.LogError("Esperando respuesta >> Respuesta : " + Answer);

                if (Answer)
                {
                    //SystemController.instance.CurrentReportData.DateTimeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

                    createNewReportToSend(true);
                    SystemController.instance.ReportReplace = true;

                    ProceedToQA();
                    //ClickNext();
                }
                else
                {
                    //--- resetear todo
                    ChassisIDField.interactable = false;
                    ChassisIDField.text = "";

                    _Image.gameObject.SetActive(false);

                    StartScanButton.SetActive(true);
                    BGNormal.SetActive(true);
                    BGScan.SetActive(false);
                    inspectChassisPanel.SetActive(false);
                    BackButton.SetActive(false);
                    NextButton.SetActive(false);
                    //StopScanbutton.SetActive(false);

                    ChassisIDField.interactable = false;
                    
                    //InputChassisButton.SetActive(true);
                    //StopScanbutton.SetActive(false);
                }

                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
          
	public void ClickStop()
	{
		if (BarcodeScanner == null)
		{
			Log.Warning("No valid camera - Click Stop");
			return;
		}

        _Image.gameObject.SetActive(false);

        BGNormal.SetActive(true);
        BGScan.SetActive(false);
        inspectChassisPanel.SetActive(true);

        //StartScanButton.SetActive(true);
        //InputChassisButton.SetActive(true);
        BackButton.SetActive(true);
        NextButton.SetActive(true);
        //StopScanbutton.SetActive(false);

        ChassisIDField.interactable = false;

        // Stop Scanning
        BarcodeScanner.Stop();
	}

    public void InputChassisAction()
    {
        ChassisIDField.interactable = true;

        StartScanButton.SetActive(false);
        InputChassisButton.SetActive(false);
        BackButton.SetActive(true);
        NextButton.SetActive(true);
        StopScanbutton.SetActive(false);
    }

    public void ClickBack()
	{
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			
			SceneManager.LoadScene("OperatorReader");
		}));
	}

	public void ClickNext()
	{

        if (string.IsNullOrEmpty(ChassisIDField.text))
        {
            Debug.LogError("NO HAY OPERADOR");
            return;
        }

        if (!int.TryParse(ChassisIDField.text, out SystemController.instance.CurrentChassisID))
        {
            Debug.LogError("NO ES NUMERICO");
            return;
        }
                
        ValidateData(ChassisIDField.text);
	}

    public void ProceedToQA()
    {
        // Try to stop the camera before loading another scene

        Debug.LogError("EN EL PROCEED TO QA");

        StartCoroutine(StopCamera(() => {
            SceneManager.LoadScene("ARScene");
        }));
    }


	/// <summary>
	/// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
	/// Trying to stop the camera in OnDestroy provoke random crash on Android
	/// </summary>
	/// <param name="callback"></param>
	/// <returns></returns>
	public IEnumerator StopCamera(Action callback)
	{
        // Stop Scanning
        //_Image = null;

        _Image.gameObject.SetActive(false);

        if (BarcodeScanner != null)
        {
            BarcodeScanner.Destroy();
            BarcodeScanner = null;
        }

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
	}

    #endregion

    public void ShowError(bool Show, string text = "")
    {
        if (!Show)
        {
            ErrorPanel.SetActive(false);
        }
        else
        {
            ErrorText.text = text;
            ErrorPanel.SetActive(true);
        }
    }
}
