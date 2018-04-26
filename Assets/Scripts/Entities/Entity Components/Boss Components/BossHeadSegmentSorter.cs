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
        Joint dyingJoint = dyingSegment.GetComponent<ConfigurableJoint>();
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

        Transform newParent = transform;
        if (index > 0)
        {
            newParent = bodySegments[index - 1];
        }

        Transform replacingSegment = bodySegments[index + 1];
        ConfigurableJoint replacingJoint = replacingSegment.GetComponent<ConfigurableJoint>();
        replacingJoint.connectedBody = null;
        replacingJoint.connectedAnchor = dyingJoint.connectedAnchor;

        bodySegments.RemoveAt(index);
        for (int i = 0; i < bodySegments.Count; i++)
        {
            bodySegments[i].name = BOSS_BODY_PREFIX + i;
        }

        StartCoroutine(SlideSegmentIntoPlace(replacingSegment, dyingSegment.position, newParent, replacingJoint));
    }

    IEnumerator SlideSegmentIntoPlace(Transform replacingSegment, Vector3 newPosition, Transform newParent, ConfigurableJoint joint)
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

        joint.connectedBody = newParent.GetComponent<Rigidbody>();

        foreach (Transform bodySegment in segmentManager.GetGroup())
        {
            bodySegment.GetComponent<Collider>().enabled = true;
        }
        entityEmitter.EmitEvent(EntityEvents.Move);
    }
}
