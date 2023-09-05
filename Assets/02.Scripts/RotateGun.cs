using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    // Start is called before the first frame update
    float xx;
    float y;



    void Start()
    {


      

    }

    // Update is called once per frame
    void Update()
    {


        

        // transform.Rotate(Vector3.up * Time.deltaTime * 30f);
        y += 50 * Time.deltaTime;

        if (this.gameObject.name == "SynTaxP40_Attachments_ON") // ^ this.gameObject.name == "MAC_SciFiSniperRifle")
        {
            transform.eulerAngles = new Vector3(100, -200 + y, 0);
            Debug.Log("39");
        }

        if (this.gameObject.name != "Sci-Fi Rifle_fbx")
        {
            transform.eulerAngles = new Vector3(0, y, 0);
        }

        else
        {
            transform.eulerAngles = new Vector3(-90, y, 0);
        }



    }
}
