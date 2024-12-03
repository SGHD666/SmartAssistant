using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace SmartAssistant.Tests.UI.Helpers
{
    /// <summary>
    /// 提供UI测试中模拟用户交互的辅助方法
    /// </summary>
    public static class UserInteractionHelper
    {
        /// <summary>
        /// 模拟点击按钮
        /// </summary>
        public static async Task ClickButton(Button button)
        {
            if (button == null)
                throw new ArgumentNullException(nameof(button));

            var clickEvent = new RoutedEventArgs(Button.ClickEvent);
            await button.RaiseEventAsync(clickEvent);
        }

        /// <summary>
        /// 模拟在TextBox中输入文本
        /// </summary>
        public static async Task EnterText(TextBox textBox, string text)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.Text = text;
            var textInputEvent = new TextInputEventArgs
            {
                Text = text
            };
            await textBox.RaiseEventAsync(textInputEvent);
        }

        /// <summary>
        /// 查找控件中的子控件
        /// </summary>
        public static T FindControl<T>(IVisual parent, string name) where T : class, IControl
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            return parent.FindControl<T>(name);
        }

        /// <summary>
        /// 模拟选择ComboBox项
        /// </summary>
        public static async Task SelectComboBoxItem(ComboBox comboBox, object item)
        {
            if (comboBox == null)
                throw new ArgumentNullException(nameof(comboBox));

            comboBox.SelectedItem = item;
            var selectionChangedEvent = new SelectionChangedEventArgs(
                ComboBox.SelectionChangedEvent,
                Array.Empty<object>(),
                new[] { item });
            await comboBox.RaiseEventAsync(selectionChangedEvent);
        }

        /// <summary>
        /// 模拟切换CheckBox状态
        /// </summary>
        public static async Task ToggleCheckBox(CheckBox checkBox, bool isChecked)
        {
            if (checkBox == null)
                throw new ArgumentNullException(nameof(checkBox));

            checkBox.IsChecked = isChecked;
            var toggledEvent = new RoutedEventArgs(CheckBox.CheckedEvent);
            await checkBox.RaiseEventAsync(toggledEvent);
        }
    }
}
