using UnityEngine;
using System.Collections.Generic;
using MailSorting.Data;

public class MailDatabase : MonoBehaviour
{
    // 1. The Singleton Instance
    public static MailDatabase Instance { get; private set; }

    [Header("Master Mail Vault")]
    [Tooltip("Drag EVERY random filler Mail_SO in your project into this list.")]
    public List<Mail_Items_SO> masterMailPool = new List<Mail_Items_SO>();

    void Awake()
    {
        // 2. Make it Persistent
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Survives scene loads
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates if returning to the Main Menu
        }
    }

    // 3. A helper method for the Spawner to use later
    public Mail_Items_SO GetRandomMail()
    {
        if (masterMailPool.Count == 0)
        {
            Debug.LogError("[MailDatabase] The master pool is empty! Cannot draw mail.");
            return null;
        }

        int randomIndex = Random.Range(0, masterMailPool.Count);
        return masterMailPool[randomIndex];
    }
}