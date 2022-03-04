namespace InstructionSetProject.Backend.Execution
{
    public interface IExecution
    {
        void Step();
        void ClockTick();
        void Continue();
    }
}
