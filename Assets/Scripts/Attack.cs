using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Attack : MonoBehaviour
{
    private Animator animator;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Player player;

    [Header("Punch Attack")]
    [SerializeField] private Transform punchPoint;
    [SerializeField] private float punchRadius = 3f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float punchTimeRate = 3f;
    [SerializeField] private int punchDamage = 3;
    [Space(10)]

    [Header("Shoot Attack")]
    [SerializeField] private bool haveGun = false;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int gunDamage = 6;
    [SerializeField] private float fireRate = 60f;
    [SerializeField] private float waitToDamage = 1f;
    private float nextTimeToFire = 0;

    [Header("Shooting Particles")]
    [SerializeField] private GameObject impactBodyEffect;
    [SerializeField] private GameObject airTrace;
    [SerializeField] private float trailTime = 0.3f;
    [SerializeField] private float spawnTrailTime = 0.3f;

    [Header("Damage UI")]
    [SerializeField] private GameObject canSpellImageObject;
    [SerializeField] private GameObject crosshairImageObject;
    [SerializeField] private GameObject meleeIcon;
    private UnityEngine.UI.Image canSpellImage;
    private UnityEngine.UI.Image crosshairImage;

    private float currentTime;
    private bool startCountingPunchRate = false;
    private float elapsedTime = 0f;
    [SerializeField] private float waitTimeRotateToTarget = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        player = GetComponent<Player>();
        canSpellImage = canSpellImageObject.GetComponent<UnityEngine.UI.Image>();
        crosshairImage = crosshairImageObject.GetComponent<UnityEngine.UI.Image>();
    }

    private void Update()
    {
        SwitchMelee();
    }

    private void SwitchMelee()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            haveGun = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            haveGun = false;
        }
        if (!haveGun)
        {
            CheckPunch();
            canSpellImageObject.SetActive(false);
            meleeIcon.SetActive(true);
        }
        else
        {
            CheckShoot();
            canSpellImageObject.SetActive(true);
            meleeIcon.SetActive(false);
        }
    }

    private void CheckShoot()
    {
        if (Time.time > nextTimeToFire)
        {
            canSpellImage.color = new Color32(255, 255, 225, 255);
            crosshairImage.color = new Color32(255, 255, 225, 255);
        }
        else
        {
            canSpellImage.color = new Color32(108, 108, 108, 255);
            crosshairImage.color = new Color32(0, 0, 0, 255);
        }
        if (starterAssetsInputs.attack && Time.time > nextTimeToFire && thirdPersonController.Grounded && player.GetHealth() > 1)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            animator.SetBool("Shoot", true);
            StartCoroutine(ShootAttack());
        }
        else
        {
            animator.SetBool("Shoot", false);
        }
    }

    IEnumerator PunchAttack()
    {
        float waitAnimationToHit = 1f;
        yield return new WaitForSeconds(waitAnimationToHit);
        // Do Damage to Target

        Collider[] hits = Physics.OverlapSphere(punchPoint.transform.position, punchRadius, targetLayer);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Target target))
            {
                target.Damage(punchDamage);
            }
        }
    }

    IEnumerator ShootAttack()
    {
        yield return new WaitForSeconds(0.07f);
        // Do Damage to Target
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        float shootingDistance = 500f;
        string[] layerNames = { "Default", "Target", "Water" };
        if (Physics.Raycast(ray, out RaycastHit hit, shootingDistance, LayerMask.GetMask(layerNames)))
        {
            Vector3 dirToObject = (hit.transform.position - transform.position).normalized;
            Quaternion TargetRotation = Quaternion.LookRotation(dirToObject, Vector3.up);
            StartCoroutine(RotateToTarget(TargetRotation));
            ShootAirTracer(hit);
            if (hit.transform.TryGetComponent(out Target target))
            {
                player.MinusHealth();
                StartCoroutine(DamageTarget(target, hit));
            }
        }
    }

    IEnumerator DamageTarget(Target target, RaycastHit hit)
    {
        yield return new WaitForSeconds(waitToDamage);
        // target's health - gunDamage
        target.Damage(gunDamage);

        //play impact effect to hit point
        //GameObject impactPlayerEffect = Instantiate(impactBodyEffect, hit.point, Quaternion.LookRotation(hit.normal));
        //Destroy(impactPlayerEffect, 3f);

        // Hit mark icon
        //CreateHitMark(gunDamage, hit.transform);
    }

    IEnumerator RotateToTarget(Quaternion targetRotation)
    {
        while (elapsedTime < waitTimeRotateToTarget)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (elapsedTime / waitTimeRotateToTarget));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.rotation = targetRotation;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchPoint.transform.position, punchRadius);
        Gizmos.DrawRay(ray);
    }

    private void CheckPunch()
    {
        if (startCountingPunchRate) currentTime += Time.deltaTime;
        if (currentTime >= punchTimeRate) startCountingPunchRate = false;
        if (starterAssetsInputs.attack && !startCountingPunchRate)
        {
            animator.SetBool("Punch", true);
            StartCoroutine(PunchAttack());
            startCountingPunchRate = true;
            currentTime = 0;
        }
        else
        {
            animator.SetBool("Punch", false);
        }
    }

    private IEnumerator SpawnTrail(GameObject traceObject, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = traceObject.transform.position;

        while (time < spawnTrailTime)
        {
            traceObject.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime * trailTime;
            yield return null;
        }
        traceObject.transform.position = hit.point;
        Destroy(traceObject, 2f);
    }

    private void ShootAirTracer(RaycastHit hit)
    {
        GameObject airTraceGameObject = Instantiate(airTrace, firePoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(airTraceGameObject, hit));
    }
}
