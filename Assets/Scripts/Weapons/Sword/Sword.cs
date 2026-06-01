using UnityEngine;
using System;

public class Sword : MonoBehaviour
{

    [SerializeField] private int _damageAmount = 2;

    public event EventHandler OnSwordSwing;

    private PolygonCollider2D _PolygonCollider2D;

    private void Awake()
    {
        _PolygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        AttackColliderTurnOff();
        _damageAmount += GameManager.Instance.BonusDamage;
    }

    public void Attack() //метод атаки
    {
        AttackColliderTurnOffOn();
        OnSwordSwing?.Invoke(this, EventArgs.Empty); //вызов события удара
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(_damageAmount);
        }
    }
    public void AddDamage(int amount)
    {
        _damageAmount += amount;

        GameManager.Instance.BonusDamage += amount;
    }
    public void AttackColliderTurnOff()
    {
        _PolygonCollider2D.enabled = false;
    }

    private void AttackColliderTurnOn()
    {
        _PolygonCollider2D.enabled = true;
    }

    private void AttackColliderTurnOffOn()
    {
        AttackColliderTurnOff();
        AttackColliderTurnOn();
    }
}
