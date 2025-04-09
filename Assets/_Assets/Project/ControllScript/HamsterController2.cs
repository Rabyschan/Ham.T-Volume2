using System.Collections;
using UnityEngine;

public class HamsterController2 : MonoBehaviour
{
    public bool allowInput = true;

    [SerializeField] float raycastDist = 0.1f;

    [Header("Player")]
    public float moveSpeed;
    public float rotateSpeed ;
    public float sprintSpeed;
    public float jumpPower;
    public float jumpWaitTime = 0.8f;

    [Tooltip("메인 카메라")]
    public Camera _camera; // 메인 카메라

    [Tooltip("변경하고 싶은 포지션 값이 담긴 오브젝트")]
    public GameObject targetObject; // 변경하고 싶은 포지션 값이 담긴 오브젝트

    private float _frontInput; // frontinput 값
    private Rigidbody _rigidbody;

    [Header("Animation")]
    // 애니메이션 참조를 위한 bool값
    public bool isSprint; // Sprint 여부
    public bool isJumping; // 점프 여부
    public bool isClimbing;  // 벽을 타고 있는지 여부
    public bool isWalking; // Walk 여부
    public bool stopClimbing; // Climbing 종료 중
    public bool startClimbing; // Climbing 종료 중
    public bool climbing;
    public bool inAir;
    public bool awake;
    //public bool isGrounded;

    [Header ("-------------------------------------------------------")]
    public bool canMove = false;  // 이동 가능 여부
    private bool canJump = true;
    private bool isNearObject = false; // 콜라이더에 닿았는지 여부

