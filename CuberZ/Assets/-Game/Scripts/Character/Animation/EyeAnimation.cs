using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAnimation : MonoBehaviour 
{
	private SkinnedMeshRenderer skinnedMesh_;

	private int index_;
	private float countTime_;
    private float secondCountTime_;
    private float waitTime_;
    private float morph_;


	void Start () 
    {
		skinnedMesh_ = GetComponent<SkinnedMeshRenderer> ();
		morph_ = 0;
	}

	void Update () 
    {
		skinnedMesh_.SetBlendShapeWeight (0, Mathf.Lerp (skinnedMesh_.GetBlendShapeWeight (0), morph_, 0.4f) );

		if (index_ < 2) 
        {
			if (countTime_ < waitTime_) 
            {
				countTime_ += 1f;
			} 
            else 
            {
				if (morph_ <=1) 
                {
					morph_ = 100;
					waitTime_ = 3;
				} 
                else 
                {
					morph_ = 0;
					waitTime_ = 50;
				}
				index_++;
				countTime_ = 0;
			}
		}
		if (index_ >= 2) 
        {
			if (secondCountTime_ < waitTime_ * 2) 
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
