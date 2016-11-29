using System;
using System.Configuration;

namespace DtoGenerator.Classes
{
    internal static class ConfigManager
    {
        internal static bool TryGetTasksNumber(out int tasksCount)
        {
            var tasksNumberString = ConfigurationManager.AppSettings["tasksNumber"];

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
            var classesNamespaceString = ConfigurationManager.AppSettings["namespace"];

            if (!string.IsNullOrEmpty(classesNamespaceString))
            {
                classesNamespace = classesNamespaceString;

                return true;
            }

            classesNamespace = string.Empty;
            return false;
        }
    }
}