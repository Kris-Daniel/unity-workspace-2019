using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRayController : MonoBehaviour
{
    [SerializeField] Transform ArmRayPoint;
    [SerializeField] Transform LegTarget;

    int _layerMask;
    bool _inStep;
    
    // Start is called before the first frame update
    void Start()
    {
        _layerMask = LayerMask.GetMask("Ground");
        ArmRayPoint.position = LegTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, _layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            ArmRayPoint.position = hit.point;
        }

        float distance = Vector3.Distance(ArmRayPoint.position, LegTarget.position);

        if (distance > 0.7f && !_inStep)
        {
            _inStep = true;
            StartCoroutine(MakeStep());
        }
        
    }

    IEnumerator MakeStep()
    {
        while (Vector3.Distance(ArmRayPoint.position, LegTarget.position) > 0.1f)
        {
            LegTarget.position = Vector3.MoveTowards(LegTarget.position, ArmRayPoint.position, Time.deltaTime);
            yield return null;
        }

        _inStep = false;
    }
    
}
