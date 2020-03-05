using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseSession : MonoBehaviour
{
    public void CloseThisSession()
    {
        SceneManager.LoadScene("OperatorReader");
    }

}
