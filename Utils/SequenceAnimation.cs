using UnityEngine;
using System.Collections;

public class SequenceAnimation : MonoBehaviour
{
	public float FPS = 30.0f;
	public bool Loop = true;
	public string folderName = "";
    public int startFrameNum = 0;
    public bool autoRun = true;

	float secondsToWait;
	Texture2D[] frames;
	int currentFrame;
    Material mat;

    // Use this for initialization
    void Start ()
	{
        frames = Resources.LoadAll<Texture2D>(folderName);
        mat = GetComponent<Renderer>().material;

        currentFrame = startFrameNum;
        secondsToWait = 1.0f / FPS;

        if (autoRun)
			StartCoroutine("Animate");
	}

	public void Reset ()
	{
        StopAllCoroutines();

		currentFrame = startFrameNum;

        if (mat)
            mat.mainTexture = frames[startFrameNum];
    }

	public void Restart ()
	{
		StartCoroutine("Animate");
	}
	
	IEnumerator Animate()
	{
		bool stop = false;

		if(currentFrame >= frames.Length)
		{
			if(Loop == false)
			{
				currentFrame = frames.Length - 1;
				stop = true;
			}
			else 
			{
				currentFrame = 0;
			}
		}

		yield return new WaitForSeconds(secondsToWait);

        if (mat)
            mat.mainTexture = frames[currentFrame];
        
        currentFrame++;
		
		if(stop == false)
			StartCoroutine(Animate());
	}

}

