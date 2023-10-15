using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private int health;
    [SerializeField] private int maxHealth = 10;
    //[SerializeField]  private UnityEngine.UI.Image healthBarImage;
    //[SerializeField] private Transform healthBarCanvas;
    [SerializeField] private Transform playerTransform;
    [SerializeField] Player player;
    private Animator animator;
    [SerializeField] private bool boss = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip mainSong;
    [SerializeField] private AudioClip takeDamageClip;
    private Rigidbody[] _ragdollRigidbodies;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    /*private void Update()
    {
        /* Vector3 dirToPlayer = (playerTransform.position - healthBarCanvas.transform.position).normalized;
        dirToPlayer = new Vector3(dirToPlayer.x, 0, dirToPlayer.z);
        Quaternion rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up);
        healthBarCanvas.rotation = rotation;
    }*/
    public void Damage(int damage)
    {
        health -= damage;
        animator.SetBool("TakeDamage", true);
        AudioSource.PlayClipAtPoint(takeDamageClip, transform.position);
        //healthBarImage.fillAmount = (float)health / (float)maxHealth;
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (boss)
        {
            audioSource.clip = mainSong;
            audioSource.Play();
        }
        player.SetHealth(4);
        if (gameObject.TryGetComponent(out DropKey dropKey))
        {
            dropKey.SpawnKey();
        }
        EnableRagdoll();
        StartCoroutine(DisableRagdollAfterTime());
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            if (rigidbody.gameObject.GetComponent<BoxCollider>() != null) rigidbody.gameObject.GetComponent<BoxCollider>().enabled = false;
            if (rigidbody.gameObject.GetComponent<CapsuleCollider>() != null) rigidbody.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            if (rigidbody.gameObject.GetComponent<SphereCollider>() != null) rigidbody.gameObject.GetComponent<SphereCollider>().enabled = false;
            rigidbody.isKinematic = true;
        }
    }

    public void EnableRagdoll()
    {
        if (transform.TryGetComponent(out CharacterController characterController))
        {
            characterController.enabled = false;
        }
        if (transform.TryGetComponent(out EnemyAI enemyAi))
        {
            enemyAi.enabled = false;
        }
        animator.enabled = false;
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            if (rigidbody.gameObject.GetComponent<BoxCollider>() != null) rigidbody.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (rigidbody.gameObject.GetComponent<CapsuleCollider>() != null) rigidbody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            if (rigidbody.gameObject.GetComponent<SphereCollider>() != null) rigidbody.gameObject.GetComponent<SphereCollider>().enabled = true;
            rigidbody.isKinematic = false;
        }
    }

    IEnumerator DisableRagdollAfterTime()
    {
        yield return new WaitForSeconds(10f);
        DisableRagdoll();
    }

    public int GetHealth()
    {
        return health;
    }
}
