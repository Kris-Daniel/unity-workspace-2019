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
    [SerializeField] Transform midPointObject;
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
        midPointObject.position = GetMidPoint();

        var clearedBodyPos = transform.position;
        clearedBodyPos.y = midPointObject.position.y;
        
        var direction = clearedBodyPos.x - midPointObject.position.x;
        
        if(_inMoveProcess) return;
        
        bool inMoveProcess = false;
        
        for (int i = 0; i < rayLegDataList.Count; i++)
        {
            inMoveProcess = ArmRayCast(rayLegDataList[i]) || inMoveProcess;
        }
        
        _inMoveProcess =  inMoveProcess;
        if (_inMoveProcess)
        {
            var moveRoutine = direction > 0 ? ForwardMove(0) : BackMove(1);
            StartCoroutine(moveRoutine);
        }
    }

    Vector3 GetMidPoint()
    {
        Vector3[] midPoints = new Vector3[2];
        for (int i = 0, j = 0; i < rayLegDataList.Count; i += 2, j++)
        {
            midPoints[j] = (rayLegDataList[i].legTarget.position + rayLegDataList[i + 1].legTarget.position) / 2f;
        }
        
        Vector3 midPoint = (midPoints[0] + midPoints[1]) / 2f;

        return midPoint;
    }

    IEnumerator ForwardMove(int i)
    {
        for (int j = 0; j < 4; j++)
        {
            var direction = rayLegDataList[i].rayHitObject.position - rayLegDataList[i].legTarget.position;
            rayLegDataList[i].legTarget.DOMove(direction * 1.5f, 0.1f).SetRelative();
            i++;
            i = i >= 4 ? 0 : i;
            yield return new WaitForSeconds(0.1f);
        }

        _inMoveProcess = false;
    }
    
    IEnumerator BackMove(int i)
    {
        for (int j = 0; j < 4; j++)
        {
            var direction = rayLegDataList[i].rayHitObject.position - rayLegDataList[i].legTarget.position;
            rayLegDataList[i].legTarget.DOMove(direction * 1.5f, 0.1f).SetRelative();
            i--;
            i = i < 0 ? 3 : i;
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

        return distance > 0.5f;
    }
}
