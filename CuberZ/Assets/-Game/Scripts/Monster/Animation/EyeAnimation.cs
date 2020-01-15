using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAnimation : MonoBehaviour 
{
	private SkinnedMeshRenderer skinnedMesh_;

	[SerializeField] private int index_;
	private public float countTime_;
    public float secondCountTime_;

	public float waitTime;
	public float MORPH;


	void Start () 
    {

		skinnedMesh_ = GetComponent<SkinnedMeshRenderer> ();
		MORPH = 0;
	}

	void Update () 
    {
		skinnedMesh_.SetBlendShapeWeight (0, Mathf.Lerp (skinnedMesh_.GetBlendShapeWeight (0), MORPH,0.4f));

		if (index_ < 2) 
        {
			if (countTime_ < waitTime) 
            {
				countTime_ += 1f;
			} 
            else 
            {
				if (MORPH <=1) 
                {
					MORPH = 100;
					waitTime = 3;
				} 
                else 
                {
					MORPH = 0;
					waitTime = 50;
				}
				index_++;
				countTime_ = 0;
			}
		}
		if (index_ >= 2) 
        {
			if (secondCountTime_ < waitTime * 2) 
            {
				secondCountTime_ += 1;
			} 
            else 
            {
				index_ = 0;
				secondCountTime_ = 0;
			}
		}
	}
}
