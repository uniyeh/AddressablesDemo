using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadEventArgs
{
    public DownloadEventArgs(object key, float percentage, bool success) 
    {
        Key = key;
        Percentage = percentage;
        Success = success;
    }
    
    public object Key { get; }
    public float Percentage { get; }
    public bool Success { get; }
}
