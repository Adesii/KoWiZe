using System;

 [Serializable]
public class Resources
{
    public enum ResourceTypes{
        Wood,
        Stone,
        Iron,
        Science,
        Food
    }

    [Serializable]
    public class ResourceClass
    {
        public ResourceTypes ResourceType;
        public float currentAmount;
        public float maxCapacity;

        public bool AddResource(float amount)
        {
            if (currentAmount + amount <= maxCapacity)
                currentAmount += amount;
            else
                return false;
            return true;
        }
        public bool RemoveResource(float amount)
        {
            if (currentAmount - amount > 0)
                currentAmount -= amount;
            else
                return false;
            return true;
        }

        public void AddToLimit(float amount)
        {
            maxCapacity += amount;
        }
        public void RemoveFromLimit(float amount)
        {
            if (maxCapacity >= 0f) maxCapacity -= amount;
        }

        public void SetLimit(float amount)
        {
            maxCapacity = amount;
        }
        public void SetAmount(float amount)
        {
            currentAmount = amount;
        }

        public ResourceClass(ResourceTypes type,float maxAmount,float startingAmount)
        {
            ResourceType = type;
            currentAmount = startingAmount;
            maxCapacity = maxAmount;
        }

        public ResourceClass(ResourceTypes type)
        {
            ResourceType = type;
            currentAmount = 0;
            maxCapacity = 0;
        }

        
       
    }
}