    public void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        if (canMove)  // canMove가 true일 때만 이동 처리
        {
            if (!allowInput) return;

            if (climbing)
            {
                HandleClimbing();
            }

            HandleMove();
            HandleSprint();

            if (canJump)
            {
                if (IsGrounded())
                {
                    HandleJump();
                }
            }

            if (isNearObject && Input.GetKeyDown(KeyCode.E) && !climbing)
            {
                // E 키를 눌렀을 때 클라이밍 시작
                if (!startClimbing) // 이미 시작 중이 아니면 클라이밍을 시작
                {

                    startClimbing = true;
                    StartCoroutine(WaitStart(0.1f));  // 1초 대기 후 클라이밍 시작
                }
            }
        }
        else
        {
            // 키보드 입력 감지 시 awake를 true로 설정하고, 2초 대기 후 canMove 활성화
            if (!awake && (Input.anyKeyDown || Input.anyKey))  // 키 입력이 있을 때
            {
                awake = true;
                StartCoroutine(WaitForInputAndEnableMovement(2f));  // 2초 대기 후 이동 가능하게 설정
            }
        }
    }

    void Update()
    {
        inAir = !IsGrounded();
    }

    public bool IsGrounded()
    {
        // 아래로 레이캐스트를 쏴서 바닥에 닿았는지 체크
        return Physics.Raycast(transform.position, Vector3.down, raycastDist); // 작은 거리로 바닥 체크
    }

    // 2초 후 canMove를 활성화하는 코루틴
    private IEnumerator WaitForInputAndEnableMovement(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  // 2초 대기
        canMove = true;  // 이동 가능
        awake = false;
    }
    private IEnumerator StopJump(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  
        canJump = true;
    }



    #region Move/Sprint/Jump
    public void HandleMove()
{
    if (!climbing)
    {
        isClimbing = false;
        _frontInput = Input.GetAxis("Vertical");  // 상하 방향 입력 (W, S 또는 위, 아래 방향 키)
        float horizontalInput = Input.GetAxis("Horizontal");  // 좌우 방향 입력 (A, D 또는 왼쪽, 오른쪽 방향 키)

        if (_frontInput == 0 && horizontalInput == 0) // 상하와 좌우 입력이 모두 없으면 걷는 상태를 해제
        {
            isWalking = false;
            return;
        }

        float realMoveSpeed = isSprint ? sprintSpeed : moveSpeed;

        // 카메라의 방향에 맞춰 플레이어가 회전 (앞 방향으로 이동)
        Vector3 forward = _camera.transform.forward;
        forward.y = 0f;  // y 방향은 무시
        forward.Normalize();  // 벡터를 단위 벡터로 정규화

        // 카메라의 오른쪽 방향 벡터 계산 (플레이어가 카메라를 기준으로 좌우로 이동하도록)
        Vector3 right = _camera.transform.right;
        right.y = 0f;  // y 방향은 무시
        right.Normalize();  // 벡터를 단위 벡터로 정규화

        // 플레이어의 이동 방향 계산 (상하 + 좌우)
        Vector3 moveDirection = forward * _frontInput + right * horizontalInput;
        moveDirection.Normalize();  // 방향 벡터를 정규화하여 속도 일관성 유지

        // 회전 처리 (플레이어가 이동 방향으로 자연스럽게 회전)
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection),
                Time.deltaTime * rotateSpeed / 10f);
        }

        // 실제 이동 처리
        Vector3 position = transform.position + moveDirection * Time.deltaTime * realMoveSpeed;
        _rigidbody.MovePosition(position);

        isWalking = true;
    }
}

    public void HandleSprint()
    {
        isSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    public void HandleJump()
    {
        if (isJumping)
            return;

        if (Input.GetKey(KeyCode.Space) == false)
            return;

        if (climbing)
        {
            // 점프할 때 벽을 타고 있는 상태를 해제
            climbing = false;
            _rigidbody.useGravity = true; // 벽에서 떨어지면 Rigidbody 다시 활성화
        }

        _rigidbody.AddForce(transform.up * jumpPower * 100f);
        isJumping = true;
        canJump = false;
        StartCoroutine(StopJump(jumpWaitTime));
    }
    #endregion

    #region CollisionEnter/Exit
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isJumping = false;
            _rigidbody.useGravity = true;

            //inAir = false;
            //isGrounded =true;
        }
        else if (collision.collider.CompareTag("Curtain"))
        {
            isNearObject = true;
            //inAir = false;
            //isGrounded = true;
        }
        else if (collision.collider.CompareTag("Stop"))
        {
            stopClimbing = true;
            ClimbingStop();
            _rigidbody.useGravity = true;
        }

    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            //inAir = true;
            //isGrounded = false;
        }
        else if (collision.collider.CompareTag("Curtain"))
        {
            isNearObject = false;
            isClimbing = false;
            startClimbing = false;
            //inAir = true;
        }
        else if (collision.collider.CompareTag("Stop"))
        {
            startClimbing = false;
        }
    }

    #endregion

    #region Climbing Start/Stop

    // 벽을 탈 때 상하로만 움직이게 하는 함수

    public void HandleClimbing()
    {
        _rigidbody.useGravity = false; // 벽을 탈 때 중력 비활성화

        _frontInput = Input.GetAxis("Vertical");  // 상하 입력

        // 입력이 있을 때만 climbing 상태를 true로 설정
        if (Mathf.Abs(_frontInput) > 0.1f)  // 입력이 있으면 isClimbing을 true로 설정
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }

        // 상하로만 움직이도록 설정
        Vector3 moveDirection = Vector3.up * _frontInput * Time.deltaTime * moveSpeed;
        _rigidbody.MovePosition(transform.position + moveDirection);
    }
    public void ClimbingStop()
    {
        climbing = false;
        transform.position = targetObject.transform.position; // 포지션 변경
        StartCoroutine(WaitStop(1f)); // Stop 후 대기
        isClimbing = false; // 클라이밍 상태 종료
        startClimbing = false; // startClimbing을 false로 설정하여 중복 호출 방지
    }
    private IEnumerator WaitStop(float waitTime)
    {
        canMove = false;  // 2초 동안 입력 받지 않도록 설정
        yield return new WaitForSeconds(waitTime);  // 기다리기
        canMove = true;  // 이동 가능하도록 설정
        stopClimbing = false; // stopClimbing 애니메이션을 트리거
    }
    private IEnumerator WaitStart(float waitTime)
    {
        canMove = false;  // 2초 동안 입력 받지 않도록 설정
        yield return new WaitForSeconds(waitTime);  // 기다리기
        canMove = true;  // 이동 가능하도록 설정

        startClimbing = false;
        climbing = true;
    }
    #endregion
}
