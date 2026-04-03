
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class VFXSystem : MonoBehaviour, IVFXSystem
    {
        private VFXLibrary _library;

        private readonly Dictionary<GameObject, List<GameObject>> _pools =
            new Dictionary<GameObject, List<GameObject>>();

        [Inject]
        public void Construct(VFXLibrary library)
        {
            _library = library;
            PrewarmPools();
        }

        private void PrewarmPools()
        {
            foreach (var mapping in _library.effects)
            {
                var ev = mapping.vfxEvent;
                if (ev == null || ev.effectPrefab == null) continue;

                if (!_pools.ContainsKey(ev.effectPrefab))
                {
                    _pools[ev.effectPrefab] = new List<GameObject>(ev.poolSize);
                    for (int i = 0; i < ev.poolSize; i++)
                    {
                        CreateNewInstance(ev.effectPrefab);
                    }
                }
            }
        }

        private GameObject CreateNewInstance(GameObject prefab)
        {
            var instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            _pools[prefab].Add(instance);
            return instance;
        }
        
        public void Play(VFXType type, Vector3 position, Quaternion rotation, Transform parent)
        {
            var vfxEvent = _library.GetEvent(type);
            if (vfxEvent == null || vfxEvent.effectPrefab == null) return;

            GameObject instance = GetFromPool(vfxEvent);
            if (instance == null) return;

            instance.SetActive(false);

            vfxEvent.Play(instance, position, rotation, parent);
            instance.SetActive(true);

            StartCoroutine(ReturnToPoolAfterTime(instance, vfxEvent.lifetime));
        }

        private GameObject GetFromPool(VFXEvent vfxEvent)
        {
            GameObject prefab = vfxEvent.effectPrefab;
            if (!_pools.ContainsKey(prefab)) _pools[prefab] = new List<GameObject>();

            var pool = _pools[prefab];

            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeSelf) return pool[i];
            }

            if (pool.Count < vfxEvent.poolSize * 2)
            {
                return CreateNewInstance(prefab);
            }

            var oldest = pool[0];
            pool.RemoveAt(0);
            pool.Add(oldest);
            return oldest;
        }

        private IEnumerator ReturnToPoolAfterTime(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (instance != null)
            {
                instance.SetActive(false);
                instance.transform.SetParent(transform);
            }
        }
    }
}
