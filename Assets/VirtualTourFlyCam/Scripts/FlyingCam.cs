/*
Vizarion Software. 2016. All Rights Reserved.
http://vizarion.com
contact@vizarion.com
*/

using System;
using UnityEngine;

/// <summary>
/// Assign script to any camera
/// </summary>
[RequireComponent(typeof(Camera))]
public class FlyingCam : MonoBehaviour
{
    #region Motor block

    void Awake()
    {
        _startRot = transform.localRotation;
        _startPos = transform.localPosition;
        _defaultSpeed = _flySpeed;
    }

    void Update()
    {
        DoFly();
    }

    /// <summary>
    /// Main motor method. Here is done all fly logic
    /// </summary>
    private void DoFly()
    {
        if (!_isAnimate) return;

        //Calculate flyght time. Should be from 0 to 1. Speed parameter allows to control animation time. 
        _animationTime += Time.deltaTime * _flySpeed;

        //Do fly
        if (_animationTime <= 1f)
        {
            //Evaluate animation curve, for non-linear movement
            float curvePos = _curve.Evaluate(_animationTime);

            //Lerp new position and rotation
            Vector3 newPos = Vector3.Lerp(_startPos, _destPos, curvePos);
            Quaternion newRot = Quaternion.Lerp(_startRot, _destRot, curvePos);

            //Set new position and rotation
            transform.localPosition = newPos;
            transform.localRotation = newRot;

            //Call event 
            if (OnFly != null)
            {
                OnFly();
            }
        }
        //Stop fly
        else
        {
            //Reset flag and animation time
            _isAnimate = false;
            _animationTime = 0;
            //return speed to default one(the one that was before, if used a method with speed param),
            //or if FlySpeed property was modyfied - ensure we keep the same speed
            _flySpeed = _defaultSpeed;

            //Call not event callback
            if (_onFlyCompleteAction != null)
            {
                _onFlyCompleteAction();
                //remove a callback from memory
                _onFlyCompleteAction = null;
            }

            //Fire event
            if (OnFlyEnd != null)
            {
                OnFlyEnd();
            }
        }
    }
    #endregion

    #region API.

    /// <summary>
    /// Basic fly method, but with Transform, instead of direct position/rotation parameter
    /// </summary>
    /// <param name="to"></param>
    public void FlyFromTo(Transform to)
    {
        this.FlyFromTo(to.position, to.rotation);
    }

    /// <summary>
    /// Basic fly method, with Transform as flyght destination and speed parameter.
    /// Speed is set only for this particular transition.
    /// For constant change use FlySpeed property
    /// </summary>
    /// <param name="to"></param>
    /// <param name="speed"></param>
    public void FlyFromTo(Transform to, float speed)
    {
        _defaultSpeed = _flySpeed;
        _flySpeed = speed;
        this.FlyFromTo(to);
    }

    /// <summary>
    /// Basic fly method, with Transform as flyght destination and onComplete action parameter
    /// </summary>
    /// <param name="to"></param>
    /// <param name="speed"></param>
    public void FlyFromTo(Transform to, Action onComplete)
    {
        //ensure we have not overwritten current action callback
        if (_onFlyCompleteAction != null)
        {
            _onFlyCompleteAction = onComplete;
        }
        this.FlyFromTo(to);
    }

    /// <summary>
    /// Basic fly method, with Transform as flyght destination and speed, onComplete action parameters,  
    /// Speed is set only for this particular transition.
    /// For constant change use FlySpeed property
    /// </summary>
    /// <param name="to"></param>
    /// <param name="speed"></param>
    public void FlyFromTo(Transform to, float speed, Action onComplete)
    {
        _defaultSpeed = _flySpeed;
        _flySpeed = speed;
        this.FlyFromTo(to, onComplete);
    }

    /// <summary>
    /// Basic fly method, but with non-event callback for fly finish
    /// </summary>
    /// <param name="toPos"></param>
    /// <param name="toRot"></param>
    /// <param name="onComplete"></param>
    public void FlyFromTo(Vector3 toPos, Quaternion toRot, Action onComplete)
    {
        //ensure we have not overwritten current action callback
        if (_onFlyCompleteAction != null)
        {
            _onFlyCompleteAction = onComplete;
        }
        this.FlyFromTo(toPos, toRot);
    }

