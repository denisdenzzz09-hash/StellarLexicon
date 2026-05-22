using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [Header("Tembakan")]
    public GameObject laserPrefab;
    public Transform firePoint;

    [Header("Rotasi")]
    public float rotationSpeed = 10f;

    private Transform currentTarget;

    void Update()
    {
        UpdateTarget();
        RotateToTarget();
    }

    void UpdateTarget()
    {
        Transform closest = null;
        float minX = float.MaxValue;

        // Cek enemy biasa
        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            if (enemy.transform.position.x < minX)
            {
                minX = enemy.transform.position.x;
                closest = enemy.transform;
            }
        }

        // Cek boss
        BossEnemy boss = FindFirstObjectByType<BossEnemy>();
        if (boss != null && boss.transform.position.x < minX)
            closest = boss.transform;

        currentTarget = closest;
    }

    void RotateToTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Shoot ke enemy biasa
    public void ShootAt(EnemyAI target)
    {
        if (laserPrefab == null || firePoint == null) return;

        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        Laser laserScript = laser.GetComponent<Laser>();
        if (laserScript != null)
            laserScript.SetTarget(target); // ← kirim target langsung, bukan .transform
    }

    // Shoot ke boss
    public void ShootAt(BossEnemy target)
    {
        if (laserPrefab == null || firePoint == null) return;

        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        Laser laserScript = laser.GetComponent<Laser>();
        if (laserScript != null)
            laserScript.SetTarget(target); // ← kirim target langsung, bukan .transform
    }

    public Transform GetCurrentTarget() => currentTarget;
}