using System;
using System.Collections;
using System.Collections.Generic;
using Ivyyy.Network;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkBehaviour))]
[RequireComponent(typeof(Ball))]
[RequireComponent(typeof(Rigidbody))]
public class CrawlyBrain : MonoBehaviour
{

    [Header("Parameters")] 
    [SerializeField] private TimeCounter idleTime;
    [SerializeField] private float directionRandomization;
    [Min(0)][SerializeField] private float chaosSpeed = 1f;
    [SerializeField] private float randomBurstFrequencySeconds = 5f;
    [SerializeField] private float randomBurstForce = 10f;
    [SerializeField] private TimeCounter randomBurstTime;
    [SerializeField] private AnimationCurve randomBurstDistribution;
    [SerializeField] private float wallEvadeStrength = 1;

    [Header("References")] 
    [SerializeField] private Transform ballModel;
    
    private NetworkBehaviour _networkBehaviour;
    private Ball _ball;
    private Rigidbody _rigidbody;
    private Vector3 _randomDirection;
    private float _initialSeed;
    private bool _isRaycasting;
    private RaycastHit _raycastHit;
    private bool _raycastIsHit;

    public bool BrainActive => _networkBehaviour.Owner && _ball.CurrentPlayerId == -1;
    public bool CanThink => BrainActive && idleTime.TimeRemaining < 0;
    void Start()
    {
        _networkBehaviour = GetComponent<NetworkBehaviour>();
        _ball = GetComponent<Ball>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        ResetBrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (BrainActive)
        {
            idleTime.Update();
            randomBurstTime.Update();
        }
        if (CanThink) Think();
    }

    void Think()
    {
        var seed = Time.time * chaosSpeed + _initialSeed;
        var x = Mathf.PerlinNoise1D(seed) - 0.5f;
        var y = Mathf.PerlinNoise1D(seed + 124) - 0.5f;
        var z = Mathf.PerlinNoise1D(seed -45) - 0.5f;
        _randomDirection = (new Vector3(x, y, z)).normalized;
        var velocity = _rigidbody.velocity;
        
        if (randomBurstTime.ProgressNormalized < 1)
        {
            var burstSpeed = randomBurstDistribution.Evaluate(randomBurstTime.ProgressNormalized);
            burstSpeed *= Time.deltaTime * randomBurstForce;
            velocity += ballModel.transform.forward * burstSpeed;
            _rigidbody.velocity = velocity;
        }

        _isRaycasting = false;
        //stearing
        var speed = _rigidbody.velocity.magnitude;
        if (speed > 1f)
        {
            var velocityOffset = directionRandomization * Time.deltaTime * _randomDirection;
            velocity += velocityOffset * Mathf.Clamp01(speed);

            _rigidbody.velocity = velocity.normalized * speed;
            
        }
        
        _isRaycasting = true;
        Ray ray = new(transform.position, ballModel.forward);
        _raycastIsHit = Physics.Raycast(ray, out _raycastHit, 5);
        if (_raycastIsHit)
        {
            velocity = _rigidbody.velocity;
            speed = velocity.magnitude;

            var evadeStrength = 1 / Mathf.Pow(_raycastHit.distance, 2);
            evadeStrength *= wallEvadeStrength;
            velocity += Time.deltaTime * evadeStrength * _raycastHit.normal;
            _rigidbody.velocity = velocity.normalized * speed;
        }
        
        if (velocity.magnitude < 0.4f)
        {
            var shouldBoost = Random.value < randomBurstFrequencySeconds * Time.deltaTime;
            if (shouldBoost)
            {
                Debug.Log("Boost");
                randomBurstTime.Reset();
            }
        }
    }
    
    public void ResetBrain()
    {
        idleTime.Reset();
        randomBurstTime.MakeReady();
        _initialSeed = Random.value * 1000;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _randomDirection * 4);
        if (_isRaycasting)
        {
            Gizmos.color = _raycastIsHit? Color.yellow : Color.black;
            Gizmos.DrawLine(transform.position, _raycastHit.point);
            Gizmos.DrawSphere(_raycastHit.point, 0.1f);
        }
    }
    #endif
}
