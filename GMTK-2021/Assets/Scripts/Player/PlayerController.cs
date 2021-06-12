using System;
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

    private SpriteRenderer _sprite;
    private Animator _anim;
    private Rigidbody2D _rigidbody;
    private bool[] _captureCmds;
    private bool _isOnGround;
    private bool _ignoreNextStay;
    private bool _isMoveFrame;
    private bool _lastDirLeft;

    public bool IsControllable { get; set; } = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _captureCmds = new bool[Enum.GetValues(typeof(PlayerCommands)).Length];
    }

    public void ReassignToLevel(GameLevel level)
    {
        _levelAttachedToPlayer = level;
    }

    private void Update()
    {
        _rigidbody.velocity = new Vector2(0F, _rigidbody.velocity.y);

        if (IsControllable)
        {
            if (Input.GetKey(KeyCode.A))
                MoveLeft();
            else if (Input.GetKey(KeyCode.D))
                MoveRight();

            if (Input.GetKeyDown(KeyCode.Space) && _isOnGround)
                Jump();
        }

        if (_isMoveFrame)
        {
            _sprite.flipX = _lastDirLeft;
            _anim.SetBool("IsPointedLeft", _lastDirLeft);
            _anim.SetBool("IsMoving", true);
        }
        else
        {
            _anim.SetBool("IsMoving", false);
        }

        _anim.SetBool("IsOffGround", !_isOnGround);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (_ignoreNextStay)
        {
            _ignoreNextStay = false;
            return;
        }

        foreach (var contact in other.contacts)
        {
            if (contact.normal.y > 0.8F)
                _isOnGround = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (var contact in other.contacts)
        {
            if (contact.normal.y > 0.8F)
                _isOnGround = true;
        }
    }

    private void ResetCapture()
    {
        for (var i = 0; i < _captureCmds.Length; i++)
            _captureCmds[i] = false;
    }

    private void FixedUpdate()
    {
        _isMoveFrame = false;
        
        if (!IsControllable)
            return;
        
        for (var i = 0; i < _captureCmds.Length; i++)
        {
            if (_captureCmds[i])
                _levelAttachedToPlayer.SavePlayerCommand((PlayerCommands) i);
        }

        _levelAttachedToPlayer.FinalizeSavedFrame();
        
        ResetCapture();
    }

    public void MoveLeft()
    {
        _rigidbody.velocity = new Vector2((_playerSpeed * Vector2.left).x, _rigidbody.velocity.y);
        _captureCmds[(int) PlayerCommands.MoveLeft] = true;
        _captureCmds[(int) PlayerCommands.MoveRight] = false;
        _isMoveFrame = true;
        _lastDirLeft = true;
    }

    public void MoveRight()
    {
        _rigidbody.velocity = new Vector2((_playerSpeed * Vector2.right).x, _rigidbody.velocity.y);
        _captureCmds[(int) PlayerCommands.MoveRight] = true;
        _captureCmds[(int) PlayerCommands.MoveLeft] = false;
        _isMoveFrame = true;
        _lastDirLeft = false;
    }

    public void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
        _captureCmds[(int) PlayerCommands.Jump] = true;
        _isOnGround = false;
        _ignoreNextStay = true;
    }

    public void PlayCommand(PlayerCommand command)
    {
        switch (command.Type)
        {
            case PlayerCommands.MoveLeft: 
                MoveLeft();
                break;
            case PlayerCommands.MoveRight: 
                MoveRight();
                break;
            case PlayerCommands.Jump:
                Jump();
                break;
        }
    }

    public void SetupDummy(PlayerController real)
    {
        _jumpForce = real._jumpForce;
        _playerSpeed = real._playerSpeed;

        IsControllable = false;
    }
}
