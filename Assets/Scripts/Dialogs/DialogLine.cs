using System;
using UnityEngine;

[Serializable]
public class DialogLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string    sentence;
    public int       cameraIndex;
    public AudioClip voiceClip;
}