using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float turboForce = 1000;
    [SerializeField] float gravityMultiplier;
    [SerializeField] ParticleSystem driftVFX;

    CapsuleCollider boxCollider;
    Rigidbody rb;
    Material mat;

    Vector2 moveInput = new Vector2 (0, 0);
    bool isInGround = false;
    bool isJumping = false;
    bool jump1Pressed = false;
    bool jump2Pressed = false;
    bool isDrifting = false;
    bool turboUsed = false;
    bool jumpForceApplied = false;
    Color defaultColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<CapsuleCollider>();
        mat = GetComponent<MeshRenderer>().material;      
        defaultColor = mat.color;
        //driftVFX = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        //Debug.Log(moveInput);
        if (rb != null)
        {
            Move();
            Turn();
            Fall();
            Jump();
            PreventZRotXVel();
            JumpingAnimation();
            Drift();
            DriftAnimatiom();
            DefaultAnimation();          
            
            
        }
    }
    private void DefaultAnimation()
    {
        if (!isDrifting && !isJumping)
        {
            mat.color = defaultColor;
        }
    }

    void Drift()
    {
        if (isDrifting && jump2Pressed && jump1Pressed && !turboUsed)
        {
            driftVFX.Play();
            rb.AddRelativeForce(Vector3.forward * turboForce);
            turboUsed = true;
        }
        if (jump2Pressed && !jump1Pressed || !jump2Pressed && jump1Pressed)
            turboUsed = false;
        if (!jump1Pressed && !jump2Pressed)
        {
            isDrifting = false;
            turboUsed = false;
        }
    }


    private void DriftAnimatiom()
    {
        if (isDrifting && !isJumping)
        {
            mat.color = Color.red;
        }        
    }

    private void JumpingAnimation()
    {        
        if (isJumping && !isDrifting)
        {
            mat.color = Color.blue;
            Vector3 rotation = transform.localEulerAngles;
            transform.localEulerAngles = rotation;
        }        
    }

    private void Jump()
    {
        if (boxCollider != null && isInGround && !jumpForceApplied && isJumping && !isDrifting)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce);
            jumpForceApplied = true;
        }
    }

    private void PreventZRotXVel()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

        Vector3 lovalVelocity = transform.InverseTransformDirection(rb.velocity);
        lovalVelocity.x = 0;
        rb.velocity = transform.TransformDirection(lovalVelocity);
    }

    private void Fall()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * gravityMultiplier);
    }

    private void Turn()
    {
        if (boxCollider != null && isInGround)
            rb.AddRelativeTorque(Vector3.up * moveInput.x * Time.deltaTime * turnSpeed * moveInput.y);
    }

    private void Move()
    {
        if (boxCollider != null && isInGround)
            rb.AddRelativeForce(Vector3.forward * moveInput.y * Time.deltaTime * moveSpeed);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (!(collision.gameObject.tag == "Ground")) return;

        isInGround = true;
        isJumping = false;

        if ((jump1Pressed || jump2Pressed) && Mathf.Abs(moveInput.x) > 0.5f)
            isDrifting = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!(collision.gameObject.tag == "Ground")) return;

        isInGround=false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        jump1Pressed = value.isPressed;
        if (isInGround) isJumping = value.isPressed;
        jumpForceApplied = false;
    }

    void OnJump2(InputValue value)
    {
        jump2Pressed = value.isPressed;
        if (isInGround) isJumping = value.isPressed;
        jumpForceApplied = false;
    }

    //void OnFire()
    //{
    //    driftVFX.Play();
    //}

}
