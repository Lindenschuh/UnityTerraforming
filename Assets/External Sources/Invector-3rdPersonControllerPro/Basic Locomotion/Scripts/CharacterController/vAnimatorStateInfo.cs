using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.EventSystems
{
    public class vAnimatorStateInfos
    {       
        public vAnimatorStateInfos(Animator animator)
        {
            var bhv = animator.GetBehaviours<vAnimatorStateListener>();
            for (int i = 0; i < bhv.Length; i++) bhv[i].stateInfos = this;
        }

        Dictionary<string, List<AnimatorStateInfo>> statesRunning = new Dictionary<string, List<AnimatorStateInfo>>();
        List<AnimatorStateInfo> emptyList = new List<AnimatorStateInfo>();
        internal void AddStateInfo(string tag, AnimatorStateInfo info)
        {
            if (!statesRunning.ContainsKey(tag)) statesRunning.Add(tag, new List<AnimatorStateInfo>() { info });
            else statesRunning[tag].Add(info);
        }
        internal void UpdateStateInfo(string tag, AnimatorStateInfo info)
        {
            if (statesRunning.ContainsKey(tag) && statesRunning[tag].Exists(_info => _info.fullPathHash.Equals(info.fullPathHash)))
            {
                var inforef = statesRunning[tag].Find(_info => _info.fullPathHash.Equals(info.fullPathHash));
                var index = statesRunning[tag].IndexOf(inforef);
                statesRunning[tag][index] = info;              
            }
        }
        internal void RemoveStateInfo(string tag, AnimatorStateInfo info)
        {
            if (statesRunning.ContainsKey(tag) && statesRunning[tag].Exists(_info => _info.fullPathHash.Equals(info.fullPathHash)))
            {
                var inforef = statesRunning[tag].Find(_info => _info.fullPathHash.Equals(info.fullPathHash));
                statesRunning[tag].Remove(inforef);
                if (statesRunning[tag].Count == 0)
                    statesRunning.Remove(tag);
            }
        }

        /// <summary>
        /// Check If StateInfo list contains tag
        /// </summary>
        /// <param name="tag">tag to check</param>
        /// <returns></returns>
        public bool HasTag(string tag)
        {
            return statesRunning.ContainsKey(tag);
        }
        
        /// <summary>
        /// Check if All tags in in StateInfo List
        /// </summary>
        /// <param name="tags">tags to check</param>
        /// <returns></returns>
        public bool HasAllTags(params string[] tags)
        {
            var has = tags.Length>0?true: false;
            for(int i =0;i<tags.Length;i++)
            {
                if(!HasTag(tags[i]))
                {
                    has = false;
                    break;
                }
            }
            return has;
        }
        /// <summary>
        /// Check if StateInfo List Contains any tag
        /// </summary>
        /// <param name="tags">tags to check</param>
        /// <returns></returns>
        public bool HasAnyTag(params string[] tags)
        {
            var has =false;
            for (int i = 0; i < tags.Length; i++)
            {
                if (HasTag(tags[i]))
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public List<AnimatorStateInfo> GetStateInfoListFromTag(string tag)
        {
            if (HasTag(tag)) return statesRunning[tag];
            else return emptyList;
        }
    }

}
