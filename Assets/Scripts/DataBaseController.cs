using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Net;
using System;
using System.IO;


//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;

public class DataBaseController : MonoBehaviour
{
    public delegate void ServerMessageGetReport(ReportDataRegistry dataRegistry);
    public delegate void ServerMessagePutReport(bool success);

    public delegate void ServerMessageEvidenceImagePut(bool success);

    public static DataBaseController instance;
    public string API_Token;
    public string ServerURL;

    public const string ChassisCollectionURL = "chassis";
    public const string ReportsCollectionURL = "reports";
    public const string OperatorCollectionURL = "operators";
    public const string ScreenShotURL = "upload";

    public OperatorTable OperatorRegistryCollection;
    public ReportDataTable ReportDataRegistryCollection;
    public ChassisTable ChassisRegistryCollection;    
    
    public enum HTTP_CALL
    {
        POST,
        PUT,
        GET
    }

    private void Awake()
    {
        //-- SINGLETON PERSISTANCY ----------------
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
        //-----------------------------------------
    }

    private void Start()
    {
        API_Token = "6dd9e3feb1936ffb83383cb286cac7";
        //ServerURL = "http ://192.168.5.142/";
        ServerURL = "http://64.227.8.184:3000/";
        //StartCoroutine(GetReportData());
        StartCoroutine(GetChassisData());
        StartCoroutine(GetOperatorData());
    }
    
