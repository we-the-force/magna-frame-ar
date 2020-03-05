using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizcorp.Utils.Logger;

public class OperatorReader : MonoBehaviour {

	private IScanner BarcodeScanner;
	public Text ScannerStatusText;

    public GameObject bg_normal;
    public GameObject bg_scan;

    public GameObject ErrorPanel;
    public Text ErrorText;
    
	public RawImage _Image;
	public AudioSource Audio;
    
    public InputField OperatorIDField;
    public GameObject StartScanButton;
    public GameObject InputOperatorButton;
    public GameObject StopScanbutton;
    public GameObject NextButton;
    public GameObject BackButton;
    
	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

    void StartScanBarcode () {
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
			ScannerStatusText.text = "Scanner Status: " + BarcodeScanner.Status;			
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
        //StartCoroutine("iStartScanner");
    }

    IEnumerator iStartScanner()
    {
        yield return new WaitForSeconds(0.25f);

        OperatorIDField.interactable = false;
               
        yield return new WaitForSeconds(0.25f);

        if (BarcodeScanner == null)
		{
			Log.Warning("No valid camera - Click Start");

            yield break;
            //return;
		}

        bg_normal.SetActive(false);
        bg_scan.SetActive(true);

        OperatorIDField.interactable = false;

        StartScanButton.SetActive(false);
        //InputOperatorButton.SetActive(false);
        //BackButton.SetActive(true);
        //NextButton.SetActive(true);
        //StopScanbutton.SetActive(true);

        // Start Scanning
        BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();

            OperatorIDField.text = barCodeValue;
            //TextHeader.text = " User: " + barCodeValue;
	
			string codigodeOperador;
			codigodeOperador = barCodeValue.ToString();

			

			// Feedback
			Audio.Play();

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#endif

            ClickNext();
			//dataCtrl.report.operador.id = codigodeOperador;
		});
	}

	public void ClickStop()
	{
		if (BarcodeScanner == null)
		{
			Log.Warning("No valid camera - Click Stop");
			return;
		}

		// Stop Scanning               
		BarcodeScanner.Stop();

        _Image.gameObject.SetActive(false);

        bg_normal.SetActive(true);
        bg_scan.SetActive(false);

        StartScanButton.SetActive(true);
        //InputOperatorButton.SetActive(true);
        //BackButton.SetActive(true);
        //NextButton.SetActive(true);
       // StopScanbutton.SetActive(false);

        OperatorIDField.interactable = false;
    }

    public void InputOperatorAction()
    {
        OperatorIDField.interactable = true;
        
        StartScanButton.SetActive(false);
        InputOperatorButton.SetActive(false);
        BackButton.SetActive(true);
        NextButton.SetActive(true);
        StopScanbutton.SetActive(false);
    }


	public void ClickBack()
	{
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			
			SceneManager.LoadScene("MenuMain");
		}));
	}

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

    public void ClickNext()
	{
        if (string.IsNullOrEmpty(OperatorIDField.text))
        {
            ShowError(true, "El campo de ID de operador esta vacio");
            Debug.LogError("NO HAY OPERADOR");
            return;
        }

        if (!int.TryParse(OperatorIDField.text, out SystemController.instance.CurrentOperator))
        {
            ShowError(true, "El id de operador no es numerico");
            Debug.LogError("NO ES NUMERICO");
            return;
        }

        SystemController.instance.CurrentOperatorData = SystemController.instance.OperatorExist(SystemController.instance.CurrentOperator);
        if (SystemController.instance.CurrentOperatorData == null)
        {
            //-- In this case the operator does not exist in the database
            ShowError(true, "El operador no existe en la Base de Datos");
            return;
        }

        // Try to stop the camera before loading another scene
        StartCoroutine(StopCamera(() => {
			SceneManager.LoadScene("ChassisReader");			
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
}
