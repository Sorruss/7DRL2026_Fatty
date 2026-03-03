using System;
using UnityEngine;

namespace FG
{
    public class AStarNode : IComparable<AStarNode>
    {
        // BASE
        public Vector2Int gridPosition;

        // CALCULATION VALUES
        public int HValue;      // DISTANCE TO DESTINATION
        public int GValue;      // DISTANCE TO STARTING POINT
        public int FValue       // HVALUE + GVALUE
        { 
            get
            {
                return HValue + GValue;
            }
        }

        // RELATION
        public AStarNode parent;

        public AStarNode(Vector2Int position)
        {
            gridPosition = position;
        }

        public int CompareTo(AStarNode other)
        {
            int mainComparison = FValue.CompareTo(other.FValue);
            if (mainComparison == 0)
            {
                // SECONDARY COMPARISON
                mainComparison = HValue.CompareTo(other.HValue);
            }

            return mainComparison;
        }
    }
}
