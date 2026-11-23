using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
namespace QA_Master.Models.Extensions
{
    public static class Extension
    {
        public static string GetDescription(this Enum value)
        {
            MemberInfo[] memberInfos = value.GetType().GetMember(value.ToString());

            if (memberInfos != null && memberInfos.Length > 0)
            {
                object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return value.ToString();
        }

        public static string GetDescription<T>(this T obj) where T : class
        {
            Attribute? attribute = obj.GetType().GetCustomAttribute(typeof(DescriptionAttribute), false);

            if (attribute != null)
                return ((DescriptionAttribute)attribute).Description;

            return obj.GetType().Name;
        }

        public static List<Core.Action?> GetAllActionClasses(out Core.Action? selectedAction, string insertDefault = "Select Action", Core.Action? editAction = null)
        {
            List<Core.Action?> returnValues =
            [.. Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(X => X.IsSubclassOf(typeof(Core.Action)))
                .Select(type => Activator.CreateInstance(type) as Core.Action)
                .OrderBy(X => X?.ToString())
                ];

            if (string.IsNullOrWhiteSpace(insertDefault) == false) returnValues.Insert(0, new Core.Action(insertDefault));

            selectedAction = returnValues.FirstOrDefault(X => X?.GetType() == editAction?.GetType());
            if (selectedAction != null && editAction != null) selectedAction.EditModel = editAction?.CopyTo();

            returnValues.ForEach(X => { if (X != null && X?.EditModel == null) if (X != null) X.EditModel = X?.CopyTo(); });

            return returnValues;
        }

        //public static List<Core.Workflow> GetAllProjectWorkflow(this Core.Project project, string insertDefault = "Add Workflow")
        //{
        //    List<Core.Workflow?> workflows = new List<Core.Workflow>();
        //    if (string.IsNullOrWhiteSpace(insertDefault) == false)
        //        workflows.Add(new Core.Workflow(insertDefault));
        //
        //    workflows.AddRange(project?.Workflows.ToArray());
        //
        //    return workflows;
        //}
        //
        //public static Core.Perform?[] GetAllPerformClasses()
        //{
        //    return
        //        Assembly
        //        .GetExecutingAssembly()
        //        .GetTypes()
        //        .Where(X => X.IsSubclassOf(typeof(Core.Perform)))
        //        .Select(type => Activator.CreateInstance(type) as Core.Perform)
        //        .ToArray();
        //}

        //public static List<Control> GetControls(this Control control)
        //{
        //    List<Control> controls = new List<Control>();
        //
        //    if (control == null) return controls;
        //
        //    foreach (Control eachControl in control.Controls)
        //    {
        //        controls.Add(eachControl);
        //        controls.AddRange(eachControl.GetControls().ToArray());
        //    }
        //
        //    return controls;
        //}

        public static string[] GetAllEnumNames(this Type value)
        {
            return Enum.GetNames(value).ToArray();
        }

        public static T? CloneFirstLevel<T>(this T? source) where T : class
        {
            if (source == null) return null;
        
            T? destination = (T?)Activator.CreateInstance(source.GetType());
        
            source.CopyFirstLevel(destination);
        
            return destination;
        }

        //public static T? Clone<T>(this T? source) where T : class
        //{
        //    //create deep clone
        //}

        public static void CopyFirstLevel<T>(this T? source, T? destination)
        {
            if (source == null || destination == null) return;

            foreach (PropertyInfo propertyInfo in (source.GetType().GetProperties()))
            {
                //System.Diagnostics.Debug.WriteLine($"{propertyInfo.MemberType} : {propertyInfo.Name} [{propertyInfo.PropertyType.Name}]");

                if (propertyInfo.GetMethod != null && propertyInfo.SetMethod != null)
                {
                    object? sourceValue = propertyInfo.GetValue(source);
                    object? destinationValue = propertyInfo.GetValue(destination);

                    if (sourceValue != null && sourceValue.Equals(destinationValue) == false)
                        propertyInfo.SetValue(destination, sourceValue);
                }
            }
        }

        //public static void CopyTo<T>(this T? source, T? destination)
        //{
        //    if (source == null || destination == null) return;
        //
        //    foreach (PropertyInfo sourcePropertyInfo in (source.GetType().GetProperties()))
        //    {
        //        if (sourcePropertyInfo.CanRead && sourcePropertyInfo.CanWrite)
        //        {
        //            if (sourcePropertyInfo.GetType() == typeof(IEnumerable<>))
        //
        //            object? sourceValue = sourcePropertyInfo.GetValue(source);
        //            object? destinationValue = sourcePropertyInfo.GetValue(destination);
        //
        //            if (sourceValue != null && sourceValue.Equals(destinationValue) == false)
        //                sourcePropertyInfo.SetValue(destination, sourceValue);
        //        }
        //    }
        //}

        public static Binding? AddDataBinding<T>(this T? dataObject, Expression<Func<T?, object?>>? property, Control? control, Label? lblControl = null, bool addLableColonAtEnd = true) where T : class
        {
            MemberExpression? memberExpression = GetMemberExpression(dataObject, property);

            if (memberExpression != null)
            {
                Binding? binding = null;

                switch (control)
                {
                    case TextBox:
                        BindingOperations.ClearBinding(control, TextBox.TextProperty);
                        binding = new(memberExpression.Member.Name) { Source = dataObject };
                        BindingOperations.SetBinding(control, TextBox.TextProperty, binding);
                        break;
                    case ToggleButton:
                        BindingOperations.ClearBinding(control, ToggleButton.IsCheckedProperty);
                        binding = new(memberExpression.Member.Name) { Source = dataObject };
                        BindingOperations.SetBinding(control, ToggleButton.IsCheckedProperty, binding);
                        break;
                }
        
                if (lblControl != null)
                {
                    lblControl.Content = $"{dataObject.GetDescription(property)}" + (addLableColonAtEnd ? " :" : "");
                }
        
                return binding;
            }
        
            return null;
        }

        public static Binding? AddDataBinding<T>(this T? dataObject, Expression<Func<T?, object?>>? source, Selector? selector) where T : class
        {
            if (source != null && selector != null && source.Body.GetType().IsSubclassOf(typeof(MemberExpression)))
            {
                BindingOperations.ClearBinding(selector, ItemsControl.ItemsSourceProperty);
                selector.Items.Clear();
                
                Binding binding = new(((MemberExpression)source.Body).Member.Name) { Source = dataObject };
                BindingOperations.SetBinding(selector, ItemsControl.ItemsSourceProperty, binding);

                if (selector.Items.Count > 0) selector.SelectedIndex = 0;

                return binding;
            }

            return null;
        }

        private static MemberExpression? GetMemberExpression<T>(this T? classObject, Expression<Func<T?, object?>>? property) where T : class
        {
            if(classObject == null ||  property == null) return null;

            //if (property.Body.GetType().IsSubclassOf(typeof(LambdaExpression)))
            //{
                LambdaExpression lambda = property;
                MemberExpression memberExpression;

                if (lambda.Body is UnaryExpression unaryExpression)
                {
                    memberExpression = (MemberExpression)unaryExpression.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression)lambda.Body;
                }

                return memberExpression;
            //}
            //else
            //{
            //    return null;
            //}
        }
        

