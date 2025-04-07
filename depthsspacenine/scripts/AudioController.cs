using Godot;
using System;

public partial class AudioController : AudioStreamPlayer
{
    [Export]
    public AudioStreamWav Alarm { get; set; }

    [Export]
    public AudioStreamWav JumpStartup { get; set; }

    [Export]
    public AudioStreamWav JumpPing { get; set; }

    [Export]
    public AudioStreamWav Scan { get; set; }

    public void PlayJumpStartup()
    {
        Stream = JumpStartup;
        Play(0);
    }

    public void PlayJumpPing()
    {
        Stream = JumpPing;
        Play(0);
    }

    public void PlayAlarm()
    {
        Stream = Alarm;
        Play(0);
    }

    public void PlayScan()
    {
        Stream = Scan;
        Play(0);
    }

    [Export]
    public GameStateController GameState { get; set; }

    public static AudioController Instance;
    public bool JumpPingPlayed { get; set; } 

    public override void _Ready()
    {
        Instance = this;

        detectionAudio = new AudioStreamWav[]
        {
            // first scan
            Scan,
            // second scan
            Scan,
            Scan,
            // third scan
            Scan,
            Scan,
            Scan,
            // catched
            Alarm,
            // oob
            Scan,
        };
    }

    int detectionIndex = 0;

    Func<float, bool>[] detectionTimings = new Func<float, bool>[]
    {
        // first scan
        f => f > 0.5,
        // second scan
        f => f > 0.75,
        f => f > 0.78,
        // third scan
        f => f > 0.9,
        f => f > 0.93,
        f => f > 0.96,
        // catched
        f => f >= 1,
        // oob
        f => false
    };

    private AudioStreamWav[] detectionAudio;

    bool firstAlarmPlayed = false;
    bool secondAlarmPlayed = false;
    bool thirdAlarmPlayed = false;
    bool loseAlarmPlayed = false;

    public override void _Process(double delta)
    {
        if (GameState.DetectionProgress <= 0.1)
        {
            detectionIndex = 0;
        }

        if (detectionTimings[detectionIndex](GameState.DetectionProgress))
        {
            Stream = detectionAudio[detectionIndex];
            Play(0);
            detectionIndex++;
        }
    }
}
