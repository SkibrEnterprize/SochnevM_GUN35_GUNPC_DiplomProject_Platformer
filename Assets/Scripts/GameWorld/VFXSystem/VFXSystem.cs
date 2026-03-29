
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class VFXSystem : MonoBehaviour, IVFXSystem
    {
        private VFXLibrary _library;

        // Словарь для хранения списков объектов под каждый префикс
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

            // Если объект был активен (мы его "украли"), принудительно выключаем и включаем
            instance.SetActive(false);

            // Настраиваем позицию и запускаем
            vfxEvent.Play(instance, position, rotation, parent);
            instance.SetActive(true);

            // Запускаем таймер возврата
            StartCoroutine(ReturnToPoolAfterTime(instance, vfxEvent.lifetime));
        }

        private GameObject GetFromPool(VFXEvent vfxEvent)
        {
            GameObject prefab = vfxEvent.effectPrefab;
            if (!_pools.ContainsKey(prefab)) _pools[prefab] = new List<GameObject>();

            var pool = _pools[prefab];

            // 1. Ищем свободный
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeSelf) return pool[i];
            }

            // 2. Если свободных нет, пробуем расширить (лимит: x2 от базового)
            if (pool.Count < vfxEvent.poolSize * 2)
            {
                return CreateNewInstance(prefab);
            }

            // 3. Если расширять нельзя, "воруем" самый старый (первый в списке)
            var oldest = pool[0];
            pool.RemoveAt(0);
            pool.Add(oldest); // Перемещаем в конец, чтобы в следующий раз взять другой
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
