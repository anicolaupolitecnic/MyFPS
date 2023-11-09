using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    [SerializeField] private AudioClip hit;
    AudioSource fxSource;
    private Animator anim;

    void Start() {
        fxSource = GameObject.FindGameObjectWithTag("FX_AudioSource").GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Target")) {
            //print("Hit: " + collision.gameObject.name + "!");
            fxSource.clip = hit;
            fxSource.Play();
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Enemy")) {
            //StartCoroutine(DestroyEnemyAfterTime(gameObject, 5f));
            //Debug.Log("die!!!!");
            collision.gameObject.GetComponent<Animator>().SetBool("die", true);   //Play("die");
        } else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground")) {
          Destroy(gameObject);
        }
    }

    private IEnumerator DestroyEnemyAfterTime(GameObject enemy, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(enemy);
    }


}
