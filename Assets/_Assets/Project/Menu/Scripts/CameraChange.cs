using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] CameraType camType;
    [SerializeField] GameObject hamster;
    [SerializeField] GameObject c_hamster;

    [SerializeField] CinemachineDollyCart cart;
    [SerializeField] Transform hamsterPosition;

    private void OnTriggerEnter(Collider other)
    {
        switch(camType)
        {
            case CameraType.HAMSTER:
                // 햄스터 메쉬렌더러 활성화
                // 햄스터 위치를 도착지점으로 옮기기
                // 돌리 카트 속도 0
                // 카트햄 메쉬렌더러 비활성화

                hamster.SetActive(true);
                hamster.transform.position = hamsterPosition.position;

                cart.m_Speed = 0;
                c_hamster.SetActive(false);

                // 플레이어 이동 입력 초기화
                ResetPlayerMovement();

                break;

            case CameraType.AIR_PLANE:
                // 햄스터 메쉬렌더러 비활성화
                // 카트햄 메쉬렌더러 활성화
                // 돌리 카트 속도 올리기

                hamster.SetActive(false);
                c_hamster.SetActive(true);
                cart.m_Speed = 5f;
                break;
        }    

        CameraSet.Instance.ChangeCamera(camType);
    }

    private void ResetPlayerMovement()
    {
          
    }
}
