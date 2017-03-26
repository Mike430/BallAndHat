using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCTRL : MonoBehaviour
{
    [SerializeField]
    private Transform _mMoveDriverTargetTransform;
    [SerializeField]
    private Transform _mCameraTransform;
    [SerializeField]
    private Transform _mFaceTransform;
    [SerializeField]
    private float _mWalkSpeed;
    [SerializeField]
    private float _mRunSpeed;

    [SerializeField]
    private float _mJumpForce;
    [SerializeField]
    private float _mDeceleration = 0.75f;

    [SerializeField]
    float _mMaxAngularVel;

    private Rigidbody _mRigidBody;
    private bool _mIsJumping = false;
    private Vector3 _mCameraVecOffset;
    private Vector3 _mDeviceIntMovement;


	// Use this for initialization
	void Start ()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _mRigidBody = this.GetComponent<Rigidbody>();
        _mCameraVecOffset = _mCameraTransform.position - this.transform.position;
        _mRigidBody.maxAngularVelocity = _mMaxAngularVel;
	}
	


	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 movement = Input.acceleration.normalized;
        movement.y = 0;
        _mDeviceIntMovement = movement;

        _mDeviceIntMovement.z *= -1;
        float magnitude = movement.magnitude;
        if (magnitude > 0.25f && magnitude < 0.5f)
        {
            movement = movement.normalized;
            movement *= _mWalkSpeed;
            movement.z *= -1;
            _mDeviceIntMovement = movement;
            MovePlayer(movement, _mWalkSpeed);
        }
        else if (magnitude >= 0.5f)
        {
            movement = movement.normalized;
            movement *= _mRunSpeed;
            movement.z *= -1;
            _mDeviceIntMovement = movement;
            MovePlayer(movement, _mRunSpeed);
        }
        else
        {
            Vector3 stoppingForce = (-_mRigidBody.velocity * _mDeceleration) * Time.deltaTime;
            _mRigidBody.AddForce(stoppingForce);
        }

        if (Input.touchCount > 0 && !_mIsJumping)
        {
            Jump();
        }

        if (Input.touchCount == 0)
        {
            _mIsJumping = false;
        }
    }


    private void LateUpdate()
    {
        TurnAndMoveThePlayersFace(_mDeviceIntMovement);

        _mCameraTransform.position = (this.transform.position + _mCameraVecOffset * (1 + (1 *_mRigidBody.velocity.magnitude * 0.5f)));
    }



    private void MovePlayer(Vector3 movement, float force)
    {
        //_mMovePosTargetDriver.position = this.transform.position + movement;
        //Debug.Log(_mMovePosTargetDriver.position - this.transform.position);
        //_mRigidBody.MovePosition(this.transform.position + ((_mMovePosTargetDriver.position - this.transform.position) * Time.deltaTime));

        _mMoveDriverTargetTransform.position = this.transform.position + movement;
        Vector3 moveDir = _mMoveDriverTargetTransform.position - this.transform.position;
        Vector3 rotAngle = (-Vector3.Cross(moveDir, Vector3.up) * force) * Time.deltaTime;
        _mRigidBody.AddTorque(rotAngle * force, ForceMode.VelocityChange);
    }


    private void TurnAndMoveThePlayersFace(Vector3 heading)
    {
        // http://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
        float step = 10 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(_mFaceTransform.transform.forward, heading, step, 0.0F);
        Debug.DrawRay(_mFaceTransform.transform.position, newDir, Color.red);
        _mFaceTransform.transform.rotation = Quaternion.LookRotation(newDir);

        _mFaceTransform.position = this.transform.position;
    }


    private void Jump()
    {
        _mIsJumping = true;
        _mRigidBody.AddForce(Vector3.up * _mJumpForce);
    }
}
