using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    public float BaseSpeed;
    public float Drag;
    public float XRotationSpeed;
    public float ZRotationSpeed;
    

    public Rigidbody Rb;

    private Vector3 Rotation;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Rotation = transform.eulerAngles;
    }

    private void Update()
    {
        Rb.drag = Drag;

        // rotate player
        Rotation.x += XRotationSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        Rotation.y += ZRotationSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

        
        // add forward force
        Vector3 forwardForce = transform.rotation * Vector3.up * BaseSpeed;
        
        Rb.AddForce(forwardForce, ForceMode.Force);
        transform.rotation = Quaternion.Euler(Rotation);

        // debug
        Debug.DrawRay(transform.position, forwardForce, Color.green);
        
        // todo: fix Y rotation
    }
}
