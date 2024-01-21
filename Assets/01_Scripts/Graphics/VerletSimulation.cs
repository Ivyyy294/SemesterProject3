using System.Collections;
using System.Collections.Generic;
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

        public void Step(float damping)
        {
            var velocity = position - _prev;
            var next = position + velocity * (1 - damping);
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
            length = (a.position - b.position).magnitude;
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
        private readonly Node[] _nodes;

        public VerletSimulation(Node[] nodes)
        {
            _nodes = nodes;
        }

        public void Simulate(int iterations, float damping = 0f)
        {
            Step(damping);
            Solve(iterations);
        }

        void Step(float damping)
        {
            foreach (var node in _nodes) node.Step(damping);
        }

        void Solve(int iterations)
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
                
                // Move them together to restore their edge length
                var delta = node.position - other.position;
                var distance = delta.magnitude;
                var halfMoveDistance = ((distance - e.Length) / distance) * 0.5f;
                node.position -= halfMoveDistance * delta;
                other.position += halfMoveDistance * delta;
            });
        }

        public void DrawGizmos(float radius = 0.2f, params Color[] colors)
        {
            if (colors.Length < 2)
            {
                colors = new[] { Color.yellow, Color.white };
            }
            for (int i = 0; i < _nodes.Length; i++)
            {
                var node = _nodes[i];
                Gizmos.color = colors[0];
                Gizmos.DrawSphere(node.position, radius);

                Gizmos.color = colors[1];
                node.Edges.ForEach(e =>
                {
                    var other = e.Other(node);
                    Gizmos.DrawLine(node.position, other.position);
                });
            }
        }
    }
}

