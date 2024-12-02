using System;
using Godot;
using Thunder.Classes;
using Thunder.Globals.Extensions;

namespace Thunder;

public partial class Player : CharacterBody2D, IStateMachine<Player.State>
{
    #region State enum

    public enum State
    {
        Idle,
        Move
    }

    #endregion

    private StateMachine<State> _stateMachine;

    private Player()
    {
        _stateMachine = StateMachine<State>.Create(this);
    }

    [Export] public float Speed { get; set; } = 300;

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

    private void ResetBodyTurnTimer()
    {
        BodyTurnDurationTimer.SetWaitTime(BodyTurnDuration);
        BodyTurnDurationTimer.OneShot = true;
        BodyTurnDurationTimer.Start();
    }

    private static Vector2 GetMoveDirection()
    {
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        return direction;
    }

    private void TickMove(double delta)
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

    #region Child

    [ExportGroup("ChildDontChange")]
    [Export]
    public Sprite2D BodySprite { get; set; } = null!;

    [Export] public Timer BodyTurnDurationTimer { get; set; } = null!;
    [Export] public double BodyTurnDuration { get; set; } = 0.8;

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
}