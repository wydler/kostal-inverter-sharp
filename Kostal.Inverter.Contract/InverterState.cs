namespace Kostal.Inverter.Contract
{
    public enum InverterState
    {
        Off = 0,
        Idle = 1,
        Starting = 2,
        RunningMpp = 3,
        RunningRegulated = 4,
        Running = 5
    }
}
