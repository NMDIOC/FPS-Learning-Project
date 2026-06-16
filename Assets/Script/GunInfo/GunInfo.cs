using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun/Info")]
public class GunInfo : ScriptableObject
{
    [Header("Configuración Base")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _reloadTime;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private float _daño;

    [Header("Sonidos")]
    [SerializeField] private AudioClip _sonidoDisparo;
    [SerializeField] private AudioClip _sonidoRecarga;

    public GameObject prefab       => _prefab;
    public float fireRate          => _fireRate;
    public float reloadTime        => _reloadTime;
    public int maxAmmo             => _maxAmmo;
    public float daño              => _daño;
    public AudioClip sonidoDisparo => _sonidoDisparo;
    public AudioClip sonidoRecarga => _sonidoRecarga;
}