using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody[] _ragdollRigidbodies;
    Animator animator;
    PlayerInput playerInput;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private TextMeshProUGUI soulsText;
    [SerializeField] private Image soulsMeterImage;
    private Color lowHealthColor = new Color(155, 0, 0, 255);
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs
    {
        public int health;
        public int damage;
        public int maxHealth;
    }
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();

        health = maxHealth;
    }

    private void Update()
    {
        soulsText.text = health.ToString();
        soulsMeterImage.fillAmount = Mathf.Lerp(soulsMeterImage.fillAmount, (float)health / (float)maxHealth, 3f * Time.deltaTime);
        if (((float)health / (float)maxHealth) < 0.35f)
        {
            soulsText.color = lowHealthColor;
            soulsMeterImage.color = lowHealthColor;
        }
        else
        {
            soulsText.color = Color.white;
            soulsMeterImage.color = Color.white;
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            health = health,
            damage = damage,
            maxHealth = maxHealth
        });

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator SpawnPlayerAfterTime()
    {
        yield return new WaitForSeconds(spawnTime);
        Review();
    }

    private void Die()
    {
        playerInput.enabled = false;
        EnableRagdoll();
    }

    private void Review()
    {
        transform.position = spawnPoint.transform.position;
        playerInput.enabled = true;
        DisableRagdoll();
        health = maxHealth;
        TakeDamage(0);
    }


    public void DisableRagdoll()
    {
        playerInput.enabled = true;
        animator.enabled = true;
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
        animator.enabled = false;
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            if (rigidbody.gameObject.GetComponent<BoxCollider>() != null) rigidbody.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (rigidbody.gameObject.GetComponent<CapsuleCollider>() != null) rigidbody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            if (rigidbody.gameObject.GetComponent<SphereCollider>() != null) rigidbody.gameObject.GetComponent<SphereCollider>().enabled = true;
            rigidbody.isKinematic = false;
        }
    }

    public void SetHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public int GetHealth()
    {
        return health;
    }

    public void MinusHealth()
    {
        health -= 1;
    }
}
