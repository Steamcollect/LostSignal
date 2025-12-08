using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Title("CONFIGURATION")]
    [ValueDropdown("@SSO_EnemyCatalog.GetDirectPrefabDropdown()")]
    [ListDrawerSettings(ShowIndexLabels = true, ShowItemCount = true, ShowFoldout = false)]
    [SerializeField] private List<GameObject> m_Wave = new List<GameObject>();
    public int ConfiguredWaveCount => m_Wave.Count;

    public void SpawnWave(int waveIndex, System.Action<EntityController> onSpawnCallback)
    {
        if (waveIndex < 0 || waveIndex >= m_Wave.Count) return;

        GameObject content = m_Wave[waveIndex];
        if (content != null)
        {

            // DO SOMETHING

            // SPAWN ENEMY
            if(content.TryGetComponent<EntityController>(out EntityController entity))
            {
                Instantiate(entity, transform.position, transform.rotation);

                if (entity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();
            }

            onSpawnCallback?.Invoke(entity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .4f);
    }
}