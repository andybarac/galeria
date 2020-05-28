/*
Vizarion Software. 2016. All Rights Reserved.
http://vizarion.com
contact@vizarion.com
*/
using UnityEngine;

public class ObservationPosition : MonoBehaviour
{
    void Update()
    {
        //Translate to this position on object click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    _flyCam.FlyFromTo(transform);
                }
            }
        }
    }
    [SerializeField]
    private FlyingCam _flyCam;
}
