using System.Windows;

namespace IgrisLib.MessageBox
{
    public sealed class IgrisMessageBox
    {
        private IgrisMessageBox()
        {
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText);
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption);
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText)
            {
                Owner = owner
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption)
            {
                Owner = owner
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, button);
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, button, icon);
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowOK(string messageBoxText, string caption, string okButtonText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.OK)
            {
                OkButtonText = okButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowOK(string messageBoxText, string caption, string okButtonText, MessageBoxImage icon)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.OK, icon)
            {
                OkButtonText = okButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowOKCancel(string messageBoxText, string caption, string okButtonText, string cancelButtonText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.OKCancel)
            {
                OkButtonText = okButtonText,
                CancelButtonText = cancelButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowOKCancel(string messageBoxText, string caption, string okButtonText, string cancelButtonText, MessageBoxImage icon)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.OKCancel, icon)
            {
                OkButtonText = okButtonText,
                CancelButtonText = cancelButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowYesNo(string messageBoxText, string caption, string yesButtonText, string noButtonText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.YesNo)
            {
                YesButtonText = yesButtonText,
                NoButtonText = noButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowYesNo(string messageBoxText, string caption, string yesButtonText, string noButtonText, MessageBoxImage icon)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.YesNo, icon)
            {
                YesButtonText = yesButtonText,
                NoButtonText = noButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowYesNoCancel(string messageBoxText, string caption, string yesButtonText, string noButtonText, string cancelButtonText)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.YesNoCancel)
            {
                YesButtonText = yesButtonText,
                NoButtonText = noButtonText,
                CancelButtonText = cancelButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }

        public static MessageBoxResult ShowYesNoCancel(string messageBoxText, string caption, string yesButtonText, string noButtonText, string cancelButtonText, MessageBoxImage icon)
        {
            IgrisMessageBoxWindow IgrisMessageBoxWindow = new IgrisMessageBoxWindow(messageBoxText, caption, MessageBoxButton.YesNoCancel, icon)
            {
                YesButtonText = yesButtonText,
                NoButtonText = noButtonText,
                CancelButtonText = cancelButtonText
            };
            IgrisMessageBoxWindow.ShowDialog();
            return IgrisMessageBoxWindow.Result;
        }
    }
}
