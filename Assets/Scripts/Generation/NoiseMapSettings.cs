using System;
using UnityEngine;

namespace Generation
{
    [Serializable]
    public struct NoiseMapSettings
    {
        [SerializeField] public float freq;
        [SerializeField] public int octaves;
        [SerializeField] public float multiplier;
        [HideInInspector] public int seed;

        public void InitSeed()
        {
            seed = SRnd.RangeInt(0, 10000);
        }
    }
}

