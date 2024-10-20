using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerCast", menuName = "Cast/CastData")]
public class Cast : ScriptableObject
{
    public string name;
    public enum Positions { Center, Spawner, Melee };
    public Positions position = Positions.Center;
    public GameObject prefab;
    public float attackRate;
    public float length;
    public float cooldown;
    public float generalCooldown;
}