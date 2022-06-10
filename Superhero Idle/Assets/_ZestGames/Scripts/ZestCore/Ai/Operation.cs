using System.Collections.Generic;
using UnityEngine;

namespace ZestCore.Ai
{
    public static class Operation
    {
        /// <summary>
        /// Finds and returns the closest Transform from given list of transforms.
        /// </summary>
        /// <param name="thisTransform">Transform who is doing this search.</param>
        /// <param name="list">Transform list that we want to find the closest. Like a target list.</param>
        /// <returns></returns>
        public static Transform FindClosestTransform(Transform thisTransform, List<Transform> list)
        {
            if (list == null || list.Count == 0) return null;

            float shortestDistance = Mathf.Infinity;
            Transform closestTransform = null;

            for (int i = 0; i < list.Count; i++)
            {
                float distanceToTransform = (thisTransform.position - list[i].position).sqrMagnitude;
                if (distanceToTransform < shortestDistance && thisTransform != list[i])
                {
                    shortestDistance = distanceToTransform;
                    closestTransform = list[i];
                }
            }

            return closestTransform;
        }

        /// <summary>
        /// Cheks if object has reached Target point according to given distance limit.
        /// </summary>
        /// <param name="thisTransform"></param>
        /// <param name="target">This point will be checked if reached or not.</param>
        /// <param name="limit">This is distance limit. If we're closer than this limit we've reached. Default value: 2f</param>
        /// <returns>True if reached, False if not.</returns>
        public static bool IsTargetReached(Transform thisTransform, Vector3 target, float limit = 2f)
        {
            return (target - thisTransform.position).sqrMagnitude <= limit ? true : false;
        }
    }
}