        public static string? GetDescription<T>(this T? classObject, Expression<Func<T?, object?>>? property) where T : class
        {
            MemberExpression? memberExpression = GetMemberExpression(classObject, property);

            if (memberExpression != null)
            {
                IEnumerable<DescriptionAttribute> descriptions = memberExpression.Member.GetCustomAttributes<DescriptionAttribute>();
                if (descriptions != null && descriptions.Any())
                {
                    return descriptions.First().Description;
                }
                else
                {
                    return memberExpression.Member.Name;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetPassFailString(this bool value, string passString = "Passed", string failString = "Failed")
        {
            return value ? passString : failString;
        }

        //public static void SelectListBoxItem(this ListBox listBox, int? index = null)
        //{
        //    if (index == null) index = listBox.Items.Count - 1;
        //    
        //    if (index.Value < 2)
        //        listBox.TopIndex = index.Value;
        //    else
        //        listBox.TopIndex = index.Value - 2;
        //
        //    listBox.SelectedIndex = index.Value;
        //
        //    MethodInfo onSelectedIndexChanged = listBox.GetType().GetMethod("OnSelectedIndexChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        //    onSelectedIndexChanged.Invoke(listBox, new object[] { EventArgs.Empty });
        //}

        public static string GetShorter(this string value, bool ignoreShorterDescription = false, int maxChar = 56)
        {
            if (ignoreShorterDescription || string.IsNullOrWhiteSpace(value)) return value;
            if (value.Length <= maxChar) return value;

            int subLength = (maxChar - 3) / 2;
            return $"{value.Substring(0, subLength)}...{value.Substring(value.Length - subLength)}";
        }

        public static int ConvertToInt(this string? value, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value) == true) return defaultValue;

            int returnValue = defaultValue;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        public static double ConvertToDouble(this string? value, double defaultValue = 0d)
        {
            if (string.IsNullOrWhiteSpace(value) == true) return defaultValue;

            double returnValue = defaultValue;
            double.TryParse(value, out returnValue);
            return returnValue;
        }

        public static string? GetAbsolutePath(this string? relativePath, string? absoluteDirectoryPath)
        {
            if (relativePath != null && relativePath.Contains(":\\")) return relativePath;

            List<string>? relativePathArr = relativePath?.Split('\\').ToList();
            List<string>? absoluteDirectoryPathArr = absoluteDirectoryPath?.Split('\\').ToList();

            if (relativePathArr != null && relativePathArr.First() == ".") relativePathArr.RemoveAt(0);

            while (relativePathArr?.First() == "..")
            {
                relativePathArr.RemoveAt(0);
                absoluteDirectoryPathArr?.RemoveAt(absoluteDirectoryPathArr.Count - 1);
            }

            if (absoluteDirectoryPathArr != null && relativePathArr != null)
                return Path.Combine(string.Join("\\", absoluteDirectoryPathArr.ToArray()), string.Join("\\", relativePathArr.ToArray()));
            else
                return null;
        }

        public static string? GetRelativePath(this string? relativePathTo, string? absoluteDirectoryPath)
        {
            List<string>? absoluteDirectoryPathArr = absoluteDirectoryPath?.Split('\\').ToList();
            List<string>? relativePathToArr = relativePathTo?.Split('\\').ToList();

            bool driveRemoved = false;
            while ((absoluteDirectoryPathArr?.Count > 0) && (relativePathToArr?.Count > 0) && (absoluteDirectoryPathArr.First() == relativePathToArr.First()))
            {
                absoluteDirectoryPathArr.RemoveAt(0);
                relativePathToArr.RemoveAt(0);
                driveRemoved = true;
            }

            string? relativePath;
            if (driveRemoved && absoluteDirectoryPathArr != null && relativePathToArr != null)
            {
                relativePath = ".\\" + Path.Combine(string.Join("\\", Enumerable.Repeat("..", absoluteDirectoryPathArr.Count).ToArray()), string.Join("\\", relativePathToArr.ToArray()));
            }
            else
            {
                relativePath = relativePathTo;
            }

            return relativePath;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
