using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class RayLegData
{
    public Transform rayFrom;
    public Transform rayHitObject;
    public Transform legTarget;
    public bool canMove;
}

public class RayPoints : MonoBehaviour
{
    [SerializeField] List<RayLegData> rayLegDataList;
    bool _inMoveProcess;
    int _layerMask;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if(_inMoveProcess) return;
        
        for (int i = 0; i < rayLegDataList.Count; i++)
        {
            _inMoveProcess = ArmRayCast(rayLegDataList[i]);
            if (_inMoveProcess)
            {
                StartCoroutine(ForwardMove(i));
                break;
            }
        }
    }

    IEnumerator ForwardMove(int i)
    {
        for (int j = 0; j < 4; j++)
        {
            rayLegDataList[i].legTarget.DOMove(rayLegDataList[i].rayHitObject.position, 0.1f);
            i++;
            i = i >= 4 ? 0 : i;
            yield return new WaitForSeconds(0.1f);
        }

        _inMoveProcess = false;
    }
    

    bool ArmRayCast(RayLegData rayLegData)
    {
        if (Physics.Raycast(rayLegData.rayFrom.position, -rayLegData.rayFrom.up, out var hit, Mathf.Infinity, _layerMask))
        {
            Debug.DrawRay(rayLegData.rayFrom.position, -rayLegData.rayFrom.up * hit.distance, Color.yellow);
            rayLegData.rayHitObject.position = hit.point;
        }
        
        float distance = Vector3.Distance(rayLegData.rayHitObject.position, rayLegData.legTarget.position);

        return distance > 0.3f;
    }
}
