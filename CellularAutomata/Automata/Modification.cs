using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Automata
{
    public abstract class Modification
    {
        //Finds infomation related to a modification including name and arguments
        public static Dictionary<string, (Type modification, Dictionary<string, Type> arguments)> GetModifications(IEnumerable<Type> modifications)
        {
            Dictionary<string, (Type modification, Dictionary<string, Type> arguments)> modificationsFull = 
                new Dictionary<string, (Type modification, Dictionary<string, Type> arguments)>();

            foreach (Type modification in modifications)
            {
                //Ensure that only modifications have been passed in
                if (!modification.IsSubclassOf(typeof(Modification)))
                {
                    throw new ArrayTypeMismatchException("Only objects of Type Modification can be stored in the modifications array");
                }

                //Get list of arguments for modification
                ConstructorInfo constructor = modification.GetConstructors()[0];

                Dictionary<string, Type> arguments = constructor.GetParameters()
                        .ToDictionary(parameter => parameter.Name, parameter => parameter.ParameterType);

                (Type modification, Dictionary<string, Type> arguments) modificationFull = (modification, arguments);

                //Get class display name
                DisplayNameAttribute displayNameAttribute = modification.GetCustomAttribute<DisplayNameAttribute>();

                modificationsFull.Add(
                    displayNameAttribute != null ? displayNameAttribute.DisplayName : modification.Name,
                    modificationFull);
            }

            return modificationsFull;
        }

        public static Modification CreateModification(Type modification, object[] arguments)
        {
            if (!modification.IsSubclassOf(typeof(Modification)))
            {
                throw new ArgumentException("Modification is not of type Modification", nameof(modification));
            }

            ParameterInfo[] modificationArguments = modification.GetConstructors()[0].GetParameters();

            for (int i = 0; i < arguments.Length; i++)
            {
                object argumentType = arguments[i].GetType();
                object expectedType = modificationArguments[i].ParameterType;

                if (argumentType != expectedType)
                {
                    throw new ArgumentException("Argument array must only contain arguments used by the modification", nameof(arguments));
                }
            }

            return (Modification)Activator.CreateInstance(modification, arguments);
        }

        public static Modification CreateModification(Type modification, Device device)
        {
            if (!modification.IsSubclassOf(typeof(Modification)))
            {
                throw new ArgumentException("Modification is not of type Modification", nameof(modification));
            }

            ParameterInfo[] modificationArguments = modification.GetConstructors()[0].GetParameters();
            object[] arguments = new object[modificationArguments.Length];

            for (int i = 0; i < modificationArguments.Length; i++)
            {
                while (true)
                {
                    string modificationName = modificationArguments[i].Name;

                    if (device.SecondaryDisplay != null)
                    {
                        device.SecondaryDisplay.Write(modificationName + ": ");
                    }
                    else
                    {                        
                        Console.Write(modificationName + ": ");
                    }

                    Type conversionType = modificationArguments[i].ParameterType;

                    try
                    {
                        if (conversionType.IsEnum)
                        {
                            //TODO switch to device.getoption
                            arguments[i] = Enum.Parse(conversionType, device.Input.ReadLine());
                        }
                        else
                        {
                            //TODO fix supported types, currently only works with int as keypad can not enter any other types
                            arguments[i] = Convert.ChangeType(device.Input.ReadLine(), conversionType);
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            return (Modification)Activator.CreateInstance(modification, arguments);
        }

        public abstract void ApplyModification(CellularAutomata automata);
    }

    public class ChangeAllColours : Modification
    {
        private readonly ConsoleColor[] _colours;

        public ChangeAllColours(ConsoleColor[] colours)
        {
            _colours = colours;
        }

        public override void ApplyModification(CellularAutomata automata)
        {
            if (_colours.Length == automata.Colours.Length)
            {
                _colours.CopyTo(automata.Colours, 0);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_colours), _colours, "New colours array should have contained " + automata.Colours.Length + " colours");
            }
        }
    }

    [DisplayName("Change Colour")]
    public class ChangeColour : Modification
    {
        private readonly int _colourPosition;
        private readonly ConsoleColor _colour;

        public ChangeColour(int colourPosition, ConsoleColor colour)
        {
            _colourPosition = colourPosition;
            _colour = colour;
        }

        public override void ApplyModification(CellularAutomata automata)
        {
            if (_colourPosition < automata.Colours.Length)
            {
                automata.Colours[_colourPosition] = _colour;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_colourPosition), _colourPosition, "Colour at position " + _colourPosition + " does not exist");
            }
        }
    }

    //TODO join with ToggleRunning
    [DisplayName("Set Running")]
    public class SetRunning : Modification
    {
        private readonly bool _running;

        public SetRunning(bool running)
        {
            _running = running;
        }

        public override void ApplyModification(CellularAutomata automata)
        {
            automata.Running = _running;
        }
    }

    [DisplayName("Toggle Running")]
    public class ToggleRunning : Modification
    {
        public override void ApplyModification(CellularAutomata automata)
        {
            automata.Running = !automata.Running;
        }
    }
}