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

    [Header("References")]
    public Collider2D dropZoneCollider;
    public MailInspectionManager inspectionManager;
    public Collider2D rulerCollider;
    public Collider2D weighingscaleCollider;
    public GameObject inspectButton;

    [Header("Day 1 Tutorial")]
    public bool isTutorialDay = false;

    private List<Mail_Items_SO> tutorialQueue = new List<Mail_Items_SO>();
    private List<Mail_Items_SO> mainQueue = new List<Mail_Items_SO>();
    private List<Mail_Items_SO> spawnedRandomToday = new List<Mail_Items_SO>();

    private bool waitingForSort = false;
    private bool inTutorial = false;

    void Start()
    {
        BuildQueues();
        StartCoroutine(SpawnLoop());
    }

    void BuildQueues()
    {
        tutorialQueue.Clear();
        mainQueue.Clear();
        spawnedRandomToday.Clear();

        // tutorial queue (Day 1 only, fixed order)
        if (isTutorialDay)
        {
            if (dayConfig.tutorialAccept != null) tutorialQueue.Add(dayConfig.tutorialAccept);
            if (dayConfig.tutorialReply != null) tutorialQueue.Add(dayConfig.tutorialReply);
            if (dayConfig.tutorialReject != null) tutorialQueue.Add(dayConfig.tutorialReject);
            if (dayConfig.tutorialReport != null) tutorialQueue.Add(dayConfig.tutorialReport);
        }

        // guaranteed mails
        List<Mail_Items_SO> guaranteed = new List<Mail_Items_SO>();
        if (dayConfig.characterAMail != null) guaranteed.Add(dayConfig.characterAMail);
        if (dayConfig.characterBMail != null) guaranteed.Add(dayConfig.characterBMail);
        if (dayConfig.characterCMail != null) guaranteed.Add(dayConfig.characterCMail);
        if (dayConfig.generalGuaranteedMail != null) guaranteed.Add(dayConfig.generalGuaranteedMail);

        // track guaranteed for end of day
        GameManager.Instance.spawnedGuaranteedToday.AddRange(guaranteed);

        // random pool mails
        List<Mail_Items_SO> random = new List<Mail_Items_SO>();

        // add carried over mails from previous day first
        if (GameManager.Instance.carriedOverMail != null)
            random.AddRange(GameManager.Instance.carriedOverMail);

        // add this day's random pool
        foreach (var entry in dayConfig.randomPool)
            if (entry.mailData != null)
                random.Add(entry.mailData);

        // shuffle guaranteed + random together into main queue
        mainQueue.AddRange(guaranteed);
        mainQueue.AddRange(random);
        Shuffle(mainQueue);

        // track random mails for carry over check at end of day
        spawnedRandomToday.AddRange(random);
        Debug.Log($"[Spawner] spawnedRandomToday count: {spawnedRandomToday.Count}");
        Debug.Log($"[Spawner] Day {dayConfig.dayNumber} — Tutorial: {tutorialQueue.Count}, Main: {mainQueue.Count}");
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        // tutorial first if Day 1
        if (isTutorialDay)
        {
            inTutorial = true;
            foreach (var mail in tutorialQueue)
            {
                waitingForSort = true;
                SpawnMail(mail, isTutorial: true);
                yield return new WaitUntil(() => waitingForSort == false);
            }
            inTutorial = false;
            Debug.Log("[Spawner] Tutorial complete, starting main game.");
        }

        // main queue
        foreach (var mail in mainQueue)
        {
            waitingForSort = true;
            SpawnMail(mail, isTutorial: false);
            yield return new WaitUntil(() => waitingForSort == false);
        }

        Debug.Log("[Spawner] All mail delivered for today.");

        // notify game manager of day end
        GameManager.Instance.OnDayEnd(spawnedRandomToday);

        if (DayEndPanel.Instance != null)
            DayEndPanel.Instance.Show();
        else
            Debug.LogError("[Spawner] DayEndPanel.Instance is null!");
    }

    public void OnMailSorted()
    {
        waitingForSort = false;
    }

    public bool IsInTutorial() => inTutorial;

    void SpawnMail(Mail_Items_SO data, bool isTutorial)
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
            drag.isTutorialMail = isTutorial;
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

    void Shuffle(List<Mail_Items_SO> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void NotifyDayEnd()
    {
        GameManager.Instance.OnDayEnd(spawnedRandomToday);
    }
}

