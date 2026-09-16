using System.Collections.Generic;
using UnityEngine;

public class PlayerPublicRef : MonoBehaviour
{
    public static PlayerPublicRef Instance;

    void Awake() => Instance = this;
}
