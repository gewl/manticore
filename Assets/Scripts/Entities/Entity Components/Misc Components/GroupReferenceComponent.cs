﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class GroupReferenceComponent : SerializedMonoBehaviour {

    [SerializeField]
    List<Transform> groupMembers;

    public List<Transform> GetGroup()
    {
        return groupMembers;
    }

    public Transform GetGroupMember(int index)
    {
        if (index < 0 || index > groupMembers.Count)
        {
            Debug.LogError("Trying to get out-of-range index from GroupReferenceComponent.");
            return null;
        }

        return groupMembers[index];
    }

    public void UpdateGroup(List<Transform> newGroup)
    {
        groupMembers = newGroup;
    }

    public bool AreAllGroupMembersDestroyed()
    {
        return groupMembers.All(m => m == null);
    }
}
