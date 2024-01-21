using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class VerletBehavior : MonoBehaviour
{
    [SerializeField] private int count = 20;
    [SerializeField] private int iterations = 12;
    [SerializeField] private bool useGravity = true;
    [SerializeField] private float gravity = 1f;
    [Range(0,1)][SerializeField] private float damping = 0f;

    private VerletSimulation _simulation;
    private Node[] _nodes;

    void Start()
    {
        _nodes = new Node[count];

        for (int i = 0; i < count; i++)
        {
            var node = new Node(Vector3.up * i * 0.5f + transform.position);
            _nodes[i] = node;
        }

        for (int i = 0; i < count - 1; i++)
        {
            var a = _nodes[i];
            var b = _nodes[i + 1];
            Edge.Connect(a, b);
        }

        _simulation = new VerletSimulation(_nodes);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (useGravity)
        {
            var g = gravity * Vector3.down;
            foreach (var node in _nodes)
            {
                node.position += dt * g;
            }
        }
        _simulation.Simulate(iterations, damping);
        _nodes[0].position = transform.position;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_simulation is not null) _simulation.DrawGizmos(0.2f,Color.red, Color.green);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
    #endif
}
