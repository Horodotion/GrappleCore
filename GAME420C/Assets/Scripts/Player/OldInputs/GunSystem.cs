using TMPro;
using UnityEngine;

public class GunSystem : MonoBehaviour
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


    [Header("Inputs")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode pistolKey = KeyCode.Alpha1;
    public KeyCode shotgunKey = KeyCode.Alpha2;


    [Header("Graphics")]
    public GameObject bulletHoleGraphic;


    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowFireHold;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;


    private void Start()
    {
        shottyMesh.SetActive(false);
        EquipPistol();
        bulletsLeft = magSize;
        readyToShoot = true;
        currentWep.SetText("Pistol");
    }

    private void Update()
    {
        MyInput();

        ammoStatus.SetText(bulletsLeft + "/" + magSize);
    }

    private void MyInput()
    {
        if(allowFireHold)
        {
            shooting = Input.GetKey(shootKey);
        }
        else
        {
            shooting = Input.GetKeyDown(shootKey);
        }

        //Reload
        if(Input.GetKeyDown(reloadKey) && bulletsLeft < magSize && !reloading)
        {
            Reload();
        }
        
        //Shoot
        if(readyToShoot && shooting && !reloading && bulletsLeft >0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        //Swap Weapons
        if (Input.GetKeyDown(pistolKey) && !reloading)
        {
            EquipPistol();
            Reload();
        }

        if (Input.GetKeyDown(shotgunKey) && !reloading)
        {
            EquipShotgun();
            Reload();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        Vector3 direction = playerCam.transform.forward + new Vector3(x, y, z);


        //Raycast Out
        if(Physics.Raycast(playerCam.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if(rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }

        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

        if(bulletsLeft == 0)
        {
            Reload();
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }

    private void EquipPistol()
    {
        shottyMesh.SetActive(false);
        pistolMesh.SetActive(true);
        currentWep.SetText("Pistol");
        damage = 40;
        timeBetweenShooting = 0.1f;
        spread = 0.01f;
        range = 40;
        reloadTime = 1.5F;
        timeBetweenShots = 0.2f;
        magSize = 6;
        bulletsPerTap = 1;
    }

    private void EquipShotgun()
    {
        shottyMesh.SetActive(true);
        pistolMesh.SetActive(false);
        currentWep.SetText("Shotgun");
        damage = 20;
        timeBetweenShooting = 0.05f;
        spread = .1f;
        range = 20;
        reloadTime = 2;
        timeBetweenShots = 0f;
        magSize = 24;
        bulletsPerTap = 6;
    }
}
