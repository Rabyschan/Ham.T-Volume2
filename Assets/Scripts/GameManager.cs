using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private AudioSource systemAudioPlayer;
    public AudioClip eatingSound;
    public AudioClip jumpSound;
    public AudioClip pickItemSound;
    public AudioClip dropItemSound;

    // �̱��� ���ٿ� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static GameManager m_instance; //�̱����� �Ҵ�� static ����

    private int score = 0;

    private InteractableItem currentInteractable; // ���� ��ȣ�ۿ� ������ ������Ʈ
    public InteractableItem heldItem; // ���� �÷��̾ ��� �ִ� ������
    public Transform chinTransform; // �������� ���� ��ġ(�÷��̾��� ��)
    public GameObject interactText; // "E Ű�� ���� ����" UI

    private void Awake()
    {
        systemAudioPlayer = GetComponent<AudioSource>();

        if(instance != this)
        {
            Destroy(this.gameObject);
        } 
    }

    private void Start()
    {
        if(interactText != null)
        {
            interactText.gameObject.SetActive(false); // ������ �� �����
        }
    }

    // ���� �Ҹ��� ����ϴ� �Լ�
    public void PlayJumpSound()
    {
        // �Ҹ��� �̹� ��� ������ Ȯ��
        if (systemAudioPlayer != null && jumpSound != null)
        {
            systemAudioPlayer.PlayOneShot(jumpSound); // ���� �Ҹ� ���
        }
    }

    private void Update()
    {
        if(currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            ToggleItem();
        }
    }
    #region CollectItem() �������� �����ϴ� �ڵ�
    /*
    public void CollectItem()
    {
        if (currentInteractable != null)
        {
            Debug.Log("�������� ȹ���߽��ϴ�: " + currentInteractable.name);
            Destroy(currentInteractable); // ������ ����
            currentInteractable = null;

            if (interactText != null)
            {
                interactText.gameObject.SetActive(false); // UI �����
            }
        }
    }*/
    #endregion

    public void ToggleItem()
    {
        if (heldItem == null)
        {
            systemAudioPlayer.PlayOneShot(pickItemSound);
            PickupItem(); // ������ ����
        }
        else
        {
            systemAudioPlayer.PlayOneShot(dropItemSound);
            DropItem(); // ������ ��������
        }
    }

    // �������� �÷��̾� �տ� ����
    public void PickupItem()
    {
        if (currentInteractable != null)
        {
            heldItem = currentInteractable;

            // Rigidbody ��Ȱ��ȭ (���� ���� ����)
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            heldItem.transform.SetParent(chinTransform); // �θ� ������ ����
            heldItem.transform.localPosition = Vector3.zero; // ���� ��ġ�� �̵�
            heldItem.transform.localRotation = Quaternion.identity; // ���� ȸ���� ����

            Debug.Log("�������� �������ϴ�: " + heldItem.name);
        }
    }

    // �������� ��������
    public void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null); // �θ� ���� ����

            // Rigidbody Ȱ��ȭ (�ڿ������� ����������)
            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                // ������ Rigidbody�� ������ ���� ���� �̵��ϰ� ��
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Vector3.down * 2, ForceMode.Impulse);
            }

            Debug.Log("�������� �������ҽ��ϴ�: " + heldItem.name);
            heldItem = null; // ��� �ִ� ������ ����
        }
    }

    //�÷��̾ ������ ���� �ȿ� ������ �� ȣ��
    public void SetInteractable(InteractableItem item)
    {
        currentInteractable = item;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(true);
        }
    }

    //�÷��̾ ������ �������� ����� �� ȣ��
    public void ClearInteractable()
    {
        if (heldItem != null)
            return;

        currentInteractable = null;

        if(interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    //������ ������Ŵ
    public void CollectSeed(int newScore)
    {
        score += newScore;
        UIManager.instance.UpdateScoreText(score);

        //�Դ� �Ҹ� ���
        if (systemAudioPlayer != null && eatingSound != null)
        {
            systemAudioPlayer.PlayOneShot(eatingSound);
        }
    }

    //���� ������ ��ȯ��
    public int GetScore()
    {
        return score; // ������ ��ȯ
    }
}
