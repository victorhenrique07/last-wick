using Assets._Project.Code.Runtime.Save;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets._Project.Code.Runtime.Run
{
    public static class RewardRules
    {
        public static ResourceBag Deliver(ResourceBag collected, RunEndReason reason)
        {
            if (reason == RunEndReason.Victory)
                return collected;

            return collected.Halved();
        }

        public static ResourceBag RandomMaterials(int count, System.Random rng)
        {
            ResourceBag materials = new ResourceBag();
            
            for (int i = 0; i < count; i++)
            {
                MaterialType material = (MaterialType)rng.Next(0, Enum.GetValues(typeof(MaterialType)).Length);

                switch (material)
                {
                    case MaterialType.Bone:
                        materials.Bones++;
                        break;
                    case MaterialType.Iron:
                        materials.Iron++;
                        break;
                    case MaterialType.Wax:
                        materials.Wax++;
                        break;
                }
            }

            return materials;
        }
    }
}
