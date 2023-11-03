using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    [SerializeField] private AudioClip hit;
    AudioSource fxSource;

    void Start() {
        fxSource = GameObject.FindGameObjectWithTag("FX_AudioSource").GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Target")) {
            print("Hit: " + collision.gameObject.name + "!");
            fxSource.clip = hit;
            fxSource.Play();
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground")) {
          Destroy(gameObject);
        }
    }
}
