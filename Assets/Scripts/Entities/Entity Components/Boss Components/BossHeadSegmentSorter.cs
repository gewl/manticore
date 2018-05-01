using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadSegmentSorter : EntityComponent {

    const string BOSS_BODY_PREFIX = "BossBody_";
    GroupReferenceComponent segmentManager;
    float slideTime = 2f;

    protected override void Subscribe()
    {
        segmentManager = GetComponent<GroupReferenceComponent>();
    }

    protected override void Unsubscribe()
    {
    }

    public void OnSegmentDeath(int index)
    {
        List<Transform> bodySegments = segmentManager.GetGroup();
        Transform dyingSegment = bodySegments[index];
        ConfigurableJoint dyingJoint = dyingSegment.GetComponent<ConfigurableJoint>();
        dyingJoint.connectedBody = null;

        if (index == bodySegments.Count - 1)
        {
            bodySegments.RemoveAt(index);
            return;
        }
        else if (index >= bodySegments.Count)
        {
            return;
        }

        foreach (Transform segment in bodySegments)
        {
            segment.GetComponent<Collider>().enabled = false;
        }
        entityEmitter.EmitEvent(EntityEvents.Stop);

        Transform newConnectedBody = transform;
        if (index > 0)
        {
            newConnectedBody = bodySegments[index - 1];
        }

        Transform replacingSegment = bodySegments[index + 1];

        bodySegments.RemoveAt(index);

        if (bodySegments.Count <= 0)
        {
            Debug.Log("All segments destroyed.");
            return;
        }

        Vector3 lastSegmentPosition = dyingSegment.position;
        Rigidbody nextConnectedBody = newConnectedBody.GetComponent<Rigidbody>();
        Vector3 lastAnchor = dyingJoint.connectedAnchor;    
        for (int i = 0; i < bodySegments.Count; i++)
        {
            Transform bodySegment = bodySegments[i];
            bodySegment.name = BOSS_BODY_PREFIX + i;

            if (i >= index)
            {
                ConfigurableJoint segmentJoint = bodySegment.GetComponent<ConfigurableJoint>();
                segmentJoint.connectedBody = null;
                Vector3 newAnchor = lastAnchor;
                lastAnchor = segmentJoint.connectedAnchor;
                segmentJoint.connectedAnchor = newAnchor;

                if (i == bodySegments.Count - 1)
                {
                    StartCoroutine(SlideSegmentIntoPlace(bodySegment, lastSegmentPosition, nextConnectedBody, segmentJoint, true));
                }
                else
                {
                    StartCoroutine(SlideSegmentIntoPlace(bodySegment, lastSegmentPosition, nextConnectedBody, segmentJoint));
                }

                lastSegmentPosition = bodySegment.position;
                nextConnectedBody = bodySegment.GetComponent<Rigidbody>();
            }
        }
    }

    IEnumerator SlideSegmentIntoPlace(Transform replacingSegment, Vector3 newPosition, Rigidbody newConnectedBody, ConfigurableJoint segmentJoint, bool isLast = false)
    {
        float timeElapsed = 0.0f;
        Vector3 originalPosition = replacingSegment.position;

        while (timeElapsed < slideTime)
        {
            float percentageComplete = timeElapsed / slideTime;

            float curvedPercentage = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            replacingSegment.position = Vector3.Lerp(originalPosition, newPosition, curvedPercentage);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        segmentJoint.connectedBody = newConnectedBody;

        if (isLast)
        {
            foreach (Transform bodySegment in segmentManager.GetGroup())
            {
                bodySegment.GetComponent<Collider>().enabled = true;
            }
            entityEmitter.EmitEvent(EntityEvents.Move);
        }
    }
}
