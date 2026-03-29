using UnityEngine;

[CreateAssetMenu(menuName = "VFX/VFX Event")]
public class VFXEvent : ScriptableObject
{
    public GameObject effectPrefab; // Префиб с ParticleSystem
    public float lifetime = 2f;     // Через сколько вернуть в пул
    public Vector3 offset;          // Смещение относительно точки спавна
    public bool followPlayer;       // Должен ли эффект «прилипнуть» к игроку?
    public int poolSize;

    public void Play(GameObject instance, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        instance.transform.position = position + offset;
        instance.transform.rotation = rotation;
        instance.transform.SetParent(parent);

        var ps = instance.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
    }
}
