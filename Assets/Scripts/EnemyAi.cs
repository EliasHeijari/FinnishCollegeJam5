using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform punchPoint;
    [SerializeField] private GameObject hitIcon;

    [Header("Detection and Movement")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Melee Damage")]
    [SerializeField] private int minMeleeDamage = 0;
    [SerializeField] private int maxMeleeDamage = 3;

    [Header("Patrol")]
    public Vector3 patrolAreaCenter;
    public Vector3 patrolAreaSize;
    public int numberOfPatrolPoints = 3;

    private CharacterController characterController;
    private Animator animator;
    private Vector3[] patrolPoints;
    private int currentPatrolPoint = 0;

    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool startCountingPunchRate = false;
    private float counterTime = 0;
    [SerializeField] private float waitTimeUntilNextPunch = 3f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        patrolPoints = new Vector3[numberOfPatrolPoints];

        for (int i = 0; i < numberOfPatrolPoints; i++)
        {
            float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
            float randomZ = Random.Range(-patrolAreaSize.z / 2, patrolAreaSize.z / 2);
            patrolPoints[i] = patrolAreaCenter + new Vector3(randomX, 0f, randomZ);
        }
    }

    private void FixedUpdate()
    {
        velocity.y += gravity * Time.fixedDeltaTime;
        characterController.Move(velocity * Time.fixedDeltaTime);

        float distanceToPlayer = Vector3.Distance(punchPoint.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                Move();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsWalking", false);
    }

    private void Attack()
    {
        if (startCountingPunchRate) counterTime += Time.fixedDeltaTime;

        if (counterTime >= waitTimeUntilNextPunch)
        {
            startCountingPunchRate = false;
        }

        if (!startCountingPunchRate)
        {
            Collider[] hits = Physics.OverlapSphere(punchPoint.position, attackRange, playerLayer);
            foreach (Collider hit in hits)
            {
                StartCoroutine(DoDamageToPlayer(hit));
            }

            startCountingPunchRate = true;
            counterTime = 0;
        }

        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    private IEnumerator DoDamageToPlayer(Collider hit)
    {
        yield return new WaitForSeconds(1.2f);

        if (hit.gameObject.TryGetComponent(out Player player))
        {
            int meleeDamage = Random.Range(minMeleeDamage, maxMeleeDamage);
            player.TakeDamage(meleeDamage);
            CreateHitMark(meleeDamage, hit.transform);
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        Vector3 targetPatrolPoint = patrolPoints[currentPatrolPoint];
        Vector3 direction = targetPatrolPoint - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        transform.Translate(Vector3.forward * patrolSpeed * Time.fixedDeltaTime);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, targetPatrolPoint);
        if (distanceToPatrolPoint < 1f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsRunning", false);
    }

    private void CreateHitMark(int damage, Transform hit)
    {
        Vector3 dirToPlayer = (hit.position - transform.position).normalized;
        Vector3 damageIconPos = hit.position + dirToPlayer + Vector3.up;
        dirToPlayer += Vector3.up;
        Quaternion rotation = Quaternion.LookRotation(-dirToPlayer, Vector3.up);
        Vector3 offsetMark = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.3f), -0.3f);
        GameObject damageIcon = Instantiate(hitIcon, damageIconPos + offsetMark, rotation);

        GameObject damageCountGO = damageIcon.transform.GetChild(1).gameObject;
        TextMeshProUGUI textMeshCount = damageCountGO.GetComponent<TextMeshProUGUI>();
        if (textMeshCount != null)
        {
            textMeshCount.text = damage.ToString("D2");
        }

        GameObject damageCountShadowGO = damageIcon.transform.GetChild(2).gameObject;
        TextMeshProUGUI textMeshShadow = damageCountShadowGO.GetComponent<TextMeshProUGUI>();
        if (textMeshShadow != null)
        {
            textMeshShadow.text = damage.ToString("D2");
        }

        Destroy(damageIcon, 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchPoint.position, attackRange);

        Gizmos.color = Color.green;
        Vector3 min = patrolAreaCenter - patrolAreaSize / 2;
        Vector3 max = patrolAreaCenter + patrolAreaSize / 2;
        Gizmos.DrawWireCube(patrolAreaCenter, patrolAreaSize);

        Gizmos.color = Color.blue;
        if (patrolPoints != null)
        {
            foreach (Vector3 point in patrolPoints)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(patrolAreaCenter, 0.5f);

        Gizmos.color = Color.cyan;
        Vector3 p1 = new Vector3(min.x, 0f, min.z);
        Vector3 p2 = new Vector3(max.x, 0f, min.z);
        Vector3 p3 = new Vector3(max.x, 0f, max.z);
        Vector3 p4 = new Vector3(min.x, 0f, max.z);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}