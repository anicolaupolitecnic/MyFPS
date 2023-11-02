using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;
    public bool canShoot = true;
    float timer;
    public float shootDelay = 1f;

    void Update() {
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (canShoot) {
                FireWeapon();
                canShoot = false;
                timer = Time.deltaTime;
            }
        }
        if (!canShoot && ((Time.deltaTime - timer) > shootDelay)) {
            canShoot = true;
        }
    }

    private void FireWeapon() {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

}
