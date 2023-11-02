using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponController : MonoBehaviour {
    private enum Mode {
        Bullets,
        Raycast
    }

    public Image panel;
    public TMP_Text counter;

    [SerializeField] private Mode mode;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    [SerializeField] public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;
    
    public bool isShooting = false;
    public bool isHot = false;
    [SerializeField] public float fireRate;
    float lastShot = 0; 

    //TEMPERATURE
    [SerializeField] private float timeCounterDuration;
    [SerializeField] private float coolSpeed;
    [SerializeField] private float heatSpeed;
    private float elapsedTime = 0.0f;
    Color newColor;
    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    void Start() {
        Gradient();
    }

    public void Shooting() {
        if (!isShooting) {
            isShooting = true;
        }
    }

    public void StopShooting() {
        if (isShooting) {
            isShooting = false;
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (!isShooting && !isHot)
                isShooting = true;
        } else {
            isShooting = false;
        }

        CheckIsShooting();
        CheckTemperature();
    }

    void CheckIsShooting() {
        if (isShooting && !isHot) {
            if (elapsedTime <= timeCounterDuration) {
                elapsedTime += Time.deltaTime * heatSpeed;
                FireWeapon();

            } else {
                isHot = true;
                //smokeParticles.SetActive(true);
            }
        } else if (!isShooting) {
            if (elapsedTime > 0) {
                elapsedTime -= Time.deltaTime * coolSpeed;
                if (elapsedTime < 0)
                    elapsedTime = 0;
            } else {
                ShootingAvailable();
            }
        }
    }

    void ShootingAvailable() {
        newColor = Color.green;
        newColor.a = 0.5f;
        panel.color = newColor;
        isHot = false;
        //audioSource.Stop();
        //smokeParticles.SetActive(false);
    }

    void Gradient() {
        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.green;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.red;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    void CheckTemperature() {
        if (!isHot) {
            //counter.text = "" + (int) (elapsedTime*10) + "%";
            newColor = gradient.Evaluate(elapsedTime/timeCounterDuration);
            newColor.a = 0.5f;
            panel.color = newColor; 
        } else {
            //PlayAudioTemp();
            counter.text = "HOT!!";
            newColor = gradient.Evaluate(elapsedTime/timeCounterDuration);
            newColor.a = 0.5f;
            panel.color = newColor; 
        }
    }

    private void FireWeapon() {
        Debug.Log("fireTime: " + Abs((Time.deltaTime - lastShot)).ToString());
        if ((Time.deltaTime - lastShot) > fireRate) {
            lastShot = Time.deltaTime;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        }
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

}
