using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } //��������� ������ �� ������
    public event EventHandler OnPlayerDeath; //������� ������ ������
    public event EventHandler OnFlashBlink;

    [SerializeField] private float movingSpeed = 6f; //�������� ������������ ������
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    [Space(height:20)]
    [SerializeField] private int dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float dashCoolDownTime = 0.5f;

    Vector2 inputVector;

    private Rigidbody2D rb;
    private KnockBack _knockBack;

    private float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;

    private int _currentHealth; //������� ���-�� ��������
    private bool _canTakeDamage;
    private bool _IsAlive;
    private bool _isDashing;
    private float _initialMovingSpeed;

    private Camera _mainCamera;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();

        _mainCamera = Camera.main;

        _initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        // Включаем управление при старте игровой сцены
        if (GameInput.Instance != null)
        {
            GameInput.Instance.EnableMovement();
        }

        if (GameManager.Instance.CurrentHealth > 0)
        {
            maxHealth = GameManager.Instance.MaxHealth;
            _currentHealth = GameManager.Instance.CurrentHealth;
        }
        else
        {
            _currentHealth = maxHealth;

            GameManager.Instance.MaxHealth = maxHealth;
            GameManager.Instance.CurrentHealth = _currentHealth;
        }
        movingSpeed += GameManager.Instance.BonusSpeed;
        _canTakeDamage = true;
        _IsAlive = true;

        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash += Player_OnPlayerDash;
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockedBack)
            return;

        HandleMovement();
    }

    public bool IsAlive() => _IsAlive; //�������� ��� �� �����
    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(Transform damageSource, int damage) //����� ��������� �����
    {
        if (_canTakeDamage && _IsAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            GameManager.Instance.CurrentHealth = _currentHealth;
            Debug.Log(_currentHealth);
            _knockBack.GetKnockedBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    private void DetectDeath()
    {
        if (_currentHealth == 0 && _IsAlive)
        {
            _IsAlive = false;
            _knockBack.StopKnockBackMovement();
            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Player_OnPlayerDash(object sendler, EventArgs e)
    {
        Dash();
    }

    private void Dash()
    {
        if (!_isDashing)
            StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        movingSpeed *= dashSpeed;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);

        trailRenderer.emitting = false;
        movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(dashCoolDownTime);
        _isDashing = false;
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void HandleMovement()
    {
        inputVector = inputVector.normalized;
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > _minMovingSpeed || Mathf.Abs(inputVector.y) > _minMovingSpeed) {
            _isRunning = true;
        } else {
            _isRunning = false;
        }
    }

    public void AddSpeed(float amount)
    {
        movingSpeed += amount;

        GameManager.Instance.BonusSpeed += amount;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

    public void SavePlayerData()
    {
        GameManager.Instance.CurrentHealth = _currentHealth;
        GameManager.Instance.MaxHealth = maxHealth;
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack -= Player_OnPlayerAttack;
            GameInput.Instance.OnPlayerDash -= Player_OnPlayerDash;
        }
    }
}
