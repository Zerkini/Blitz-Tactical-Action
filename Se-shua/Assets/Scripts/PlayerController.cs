using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rigidbody2D;

    [SerializeField]
    private float speed;

    private void Start(){
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("PickUp"))
        {
            collision.gameObject.SetActive(false);
        }
    }

    void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movementVector = new Vector2(moveHorizontal, moveVertical);
        rigidbody2D.AddForce(movementVector * speed);
    }
}
