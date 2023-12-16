using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

namespace Verlet
{
    public class Node
    {
        public List<Edge> Edges => _edges;
        private List<Edge> _edges;

        public Vector3 position;
        protected Vector3 _prev;

        public Node(Vector3 p)
        {
            position = _prev = p;
            _edges = new();
        }

        public void Step()
        {
            var velocity = position - _prev;
            var next = position + velocity;
            _prev = position;
            position = next;
        }

        public void Connect(Edge e)
        {
            _edges.Add(e);
        }
    }

    public class Edge
    {
        public float Length => length;
        private float length;
        private Node a;
        private Node b;

        public Edge(Node a, Node b)
        {
            this.a = a;
            this.b = b;
            this.length = (a.position - b.position).magnitude;
        }

        public Edge(Node a, Node b, float length)
        {
            this.a = a;
            this.b = b;
            this.length = length;
        }

        public Node Other(Node p)
        {
            return a == p ? b : a;
        }

        public static void Connect(Node a, Node b)
        {
            Edge e = new(a, b);
            a.Connect(e);
            b.Connect(e);
        }
    }
    
    public class VerletSimulation
    {
        private Node[] _nodes;
        private Color _nodeColor = Color.yellow;
        private Color _edgeColor = Color.white;

        public VerletSimulation(Node[] nodes)
        {
            this._nodes = nodes;
        }

        public void SetDebugColors(Color nodeColor, Color edgeColor)
        {
            _nodeColor = nodeColor;
            _edgeColor = edgeColor;
        }

        public void Simulate(int iterations, float deltaTime)
        {
            Step();
            Solve(iterations, deltaTime);
        }

        void Step()
        {
            foreach (var node in _nodes) node.Step();
        }

        void Solve(int iterations, float deltaTime)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (var node in _nodes) SolveNode(node);
            }
        }

        void SolveNode(Node node)
        {
            node.Edges.ForEach(e =>
            {
                var other = e.Other(node);
                SolveNodes(node, other, e.Length);
            });
        }

        void SolveNodes(Node a, Node b, float rest)
        {
            var delta = a.position - b.position;
            var distance = delta.magnitude;
            var f = (distance - rest) / distance;
            a.position -= f * 0.5f * delta;
            b.position += f * 0.5f * delta;
        }

        public void DrawGizmos()
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                var node = _nodes[i];
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(node.position, 0.2f);

                Gizmos.color = Color.white;
                node.Edges.ForEach(e =>
                {
                    var other = e.Other(node);
                    Gizmos.DrawLine(node.position, other.position);
                });
            }
        }
    }
}

