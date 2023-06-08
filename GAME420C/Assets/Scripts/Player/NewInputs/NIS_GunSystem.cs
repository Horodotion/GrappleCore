using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NIS_GunSystem : MonoBehaviour
{
    [Header("References")]
    public Rigidbody myRB;
    public Transform playerCam;
    public Transform gunTip;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public TextMeshProUGUI ammoStatus;
    public TextMeshProUGUI currentWep;
    public GameObject pistolMesh;
    public GameObject shottyMesh;
    public NIS_PlayerMovement pM;


    [Header("Graphics")]
    public GameObject bulletHoleGraphic;


    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowFireHold;
    public int bulletsLeft, bulletsShot;

    public bool shooting, readyToShoot, reloading;


    private void Awake()
    {
        shottyMesh.SetActive(false);
        EquipShotgun();
        bulletsLeft = magSize;
        readyToShoot = true;
        currentWep.SetText("Shotgun");
    }

    private void Update()
    {
        ammoStatus.SetText(bulletsLeft + "/" + magSize);
    }

    public void Shoot()
    {
        Debug.Log("I shot!");
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        Vector3 direction = playerCam.transform.forward + new Vector3(x, y, z);


        //Raycast Out
        if (Physics.Raycast(playerCam.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
            if (rayHit.collider.CompareTag("Player"))
            {
                Debug.Log("Player Hit");
                rayHit.collider.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
            }
        }

        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

        if (bulletsLeft == 0)
        {
            Reload();
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }

    public void EquipPistol()
    {
        shottyMesh.SetActive(false);
        pistolMesh.SetActive(true);
        currentWep.SetText("Pistol");
        damage = 50;
        timeBetweenShooting = 0.1f;
        spread = 0.01f;
        range = 40;
        reloadTime = 1.5F;
        timeBetweenShots = 0.2f;
        magSize = 6;
        bulletsPerTap = 1;
    }

    public void EquipShotgun()
    {
        shottyMesh.SetActive(true);
        pistolMesh.SetActive(false);
        currentWep.SetText("Shotgun");
        damage = 25;
        timeBetweenShooting = 0.05f;
        spread = .1f;
        range = 20;
        reloadTime = 1.5f;
        timeBetweenShots = 0f;
        magSize = 30;
        bulletsPerTap = 6;
    }
}
