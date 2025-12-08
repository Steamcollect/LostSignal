using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_EnemyCatalog", menuName = "SSO/EnemyCatalog")]
public class SSO_EnemyCatalog : ScriptableObject
{
    private static SSO_EnemyCatalog s_Instance;

    public static SSO_EnemyCatalog Instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance = Resources.Load<SSO_EnemyCatalog>("SSO_EnemyCatalog");

                if(s_Instance == null)
                {
                    Debug.LogError("FATAL: Impossible de trouver 'SSO_EnemyCatalog' dans un dossier Resources !");
                }
            }

            return s_Instance;
        }
    }

    [System.Serializable]
    public struct EnemyEntry
    {
        public string Name;
        public EntityController Prefab;
    }

    [Title("CONFIGURATION")]
    [TableList]
    [SerializeField] private List<EnemyEntry> m_Entries;

    public static IEnumerable<ValueDropdownItem<EntityController>> GetDirectPrefabDropdown()
    {
        ValueDropdownList<EntityController> list = new ValueDropdownList<EntityController>();

        list.Add("Empty", null);

        if (Instance != null)
        {
            foreach (var entry in Instance.m_Entries)
            {
                if (entry.Prefab != null)
                    list.Add(entry.Name, entry.Prefab);
            }
        }

        return list;
    }
}