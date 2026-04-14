using UnityEngine;

[CreateAssetMenu(menuName = "VFX/VFX Event")]
public class VFXEvent : ScriptableObject
{
    public GameObject effectPrefab; 
    public float lifetime = 2f;    
    public Vector3 offset;         
    public bool followPlayer;      
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
