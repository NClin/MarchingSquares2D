using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBall : MonoBehaviour
{
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("hit at " + transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector2.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector2.down);
        }
    }
}
