//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    public class LevelMarkerList : IEnumerable<PropSocket>
    {
        protected List<PropSocket> markers = new List<PropSocket>();

        public IEnumerator<PropSocket> GetEnumerator()
        {
            return markers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return markers.GetEnumerator();
        }

        public virtual void Add(PropSocket marker)
        {
            markers.Add(marker);
        }

        public virtual void Remove(PropSocket marker)
        {
            markers.Remove(marker);
        }

        public virtual void Clear()
        {
            markers.Clear();
        }

        public PropSocket this[int index]
        {
            get
            {
                return markers[index];
            }
        }

        public int Count
        {
            get
            {
                return markers.Count;
            }
        }

        public virtual IEnumerable<PropSocket> GetMarkersInSearchArea(Vector2 center, float radius)
        {
            return markers;
        }
    }
    
    public class SpatialPartionedLevelMarkerList : LevelMarkerList
    {
        private float partitionCellSize = 4.0f;
        private Dictionary<IntVector2, List<PropSocket>> buckets = new Dictionary<IntVector2, List<PropSocket>>();

        public SpatialPartionedLevelMarkerList(float partitionCellSize)
        {
            this.partitionCellSize = partitionCellSize;
        }

        IntVector2 GetBucketCoord(PropSocket marker)
        {
            var position = Matrix.GetTranslation(ref marker.Transform);
            return GetBucketCoord(position.x, position.z);
        }

        IntVector2 GetBucketCoord(Vector2 position)
        {
            return GetBucketCoord(position.x, position.y);
        }

        IntVector2 GetBucketCoord(float x, float z)
        {
            int ix = Mathf.FloorToInt(x / partitionCellSize);
            int iy = Mathf.FloorToInt(z / partitionCellSize);
            return new IntVector2(ix, iy);
        }

        public override void Add(PropSocket marker)
        {
            base.Add(marker);

            var partitionCoord = GetBucketCoord(marker);
            if (!buckets.ContainsKey(partitionCoord))
            {
                buckets.Add(partitionCoord, new List<PropSocket>());
            }
            buckets[partitionCoord].Add(marker);
        }

        public override void Remove(PropSocket marker)
        {
            base.Remove(marker);

            var partitionCoord = GetBucketCoord(marker);
            if (buckets.ContainsKey(partitionCoord))
            {
                buckets[partitionCoord].Remove(marker);
            }
        }

        public override IEnumerable<PropSocket> GetMarkersInSearchArea(Vector2 center, float radius)
        {
            var extent = new Vector2(radius, radius);
            var start = GetBucketCoord(center - extent);
            var end = GetBucketCoord(center + extent);

            var searchSpace = new List<PropSocket>();
            for (int x = start.x; x <= end.x; x++)
            {
                for (int y = start.y; y <= end.y; y++)
                {
                    var key = new IntVector2(x, y);
                    if (buckets.ContainsKey(key))
                    {
                        searchSpace.AddRange(buckets[key]);
                    }
                }
            }
            return searchSpace;
        }

        public override void Clear()
        {
            base.Clear();
            buckets.Clear();
        }
    }
}
