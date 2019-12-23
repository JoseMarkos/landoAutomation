using System;

namespace landoAutomatioin
{
    sealed class Program
    {
        private static Automation GetAutomation()
        {
            return new Automation();
        }

        static void Main(string[] args)
        {
            Automation automation = GetAutomation();

            automation.Start();
        }
    }
}
