using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

public class Chest : MonoBehaviour
{
    [SerializeField] private ItemData[] possibleItems;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float itemVisibleDuration = 2f; // Время отображения предмета

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private bool playerInside = false;
    private bool isOpened = false;

    private void Start()
    {
        GameInput.Instance.OnInteract += GameInput_OnInteract;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= GameInput_OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            playerInside = false;
        }
    }

    private void GameInput_OnInteract(object sender, EventArgs e)
    {
        if (playerInside && !isOpened)
        {
            OpenChest();
        }
    }

    private void OpenChest() //открытие сундука
    {
        isOpened = true;
        animator.SetTrigger("Open");

        ItemData selectedItem = GetRandomItem();

        // Сразу применяем эффект к игроку
        ApplyItem(selectedItem);

        // Запускаем визуальное отображение предмета
        StartCoroutine(ShowItemVisual(selectedItem));
    }

    private ItemData GetRandomItem()
    {
        float totalWeight = 0f;

        foreach (var item in possibleItems)
            totalWeight += item.dropChance;

        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        float current = 0f;
        foreach (var item in possibleItems)
        {
            current += item.dropChance;
            if (randomPoint <= current)
                return item;
        }

        return possibleItems[0];
    }

    private IEnumerator ShowItemVisual(ItemData item)
    {
        // Создаем визуальный объект предмета
        GameObject itemVisual = Instantiate(itemPrefab, dropPoint.position, Quaternion.identity);

        // Настраиваем спрайт
        SpriteRenderer renderer = itemVisual.GetComponent<SpriteRenderer>();
        if (renderer != null && item.icon != null)
            renderer.sprite = item.icon;

        // Просто ждем и уничтожаем
        yield return new WaitForSeconds(itemVisibleDuration);

        // Уничтожаем объект
        if (itemVisual != null)
            Destroy(itemVisual);
    }

    private void ApplyItem(ItemData item)
    {
        switch (item.itemType)
        {
            case ItemType.SpeedBoost:
                Player.Instance.AddSpeed(item.value);
                Debug.Log($"Скорость увеличена на {item.value}");
                break;

            case ItemType.DamageBoost:
                var weapon = ActiveWeapon.Instance.GetActiveWeapon();
                if (weapon is Sword sword)
                {
                    sword.AddDamage((int)item.value);
                    Debug.Log($"Урон увеличен на {item.value}");
                }
                break;
        }
    }
}