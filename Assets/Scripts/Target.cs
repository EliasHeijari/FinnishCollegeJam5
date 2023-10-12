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

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
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
        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return health;
    }
}
