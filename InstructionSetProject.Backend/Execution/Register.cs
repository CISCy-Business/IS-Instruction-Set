using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Execution
{
    public class Register<T>
    {
        public T value
        {
            get => value;
            set
            {
                if (_modifiable)
                    this.value = value;
            }
        }

        private readonly bool _modifiable = true;

        public Register(T? value = default(T), bool modifiable = true)
        {
            this.value = value;
            this._modifiable = modifiable;
        }
    }
}
