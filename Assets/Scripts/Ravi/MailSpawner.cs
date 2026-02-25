using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MailSorting.Data;
using MailSorting.UI;

public class MailSpawner : MonoBehaviour
{
    [Header("Day Config")]
    public DayConfig dayConfig;

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject packagePrefab;

    [Header("Spawn Points")]
    public Transform letterSpawnArea;
    public Transform packageSpawnArea;

    [Header("Spawn Scatter")]
    public Vector2 scatter = new Vector2(1f, 0.5f);

    [Header("References (assign later when inspection is ready)")]
    public Collider2D dropZoneCollider;
    public MailInspectionManager inspectionManager;

    [Header("Inspection Tools")]
    public Collider2D rulerCollider;
    public Collider2D weighingscaleCollider;
    public GameObject inspectButton;

    private List<Mail_Items_SO> spawnQueue = new List<Mail_Items_SO>();
    private int spawnedCount = 0;
    private bool waitingForSort = false;

    void Start()
    {
        BuildSpawnQueue();
        StartCoroutine(SpawnLoop());
    }

    //void BuildSpawnQueue()
    //{
    //    spawnQueue.Clear();

    //    if (dayConfig.guaranteedMail != null)
    //        foreach (var mail in dayConfig.guaranteedMail)
    //            if (mail != null) spawnQueue.Add(mail);

    //    int remaining = dayConfig.totalMailCount - spawnQueue.Count;
    //    for (int i = 0; i < remaining; i++)
    //    {
    //        Mail_Items_SO picked = PickRandom();
    //        if (picked != null) spawnQueue.Add(picked);
    //    }

    //    int guaranteedCount = dayConfig.guaranteedMail?.Count ?? 0;
    //    ShuffleFrom(spawnQueue, guaranteedCount);

    //    Debug.Log($"[Spawner] Day {dayConfig.dayNumber} — {spawnQueue.Count} items queued");
    //}



    void BuildSpawnQueue()
    {
        spawnQueue.Clear();
        if (dayConfig.guaranteedMail != null)
            foreach (var mail in dayConfig.guaranteedMail)
                if (mail != null) spawnQueue.Add(mail);

        int remaining = dayConfig.totalMailCount - spawnQueue.Count;
        for (int i = 0; i < remaining; i++)
        {
            Mail_Items_SO picked = PickRandom();
            if (picked != null) spawnQueue.Add(picked);
        }

        ShuffleComplete(spawnQueue);
        Debug.Log($"[Spawner] Day {dayConfig.dayNumber} — {spawnQueue.Count} items queued");
    }

    private void ShuffleComplete(List<Mail_Items_SO> spawnQueue)
    {
        for(int i = spawnQueue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            
            Mail_Items_SO temp = spawnQueue[i];
            spawnQueue[i] = spawnQueue[j];
            spawnQueue[j] = temp;
        }
    }

    Mail_Items_SO PickRandom()
    {
        if (dayConfig.randomPool == null || dayConfig.randomPool.Count == 0) return null;

        int totalWeight = 0;
        foreach (var entry in dayConfig.randomPool)
            totalWeight += entry.weight;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var entry in dayConfig.randomPool)
        {
            cumulative += entry.weight;
            if (roll < cumulative) return entry.mailData;
        }

        return dayConfig.randomPool[0].mailData;
    }

    void ShuffleFrom(List<Mail_Items_SO> list, int startIndex)
    {
        for (int i = list.Count - 1; i > startIndex; i--)
        {
            int j = Random.Range(startIndex, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        while (spawnedCount < spawnQueue.Count)
        {
            waitingForSort = true;
            SpawnMail(spawnQueue[spawnedCount]);
            spawnedCount++;

            yield return new WaitUntil(() => waitingForSort == false);
        }

        Debug.Log("[Spawner] All mail delivered for today.");
        if (DayEndPanel.Instance != null)
            DayEndPanel.Instance.Show();
        else
            Debug.LogError("[Spawner] DayEndPanel.Instance is null!");
    }
    public void OnMailSorted()
    {
        waitingForSort = false;
    }

    void SpawnMail(Mail_Items_SO data)
    {
        bool isPackage = data.mailType == MailType.Package;

        Transform spawnArea = isPackage ? packageSpawnArea : letterSpawnArea;
        GameObject prefab = isPackage ? packagePrefab : letterPrefab;

        if (spawnArea == null || prefab == null)
        {
            Debug.LogWarning($"[Spawner] Missing spawn area or prefab for {data.mailID}");
            return;
        }

        Vector3 spawnPos = spawnArea.position + new Vector3(
            Random.Range(-scatter.x, scatter.x),
            Random.Range(-scatter.y, scatter.y),
            0f
        );

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        DragCheck drag = obj.GetComponent<DragCheck>();
        if (drag != null)
        {
            drag.mailData = data;
            drag.rulerCollider = rulerCollider;
            drag.weighingscaleCollider = weighingscaleCollider;
            drag.inspectButton = inspectButton;
            if (dropZoneCollider != null) drag.dropZoneCollider = dropZoneCollider;
            if (inspectionManager != null) drag.SetInspectionManager(inspectionManager);
        }

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null && data.mailSprite != null)
            sr.sprite = data.mailSprite;
    }
}