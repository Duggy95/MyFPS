using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    // Start is called before the first frame update
    float xx;
    float y;

    Quaternion rot;
    Vector3 rot1;

    void Start()
    {
        rot = this.transform.localRotation;
        rot.x = -90;
        rot1 = rot.eulerAngles;
        



    }

    // Update is called once per frame
    void Update()
    {

        if (this.gameObject.name == "Sci-Fi Rifle_fbx")
        {
            transform.Rotate(new Vector3(0, 0, 1));

        }
        else
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }

           
       // transform.Rotate(Vector3.up * Time.deltaTime * 30f);


       //////////////////////////////////////////

       //y += 50 * Time.deltaTime;

       // if (this.gameObject.name == "SynTaxP40_Attachments_ON") // ^ this.gameObject.name == "MAC_SciFiSniperRifle")
       // {
       //     transform.eulerAngles = new Vector3(100, -200 + y, 0);
       //     Debug.Log("39");
       // }

       // if (this.gameObject.name != "Sci-Fi Rifle_fbx")
       // {
       //     transform.eulerAngles = new Vector3(0, y, 0);
       // }

       // else
       // {
       //     transform.eulerAngles = new Vector3(-90, y, 0);
       // }



    }
}
