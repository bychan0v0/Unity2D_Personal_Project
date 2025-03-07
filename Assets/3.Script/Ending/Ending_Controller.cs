using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_Controller : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Application.Quit();
        }
    }
}
