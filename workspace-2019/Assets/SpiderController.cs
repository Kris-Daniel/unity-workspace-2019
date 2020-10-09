using System;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [SerializeField] float topSpeed = 3f;
    [SerializeField] float topForce = 100f;
    [SerializeField] RayPoints rayPoints;
    Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");

        
        if (rayPoints.IsOnGround)
        {
            var force = transform.right * (ver * topForce);
            force += -transform.up * topForce / 2f;
            
            _rb.AddForce(force);
            
            if (_rb.velocity.magnitude > topSpeed)
                _rb.velocity = _rb.velocity.normalized * topSpeed;
        }
    }
}
