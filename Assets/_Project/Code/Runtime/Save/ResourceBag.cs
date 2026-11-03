using System;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Save
{
    [Serializable]
    public struct ResourceBag
    {
        public int Gold;
        public int Oil;
        public int Bones;
        public int Iron;
        public int Wax;

        public static ResourceBag operator +(ResourceBag a, ResourceBag b)
        {
            return new ResourceBag
            {
                Gold = a.Gold + b.Gold,
                Oil = a.Oil + b.Oil,
                Bones = a.Bones + b.Bones,
                Iron = a.Iron + b.Iron,
                Wax = a.Wax + b.Wax
            };
        }

        public static ResourceBag operator -(ResourceBag a, ResourceBag b)
        {
            return new ResourceBag
            {
                Gold = a.Gold - b.Gold,
                Oil = a.Oil - b.Oil,
                Bones = a.Bones - b.Bones,
                Iron = a.Iron - b.Iron,
                Wax = a.Wax - b.Wax
            };
        }

        public bool Covers(ResourceBag cost)
        {
            return Gold >= cost.Gold &&
                   Oil >= cost.Oil &&
                   Bones >= cost.Bones &&
                   Iron >= cost.Iron &&
                   Wax >= cost.Wax;
        }

        public ResourceBag Halved()
        {
            return new ResourceBag
            {
                Gold = Gold / 2,
                Oil = Oil / 2,
                Bones = Bones / 2,
                Iron = Iron / 2,
                Wax = Wax / 2
            };
        }

        public ResourceBag ClampedToZero()
        {
            return new ResourceBag
            {
                Gold = Mathf.Max(0, Gold),
                Oil = Mathf.Max(0, Oil),
                Bones = Mathf.Max(0, Bones),
                Iron = Mathf.Max(0, Iron),
                Wax = Mathf.Max(0, Wax)
            };
        }
    }
}
