using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts
{
    public class Resource : ITurnable {

        public ResourceType Type;
        public int Quantity, RegenQuantity, MaxQuantity;
        public int RegenTurns, TurnsToRegen;

        //Me is the constructor we're gonna use with map gen.
        //Resources aren't equal: some are frequent low yields, some are long-refresh/big payout.
        public Resource(ResourceType type)
        {
            Type = type;
            var rand = Random.value;
            switch (type)
            {
                case ResourceType.Water:
                case ResourceType.Aluminum:
                    if (rand < 0.45f)
                    {
                        //Slightly higher yield/regen
                        Quantity = Random.Range(5, 7);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(4, 7);
                        RegenTurns = Random.Range(3, 6);
                    }
                    else if (rand < 0.9f)
                    {
                        //Consistent trickle
                        Quantity = Random.Range(1, 3);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(1, 3);
                        RegenTurns = Random.Range(1, 3);
                    }
                    else
                    {
                        //On rare ocassions, a deep pool which regenerates slowly.
                        Quantity = Random.Range(7, 11);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(1, 3);
                        RegenTurns = Random.Range(2, 5);
                    }
                    break;
                case ResourceType.HeavyGas:
                case ResourceType.NuclearMaterial:
                    if (rand < 0.45f)
                    {
                        //Slightly higher yield/regen
                        Quantity = Random.Range(3, 5);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(2, 5);
                        RegenTurns = Random.Range(3, 6);
                    }
                    else if (rand < 0.9f)
                    {
                        //Consistent trickle
                        Quantity = Random.Range(1, 3);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(1, 3);
                        RegenTurns = Random.Range(2, 5);
                    }
                    else
                    {
                        //On rare ocassions, a deep pool which regenerates slowly.
                        Quantity = Random.Range(5, 9);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(1, 3);
                        RegenTurns = Random.Range(2, 6);
                    }
                    break;

                    //So, some of the rarity on these come from how infrequently they get created. When they do show up, they're a little more concentrated.
                case ResourceType.DarkMatter:
                case ResourceType.NeutronMass:
                    if (rand < 0.40f)
                    {
                        //Slightly higher yield/regen
                        Quantity = Random.Range(4, 7);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(3, 6);
                        RegenTurns = Random.Range(6, 11);
                    }
                    else if (rand < 0.80f)
                    {
                        //Consistent trickle
                        Quantity = Random.Range(2, 4);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(1, 3);
                        RegenTurns = Random.Range(3, 6);
                    }
                    else
                    {
                        //For these rare resources, the deep pool is more common It's also huuuge, and takes way longer to replenish.
                        Quantity = Random.Range(7, 21);
                        MaxQuantity = Quantity;
                        RegenQuantity = Random.Range(2, 4);
                        RegenTurns = Random.Range(2, 5);
                    }
                    break;
            }
        }

        //Me constructor is for use with MapLoader/serialized maps.
        public Resource(int resourceType, int resourceQuantity, int regenQuantity , int regenTurns) {
            Type = (ResourceType)resourceType;
            Quantity = resourceQuantity;
            RegenQuantity = regenQuantity;
            RegenTurns = regenTurns;
        }

        public void OnTurn() {
            TurnsToRegen--;
            if (TurnsToRegen != 0) return;
            Quantity += RegenQuantity;
            CheckQuantity();
            TurnsToRegen = RegenTurns;
        }

        void CheckQuantity() {
            if (Quantity > MaxQuantity) Quantity = MaxQuantity;
            if (Quantity < 0) Quantity = 0;
            if (Quantity <= 0f && System.Math.Abs(RegenQuantity) < 0.01f) Type = ResourceType.Nothing;
        }

        public void AddResource(ResourceType inputType, int inputAmount) {
            if (Type == ResourceType.Nothing)
            {
                Type = inputType;
                Quantity = inputAmount;
            }
            else if (Type == inputType) {
                Quantity += inputAmount;
            }
            CheckQuantity();
        }
    }
}