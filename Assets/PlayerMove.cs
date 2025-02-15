using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float playerSpeed = 20f;
    private CharacterController characterController;
    public Animator camAnim;
    private bool isWalking;

    private Vector3 inputVector;
    private Vector3 movementVector;
    private float Gravity = -10f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        GetInput();
        MovePlayer();
        CheckForHeadBob();

        camAnim.SetBool("IsWalking", isWalking);
    }

    private void CheckForHeadBob()
    {
        if(characterController.velocity.magnitude > 0.1f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    void GetInput()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        inputVector = transform.TransformDirection(inputVector);

        movementVector = (inputVector * playerSpeed) + (Vector3.up * Gravity);
    }

    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
