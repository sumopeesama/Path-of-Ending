using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "PisitRPGKit/PlayerSO")]
public class DataSO : ScriptableObject
{
    public string playerName = "Default";
    public int playerGold = 10;
}