    IEnumerator GetOperatorData()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(ServerURL + OperatorCollectionURL))
        {
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            while (!req.isDone)
                yield return null;

            byte[] result = req.downloadHandler.data;
            string JSON_ = System.Text.Encoding.Default.GetString(result);

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
            }
            else
            {
                string JSONToParse = "{\"operators\":" + JSON_ + "}";
                //string JSONToParse = req.downloadHandler.text;

                OperatorRegistryCollection = new OperatorTable();
                OperatorRegistryCollection = JsonUtility.FromJson<OperatorTable>(JSONToParse);
                
                Debug.LogError("Total en coleccion : " + OperatorRegistryCollection.operators.Count);
            }
        }
    }

    IEnumerator GetReportData()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(ServerURL + ReportsCollectionURL))
        {
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            while (!req.isDone)
                yield return null;

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
            }
            else
            {
                string JSONToParse = "{\"reportData\":" + req.downloadHandler.text + "}";
                Debug.LogError(JSONToParse);

                ReportDataRegistryCollection = JsonUtility.FromJson<ReportDataTable>(JSONToParse);
            }
        }
    }

    public IEnumerator PutReportData(ServerMessagePutReport returnMessage)
    {
        string _url = ServerURL + ReportsCollectionURL;
        string _data = JsonUtility.ToJson(SystemController.instance.CurrentReportDataToSend);

        if (!SystemController.instance.ReportReplace)
        {
            UnityWebRequest req = new UnityWebRequest(_url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(_data);
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return req.SendWebRequest();

            if (req.isNetworkError)
            {
                Debug.LogError(req.error);
                returnMessage.Invoke(false);
            }
            else
            {
                returnMessage.Invoke(true);
            }
        }
        else
        {
            /*
            byte[] bytePostData = System.Text.Encoding.UTF8.GetBytes(postData);
            UnityWebRequest request = UnityWebRequest.Put(url, bytePostData); //use PUT method to send simple stream of bytes
            request.method = "POST"; //hack to send POST to server instead of PUT
            request.SetRequestHeader("Content-Type", "application/json");
            */

            _url = _url + "/" + SystemController.instance.CurrentReportData.id.ToString();
            WeldRegistryPOSTTable welds = new WeldRegistryPOSTTable();
            welds.report_welds = SystemController.instance.CurrentReportDataToSend.report_welds;
            _data = JsonUtility.ToJson(welds);
            //byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(_data);

            Debug.LogError("URL : " + _url);
            Debug.LogError("Data: " + _data);

            UnityWebRequest req = UnityWebRequest.Put(_url, _data);
            //req.method = "POST";
            req.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
                returnMessage.Invoke(false);
            }
            else
            {
                returnMessage.Invoke(true);
            }
        }
    }


    public IEnumerator GetReportDataFromChassis(string chassisNumber, ServerMessageGetReport returnMessage)
    {
        string _url = ServerURL + ReportsCollectionURL + "?chassis_serial=" + chassisNumber;
        Debug.LogError("URL : " + ServerURL + ReportsCollectionURL + "?chassis_serial=" + chassisNumber);

        using (UnityWebRequest req = UnityWebRequest.Get(_url))
        {
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            while (!req.isDone)
                yield return null;

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
                returnMessage.Invoke(null);
            }
            else
            {
                string JSONToParse = "{\"reportData\":" + req.downloadHandler.text + "}";
                Debug.LogError(JSONToParse);
                ReportDataTable _table = JsonUtility.FromJson<ReportDataTable>(JSONToParse);
                Debug.LogError(_table.reportData.Count);

                if (_table.reportData != null && _table.reportData.Count > 0)
                {
                    returnMessage.Invoke(_table.reportData[0]);
                }
                else
                {
                    returnMessage.Invoke(null);
                }
            }
        }
    }

    IEnumerator GetChassisData()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(ServerURL + ChassisCollectionURL))
        {
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            while (!req.isDone)
                yield return null;

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
            }
            else
            {
                string JSONToParse = "{\"chassis\":" + req.downloadHandler.text + "}";
                ChassisRegistryCollection = JsonUtility.FromJson<ChassisTable>(JSONToParse);
            }
        }
    }


    //public IEnumerator SendEvidenceImage(int currentImage, ReportDataRegistry ReportData, ServerMessageEvidenceImagePut response)
    public IEnumerator SendEvidenceImage(ServerMessageEvidenceImagePut response) //int currentImage, ServerMessageEvidenceImagePut response)
    {
        for (int i = 0; i < SystemController.instance.EvidenceByteCollection.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            byte[] bytes = SystemController.instance.EvidenceByteCollection[i].EncodeToPNG();

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("ref=report&field=report_welds[" + i + "].evidence"));
            formData.Add(new MultipartFormFileSection("files", bytes, SystemController.instance.EvidenceImageCollection[i], "image/png"));

            string _url = ServerURL + ScreenShotURL;
            UnityWebRequest www = UnityWebRequest.Post(_url, formData);

            www.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MiwiaWF0IjoxNTgzMjU1NTg4LCJleHAiOjE1ODU4NDc1ODh9.XlOu3vP9Vy1Okjc2q6k5Useq9SrQGSoBIvqS2cioyww");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                response.Invoke(false);
                Debug.LogError("Error de subida de archivo : " + www.error);
            }
            else
            {
                Debug.LogError(">>> " + www.downloadHandler.text);


                string[] pairs = www.downloadHandler.text.Split(',');
                string[] idPair = pairs[0].Split(':');

                int _id = int.Parse(idPair[1]);
                Debug.LogError("Evidence ID " + _id.ToString());

                SystemController.instance.CurrentReportDataToSend.report_welds[i].evidence = _id;
            }
        }

        response.Invoke(true);

        /*
        yield return new WaitForFixedUpdate();

        byte[] bytes = SystemController.instance.EvidenceByteCollection[currentImage].EncodeToPNG();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("ref=report&field=report_welds[" + currentImage + "].evidence"));
        formData.Add(new MultipartFormFileSection("files", bytes, SystemController.instance.EvidenceImageCollection[currentImage], "image/png"));

        string _url = ServerURL + ScreenShotURL;
        UnityWebRequest www = UnityWebRequest.Post(_url, formData);

        www.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MiwiaWF0IjoxNTgzMjU1NTg4LCJleHAiOjE1ODU4NDc1ODh9.XlOu3vP9Vy1Okjc2q6k5Useq9SrQGSoBIvqS2cioyww");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            response.Invoke(false, 0);
            Debug.LogError("Error de subida de archivo : " + www.error);
        }
        else
        {
            Debug.LogError(">>> " + www.downloadHandler.text);


            string[] pairs = www.downloadHandler.text.Split(',');
            string[] idPair = pairs[0].Split(':');

            int _id = int.Parse(idPair[1]);
            Debug.LogError("Evidence ID " + _id.ToString());

            SystemController.instance.CurrentReportDataToSend.report_welds[currentImage].evidence = _id;
            response.Invoke(true, _id);
        }
        */
    }



    /*
    IEnumerator GetData(string URL)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(ServerURL + URL))
        {
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            while (!req.isDone)
                yield return null;


            Debug.LogError("error info " + req.error);
            string _result = req.downloadHandler.text;
            Debug.LogError(_result);
        }
    }
    */

    /*
    public void dbtest()
    {
        StartCoroutine(_CallServer());
    }
    */
}
