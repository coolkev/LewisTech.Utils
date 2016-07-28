using System;
using System.Diagnostics;

#if POSTSHARP
using PostSharp.Aspects;
#endif

namespace LewisTech.Utils.Profiler
{
    public static class Profiler
    {
        static Profiler()
        {
            Provider = new DefaultProfiler();
        }

        public static IProfiler Provider { get; set; }

        public static IDisposable Step(string name)
        {
            return Provider.Step(name);
        }

    }
    public interface IProfiler
    {
        IDisposable Step(string name);
    }

    public class DefaultProfiler : IProfiler
    {
        public IDisposable Step(string name)
        {
            return null;
        }
    }

    public class TraceProfiler : IProfiler
    {
        public IDisposable Step(string name)
        {
            return new TraceProfilerStep(name);
        }
    }

    public class TraceProfilerStep : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _stopwatch;
        private static int _depth;
        public TraceProfilerStep(string name)
        {
            _name = name;
            _stopwatch = Stopwatch.StartNew();
            _depth++;
        }

        public void Dispose()
        {

            Trace.WriteLine("".PadLeft(_depth, '\t') + _name + "\t" + _stopwatch.ElapsedMilliseconds);

            _depth--;

        }
    }
    
    [Serializable]
    public sealed class ProfileAttribute :
#if POSTSHARP
 MethodInterceptionAspect
        #else
        Attribute    
        #endif
    {
        public string Name { get; set; }

        public ProfileAttribute()
        {
            
        }

        public ProfileAttribute(string name)
        {
            Name = name;
        }
#if POSTSHARP

        public override void OnInvoke(MethodInterceptionArgs args)
        {

            var name = Name;
            if (name == null)
                name = (args.Method.DeclaringType == null ? null : (args.Method.DeclaringType.Name + ".")) + args.Method.Name;

            using (Profiler.Step(name))
            {
                base.OnInvoke(args);

            }
        }
        #endif

    }
}
