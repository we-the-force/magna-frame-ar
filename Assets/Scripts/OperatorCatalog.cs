using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCatalog : MonoBehaviour
{
    public InputField OperatorID;
    public InputField OperatorName;
    public Text ErrorTxt;

    public GameObject StartButton;
    public GameObject NewOperatorButton;
    public GameObject OperatorPanel;




    public void NewOperatorButtonAction()
    {
        OperatorID.text = "";
        OperatorName.text = "";
        ErrorTxt.text = "";

        StartButton.SetActive(false);
        NewOperatorButton.SetActive(false);

        OperatorPanel.SetActive(true);
    }

    public void AddOperatorButtonAction()
    {
        if (string.IsNullOrEmpty(OperatorID.text))
        {
            ErrorTxt.text = "CAPTURE EL ID DE OPERADOR";
            return;
        }
        if (string.IsNullOrEmpty(OperatorName.text))
        {
            ErrorTxt.text = "CAPTURE EL NOMBRE DE OPERADOR";
            return;
        }

        string name = OperatorName.text;
        int id = 0;
        if (!int.TryParse(OperatorID.text, out id))
        {
            ErrorTxt.text = "ID DE OPERADOR NO ES NUMERO ENTERO";
            return;
        }

        int _testid = SystemController.instance.OperatorExist(name);
        if (_testid != -1)
        {
            ErrorTxt.text = "OPERADOR EXISTE PARA EL NOMBRE (" + _testid.ToString() + ")";
            return;
        }

        OperatorRegistry _operator = SystemController.instance.OperatorExist(id);
        if (_operator != null)
        {
            ErrorTxt.text = "OPERADOR EXISTE PARA EL ID (" + _operator.name + ")";
            return;
        }

        _operator = new OperatorRegistry();
        _operator.id = id;
        _operator.name = name;

        SystemController.instance.AddOperator(_operator);
        
        CancelButtonAction();
    }

    public void CancelButtonAction()
    {
        OperatorID.text = "";
        OperatorName.text = "";
        ErrorTxt.text = "";

        OperatorPanel.SetActive(false);

        StartButton.SetActive(true);
        NewOperatorButton.SetActive(true);
    }

}
