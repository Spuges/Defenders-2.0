using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public class AIBase : MonoBehaviour, ISpaceCraft
    {
        GameObject ISpaceCraft.GameObject => gameObject;

        Transform ISpaceCraft.Transform => transform;
    }
}
