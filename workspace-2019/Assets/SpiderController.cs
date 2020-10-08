using UnityEngine;

public class SpiderController : MonoBehaviour
{
    Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var hor = Vector3.back * Input.GetAxisRaw("Horizontal");
        var ver = Vector3.right * Input.GetAxisRaw("Vertical");

        var force = (hor + ver) * 10;
        
        _rb.AddForce(force);
    }
}
