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

    public Camera playerCamera;
    public float spreadIntensity;

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

    private Vector3 laserImpactPos;
    private bool isRayCastShooting;
    private float pointerInterpolator;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem smokePS;
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootFX;
    [SerializeField] private AudioClip tempFX;


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
        audioSource = GetComponent<AudioSource>();
        smokePS.Stop();
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
                if (mode.Equals(Mode.Bullets)) {
                    FireWeapon();
                }
                else
                    RayCastShot();
            } else {
                isHot = true;
                smokePS.Play();
            }
        } else if (!isShooting) {
            if (elapsedTime > 0) {
                elapsedTime -= Time.deltaTime * coolSpeed;
                if (elapsedTime < 0)
                    elapsedTime = 0;
            } else if (isHot) {
                ShootingAvailable();
            }
        }
    }

    void ShootingAvailable() {
        newColor = Color.green;
        newColor.a = 0.5f;
        panel.color = newColor;
        isHot = false;
        audioSource.Stop();
        smokePS.Stop();
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
            counter.text = "" + (int) (elapsedTime*10) + "%";
            newColor = gradient.Evaluate(elapsedTime/timeCounterDuration);
            newColor.a = 0.5f;
            panel.color = newColor; 
        } else {
            PlayAudioTemp();
            counter.text = "HOT!!";
            newColor = gradient.Evaluate(elapsedTime/timeCounterDuration);
            newColor.a = 0.5f;
            panel.color = newColor; 
        }
    }

    private void FireWeapon() {
        lastShot += Time.deltaTime;
        if (lastShot >=  fireRate) {
            lastShot = lastShot - fireRate;           
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
                PlayAudioShoot();
        }
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private void RayCastShot() {
        //AudioManager.I.PlaySound(SoundName.RaycastShot, shootPoint.position);
        laserImpactPos = new Vector3(0, 1000, 0);
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, Mathf.Infinity)) {
            laserImpactPos = new Vector3(0, Vector3.Distance(shootPoint.position, hit.point),0);
            //ParticleSystem laserImpact = Instantiate(laserImpactPrefab, hit.point, Quaternion.identity);
            //Destroy(laserImpact, 2f);

            if (hit.transform.gameObject.tag == "Target") {
                print("Hit: " + hit.transform.gameObject.name + "!");
            }
        }
        isRayCastShooting = true;
        pointerInterpolator = 0;
    }

    void PlayAudioTemp() {
        audioSource.clip = tempFX;
        audioSource.loop = true;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void PlayAudioShoot() {
        audioSource.clip = shootFX;
        audioSource.loop = false;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
