using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;
    public event EventHandler OnEnemyDeath;

    //[SerializeField] private int _maxHealth;
    private int _currentHealth;
    private int _maxHealth;

    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _maxHealth = _enemySO.enemyHealth;
        _currentHealth = _maxHealth;
    }

    public void ScaleHealth(float multiplier)
    {
        _maxHealth = Mathf.RoundToInt(_enemySO.enemyHealth * multiplier);
        _currentHealth = _maxHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player))
        {
            player.TakeDamage(transform, _enemySO.enemyDamageAmount);
        }
    }

    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            _boxCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;

            _enemyAI.SetDeathState();

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.simulated = false;
            }

            OnDeath?.Invoke(this, EventArgs.Empty);
            OnEnemyDeath?.Invoke(this, EventArgs.Empty);
        }
    }

}
