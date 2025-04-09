using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HamsterAnimator : MonoBehaviour
{
    private Animator playerAnimator; //플레이어 캐릭터의 애니메이터
    private HamsterController2 hamsterController; // 플레이어 이동을 알려주는 컴포넌트


    public void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        hamsterController = GetComponent<HamsterController2>();
    }

    public void Update()
    {
        if (hamsterController == null) return;

        // 애니메이션 상태 변경
        HandleAnimations();
    }

    public void HandleAnimations()
    {
        #region  Jump
        // 점프 상태 (점프가 실행될 때)
        if (hamsterController.isJumping)
        {
            playerAnimator.SetBool("IsJumping", true);
        }
        else
        {
            playerAnimator.SetBool("IsJumping", false);
        }
        #endregion

        #region Walk 애니메이션은 InAir가 false, Walk일때 실행
        if (hamsterController.isWalking)
        {
            playerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }
        #endregion

        #region Sprint
        if (hamsterController.isSprint)
        {
            playerAnimator.SetBool("IsSprint", true);
        }
        else
        {
            playerAnimator.SetBool("IsSprint", false);
        }
        #endregion


        #region Climbing
        if (hamsterController.climbing)
        {
            playerAnimator.SetBool("Climbing", true);
        }
        else
        {
            playerAnimator.SetBool("Climbing", false);
        }

        if (hamsterController.isClimbing)
        {
            playerAnimator.SetBool("IsClimbing", true);
        }
        else
        {
            playerAnimator.SetBool("IsClimbing", false);
        }


        if (hamsterController.stopClimbing)
        {
            playerAnimator.SetBool("StopClimbing", true);
        }
        else
        {
            playerAnimator.SetBool("StopClimbing", false);
        }

        if (hamsterController.startClimbing)
        {
            playerAnimator.SetBool("StartClimbing", true);
        }
        else
        {
            playerAnimator.SetBool("StartClimbing", false);
        }
        #endregion

        #region InAir InAir만 켜져있을때 실행
        if (hamsterController.inAir)
        {
            playerAnimator.SetBool("InAir", true);
        }
        else
        {
            playerAnimator.SetBool("InAir", false);
        }
        #endregion

        #region awake
        if (hamsterController.awake)
        {
            playerAnimator.SetBool("Awake", true);
        }
        else
        {
            playerAnimator.SetBool("Awake", false);
        }
        #endregion

        #region IsGrounded
        if (hamsterController.IsGrounded())
        {
            playerAnimator.SetBool("IsGrounded", true);
        }
        else
        {
            playerAnimator.SetBool("IsGrounded", false);
        }
        #endregion

        if (GameManager.Instance.isEating)
        {
            hamsterController.canMove = false;
            playerAnimator.SetBool("IsEating", true);
            GameManager.Instance.isEating = false;
            StartCoroutine(Wait(1.5f));
        }
        else
        {

            playerAnimator.SetBool("IsEating", false);
        }

        playerAnimator.SetFloat("CollectSeed", GameManager.Instance.score);

    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  // 2초 대기
        hamsterController.canMove = true;  // 이동 가능
    }
}