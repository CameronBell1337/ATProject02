using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput input;
    private CharacterController controller;
    private Animator anim;

    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float velY;

    Vector3 velocity;

    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        controller = FindObjectOfType<CharacterController>();
        input = FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        attack();
    }

    void PlayerMovement()
    {
        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(input.moveCharacter * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

    }

    public void attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("attack", true);
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("attack", false);
        }
    }
}
