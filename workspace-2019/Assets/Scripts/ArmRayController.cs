using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRayController : MonoBehaviour
{
    [SerializeField] Transform ArmRayFrom;
    [SerializeField] Transform ArmRayPoint;

    int _layerMask;
    
    // Start is called before the first frame update
    void Start()
    {
        _layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(ArmRayFrom.position, ArmRayFrom.TransformDirection(Vector3.down), out hit, Mathf.Infinity, _layerMask))
        {
            Debug.DrawRay(ArmRayFrom.position, ArmRayFrom.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            ArmRayPoint.position = hit.point;
        }
    }
}
