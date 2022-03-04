namespace InstructionSetProject.Backend.InstructionTypes
{
    internal interface ILabelInstruction
    {
        bool CheckForLabel(string line);
    }
}
