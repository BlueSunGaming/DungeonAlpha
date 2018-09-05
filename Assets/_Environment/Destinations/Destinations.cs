using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets._Environment.destinations

{


    public class Destinations : MonoBehaviour
    {

        public AnimationClip targetAnimation;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public AnimationClip GetDestinationAnimClip()
        {
            RemoveAnimationEvent();

            return targetAnimation;
        }

        //So that asset packs cannot cause crashes
        private void RemoveAnimationEvent()
        {
            targetAnimation.events = new AnimationEvent[0];
        }
    }

}