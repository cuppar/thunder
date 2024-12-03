using System;
using Godot;
using Thunder.Classes;
using Thunder.Globals.Extensions;

namespace Thunder;

public partial class Player : CharacterBody2D, IStateMachine<Player.State>
{
    #region 射击

    public void Shoot()
    {
        var projectile = Projectile.Create(ShootMarker.GlobalPosition);
        var container = GetParent().GetNodeOrNull<Node2D>("ProjectileContainer") ?? GetParent();
        container.AddChild(projectile);
    }

    #endregion

    #region 生命周期

    private Player()
    {
        _stateMachine = StateMachine<State>.Create(this);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("primary")) Shoot();
    }

    #endregion


    #region 状态机

    private StateMachine<State> _stateMachine;

    #region State enum

    public enum State
    {
        Idle,
        Move
    }

    #endregion

    #region IStateMachine<State> Members

    public void TransitionState(State fromState, State toState)
    {
        GD.Print($"{fromState} => {toState}");
        switch (toState)
        {
            case State.Idle:
                break;
            case State.Move:
                ResetBodyTurnTimer();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(toState), toState, null);
        }
    }


    public State GetNextState(State currentState, out bool keepCurrent)
    {
        keepCurrent = false;

        var direction = GetMoveDirection();

        switch (currentState)
        {
            case State.Idle:
                if (direction.Length() != 0)
                    return State.Move;
                break;
            case State.Move:
                if (direction.Length() == 0 && CurrentBodyRotation == BodyRotation.Idle)
                    return State.Idle;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
        }

        keepCurrent = true;
        return currentState;
    }

    public void TickPhysics(State currentState, double delta)
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Move:
                TickMove(delta);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
        }
    }

    #endregion

    #endregion

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public Sprite2D BodySprite { get; set; } = null!;

    [Export] public Timer BodyTurnDurationTimer { get; set; } = null!;

    [Export] public double BodyTurnDuration { get; set; } = 0.8;

    [ExportSubgroup("Markers")] [Export] public Marker2D ShootMarker { get; set; } = null!;

    #endregion

    #region 移动

    [ExportGroup("")] [Export] public float Speed { get; set; } = 300;

    private static Vector2 GetMoveDirection()
    {
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        return direction;
    }

    private void TickMove(double _)
    {
        var vDirection = Input.GetAxis("move_up", "move_down");

        CurrentBodyRotation = (vDirection, CurrentBodyRotation) switch
        {
            // turn down
            (> 0, BodyRotation.Idle) => BodyRotation.Down1,
            (> 0, BodyRotation.Up1) => BodyRotation.Idle,
            (> 0, BodyRotation.Up2) => BodyRotation.Up1,
            (> 0, BodyRotation.Down1) => BodyRotation.Down2,
            (> 0, BodyRotation.Down2) => BodyRotation.Down2,
            // turn up
            (< 0, BodyRotation.Idle) => BodyRotation.Up1,
            (< 0, BodyRotation.Up1) => BodyRotation.Up2,
            (< 0, BodyRotation.Up2) => BodyRotation.Up2,
            (< 0, BodyRotation.Down1) => BodyRotation.Idle,
            (< 0, BodyRotation.Down2) => BodyRotation.Down1,
            // no turn up or down
            (0, BodyRotation.Idle) => BodyRotation.Idle,
            (0, BodyRotation.Up1) => BodyRotation.Idle,
            (0, BodyRotation.Up2) => BodyRotation.Up1,
            (0, BodyRotation.Down1) => BodyRotation.Idle,
            (0, BodyRotation.Down2) => BodyRotation.Down1,
            _ => throw new InvalidOperationException()
        };

        var direction = GetMoveDirection();
        Velocity = Speed * direction;
        MoveAndSlide();
    }

    #region 飞机移动时转体

    private void ResetBodyTurnTimer()
    {
        BodyTurnDurationTimer.SetWaitTime(BodyTurnDuration);
        BodyTurnDurationTimer.OneShot = true;
        BodyTurnDurationTimer.Start();
    }

    #region Nested type: BodyRotation

    private enum BodyRotation
    {
        Idle,
        Up1,
        Up2,
        Down1,
        Down2
    }

    #endregion

    #region CurrentBodyRotation

    private BodyRotation _currentBodyRotation = BodyRotation.Idle;

    private BodyRotation CurrentBodyRotation
    {
        get => _currentBodyRotation;
        set => SetCurrentBodyRotation(value);
    }

    private async void SetCurrentBodyRotation(BodyRotation value)
    {
        await this.EnsureReadyAsync();
        if (value == _currentBodyRotation) return;
        if (BodyTurnDurationTimer.TimeLeft > 0) return;
        _currentBodyRotation = value;
        BodySprite.Frame = (int)_currentBodyRotation;
        ResetBodyTurnTimer();
    }

    #endregion

    #endregion

    #endregion
}