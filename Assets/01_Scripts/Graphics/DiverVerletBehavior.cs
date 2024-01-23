using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Verlet;

public class DiverVerletBehavior : MonoBehaviour
{
    [SerializeField] private int count = 5;
    [SerializeField] private float nodeDistance = 0.2f;
    [SerializeField] private int iterations = 30;
    [SerializeField] private float currentStrength = 1f;
    [Range(0, 1), SerializeField] private float damping = 0.2f;

    private VerletSimulation _simulation;
    private Node[] _nodes;
    private ValueSmoother<Vector3> _lastNodeSmoother;

    public Vector3 GetNode(int i) => _nodes[i].position;
    public Vector3 GetLastNode() => _nodes[count - 1].position;
    public Vector3 SmoothTarget => _lastNodeSmoother.SmoothTarget;
    
    public void ResetSimulation()
    {
        _nodes = new Node[count];
        for (int i = 0; i < count; i++)
        {
            _nodes[i] = new Node(transform.position - nodeDistance * i * transform.forward);
        }

        for (int i = 0; i < count - 1; i++)
        {
            Edge.Connect(_nodes[i], _nodes[i+1]);
        }
        _simulation = new(_nodes);
        _lastNodeSmoother = ValueSmoother<Vector3>.Vector3Smoother(GetLastNode());
    }

    void FixedUpdate()
    {
        if (_simulation == null) ResetSimulation();

        float dt = Time.fixedDeltaTime;
        var current = - transform.forward * currentStrength;
        foreach (var node in _nodes)
        {
            node.position += dt * current;
        }
        _simulation.Simulate(iterations, damping);
        _nodes[0].position = transform.position;
        _lastNodeSmoother.FixedUpdate(GetLastNode());
    }

    private void Update()
    {
        _lastNodeSmoother.Update();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_simulation is not null) _simulation.DrawGizmos(0.05f, Color.red, Color.green);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.blue;
        if(_lastNodeSmoother != null) Gizmos.DrawSphere(_lastNodeSmoother.SmoothTarget, 0.04f);
    }
    #endif
}