    /// <summary>
    /// Basic fly method, with on fly end non-event action and speed param
    /// Speed is set only for this particular transition.
    /// For constant change use FlySpeed property
    /// </summary>
    /// <param name="toPos"></param>
    /// <param name="toRot"></param>
    /// <param name="speed"></param>
    /// <param name="onComplete"></param>
    public void FlyFromTo(Vector3 toPos, Quaternion toRot, float speed, Action onComplete)
    {
        _defaultSpeed = _flySpeed;
        _flySpeed = speed;
        this.FlyFromTo(toPos, toRot, onComplete);
    }

    /// <summary>
    /// Basic fly method, with speed param
    /// Speed is set only for this particular transition.
    /// For constant change use FlySpeed property
    /// </summary>
    /// <param name="toPos"></param>
    /// <param name="toRot"></param>
    /// <param name="speed"></param>
    public void FlyFromTo(Vector3 toPos, Quaternion toRot, float speed)
    {
        _defaultSpeed = _flySpeed;
        _flySpeed = speed;
        this.FlyFromTo(toPos, toRot);
    }

    /// <summary>
    /// Basic method to start flyght. Just pass destination position and rotation. No other parametrs
    /// </summary>
    /// <param name="toPos"></param>
    /// <param name="toRot"></param>
    public void FlyFromTo(Vector3 toPos, Quaternion toRot)
    {
        //restrict change fly position while we are already moving,
        //and save the same speed
        if (_isAnimate)
        {
            _flySpeed = _defaultSpeed;
            return;
        }

        _startPos = transform.localPosition;
        _startRot = transform.localRotation;

        _destPos = toPos;
        _destRot = toRot;

        if (OnFlyStart != null)
        {
            OnFlyStart();
        }

        _isAnimate = true;
    }

    /// <summary>
    /// Stop flyght, and immideatly return to start position.
    /// Works only while flying
    /// </summary>
    public void AbortFlight()
    {
        if (!_isAnimate) return;

        _isAnimate = false;
        _animationTime = 0;
        _flySpeed = _defaultSpeed;
        _onFlyCompleteAction = null;
        transform.localPosition = _startPos;
        transform.localRotation = _startRot;
    }
    #endregion

    #region Callbacks
    /// <summary>
    /// On fly finish action-callback. For cases, when we dont need to sign to event,
    /// but just do a simple a simple action. Assigned through FlyFromTo(Vector3 toPos, Quaternion toRot, Action onComplete)
    /// </summary>
    private Action _onFlyCompleteAction;

    /// <summary>
    /// Event on fly start
    /// </summary>
    public event Action OnFlyStart;

    /// <summary>
    /// Event that happens during the translation
    /// </summary>
    public event Action OnFly;

    /// <summary>
    /// Event on movement finished
    /// </summary>
    public event Action OnFlyEnd;
    #endregion

    #region Visible Parametrs
    /// <summary>
    /// Translation speed, assign in editor
    /// </summary>
    [SerializeField]
    private float _flySpeed = 1f;
    /// <summary>
    /// Animation curve. Assign in Editor, for non-linear movement
    /// </summary>
    [SerializeField]
    private AnimationCurve _curve;
    #endregion

    #region Private Members

    /// <summary>
    /// Default speed set on start, 
    /// or speed where to return, after translation with speed param
    /// </summary>
    private float _defaultSpeed;
    /// <summary>
    /// Original position, from where translation was started
    /// </summary>
    private Vector3 _startPos;
    /// <summary>
    /// Destination position, where to fly
    /// </summary>
    private Vector3 _destPos;
    /// <summary>
    /// Original rotation, from where translation was started
    /// </summary>
    private Quaternion _startRot;
    /// <summary>
    /// Destination rotation, where to fly
    /// </summary>
    private Quaternion _destRot;
    /// <summary>
    /// Flag that gives a start of movement
    /// </summary>
    private bool _isAnimate;
    /// <summary>
    /// Movement time. From 0 to 1. Hovewer, we can controll speed of movement, and as a result time changing _flySpeed parameter
    /// </summary>
    private float _animationTime;
    #endregion

    #region Public Members
    public float FlySpeed
    {
        get { return _flySpeed; }
        set
        {
            _flySpeed = value;
            _defaultSpeed = _flySpeed;
        }
    }
    #endregion
}
