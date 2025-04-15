using System.Collections;
using UnityEngine;

public class HamsterController2 : MonoBehaviour
{
    public static bool NeedLoadPosition { get; set; }
    public static Vector3 PositionOnLoad { get; set; }

    [SerializeField] float groundRaycastDist = 0.1f;
    [SerializeField] float curtainRaycastDist = 0.1f;

    [Header("Player")]
    public float moveSpeed; //1
    public float rotateSpeed; //90
    public float sprintSpeed;//2
    public float jumpPower; //5
    public float jumpWaitTime = 0.8f;

    [Tooltip("메인 카메라")]
    public Camera _camera; // 메인 카메라

    [Header("Animation")]

    public bool isWalking;
    public bool isSprint;
    public bool isJumping;

    public bool startClimbing;
    public bool climbing; //ClimbingIdle
    public bool isClimbing;
    public bool stopClimbing;

    public bool inAir;
    public bool awake;

    [Header("-------------------------------------------------------")]
    public bool canMove = false;  // 이동 가능 여부
    private bool canJump = true;
    private bool isNearObject = false; // 콜라이더에 닿았는지 여부
    private float _frontInput; // frontinput 값
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private float realMoveSpeed;

    public void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (NeedLoadPosition)
            LoadPosition();
    }

    private void LoadPosition()
    {
        transform.position = PositionOnLoad;
        NeedLoadPosition = false;
    }

    public void FixedUpdate()
    {
        if (canMove)  // canMove가 true일 때만 이동 처리
        {
            HandleMove();
            HandleSprint();

            if (canJump)
            {
                if (IsGrounded())
                {
                    HandleJump();
                }
            }

            if (climbing)
            {
                if (IsFrontCurtain()) // 앞에 Curtain이 있을 때만 클라이밍 유지
                {
                    HandleClimbing();
                    ClimbingJump();
                }
                else
                {
                    ClimbingStop(); // Curtain이 없으면 자동으로 클라이밍 멈춤
                }
            }
            if (Input.GetKeyDown(KeyCode.E) && !climbing)
            {
                // E 키를 눌렀을 때 클라이밍 시작
                if (!startClimbing && IsFrontCurtain()) // 이미 시작 중이 아니면 클라이밍을 시작
                {
                    startClimbing = true;
                    StartCoroutine(WaitStartClimbing(0.1f));  // 1초 대기 후 클라이밍 시작
                }
            }
        }
        Wakeup();
    }

    public void Update()
    {
        inAir = !IsGrounded();

        if (IsGrounded())
        {
            isJumping = false;
            _rigidbody.useGravity = true;
        }
    }

    public bool IsGrounded()
    {
        // 아래로 레이캐스트를 쏴서 바닥에 닿았는지 체크
        return Physics.Raycast(transform.position, Vector3.down, groundRaycastDist); // 작은 거리로 바닥 체크

    }

    private bool IsFrontCurtain()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, curtainRaycastDist))
        {
            if (hit.collider.CompareTag("Curtain"))
            {
                return true;
            }
        }
        return false;
    }

    #region Move/Sprint/Jump
    public void HandleMove()
    {
        if (!climbing)
        {
            isClimbing = false;
            _frontInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (_frontInput == 0 && horizontalInput == 0)
            {
                isWalking = false;
                realMoveSpeed = 0f;
                return;
            }

            realMoveSpeed = isSprint ? sprintSpeed : moveSpeed;

            Vector3 forward = _camera.transform.forward;// 카메라의 방향에 맞춰 플레이어가 회전 (앞 방향으로 이동)
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = _camera.transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 moveDirection = forward * _frontInput + right * horizontalInput;
            moveDirection.Normalize();  // 방향 벡터를 정규화하여 속도 일관성 유지

            // 실제 이동 처리
            Vector3 position = transform.position + moveDirection * Time.deltaTime * realMoveSpeed;
            _rigidbody.MovePosition(position);

            isWalking = true;

            // 회전 처리 (플레이어가 이동 방향으로 자연스럽게 회전)
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection),
                    Time.deltaTime * rotateSpeed / 10f);
            }
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

        _rigidbody.AddForce(transform.up * jumpPower * 100f);

        canJump = false;
        StartCoroutine(StopJump(jumpWaitTime));
    }
    #endregion

    #region CollisionEnter/Exit
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Curtain"))
        {
            isNearObject = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Curtain"))
        {
            isNearObject = false;
        }
    }
    #endregion

    #region Climbing Start/Stop
    public void HandleClimbing()
    {
        _rigidbody.useGravity = false; // 벽을 탈 때 중력 비활성화
        _capsuleCollider.direction = 1;
        _frontInput = Input.GetAxis("Vertical");  // 상하 입력

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
        isClimbing = false; // 클라이밍 상태 종료
        climbing = false;
        stopClimbing = true;
        _capsuleCollider.direction = 2;
        transform.position += transform.forward * 0.2f;
        StartCoroutine(WaitStop(1f)); // Stop 후 대기
    }

    public void ClimbingJump()
    {
        if (climbing)
        {
            if (isJumping)
                return;

            if (Input.GetKey(KeyCode.Space) == false)
                return;
            isJumping = true;
            climbing = false;
            _capsuleCollider.direction = 2;
            _rigidbody.useGravity = true; // 벽에서 떨어지면 Rigidbody 다시 활성화
        }
    }
    #endregion

    #region Coroutine
    private IEnumerator WaitStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  // 2초 대기
        canMove = true;  // 이동 가능
    }

    private IEnumerator WaitStartClimbing(float waitTime)
    {
        canMove = false;  // 2초 동안 입력 받지 않도록 설정
        yield return new WaitForSeconds(waitTime);  // 기다리기
        canMove = true;  // 이동 가능하도록 설정

        startClimbing = false;
        climbing = true;
    }
    private IEnumerator WaitStop(float waitTime)
    {
        canMove = false;  // 2초 동안 입력 받지 않도록 설정
        yield return new WaitForSeconds(waitTime);  // 기다리기
        canMove = true;  // 이동 가능하도록 설정
        stopClimbing = false; // stopClimbing 애니메이션을 트리거
    }

    private IEnumerator StopJump(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canJump = true;
    }

    #endregion

    void Wakeup()
    {
        if (!awake && (Input.anyKeyDown || Input.anyKey))  // 키 입력이 있을 때
        {
            awake = true;
            StartCoroutine(WaitStart(2f));  // 2초 대기 후 이동 가능하게 설정
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * curtainRaycastDist);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundRaycastDist);
    }
}
