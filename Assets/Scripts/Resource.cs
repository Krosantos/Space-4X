using System;

namespace Assets.Scripts
{
    public class Resource : ITurnable {

        public ResourceType Type;
        public int Quantity, RegenQuantity, MaxQuantity;
        public int RegenTurns, TurnsToRegen;

        public Resource(ResourceType type)
        {
            Type = type;
        }

        public Resource(int resourceType, int resourceQuantity) {
            Type = (ResourceType)resourceType;
            Quantity = resourceQuantity;
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