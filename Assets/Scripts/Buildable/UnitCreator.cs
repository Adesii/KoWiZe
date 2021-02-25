using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;

public class AORBuildCreator
{

    public Action<float> LeftForCurrentItem;
    public Action<AORQueableItem> Finished;
    public Action<AORQueableItem> QueuedNewItem;

    public Queue<AORQueableItem> itemQueue = new Queue<AORQueableItem>();
    public bool TimerActive = false;

    public float currBuildTime = 0;
    public float totalBuildTime;
    public AORQueableItem currentlyBuilding;
    public void QueueNewItem(AORQueableItem item)
    {

        if (!TimerActive) { TimerActive = true; itemQueue.Enqueue(item); ; Timer(); }
        else
            itemQueue.Enqueue(item);
        QueuedNewItem.Invoke(item);
    }

    private async void Timer()
    {
        while (TimerActive)
        {
            if (currBuildTime <= 0f)
            {

                if (currentlyBuilding != null) Finished?.Invoke(currentlyBuilding);
                if (itemQueue.Count == 0) { TimerActive = false; return; }
                currentlyBuilding = itemQueue.Dequeue();
                totalBuildTime = currentlyBuilding.build_time;
                currBuildTime = totalBuildTime;
            }
            LeftForCurrentItem?.Invoke(currBuildTime);
            currBuildTime -= 1f;
            await Task.Delay(1000);
        }

    }
}