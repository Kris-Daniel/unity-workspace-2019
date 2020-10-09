using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RayLegData
{
    public Transform rayFrom;
    public Transform rayHitObject;
    public Transform legTarget;
    public Vector3 rayHitObjectPos;
}

public class RayPoints : MonoBehaviour
{
    [SerializeField] Transform midPointObject;
    [SerializeField] AnimationCurve legMovementCurve;
    [SerializeField] List<RayLegData> rayLegDataList;
    
    public bool IsOnGround { get; private set; }
    
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
        int legIndexDir = direction > 0 ? 1 : -1;
        
        bool inMoveProcess = false;
        bool isOnGround = true;
        
        for (int i = 0; i < rayLegDataList.Count; i++)
        {
            var tuple = ArmRayCast(rayLegDataList[i]);
            inMoveProcess = tuple.Item1 || inMoveProcess;
            isOnGround = tuple.Item2 && isOnGround;
        }
        

        IsOnGround = isOnGround; 

        if (inMoveProcess && !_inMoveProcess && IsOnGround)
        {
            _inMoveProcess = true;
            StartCoroutine(MoveLegs(legIndexDir));
        }
        
    }

    Vector3 GetMidPoint()
    {
        Vector3[] midPoints = new Vector3[2];
        for (int i = 0, j = 0; i < rayLegDataList.Count; i += 2, j++)
        {
            midPoints[j] = (rayLegDataList[i].rayHitObject.position + rayLegDataList[i + 1].rayHitObject.position) / 2f;
        }
        
        Vector3 midPoint = (midPoints[0] + midPoints[1]) / 2f;

        return midPoint;
    }
    
    IEnumerator MoveLegs(int legIndexDir)
    {
        for (int j = 0; j < 4; j++)
        {
            rayLegDataList[j].rayHitObjectPos = rayLegDataList[j].rayHitObject.position;
        }
        
        int i = legIndexDir > 0 ? 0 : 1;
        for (int j = 0; j < 4; j++)
        {
            StartCoroutine(MoveLeg(rayLegDataList[i]));
            i += legIndexDir;
            i = i >= 4 ? 0 : i;
            i = i < 0 ? 3 : i;
            yield return new WaitForSeconds(0.05f);
        }

        _inMoveProcess = false;
    }

    IEnumerator MoveLeg(RayLegData rayLegData)
    {
        Vector3 initialLegPos = rayLegData.legTarget.position;
            
        Vector3 direction = rayLegData.rayHitObjectPos - initialLegPos;
        direction *= 1.5f;
            
        float distanceRate = 0;
        while (distanceRate < 1)
        {
            distanceRate = Mathf.Clamp01(distanceRate + Time.deltaTime * 10f);
            float yPos = 0.5f * legMovementCurve.Evaluate(distanceRate);

            rayLegData.legTarget.position = initialLegPos + direction * distanceRate;

            Vector3 legLocalPos = rayLegData.legTarget.localPosition;
            legLocalPos.y += yPos;
            rayLegData.legTarget.localPosition = legLocalPos;
                
            yield return null;
        }
    }


    (bool, bool) ArmRayCast(RayLegData rayLegData)
    {
        var tuple = (isTooFar: false, isOnGround: false);
        
        if (Physics.Raycast(rayLegData.rayFrom.position, -rayLegData.rayFrom.up, out var hit, 2f, _layerMask))
        {
            Debug.DrawRay(rayLegData.rayFrom.position, -rayLegData.rayFrom.up * hit.distance, Color.yellow);
            rayLegData.rayHitObject.position = hit.point;
            tuple.isOnGround = true;
        }
        else
        {
            rayLegData.rayHitObject.position = rayLegData.rayFrom.position + -rayLegData.rayFrom.up;
        }
        
        float distance = Vector3.Distance(rayLegData.rayHitObject.position, rayLegData.legTarget.position);

        tuple.isTooFar = distance > 0.5f;

        return tuple;
    }
}
