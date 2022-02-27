using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Execution
{
    public class Register<T>
    {
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

        public Register(T? value = default, bool modifiable = true)
        {
            _modifiable = modifiable;
            _value = value;
        }
    }
}
