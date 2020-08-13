using UnityEngine;

namespace RPG.Saving
{
    [System.Serializable]
    public class SerializableVector3
    {
        private float _x, _y, _z;

        public SerializableVector3(Vector3 vector)
        {
            this._x = vector.x;
            this._y = vector.y;
            this._z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}
