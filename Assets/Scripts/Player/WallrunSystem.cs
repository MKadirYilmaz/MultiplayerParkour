using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class WallrunSystem : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private LayerMask wallLayer;
    

    [Header("Adjustment")]
    [SerializeField] private float wallDetectionDist = 1f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float minVelocityToFall = 0.5f;

    private PlayerMovement pMovement;
    private RaycastHit rightHitInfo;
    private RaycastHit leftHitInfo;

    private bool rightWallrun;
    private bool leftWallrun;

    private enum WallrunSide
    {
        Right,
        Left
    };
    private WallrunSide currentSide;
    private WallrunSide prevSide;
    private enum WallrunEndReason
    {
        NoReason,
        Angle,
        LowSpeed,
        InputRelease,
        Jump,
        TouchedGround,
        WallLost
    };

    private bool wallRunning = false;
    private bool canWallrunSameSide = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pMovement.OnJumpTrigger += WallJump;
    }

    // Update is called once per frame
    void Update()
    {
        if(pMovement.IsGrounded())
            canWallrunSameSide = true;
        
        rightWallrun = CheckForWall(transform.right, out rightHitInfo);
        leftWallrun = CheckForWall(-transform.right, out leftHitInfo);
        ChooseWall();
        
        HandleWallrun();
    }
   
    private bool CheckForWall(Vector3 direction, out RaycastHit hitInfo)
    {
        Vector3 start = transform.position;
        Vector3 end = start + direction * wallDetectionDist;
        Debug.DrawLine(start, end, Color.red);

        Ray ray = new Ray(transform.position, direction);

        if(Physics.Raycast(ray, out hitInfo, wallDetectionDist, wallLayer))
        {
            float dot = Vector3.Dot(hitInfo.normal, transform.forward);
            if(dot > -0.5f && dot < 0.7f)
            {
                //Debug.Log($"Player can wall run. Dot: {dot}");
                return true;
            }
        }
        return false;
    }

    private void StartWallrun()
    {
        Debug.Log("Wallrun Started");
        pMovement.ResetCurrentAirJumpCount();
        pMovement.moveState = PlayerMovement.MovementState.Wallrunning;
        pMovement.SetGravityActive(false);
        wallRunning = true;
    }

    private void HandleWallrun()
    {
        if(!wallRunning)
            return;

        WallrunEndReason endReason = WallrunEndReason.NoReason;
        if(pMovement.IsGrounded())
        {
            endReason = WallrunEndReason.TouchedGround;
        }
        else if (pMovement.GetHorizontalVelocity().magnitude < minVelocityToFall)
        {
            endReason = WallrunEndReason.LowSpeed;
        }
        else if(pMovement.moveInput.y <= 0)
        {
            endReason = WallrunEndReason.InputRelease;
        }
        else if(!(rightWallrun || leftWallrun))
        {
            endReason = WallrunEndReason.WallLost;
        }
        if(endReason != WallrunEndReason.NoReason)
        {
            EndWallrun(endReason);
            return;
        }
        Debug.Log("Wallrunning...");
        Vector3 moveVector = FindWallrunDirection() * pMovement.GetMoveSpeed();
        pMovement.GetRigidbody().AddForce(moveVector, ForceMode.Force);
        pMovement.GetRigidbody().linearVelocity = new Vector3(pMovement.GetRigidbody().linearVelocity.x, 0, pMovement.GetRigidbody().linearVelocity.z);
    }

    private void EndWallrun(WallrunEndReason endReason)
    {
        Debug.Log($"Wallrun Ended. End Reason: {endReason}");
        pMovement.moveState = PlayerMovement.MovementState.Running;
        pMovement.SetGravityActive(true);
        wallRunning = false;
        prevSide = currentSide;
        canWallrunSameSide = false;
    }

    public Vector3 FindWallrunDirection()
    {
        RaycastHit hit = (currentSide == WallrunSide.Right) ? rightHitInfo : leftHitInfo;
        Vector3 cross = (currentSide == WallrunSide.Right) ? -Vector3.Cross(hit.normal, transform.up) : Vector3.Cross(hit.normal, transform.up);
        
        Vector3 start = transform.position;
        Vector3 end = start + cross * 2;
        Debug.DrawLine(start, end, Color.blue);

        return cross;
    }
    private void ChooseWall()
    {
        if(wallRunning || pMovement.IsGrounded())
            return;
        if(rightWallrun && leftWallrun)
        {
            if(canWallrunSameSide)
            {
                // Check which wall is closer to player
                currentSide = (Vector3.Distance(transform.position, rightHitInfo.point) > Vector3.Distance(transform.position, leftHitInfo.point)) ? WallrunSide.Left : WallrunSide.Right; 
            }
            else
            {
                currentSide = (prevSide == WallrunSide.Right) ? WallrunSide.Left : WallrunSide.Right;
            }
            StartWallrun();
        }
        else if (rightWallrun || leftWallrun)
        {
            if(canWallrunSameSide)
            {
                currentSide = rightWallrun ? WallrunSide.Right : WallrunSide.Left;
                StartWallrun();
            }
            else
            {
                if(rightWallrun && prevSide != WallrunSide.Right)
                {
                    currentSide = WallrunSide.Right;
                    StartWallrun();
                }
                else if(leftWallrun && prevSide != WallrunSide.Left)
                {
                    currentSide = WallrunSide.Left;
                    StartWallrun();
                }
            }
            
        }
    }
    private void WallJump()
    {
        Debug.Log("Delegate fired");
        if(wallRunning)
        {
            Vector3 jumpDirection = (currentSide == WallrunSide.Right) ? -transform.right : transform.right;
            Vector3 jumpForce = wallJumpForce * (jumpDirection + transform.up).normalized;
            EndWallrun(WallrunEndReason.Jump);
            pMovement.GetRigidbody().AddForce(jumpForce);
        }
    }
}
