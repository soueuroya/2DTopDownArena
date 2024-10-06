using UnityEngine;
using static PlayerClass;

[CreateAssetMenu(fileName = "NewPlayerClass", menuName = "PlayerClass/PlayerClassData")]
public class PlayerStatus : ScriptableObject
{
    public ClassType playerClass; // The class type from the enum
    public int maxHealth; // Maximum health of this class
    public float speed; // Movement speed of this class
    public float jumpCooldown;
    public float jumpGeneralCooldown;
    public float dashSpeed; // Movement speed of this class
    public float dashCooldown; // Movement speed of this class
    public float dashGeneralCooldown;
    public AnimatorOverrideController anim;
    public Cast attack1;
    public Cast attack2;
    public Cast shield;
}