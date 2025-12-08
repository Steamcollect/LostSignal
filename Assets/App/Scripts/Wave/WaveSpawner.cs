using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct WaveContent
    {
        [HideLabel]
        [ValueDropdown("@SSO_EnemyCatalog.GetDirectPrefabDropdown()")]
        [HideReferenceObjectPicker]
        public EntityController EnemyPrefab;
    }

    [Title("CONFIGURATION")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [SerializeField] private List<WaveContent> m_Entries = new List<WaveContent>();

    public int ConfiguredWaveCount => m_Entries.Count;

    public void SpawnWave(int waveIndex, System.Action<EntityController> onSpawnCallback)
    {
        if (waveIndex < 0 || waveIndex >= m_Entries.Count) return;

        WaveContent content = m_Entries[waveIndex];
        if (content.EnemyPrefab != null)
        {
            EntityController entity = Instantiate(content.EnemyPrefab, transform.position, transform.rotation);

            if(entity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();

            onSpawnCallback?.Invoke(entity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .4f);
    }
}