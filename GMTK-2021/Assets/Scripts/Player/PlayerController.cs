using System.Collections;
using System.Collections.Generic;
using Extensions;
using Levels;
using Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{ 
    [SerializeField] private GameLevel _levelAttachedToPlayer;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _downRaycastDistance = 1;
    
    private Rigidbody2D _rigidbody;

    public bool IsControllable { get; set; } = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsControllable)
            return;
        
        if (Input.GetKey(KeyCode.A))
            _levelAttachedToPlayer.SavePlayerCommand(new PlayerMoveLeftPlayerCommand(this, Time.deltaTime));
        if (Input.GetKey(KeyCode.D))
            _levelAttachedToPlayer.SavePlayerCommand(new PlayerMoveRightPlayerCommand(this, Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
            _levelAttachedToPlayer.SavePlayerCommand(new PlayerJumpCommand(this));
    }

    private bool OnGround()
    {
        return  Physics2D.Raycast(transform.position, Vector2.down, _downRaycastDistance);
    }

    public void MoveLeft(float deltaTime)
    {
        _rigidbody.AddPosition(_playerSpeed * deltaTime * Vector2.left);
    }

    public void MoveRight(float deltaTime)
    {
        _rigidbody.AddPosition(_playerSpeed * deltaTime * Vector2.right);
    }

    public void Jump()
    {
        _rigidbody.AddForce(Vector2.up * _jumpForce);
    }
}