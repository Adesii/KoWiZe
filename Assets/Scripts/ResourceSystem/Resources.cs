using System;

[Serializable]
public class ResourceClass
{
    [Serializable]
    public enum ResourceTypes
    {
        Wood,
        Stone,
        Iron,
        Food,
        Science,
        
    }
    public ResourceTypes ResourceType;
    public float currentAmount;
    public float maxCapacity;
    public int resourceIndex = 0;

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
        if (currentAmount - amount >= 0)
        {
            currentAmount -= amount;
            return true;
        }
        else
            return false;
    }
    public bool canRemoveResource(float amount)
    {
        if (currentAmount - amount >= 0)
        {
            return true;
        }
        else
            return false;
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

    public ResourceClass(ResourceTypes type, float maxAmount, float startingAmount)
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

    public ResourceClass(ResourceTypes type,float amount)
    {
        ResourceType = type;
        currentAmount = amount;
    }



}
