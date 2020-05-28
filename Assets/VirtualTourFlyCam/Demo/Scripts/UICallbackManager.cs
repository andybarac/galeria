/*
Vizarion Software. 2016. All Rights Reserved.
http://vizarion.com
contact@vizarion.com
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICallbackManager : MonoBehaviour
{
    void Start()
    {
        //sign to cam events
        _flyCam.OnFly += _flyCam_OnFly;
        _flyCam.OnFlyEnd += _flyCam_OnFlyEnd;
        _flyCam.OnFlyEnd += ResetTexts;
        //assign button handler
        _abortFlyghtButton.onClick.AddListener(AbortFlyght);
        //assign speed controller
        _scrollbar.onValueChanged.AddListener(AdjustSpeed);
    }

    private void AdjustSpeed(float newSpeed)
    {
        _flyCam.FlySpeed = newSpeed;
    }

    /// <summary>
    /// Stop current flight
    /// </summary>
    private void AbortFlyght()
    {
       _flyCam.AbortFlight();
        _onFlyEndText.text = string.Empty;
        _onFlyText.text = string.Empty;
    }

    /// <summary>
    /// Update ui text, when camera has reached destination
    /// </summary>
    private void _flyCam_OnFlyEnd()
    {
        _onFlyEndText.text = string.Format("Flyght is done");
    }

    /// <summary>
    /// Update ui text, while camera is translating to destination
    /// </summary>
    private void _flyCam_OnFly()
    {
        _onFlyText.text = string.Format("Doing flight {0}", Time.deltaTime);
    }

    /// <summary>
    /// Reset text after destination is reached, in 0.5 sec
    /// </summary>
    private void ResetTexts()
    {
        StartCoroutine(WaitBeforeReset());
    }
    private IEnumerator WaitBeforeReset()
    {
        yield return new WaitForSeconds(0.5f);
        _onFlyEndText.text = string.Empty;
        _onFlyText.text = string.Empty;
    }


    [SerializeField]
    private Button _abortFlyghtButton;
    [SerializeField]
    private Text _onFlyText;
    [SerializeField]
    private Text _onFlyEndText;
    [SerializeField]
    private Scrollbar _scrollbar;
    /// <summary>
    /// Reference to cam
    /// </summary>
    [SerializeField]
    private FlyingCam _flyCam;
}
