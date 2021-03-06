using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Improbable.Gdk.Core
{
    public class CommandLineArgs
    {
        private Dictionary<string, string> arguments;

        // Hide default constructor
        private CommandLineArgs()
        {
        }

        public static CommandLineArgs FromCommandLine()
        {
            return From(Environment.GetCommandLineArgs());
        }

        public static CommandLineArgs From(Dictionary<string, string> args)
        {
            return new CommandLineArgs
            {
                arguments = args
            };
        }

        public static CommandLineArgs From(IList<string> args)
        {
            return new CommandLineArgs
            {
                arguments = ParseCommandLineArgs(args)
            };
        }

        public string Dump()
        {
            return arguments
                .Aggregate(new StringBuilder(), (builder, pair) => builder.AppendLine($"{pair.Key}={pair.Value}"))
                .ToString();
        }

        public bool Contains(string key)
        {
            return arguments.ContainsKey(key);
        }

        public T GetCommandLineValue<T>(string key, T defaultValue)
        {
            T value = defaultValue;
            TryGetCommandLineValue(key, ref value);
            return value;
        }

        public bool TryGetCommandLineValue<T>(string key, ref T value)
        {
            var desiredType = typeof(T);
            if (arguments.TryGetValue(key, out var strValue))
            {
                if (strValue == null)
                {
                    throw new InvalidOperationException($"Cannot convert flag only argument, {key}, did you mean to call Contains?");
                }

                if (desiredType.IsEnum)
                {
                    try
                    {
                        value = (T) Enum.Parse(desiredType, strValue);
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw new FormatException($"Unable to parse argument {strValue} as enum {desiredType.Name}.",
                            e);
                    }
                }

                value = (T) Convert.ChangeType(strValue, typeof(T));
                return true;
            }

            return false;
        }

        private static bool IsFlag(string flag)
        {
            return flag.StartsWith("+") || flag.StartsWith("-");
        }

        private static Dictionary<string, string> ParseCommandLineArgs(IList<string> args)
        {
            var config = new Dictionary<string, string>();
            for (var i = 0; i < args.Count; i++)
            {
                var flag = args[i];
                if (IsFlag(flag))
                {
                    var strippedOfPlus = flag.Substring(1, flag.Length - 1);
                    if (i + 1 >= args.Count || IsFlag(args[i + 1]))
                    {
                        config[strippedOfPlus] = null;
                    }
                    else
                    {
                        config[strippedOfPlus] = args[i + 1];
                        i++;
                    }
                }
            }

            return config;
        }
    }
}
