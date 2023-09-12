using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public int SpellCost { get;  set; }
    public abstract void Cast();
}
