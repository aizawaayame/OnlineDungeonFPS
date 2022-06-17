using System;
using Protocol;
using UnityEngine;

namespace GameObjects
{
    public static class GameObjectTool
    {
        public static Vector3 ServerNV3ToWorldV3(NVector3 var)
        {
            return new Vector3(var.X / 100f, var.Y / 100f, var.Z / 100f);
        }
        public static NVector3 WorldV3ToServerNV3(Vector3 var)
        {
            return new NVector3(){
                X = Mathf.RoundToInt(var.x * 100),
                Y = Mathf.RoundToInt(var.y * 100),
                Z = Mathf.RoundToInt(var.z * 100)
            };
        }
        
        public static Vector3 LogicV3IntToWorldV3(Vector3Int var)
        {
            return new Vector3(var.x / 100f, var.y / 100f, var.z / 100f);
        }
        public static Vector3Int WorldV3ToLogicV3(Vector3 var)
        {
            return new Vector3Int(){
                x = Mathf.RoundToInt(var.x * 100),
                y = Mathf.RoundToInt(var.y * 100),
                z = Mathf.RoundToInt(var.z * 100)
            };
        }
        
        public static float LogicIntToWorldFloat(int var)
        {
            return var / 100f;
        }
        public static int WorldFloatToLogicInt(float var)
        {
            return Mathf.RoundToInt(var / 100f);
        }

        public static bool EntityUpdate(NEntity entity, UnityEngine.Vector3 position, Quaternion rotation, float speed)
        {
            NVector3 npos = WorldV3ToServerNV3(position);
            NVector3 ndir = WorldV3ToServerNV3(rotation.eulerAngles);
            int nspeed = WorldFloatToLogicInt(speed);
            bool isUpdated = false;
            if (!entity.Position.Equals(npos))
            {
                entity.Position = npos;
                isUpdated = true;
            }
            if (!entity.Direction.Equals(ndir))
            {
                entity.Direction = ndir;
                isUpdated = true;
            }
            if (entity.Speed != nspeed)
            {
                entity.Speed = nspeed;
                isUpdated = true;
            }
            return isUpdated;
        }
    }
}
