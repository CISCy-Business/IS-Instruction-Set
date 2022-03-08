namespace InstructionSetProject.Backend.Execution
{
    public class Register<T>
    {
        public string Label;
        public T? value
        {
            get => _value;
            set
            {
                if (_modifiable)
                    _value = value;
            }
        }

        private T? _value;

        private readonly bool _modifiable = true;

        public Register(string label, T? value = default, bool modifiable = true)
        {
            Label = label;
            _modifiable = modifiable;
            _value = value;
        }
    }
}
