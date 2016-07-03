using System;
using Assets.Scripts.Utility;

namespace Assets.Scripts
{
    public class Resource : ITurnable {

        public ResourceType Type;
        public int Quantity, RegenQuantity, MaxQuantity;
        public int RegenTurns, TurnsToRegen;

        //This is the one we're gonna use with map gen.
        //Resources aren't equal: some are frequent low yields, some are long-refresh/big payout.
        public Resource(ResourceType type)
        {
            Type = type;
            switch (type)
            {
                case ResourceType.Water:

                    break;
            }
        }

        //This constructor is for use with MapLoader/serialized maps.
        public Resource(int resourceType, int resourceQuantity, int regenQuantity , int turnsToRegen) {
            Type = (ResourceType)resourceType;
            Quantity = resourceQuantity;
            RegenQuantity = regenQuantity;
            TurnsToRegen = turnsToRegen;
        }

        public void OnTurn() {
            TurnsToRegen--;
            if (TurnsToRegen != 0) return;
            Quantity += RegenQuantity;
            TurnsToRegen = RegenTurns;
        }

        void CheckQuantity() {
            if (Quantity > MaxQuantity) Quantity = MaxQuantity;
            if (Quantity < 0) Quantity = 0;
            if (Quantity <= 0f && Math.Abs(RegenQuantity) < 0.01f) Type = ResourceType.Nothing;
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