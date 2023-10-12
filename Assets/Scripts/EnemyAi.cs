using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform punchPoint;

    [Header("Detection and Movement")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Melee Damage")]
    [SerializeField] private int meleeDamage = 2;
    [SerializeField] private float waitTimeToDamage = 0.8f;

    [Header("Patrol")]
    public Vector3 patrolAreaCenter;
    public Vector3 patrolAreaSize;
    public int numberOfPatrolPoints = 3;

    private CharacterController characterController;
    private Animator animator;
    private Vector3[] patrolPoints;
    private int currentPatrolPoint = 0;

    private Vector3 velocity;
    private bool startCountingPunchRate = false;
    private float counterTime = 0;
    private bool playerSeen = false;
    [SerializeField] private float waitTimeUntilNextPunch = 3f;
    [SerializeField] bool Boss = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource BossAudioSource;
    [SerializeField] private AudioSource EnemyAudioSource;
    [SerializeField] private AudioClip enemySeeClip;
    [SerializeField] private AudioClip bossClip;
    [SerializeField] private AudioClip bossStartingWords;
    [SerializeField] private AudioClip bossLaughClip;
    private bool enemySeeAdded = false;
    private bool bossSongAdded = false;

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

    private void Update()
    {
        if (animator.GetBool("TakeDamage"))
        {
            playerSeen = true;
            StartCoroutine(SetPlayerSeenToFalseAfterTime(5f));
        }
        velocity.y = -2f;
        characterController.Move(velocity * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(punchPoint.position, player.position);

        if (distanceToPlayer <= detectionRange || playerSeen)
        {
            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            
            // PLAY AUDIO, DOCTOR STARTS FOLLOWING YOU
            // IF BOSS PLAY
            if (Boss && !bossSongAdded)
            {
                bossSongAdded = true;
                audioSource.clip = bossClip;
                audioSource.Play();
                BossAudioSource.Play();
                StartCoroutine(BossLaughPlay());
            }
            else if (!enemySeeAdded) 
            {
                enemySeeAdded = true;
                EnemyAudioSource.clip = enemySeeClip;
                EnemyAudioSource.Play();
            }

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
            enemySeeAdded = false;
        }
    }

    IEnumerator BossLaughPlay()
    {
        yield return new WaitForSeconds(14f);
        BossAudioSource.clip = bossLaughClip;
        BossAudioSource.Play();
    }

    IEnumerator SetPlayerSeenToFalseAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        playerSeen = false;
    }

    private void Move()
    {
        characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsWalking", false);
        animator.SetBool("TakeDamage", false);
    }

    private void Attack()
    {
        if (animator.GetBool("TakeDamage"))
        {
            counterTime = 0;
        }
        if (startCountingPunchRate) counterTime += Time.deltaTime;

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
        animator.SetBool("TakeDamage", false);
    }

    private IEnumerator DoDamageToPlayer(Collider hit)
    {
        yield return new WaitForSeconds(waitTimeToDamage);

        if (hit.gameObject.TryGetComponent(out Player player))
        {
            player.TakeDamage(meleeDamage);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        characterController.Move(transform.forward * patrolSpeed * Time.deltaTime);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, targetPatrolPoint);
        if (distanceToPatrolPoint < 1f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("TakeDamage", false);
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