using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <CAY>

public class CameraMove : MonoBehaviour
{
    public _PlayerInput _chr;
    Vector3 pos;
    Vector3 rot;
    [SerializeField] float _obstacleDist = 0.9f;
    float camDistance;
    [SerializeField] float _rotSpeed, _camSpeed, _maxDistance;

    //스크롤 스피드
    public float scrollSpeed = 2000.0f;



    private void Start()
    {
        _maxDistance = 7f;
        if (_chr == null)
        {
            _chr = GameObject.Find("@Player").GetComponent<_PlayerInput>();
        }
        camDistance = _maxDistance;
        pos = _chr.transform.position - transform.forward * camDistance;
    }

    private void Update()
    {
        avoidObstacle();
        pos = _chr.transform.position - transform.forward * camDistance;
        rot = new Vector3(this.transform.rotation.eulerAngles.x, _chr.transform.rotation.eulerAngles.y, 0);
        pos.y += 1f;
    }

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(rot), _rotSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, pos, _camSpeed * Time.deltaTime);
    }

    void avoidObstacle()
    {
        RaycastHit hit;
        Vector3 dir = transform.position - _chr.transform.position;
        Debug.DrawRay(_chr.transform.position, dir.normalized * dir.magnitude,Color.red);
        if (Physics.Raycast(_chr.transform.position, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask("Wall")))
        {
            Debug.Log(hit.transform.name);
            Vector3 dist = hit.point - _chr.transform.position;
            camDistance = (dist.magnitude * _obstacleDist);
        }
        else
        {
            camDistance = Mathf.Clamp(camDistance + (_camSpeed * Time.deltaTime), 1f, _maxDistance);
        }
    }

    void zoomOut()
    {

    }
}
