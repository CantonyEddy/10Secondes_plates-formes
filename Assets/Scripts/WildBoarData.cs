using UnityEngine;

[CreateAssetMenu(fileName = "ennemy_data", menuName = "Scriptable Objects/ennemy_data")]
public class ennemy_data : ScriptableObject
{
    public string enemyName;
    public int health;
    public int speed;
    public float time;
    public bool isDangerous;
    public Color color;
    
}
