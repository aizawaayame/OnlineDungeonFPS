using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MinimapManager:Singleton<MinimapManager>
    {
        public UIMiniMap miniMap;
        private Collider minimapBoundBox;

        public Collider MinimapBoundBox
        {
            get { return minimapBoundBox; }
        }


        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                {
                    return null;
                }
                return User.Instance.CurrentCharacterObject.transform;
            }
            set
            {

            }
        }
        public Sprite LoadCurrentMinimap()
        {
            return Resources.Load<Sprite>("UI/Minimap/"+User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateCollider(Collider minimapBoundBox)
        {
            this.minimapBoundBox = minimapBoundBox;
            if (this.miniMap!=null)
            {
                this.miniMap.UpdateMap();
            }
        }
    }
}
