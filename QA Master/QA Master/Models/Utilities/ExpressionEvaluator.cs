using QA_Master.Models.Extensions;
using System.Windows.Controls;

namespace QA_Master.Models.Utilities
{
    public class ExpressionEvaluator
    {
        public static string? Evaluate(string? expression)
        {
            if (string.IsNullOrWhiteSpace(expression) == false)
            {
                if (expression.Contains("$(") == true)
                {
                    List<string> values = [.. expression.Split("$(")];
                    List<string?> finalValues = [values[0]];

                    for (int i = 1; i < values.Count; i++)
                    {
                        string[] each = values[i].Split(")");

                        if (each.Length >= 2)
                        {
                            if (each[0].StartsWith("Range("))
                            {
                                string[] rangeValues = each[0].Replace("Range(", "").Split(",");
                                int minValue = 0, maxValue = 0;
                                int.TryParse(rangeValues[0], out minValue);
                                if (rangeValues.Length > 1) int.TryParse(rangeValues[1], out maxValue);
                                if (minValue > maxValue) maxValue = minValue;
                                finalValues.Add(new Random(Environment.TickCount).Next(minValue, maxValue + 1).ToString());
                                finalValues.AddRange(each.Skip(2));
                            }
                            else
                            {
                                EnumExpression enumExpression = (EnumExpression)Extensions.Extension.GetAllEnumNames(typeof(EnumExpression)).ToList().IndexOf(each[0]);

                                string? expressionResult;
                                switch (enumExpression)
                                {
                                    case EnumExpression.Guid:
                                        expressionResult = Guid.NewGuid().ToString();
                                        finalValues.Add(expressionResult);
                                        finalValues.Add(each[1]);
                                        break;
                                    case EnumExpression.Date:
                                        expressionResult = DateTime.Now.ToString("dd-MMM-yyyy");
                                        finalValues.Add(expressionResult);
                                        finalValues.Add(each[1]);
                                        break;
                                    case EnumExpression.Time:
                                        expressionResult = DateTime.Now.ToString("HH:mm:ss");
                                        finalValues.Add(expressionResult);
                                        finalValues.Add(each[1]);
                                        break;
                                    case EnumExpression.DateTime:
                                        expressionResult = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
                                        finalValues.Add(expressionResult);
                                        finalValues.Add(each[1]);
                                        break;
                                    default:
                                        finalValues.Add("$(" + each[0] + ")");
                                        finalValues.Add(each[1]);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            finalValues.Add("$(" + each[0]);
                        }
                    }

                    return string.Join("", finalValues.ToArray());
                }
            }

            return expression;
        }

        const string menuItemPrefix = "expressionMenuItem";

        public TextBox? textBox;
        public List<MenuItem> DataSource(TextBox textBox)
        {
            this.textBox = textBox;

            List<MenuItem> enumNames = Extension.GetAllEnumNames(typeof(EnumExpression)).Select(X => new MenuItem { Header = X, Name = $"{menuItemPrefix}_{X}"}).ToList();

            enumNames.ForEach(X => X.Click += MenuItem_Click);

            return enumNames;
        }

        public void MenuItem_Click(object sender, EventArgs e)
        {
            string? item = (string?)((MenuItem)sender).Name;

            if (textBox != null && item != null && item.StartsWith(menuItemPrefix))
            {
                item = item.Replace($"{menuItemPrefix}_", "");

                IList<string> enumNames = [.. Extension.GetAllEnumNames(typeof(EnumExpression))];
                EnumExpression enumExpression = (EnumExpression)enumNames.IndexOf(item);

                string expression;

                switch(enumExpression)
                {
                    case EnumExpression.Range:
                        expression = "$(Range(0, 0))";
                        break;
                    default:
                        expression = $"$({enumExpression})";
                        break;
                }

                string? preText, postTest;

                preText = textBox?.Text.Substring(0, textBox.SelectionStart);
                postTest = textBox?.Text.Substring(textBox.SelectionStart + textBox.SelectionLength);

                if (textBox != null)
                {
                    textBox.Text = $"{preText}{expression}{postTest}";
                    textBox.SelectionLength = expression.Length;
                    textBox.SelectionStart = preText.Length;
                    textBox.Focus();
                }
            }
        }
    }
}
