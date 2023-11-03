using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Target")) {
            print("Hit: " + collision.gameObject.name + "!");
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground")) {
          Destroy(gameObject);
        }
    }
}
