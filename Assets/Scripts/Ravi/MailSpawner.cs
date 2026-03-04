using MailSorting.Data;
using MailSorting.Gameplay;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MailSpawner : MonoBehaviour
{
    [Header("Day Config")]
    public DayConfig_SO dayConfig;

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject packagePrefab;

    [Header("Spawn Parent (Canvas)")]
    public RectTransform spawnParent; // drag your Mail Canvas here

    [Header("Spawn Scatter")]
    public Vector2 scatter = new Vector2(50f, 30f); // canvas units

    [Header("Day 1 Tutorial")]
    public bool isTutorialDay = false;
    public List<Mail_SO> tutorialMailOrder = new List<Mail_SO>(); // drag 4 tutorial mails in order

    private List<Mail_SO> mainQueue = new List<Mail_SO>();
    private List<Mail_SO> spawnedRandomToday = new List<Mail_SO>();

    private bool waitingForSort = false;
    private bool inTutorial = false;

    private GameObject currentMailObject;

    void Start()
    {
        BuildQueues();
        StartCoroutine(SpawnLoop());
    }

    void BuildQueues()
    {
        mainQueue.Clear();
        spawnedRandomToday.Clear();

        // guaranteed mails from DayConfig_SO
        List<Mail_SO> guaranteed = new List<Mail_SO>();
        if (dayConfig.guaranteedPool != null)
        {
            foreach (var entry in dayConfig.guaranteedPool)
                if (entry.mailData != null) guaranteed.Add(entry.mailData);
        }

        // random pool — carried over + today's random
        List<Mail_SO> random = new List<Mail_SO>();

        if (GameManager.Instance?.carriedOverMail != null)
            random.AddRange(GameManager.Instance.carriedOverMail);

        // for now random pool comes from GameManager's global pool
        // designers will fill this later per day
        if (GameManager.Instance?.globalRandomPool != null)
            foreach (var mail in GameManager.Instance.globalRandomPool)
                if (mail != null) random.Add(mail);

        // shuffle guaranteed + random together
        mainQueue.AddRange(guaranteed);
        mainQueue.AddRange(random);
        Shuffle(mainQueue);

        // track random for carry over
        spawnedRandomToday.AddRange(random);

        Debug.Log($"[Spawner] Day {dayConfig.dayNumber} — Tutorial: {tutorialMailOrder.Count}, Main: {mainQueue.Count}");
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        // tutorial first if Day 1
        if (isTutorialDay && tutorialMailOrder.Count > 0)
        {
            inTutorial = true;
            foreach (var mail in tutorialMailOrder)
            {
                waitingForSort = true;
                SpawnMail(mail, isTutorial: true);
                yield return new WaitUntil(() => waitingForSort == false);
            }
            inTutorial = false;
            Debug.Log("[Spawner] Tutorial complete!");
        }

        // main queue
        foreach (var mail in mainQueue)
        {
            waitingForSort = true;
            SpawnMail(mail, isTutorial: false);
            yield return new WaitUntil(() => waitingForSort == false);
        }

        Debug.Log("[Spawner] All mail delivered for today.");
        GameManager.Instance?.OnDayEnd(spawnedRandomToday);

        if (DayEndPanel.Instance != null)
            DayEndPanel.Instance.Show();
        else
            Debug.LogError("[Spawner] DayEndPanel.Instance is null!");
    }

    public void OnMailSorted()
    {
        if (currentMailObject != null)
        {
            Destroy(currentMailObject);
            currentMailObject = null;
        }
        waitingForSort = false;
    }

    public bool IsInTutorial() => inTutorial;

    void SpawnMail(Mail_SO data, bool isTutorial)
    {
        bool isPackage = data.mailType == MailType.Package;
        GameObject prefab = isPackage ? packagePrefab : letterPrefab;

        if (prefab == null || spawnParent == null)
        {
            Debug.LogWarning($"[Spawner] Missing prefab or spawn parent for {data.senderName}");
            return;
        }

        GameObject obj = Instantiate(prefab, spawnParent);

        // random scatter position in canvas space
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(
                Random.Range(-scatter.x, scatter.x),
                Random.Range(-scatter.y, scatter.y)
            );
        }

        // inject mail data
        UI_DragCheck drag = obj.GetComponent<UI_DragCheck>();
        if (drag != null)
        {
            drag.mailData = data;
            drag.isTutorialMail = isTutorial;
            drag.spawner = this;
        }

        // inject into mail view controller for visuals
        //MailViewController mvc = obj.GetComponent<MailViewController>();
        //if (mvc != null)
            //mvc.Setup(data);

        currentMailObject = obj;
    }

    void Shuffle(List<Mail_SO> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void NotifyDayEnd()
    {
        GameManager.Instance?.OnDayEnd(spawnedRandomToday);
    }
}