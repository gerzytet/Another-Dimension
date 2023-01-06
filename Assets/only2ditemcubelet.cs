using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class only2ditemcubelet : MonoBehaviour
{
    private float originalZ;
    public static float wiggle = 0.04f;
    
    void Start()
    {
        originalZ = transform.position.z;
        transform.position += new Vector3(0, 0, Random.Range(-0.5f, 0.5f));
    }
    void Update()
    {
        float z = transform.position.z;
        z += Random.Range(-wiggle, wiggle);
        z = Mathf.Lerp(z, originalZ, 0.008f);
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
