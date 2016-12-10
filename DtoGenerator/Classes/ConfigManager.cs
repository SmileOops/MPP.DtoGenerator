using System.Configuration;

namespace DtoGenerator.Classes
{
    internal static class ConfigManager
    {
        internal static bool TryGetTasksNumber(out int tasksCount)
        {
            var tasksNumberString = ConfigurationManager.AppSettings["tasksNumber"];
            tasksCount = 0;

            if (tasksNumberString != null)
            {
                if (int.TryParse(tasksNumberString, out tasksCount)) return true;
            }

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