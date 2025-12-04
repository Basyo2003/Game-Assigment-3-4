using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LylekGames.Tools
{
    public class CharacterMovement : MonoBehaviour
    {
        public CharacterController controller;

        public float speed = 3;

        private Vector3 gravity = new Vector3(0, -9.81f, 0);
        
        // Controls whether the player can move (false = frozen, true = can move)
        private bool canMove = true;

        public void Update()
        {
            if (controller == null) return;

            // Always apply gravity to keep the controller grounded
            controller.Move(gravity * Time.deltaTime);

            // Stop here if movement is frozen
            if (!canMove) return;

            if (Input.GetKey(KeyCode.W))
            {
                controller.Move(transform.forward * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                controller.Move(-transform.right * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                controller.Move(transform.right * speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                controller.Move(-transform.forward * speed * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Freeze the player movement (useful during cutscenes, dialogues, etc.)
        /// </summary>
        public void FreezePlayer()
        {
            canMove = false;
        }
        
        /// <summary>
        /// Unfreeze the player movement and allow normal controls
        /// </summary>
        public void UnfreezePlayer()
        {
            canMove = true;
        }
    }
}