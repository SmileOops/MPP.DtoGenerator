using System;
using System.Configuration;

namespace DtoGenerator.Classes
{
    internal static class ConfigManager
    {
        internal static bool TryGetTasksNumber(out int tasksCount)
        {
            string tasksNumberString = ConfigurationManager.AppSettings["tasksNumber"];

            if (tasksNumberString != null)
            {
                try
                {
                    tasksCount = Convert.ToInt32(tasksNumberString);
                }
                catch (FormatException)
                {
                    tasksCount = 0;
                    return false;
                }

                return true;
            }

            tasksCount = 0;
            return false;
        }

        internal static bool TryGetNamespace(out string classesNamespace)
        {
            string classesNamespaceString = ConfigurationManager.AppSettings["namespace"];

            if (!string.IsNullOrEmpty(classesNamespaceString))
            {
                classesNamespace = classesNamespaceString;

                return true;
            }

            classesNamespace = String.Empty;
            return false;
        }
    }
}
