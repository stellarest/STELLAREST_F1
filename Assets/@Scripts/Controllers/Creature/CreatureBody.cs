using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_F1
{
    public class CreatureBody
    {
        public class Container
        {
            public Container(string tag, Transform tr, SpriteRenderer spr)
            {
                this.Tag = tag;
                this.TR = tr;
                this.SPR = spr;
            }

            public string Tag { get; private set; } = null;
            public Transform TR { get; private set; } = null;
            public SpriteRenderer SPR { get; private set; } = null;
        }

        public CreatureBody(Creature owner, int dataID)
        {
            this.Owner = owner;
            TemplateID = dataID;
        }

        public int TemplateID { get; protected set; } = -1;
        public Creature Owner { get; protected set; } = null;
        public T GetOwner<T>() where T : Creature => Owner as T;

        public void ShowBody(bool show)
        {
            foreach (var spr in Owner.GetComponentsInChildren<SpriteRenderer>())
                spr.enabled = show;
        }
    }
}
