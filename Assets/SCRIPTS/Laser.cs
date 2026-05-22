using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Kecepatan")]
    public float speed = 15f;

    private Transform targetTransform;
    private EnemyAI enemyTarget;
    private BossEnemy bossTarget;
    private bool hasTarget = false;

    public void SetTarget(EnemyAI targetEnemy)
    {
        enemyTarget = targetEnemy;
        targetTransform = targetEnemy.transform;
        hasTarget = true;
    }

    public void SetTarget(BossEnemy targetBoss)
    {
        bossTarget = targetBoss;
        targetTransform = targetBoss.transform;
        hasTarget = true;
    }

    void Update()
    {
        if (!hasTarget || targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // Kejar posisi target
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate laser menghadap arah geraknya
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Cek apakah sudah sampai ke target
        float dist = Vector3.Distance(transform.position, targetTransform.position);
        if (dist < 0.3f)
        {
            if (enemyTarget != null) enemyTarget.OnCorrectAnswer();
            else if (bossTarget != null) bossTarget.OnCorrectAnswer();
            Destroy(gameObject);
        }
    }
}